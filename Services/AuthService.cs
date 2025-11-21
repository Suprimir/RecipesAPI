using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using RecipesAPI.DTOs;
using RecipesAPI.Models;
using RecipesAPI.Repositories;

namespace RecipesAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IAuthRepository authRepository, IConfiguration configuration, ILogger<AuthService> logger)
        {
            _authRepository = authRepository;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<AuthResponseDTO> RegisterAsync(RegisterRequestDTO request)
        {
            if (await _authRepository.EmailExistsAsync(request.Email))
            {
                throw new InvalidOperationException("El email ya está registrado");
            }

            if (await _authRepository.UsernameExistsAsync(request.Username))
            {
                throw new InvalidOperationException("El nombre de usuario ya está en uso");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                Email = request.Email,
                PasswordHash = HashPassword(request.Password),
                IsEmailVerified = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _authRepository.CreateUserAsync(user);

            var accessToken = GenerateAccessToken(user);
            var refreshToken = await GenerateRefreshTokenAsync(user.Id);

            return new AuthResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["Jwt:ExpireMinutes"])),
                User = new UserDTO
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Bio = user.Bio,
                    ProfileImageUrl = user.ProfileImageUrl,
                    IsEmailVerified = user.IsEmailVerified,
                    CreatedAt = user.CreatedAt
                }
            };
        }

        public async Task<AuthResponseDTO> LoginAsync(LoginRequestDTO request)
        {
            var user = await _authRepository.GetUserByEmailAsync(request.Email);

            if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Email o contraseña incorrectos");
            }

            if (!user.IsActive)
            {
                throw new UnauthorizedAccessException("La cuenta está desactivada");
            }

            user.LastLoginAt = DateTime.UtcNow;
            await _authRepository.UpdateUserAsync(user);

            var accessToken = GenerateAccessToken(user);
            var refreshToken = await GenerateRefreshTokenAsync(user.Id);

            return new AuthResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["Jwt:ExpireMinutes"])),
                User = new UserDTO
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Bio = user.Bio,
                    ProfileImageUrl = user.ProfileImageUrl,
                    IsEmailVerified = user.IsEmailVerified,
                    CreatedAt = user.CreatedAt
                }
            };
        }

        public async Task<bool> LogoutAsync(Guid userId)
        {
            await _authRepository.RevokeAllRefreshTokensByUserIdAsync(userId);
            return true;
        }

        public async Task<AuthResponseDTO> RefreshTokenAsync(string refreshToken)
        {
            var token = await _authRepository.GetRefreshTokenAsync(refreshToken);

            if (token == null || !token.IsActive || token.ExpiresAt <= DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Token inválido o expirado");
            }

            var user = token.User ?? throw new InvalidOperationException("Usuario no encontrado");

            // Revocar el token usado
            token.RevokedAt = DateTime.UtcNow;

            // Generar nuevos tokens
            var newAccessToken = GenerateAccessToken(user);
            var newRefreshToken = await GenerateRefreshTokenAsync(user.Id);

            token.ReplacedByToken = newRefreshToken.Token;
            await _authRepository.UpdateRefreshTokenAsync(token);

            return new AuthResponseDTO
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["Jwt:ExpireMinutes"])),
                User = new UserDTO
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Bio = user.Bio,
                    ProfileImageUrl = user.ProfileImageUrl,
                    IsEmailVerified = user.IsEmailVerified,
                    CreatedAt = user.CreatedAt
                }
            };
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            var user = await _authRepository.GetUserByEmailAsync(email);

            if (user == null)
            {
                _logger.LogWarning($"Intento de recuperación de contraseña para email no registrado: {email}");
                return true;
            }

            await _authRepository.InvalidateAllPasswordResetTokensByUserIdAsync(user.Id);

            var resetToken = new PasswordResetToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = GenerateSecureToken(),
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                CreatedAt = DateTime.UtcNow
            };

            await _authRepository.CreatePasswordResetTokenAsync(resetToken);

            _logger.LogInformation($"Token de recuperación generado para usuario {user.Email}: {resetToken.Token}");

            return true;
        }

        public async Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            var resetToken = await _authRepository.GetPasswordResetTokenAsync(token);

            if (resetToken == null || !resetToken.IsValid || resetToken.ExpiresAt <= DateTime.UtcNow)
            {
                throw new InvalidOperationException("Token inválido o expirado");
            }

            var user = resetToken.User ?? throw new InvalidOperationException("Usuario no encontrado");

            user.PasswordHash = HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;
            await _authRepository.UpdateUserAsync(user);

            resetToken.UsedAt = DateTime.UtcNow;
            await _authRepository.UpdatePasswordResetTokenAsync(resetToken);

            await _authRepository.RevokeAllRefreshTokensByUserIdAsync(user.Id);

            return true;
        }

        // Métodos privados auxiliares

        private string GenerateAccessToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["Jwt:ExpireMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<RefreshToken> GenerateRefreshTokenAsync(Guid userId)
        {
            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Token = GenerateSecureToken(),
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            };

            await _authRepository.CreateRefreshTokenAsync(refreshToken);

            return refreshToken;
        }

        private static string GenerateSecureToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        private static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private static bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}

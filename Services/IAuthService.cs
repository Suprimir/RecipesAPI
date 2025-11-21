using RecipesAPI.DTOs;

namespace RecipesAPI.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> RegisterAsync(RegisterRequestDTO request);
        Task<AuthResponseDTO> LoginAsync(LoginRequestDTO request);
        Task<bool> LogoutAsync(Guid userId);
        Task<AuthResponseDTO> RefreshTokenAsync(string refreshToken);
        Task<bool> ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(string token, string newPassword);
    }
}

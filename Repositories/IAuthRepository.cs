using RecipesAPI.Models;

namespace RecipesAPI.Repositories
{
    public interface IAuthRepository
    {
        // User operations
        Task<User?> GetUserByIdAsync(Guid id);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> UsernameExistsAsync(string username);
        Task<User> CreateUserAsync(User user);
        Task<User> UpdateUserAsync(User user);

        // Refresh Token operations
        Task<RefreshToken?> GetRefreshTokenAsync(string token);
        Task<RefreshToken> CreateRefreshTokenAsync(RefreshToken refreshToken);
        Task<RefreshToken> UpdateRefreshTokenAsync(RefreshToken refreshToken);
        Task RevokeAllRefreshTokensByUserIdAsync(Guid userId);

        // Password Reset Token operations
        Task<PasswordResetToken?> GetPasswordResetTokenAsync(string token);
        Task<PasswordResetToken> CreatePasswordResetTokenAsync(PasswordResetToken token);
        Task<PasswordResetToken> UpdatePasswordResetTokenAsync(PasswordResetToken token);
        Task InvalidateAllPasswordResetTokensByUserIdAsync(Guid userId);
    }
}

using RecipesAPI.DTOs;

namespace RecipesAPI.Services
{
    public interface IUserService
    {
        Task<UserProfileDTO?> GetUserProfileAsync(Guid userId);
        Task<UserProfileDTO?> UpdateProfileAsync(Guid userId, UpdateProfileDTO dto);
        Task<bool> DeleteAccountAsync(Guid userId);
        Task<UserStatsDTO?> GetUserStatsAsync(Guid userId);
    }
}

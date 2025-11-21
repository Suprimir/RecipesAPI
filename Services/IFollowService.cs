using RecipesAPI.DTOs;

namespace RecipesAPI.Services
{
    public interface IFollowService
    {
        Task<IEnumerable<FollowUserDTO>> GetFollowersAsync(Guid userId, int page, int limit);
        Task<IEnumerable<FollowUserDTO>> GetFollowingAsync(Guid userId, int page, int limit);
        Task<FollowUserDTO> FollowUserAsync(Guid followerId, Guid followingId);
        Task<bool> UnfollowUserAsync(Guid followerId, Guid followingId);
        Task<bool> IsFollowingAsync(Guid followerId, Guid followingId);
        Task<IEnumerable<RecipeDTO>> GetFeedAsync(Guid userId, int page, int limit);
    }
}

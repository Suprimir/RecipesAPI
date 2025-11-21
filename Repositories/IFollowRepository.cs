using RecipesAPI.Models;

namespace RecipesAPI.Repositories
{
    public interface IFollowRepository
    {
        Task<IEnumerable<Follow>> GetFollowersAsync(Guid userId, int page, int limit);
        Task<IEnumerable<Follow>> GetFollowingAsync(Guid userId, int page, int limit);
        Task<Follow?> GetFollowAsync(Guid followerId, Guid followingId);
        Task<Follow> CreateFollowAsync(Follow follow);
        Task<bool> DeleteFollowAsync(Guid followerId, Guid followingId);
        Task<bool> IsFollowingAsync(Guid followerId, Guid followingId);
        Task<int> GetFollowersCountAsync(Guid userId);
        Task<int> GetFollowingCountAsync(Guid userId);
    }
}

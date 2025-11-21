using RecipesAPI.Models;

namespace RecipesAPI.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByUsernameAsync(string username);
        Task<User> UpdateAsync(User user);
        Task<bool> DeleteAsync(Guid id);
        Task<int> GetRecipesCountAsync(Guid userId);
        Task<int> GetPublicRecipesCountAsync(Guid userId);
        Task<int> GetPrivateRecipesCountAsync(Guid userId);
        Task<int> GetFollowersCountAsync(Guid userId);
        Task<int> GetFollowingCountAsync(Guid userId);
        Task<int> GetTotalFavoritesReceivedAsync(Guid userId);
    }
}

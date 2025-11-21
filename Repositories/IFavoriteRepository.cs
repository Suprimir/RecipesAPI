using RecipesAPI.Models;

namespace RecipesAPI.Repositories
{
    public interface IFavoriteRepository
    {
        Task<IEnumerable<Favorite>> GetUserFavoritesAsync(Guid userId, int page, int limit);
        Task<Favorite?> GetFavoriteAsync(Guid userId, Guid recipeId);
        Task<Favorite> AddFavoriteAsync(Favorite favorite);
        Task<bool> RemoveFavoriteAsync(Guid userId, Guid recipeId);
        Task<bool> IsFavoriteAsync(Guid userId, Guid recipeId);
    }
}

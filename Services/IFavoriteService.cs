using RecipesAPI.DTOs;

namespace RecipesAPI.Services
{
    public interface IFavoriteService
    {
        Task<IEnumerable<FavoriteDTO>> GetUserFavoritesAsync(Guid userId, int page, int limit);
        Task<FavoriteDTO> AddFavoriteAsync(Guid userId, Guid recipeId);
        Task<bool> RemoveFavoriteAsync(Guid userId, Guid recipeId);
        Task<bool> IsFavoriteAsync(Guid userId, Guid recipeId);
    }
}

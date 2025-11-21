using Microsoft.EntityFrameworkCore;
using RecipesAPI.Data;
using RecipesAPI.Models;

namespace RecipesAPI.Repositories
{
    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly RecipesDbContext _context;

        public FavoriteRepository(RecipesDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Favorite>> GetUserFavoritesAsync(Guid userId, int page, int limit)
        {
            return await _context.Favorites
                .Where(f => f.UserId == userId)
                .Include(f => f.Recipe)
                    .ThenInclude(r => r!.User)
                .Include(f => f.Recipe)
                    .ThenInclude(r => r!.Category)
                .OrderByDescending(f => f.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<Favorite?> GetFavoriteAsync(Guid userId, Guid recipeId)
        {
            return await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.RecipeId == recipeId);
        }

        public async Task<Favorite> AddFavoriteAsync(Favorite favorite)
        {
            favorite.CreatedAt = DateTime.UtcNow;
            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();
            return favorite;
        }

        public async Task<bool> RemoveFavoriteAsync(Guid userId, Guid recipeId)
        {
            var favorite = await GetFavoriteAsync(userId, recipeId);
            if (favorite == null) return false;

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsFavoriteAsync(Guid userId, Guid recipeId)
        {
            return await _context.Favorites
                .AnyAsync(f => f.UserId == userId && f.RecipeId == recipeId);
        }
    }
}

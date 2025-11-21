using Microsoft.EntityFrameworkCore;
using RecipesAPI.Data;
using RecipesAPI.Models;

namespace RecipesAPI.Repositories
{
    public class RatingRepository : IRatingRepository
    {
        private readonly RecipesDbContext _context;

        public RatingRepository(RecipesDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Rating>> GetRecipeRatingsAsync(Guid recipeId, int page, int limit, string sort)
        {
            IQueryable<Rating> query = _context.Ratings
                .Where(r => r.RecipeId == recipeId)
                .Include(r => r.User);

            query = sort.ToLower() switch
            {
                "oldest" => query.OrderBy(r => r.CreatedAt),
                "recent" => query.OrderByDescending(r => r.CreatedAt),
                _ => query.OrderByDescending(r => r.CreatedAt)
            };

            return await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<Rating?> GetRatingAsync(Guid id)
        {
            return await _context.Ratings
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Rating?> GetUserRatingForRecipeAsync(Guid userId, Guid recipeId)
        {
            return await _context.Ratings
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.UserId == userId && r.RecipeId == recipeId);
        }

        public async Task<Rating> CreateRatingAsync(Rating rating)
        {
            rating.CreatedAt = DateTime.UtcNow;
            rating.UpdatedAt = DateTime.UtcNow;
            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();

            await _context.Entry(rating).Reference(r => r.User).LoadAsync();
            return rating;
        }

        public async Task<Rating> UpdateRatingAsync(Rating rating)
        {
            rating.UpdatedAt = DateTime.UtcNow;
            _context.Ratings.Update(rating);
            await _context.SaveChangesAsync();

            await _context.Entry(rating).Reference(r => r.User).LoadAsync();
            return rating;
        }

        public async Task<bool> DeleteRatingAsync(Guid id)
        {
            var rating = await _context.Ratings.FindAsync(id);
            if (rating == null) return false;

            _context.Ratings.Remove(rating);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<double> GetRecipeAverageRatingAsync(Guid recipeId)
        {
            var ratings = await _context.Ratings
                .Where(r => r.RecipeId == recipeId)
                .ToListAsync();

            if (!ratings.Any()) return 0;

            return ratings.Average(r => r.RatingValue);
        }

        public async Task<int> GetRecipeRatingsCountAsync(Guid recipeId)
        {
            return await _context.Ratings
                .CountAsync(r => r.RecipeId == recipeId);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using RecipesAPI.Data;
using RecipesAPI.Models;

namespace RecipesAPI.Repositories
{
    public class LikeRepository : ILikeRepository
    {
        private readonly RecipesDbContext _context;

        public LikeRepository(RecipesDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetLikesCountAsync(Guid recipeId)
        {
            return await _context.RecipeLikes.CountAsync(l => l.RecipeId == recipeId);
        }

        public async Task<Dictionary<Guid, int>> GetLikesCountAsync(IEnumerable<Guid> recipeIds)
        {
            var ids = recipeIds.Distinct().ToList();
            return await _context.RecipeLikes
                .Where(l => ids.Contains(l.RecipeId))
                .GroupBy(l => l.RecipeId)
                .Select(g => new { RecipeId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.RecipeId, x => x.Count);
        }

        public async Task<bool> UserLikedAsync(Guid recipeId, Guid userId)
        {
            return await _context.RecipeLikes.AnyAsync(l => l.RecipeId == recipeId && l.UserId == userId);
        }

        public async Task<RecipeLike> AddLikeAsync(RecipeLike like)
        {
            like.CreatedAt = DateTime.UtcNow;
            _context.RecipeLikes.Add(like);
            await _context.SaveChangesAsync();
            await _context.Entry(like).Reference(l => l.User).LoadAsync();
            return like;
        }

        public async Task<bool> RemoveLikeAsync(Guid recipeId, Guid userId)
        {
            var like = await _context.RecipeLikes.FirstOrDefaultAsync(l => l.RecipeId == recipeId && l.UserId == userId);
            if (like == null) return false;

            _context.RecipeLikes.Remove(like);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

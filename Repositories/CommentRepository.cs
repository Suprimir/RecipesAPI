using Microsoft.EntityFrameworkCore;
using RecipesAPI.Data;
using RecipesAPI.Models;

namespace RecipesAPI.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly RecipesDbContext _context;

        public CommentRepository(RecipesDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<RecipeComment> Comments, int Total)> GetCommentsByRecipeAsync(Guid recipeId, int page, int limit, string sort)
        {
            var query = _context.RecipeComments
                .Include(c => c.User)
                .Include(c => c.Replies)
                .Where(c => c.RecipeId == recipeId);

            query = sort.ToLower() switch
            {
                "top" => query.OrderByDescending(c => c.Replies != null ? c.Replies.Count : 0).ThenByDescending(c => c.CreatedAt),
                _ => query.OrderByDescending(c => c.CreatedAt)
            };

            var total = await query.CountAsync();
            var comments = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return (comments, total);
        }

        public async Task<RecipeComment?> GetByIdAsync(Guid id)
        {
            return await _context.RecipeComments
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<RecipeComment?> GetByIdAsync(Guid recipeId, Guid commentId)
        {
            return await _context.RecipeComments
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == commentId && c.RecipeId == recipeId);
        }

        public async Task<RecipeComment?> GetParentAsync(Guid parentId)
        {
            return await _context.RecipeComments.FirstOrDefaultAsync(c => c.Id == parentId);
        }

        public async Task<int> CountByRecipeAsync(Guid recipeId)
        {
            return await _context.RecipeComments.CountAsync(c => c.RecipeId == recipeId);
        }

        public async Task<Dictionary<Guid, int>> CountByRecipesAsync(IEnumerable<Guid> recipeIds)
        {
            var ids = recipeIds.Distinct().ToList();
            return await _context.RecipeComments
                .Where(c => ids.Contains(c.RecipeId))
                .GroupBy(c => c.RecipeId)
                .Select(g => new { RecipeId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.RecipeId, x => x.Count);
        }

        public async Task<RecipeComment> AddAsync(RecipeComment comment)
        {
            comment.CreatedAt = DateTime.UtcNow;
            _context.RecipeComments.Add(comment);
            await _context.SaveChangesAsync();
            await _context.Entry(comment).Reference(c => c.User).LoadAsync();
            return comment;
        }

        public async Task<RecipeComment> UpdateAsync(RecipeComment comment)
        {
            comment.UpdatedAt = DateTime.UtcNow;
            _context.RecipeComments.Update(comment);
            await _context.SaveChangesAsync();
            await _context.Entry(comment).Reference(c => c.User).LoadAsync();
            return comment;
        }

        public async Task<bool> DeleteAsync(Guid commentId)
        {
            var comment = await _context.RecipeComments.FindAsync(commentId);
            if (comment == null) return false;

            _context.RecipeComments.Remove(comment);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

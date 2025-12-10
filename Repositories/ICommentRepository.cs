using RecipesAPI.Models;

namespace RecipesAPI.Repositories
{
    public interface ICommentRepository
    {
        Task<(IEnumerable<RecipeComment> Comments, int Total)> GetCommentsByRecipeAsync(Guid recipeId, int page, int limit, string sort);
        Task<RecipeComment?> GetByIdAsync(Guid id);
        Task<RecipeComment?> GetByIdAsync(Guid recipeId, Guid commentId);
        Task<RecipeComment?> GetParentAsync(Guid parentId);
        Task<int> CountByRecipeAsync(Guid recipeId);
        Task<Dictionary<Guid, int>> CountByRecipesAsync(IEnumerable<Guid> recipeIds);
        Task<RecipeComment> AddAsync(RecipeComment comment);
        Task<RecipeComment> UpdateAsync(RecipeComment comment);
        Task<bool> DeleteAsync(Guid commentId);
    }
}

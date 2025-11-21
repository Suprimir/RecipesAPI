using RecipesAPI.Models;

namespace RecipesAPI.Repositories
{
    public interface IRecipeRepository
    {
        Task<Recipe?> GetByIdAsync(Guid id, bool includeDetails = false);
        Task<IEnumerable<Recipe>> GetAllAsync(int page, int limit, string? sort, bool newest, int? categoryId, string? difficulty, int? maxTime, string? search);
        Task<IEnumerable<Recipe>> GetByUserIdAsync(Guid userId);
        Task<Recipe> CreateAsync(Recipe recipe);
        Task<Recipe> UpdateAsync(Recipe recipe);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task PublishAsync(Guid id);

        // Recipe Steps
        Task<IEnumerable<RecipeStep>> GetStepsByRecipeIdAsync(Guid recipeId);
        Task AddStepsAsync(IEnumerable<RecipeStep> steps);
        Task DeleteStepsByRecipeIdAsync(Guid recipeId);

        // Recipe Ingredients
        Task<IEnumerable<RecipeIngredient>> GetIngredientsByRecipeIdAsync(Guid recipeId);
        Task AddIngredientsAsync(IEnumerable<RecipeIngredient> ingredients);
        Task DeleteIngredientsByRecipeIdAsync(Guid recipeId);

        // Recipe Tags
        Task<IEnumerable<RecipeTag>> GetTagsByRecipeIdAsync(Guid recipeId);
        Task AddTagsAsync(IEnumerable<RecipeTag> tags);
        Task DeleteTagsByRecipeIdAsync(Guid recipeId);
    }
}

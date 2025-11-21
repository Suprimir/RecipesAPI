using RecipesAPI.DTOs;

namespace RecipesAPI.Services
{
    public interface IRecipeService
    {
        Task<RecipeDTO?> GetRecipeByIdAsync(Guid id, bool includeDetails = false);
        Task<IEnumerable<RecipeDTO>> GetAllRecipesAsync(int page, int limit, string? sort, bool newest, int? categoryId, string? difficulty, int? maxTime, string? search);
        Task<IEnumerable<RecipeDTO>> GetRecipesByUserIdAsync(Guid userId);
        Task<RecipeDTO> CreateRecipeAsync(CreateRecipeDTO dto, Guid userId);
        Task<bool> UpdateRecipeAsync(Guid id, UpdateRecipeDTO dto, Guid userId);
        Task<bool> DeleteRecipeAsync(Guid id, Guid userId);
        Task<bool> PublishRecipeAsync(Guid id, Guid userId);
        Task<RecipeStatsDTO?> GetRecipeStatsAsync(Guid id);
    }
}

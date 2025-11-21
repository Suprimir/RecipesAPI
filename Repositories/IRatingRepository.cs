using RecipesAPI.Models;

namespace RecipesAPI.Repositories
{
    public interface IRatingRepository
    {
        Task<IEnumerable<Rating>> GetRecipeRatingsAsync(Guid recipeId, int page, int limit, string sort);
        Task<Rating?> GetRatingAsync(Guid id);
        Task<Rating?> GetUserRatingForRecipeAsync(Guid userId, Guid recipeId);
        Task<Rating> CreateRatingAsync(Rating rating);
        Task<Rating> UpdateRatingAsync(Rating rating);
        Task<bool> DeleteRatingAsync(Guid id);
        Task<double> GetRecipeAverageRatingAsync(Guid recipeId);
        Task<int> GetRecipeRatingsCountAsync(Guid recipeId);
    }
}

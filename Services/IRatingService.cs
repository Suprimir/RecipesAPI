using RecipesAPI.DTOs;

namespace RecipesAPI.Services
{
    public interface IRatingService
    {
        Task<IEnumerable<RatingDTO>> GetRecipeRatingsAsync(Guid recipeId, int page, int limit, string sort);
        Task<RatingDTO> CreateRatingAsync(Guid userId, Guid recipeId, CreateRatingDTO dto);
        Task<RatingDTO> UpdateRatingAsync(Guid userId, Guid recipeId, UpdateRatingDTO dto);
        Task<bool> DeleteRatingAsync(Guid userId, Guid recipeId);
        Task<RatingDTO?> GetMyRatingForRecipeAsync(Guid userId, Guid recipeId);
    }
}

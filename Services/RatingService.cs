using AutoMapper;
using RecipesAPI.DTOs;
using RecipesAPI.Models;
using RecipesAPI.Repositories;

namespace RecipesAPI.Services
{
    public class RatingService : IRatingService
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly IRecipeRepository _recipeRepository;
        private readonly IMapper _mapper;

        public RatingService(
            IRatingRepository ratingRepository,
            IRecipeRepository recipeRepository,
            IMapper mapper)
        {
            _ratingRepository = ratingRepository;
            _recipeRepository = recipeRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RatingDTO>> GetRecipeRatingsAsync(Guid recipeId, int page, int limit, string sort)
        {
            var ratings = await _ratingRepository.GetRecipeRatingsAsync(recipeId, page, limit, sort);
            return ratings.Select(r => new RatingDTO
            {
                Id = r.Id,
                UserId = r.UserId,
                Username = r.User?.Username ?? string.Empty,
                RecipeId = r.RecipeId,
                Rating = r.RatingValue,
                Review = r.Review,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            });
        }

        public async Task<RatingDTO> CreateRatingAsync(Guid userId, Guid recipeId, CreateRatingDTO dto)
        {
            // Verificar que la receta existe
            var recipe = await _recipeRepository.GetByIdAsync(recipeId);
            if (recipe == null)
                throw new Exception("Receta no encontrada");

            // Verificar que el usuario no sea el dueño de la receta
            if (recipe.UserId == userId)
                throw new Exception("No puedes calificar tu propia receta");

            // Verificar que no haya calificado antes
            var existingRating = await _ratingRepository.GetUserRatingForRecipeAsync(userId, recipeId);
            if (existingRating != null)
                throw new Exception("Ya has calificado esta receta");

            var rating = new Rating
            {
                UserId = userId,
                RecipeId = recipeId,
                RatingValue = dto.Rating,
                Review = dto.Review
            };

            var createdRating = await _ratingRepository.CreateRatingAsync(rating);

            return new RatingDTO
            {
                Id = createdRating.Id,
                UserId = createdRating.UserId,
                Username = createdRating.User?.Username ?? string.Empty,
                RecipeId = createdRating.RecipeId,
                Rating = createdRating.RatingValue,
                Review = createdRating.Review,
                CreatedAt = createdRating.CreatedAt,
                UpdatedAt = createdRating.UpdatedAt
            };
        }

        public async Task<RatingDTO> UpdateRatingAsync(Guid userId, Guid recipeId, UpdateRatingDTO dto)
        {
            var rating = await _ratingRepository.GetUserRatingForRecipeAsync(userId, recipeId);
            if (rating == null)
                throw new Exception("No has calificado esta receta");

            rating.RatingValue = dto.Rating;
            rating.Review = dto.Review;

            var updatedRating = await _ratingRepository.UpdateRatingAsync(rating);

            return new RatingDTO
            {
                Id = updatedRating.Id,
                UserId = updatedRating.UserId,
                Username = updatedRating.User?.Username ?? string.Empty,
                RecipeId = updatedRating.RecipeId,
                Rating = updatedRating.RatingValue,
                Review = updatedRating.Review,
                CreatedAt = updatedRating.CreatedAt,
                UpdatedAt = updatedRating.UpdatedAt
            };
        }

        public async Task<bool> DeleteRatingAsync(Guid userId, Guid recipeId)
        {
            var rating = await _ratingRepository.GetUserRatingForRecipeAsync(userId, recipeId);
            if (rating == null)
                return false;

            return await _ratingRepository.DeleteRatingAsync(rating.Id);
        }

        public async Task<RatingDTO?> GetMyRatingForRecipeAsync(Guid userId, Guid recipeId)
        {
            var rating = await _ratingRepository.GetUserRatingForRecipeAsync(userId, recipeId);
            if (rating == null)
                return null;

            return new RatingDTO
            {
                Id = rating.Id,
                UserId = rating.UserId,
                Username = rating.User?.Username ?? string.Empty,
                RecipeId = rating.RecipeId,
                Rating = rating.RatingValue,
                Review = rating.Review,
                CreatedAt = rating.CreatedAt,
                UpdatedAt = rating.UpdatedAt
            };
        }
    }
}

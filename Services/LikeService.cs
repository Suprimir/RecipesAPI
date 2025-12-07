using RecipesAPI.DTOs;
using RecipesAPI.Models;
using RecipesAPI.Repositories;

namespace RecipesAPI.Services
{
    public class LikeService : ILikeService
    {
        private readonly ILikeRepository _likeRepository;
        private readonly IRecipeRepository _recipeRepository;

        public LikeService(ILikeRepository likeRepository, IRecipeRepository recipeRepository)
        {
            _likeRepository = likeRepository;
            _recipeRepository = recipeRepository;
        }

        public async Task<LikeResponseDTO> GetLikesAsync(Guid recipeId, Guid? currentUserId)
        {
            if (!await _recipeRepository.ExistsAsync(recipeId))
                throw new KeyNotFoundException("Receta no encontrada");

            var likesCount = await _likeRepository.GetLikesCountAsync(recipeId);
            var liked = currentUserId.HasValue && await _likeRepository.UserLikedAsync(recipeId, currentUserId.Value);

            return new LikeResponseDTO
            {
                Liked = liked,
                LikesCount = likesCount
            };
        }

        public async Task<LikeResponseDTO> AddLikeAsync(Guid userId, Guid recipeId)
        {
            if (!await _recipeRepository.ExistsAsync(recipeId))
                throw new KeyNotFoundException("Receta no encontrada");

            if (await _likeRepository.UserLikedAsync(recipeId, userId))
                throw new InvalidOperationException("Ya has dado like a esta receta");

            var like = new RecipeLike
            {
                Id = Guid.NewGuid(),
                RecipeId = recipeId,
                UserId = userId
            };

            await _likeRepository.AddLikeAsync(like);

            var likesCount = await _likeRepository.GetLikesCountAsync(recipeId);
            return new LikeResponseDTO { Liked = true, LikesCount = likesCount };
        }

        public async Task<LikeResponseDTO> RemoveLikeAsync(Guid userId, Guid recipeId)
        {
            if (!await _recipeRepository.ExistsAsync(recipeId))
                throw new KeyNotFoundException("Receta no encontrada");

            await _likeRepository.RemoveLikeAsync(recipeId, userId);

            var likesCount = await _likeRepository.GetLikesCountAsync(recipeId);
            return new LikeResponseDTO { Liked = false, LikesCount = likesCount };
        }
    }
}

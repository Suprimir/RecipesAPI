using RecipesAPI.DTOs;

namespace RecipesAPI.Services
{
    public interface ILikeService
    {
        Task<LikeResponseDTO> GetLikesAsync(Guid recipeId, Guid? currentUserId);
        Task<LikeResponseDTO> AddLikeAsync(Guid userId, Guid recipeId);
        Task<LikeResponseDTO> RemoveLikeAsync(Guid userId, Guid recipeId);
    }
}

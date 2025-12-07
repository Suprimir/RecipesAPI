using RecipesAPI.Models;

namespace RecipesAPI.Repositories
{
    public interface ILikeRepository
    {
        Task<int> GetLikesCountAsync(Guid recipeId);
        Task<bool> UserLikedAsync(Guid recipeId, Guid userId);
        Task<RecipeLike> AddLikeAsync(RecipeLike like);
        Task<bool> RemoveLikeAsync(Guid recipeId, Guid userId);
    }
}

using RecipesAPI.DTOs;

namespace RecipesAPI.Services
{
    public interface ICommentService
    {
        Task<(IEnumerable<CommentDTO> Comments, int Total)> GetCommentsAsync(Guid recipeId, int page, int limit, string sort);
        Task<CommentDTO> CreateCommentAsync(Guid userId, Guid recipeId, CreateCommentDTO dto);
        Task<CommentDTO> UpdateCommentAsync(Guid userId, Guid recipeId, Guid commentId, UpdateCommentDTO dto);
        Task<bool> DeleteCommentAsync(Guid userId, Guid recipeId, Guid commentId);
    }
}

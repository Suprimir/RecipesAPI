using AutoMapper;
using RecipesAPI.DTOs;
using RecipesAPI.Models;
using RecipesAPI.Repositories;

namespace RecipesAPI.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IRecipeRepository _recipeRepository;
        private readonly IMapper _mapper;

        public CommentService(
            ICommentRepository commentRepository,
            IRecipeRepository recipeRepository,
            IMapper mapper)
        {
            _commentRepository = commentRepository;
            _recipeRepository = recipeRepository;
            _mapper = mapper;
        }

        public async Task<(IEnumerable<CommentDTO> Comments, int Total)> GetCommentsAsync(Guid recipeId, int page, int limit, string sort)
        {
            if (!await _recipeRepository.ExistsAsync(recipeId))
                throw new KeyNotFoundException("Receta no encontrada");

            var normalizedSort = string.IsNullOrWhiteSpace(sort) ? "recent" : sort.ToLower();
            var (comments, total) = await _commentRepository.GetCommentsByRecipeAsync(recipeId, page, limit, normalizedSort);
            return (comments.Select(_mapper.Map<CommentDTO>), total);
        }

        public async Task<CommentDTO> CreateCommentAsync(Guid userId, Guid recipeId, CreateCommentDTO dto)
        {
            if (!await _recipeRepository.ExistsAsync(recipeId))
                throw new KeyNotFoundException("Receta no encontrada");

            var content = dto.Content?.Trim();
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("El contenido es requerido");

            if (dto.ParentCommentId.HasValue)
            {
                var parent = await _commentRepository.GetParentAsync(dto.ParentCommentId.Value);
                if (parent == null || parent.RecipeId != recipeId)
                    throw new ArgumentException("El comentario padre no existe o no pertenece a la receta");
            }

            var comment = new RecipeComment
            {
                Id = Guid.NewGuid(),
                RecipeId = recipeId,
                UserId = userId,
                Content = content,
                ParentCommentId = dto.ParentCommentId,
                IsEdited = false
            };

            var created = await _commentRepository.AddAsync(comment);
            return _mapper.Map<CommentDTO>(created);
        }

        public async Task<CommentDTO> UpdateCommentAsync(Guid userId, Guid recipeId, Guid commentId, UpdateCommentDTO dto)
        {
            var comment = await _commentRepository.GetByIdAsync(recipeId, commentId);
            if (comment == null)
                throw new KeyNotFoundException("Comentario no encontrado");

            if (comment.UserId != userId)
                throw new UnauthorizedAccessException("No tienes permiso para editar este comentario");

            var content = dto.Content?.Trim();
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("El contenido es requerido");

            comment.Content = content;
            comment.IsEdited = true;

            var updated = await _commentRepository.UpdateAsync(comment);
            return _mapper.Map<CommentDTO>(updated);
        }

        public async Task<bool> DeleteCommentAsync(Guid userId, Guid recipeId, Guid commentId)
        {
            var comment = await _commentRepository.GetByIdAsync(recipeId, commentId);
            if (comment == null)
                throw new KeyNotFoundException("Comentario no encontrado");

            if (comment.UserId != userId)
                throw new UnauthorizedAccessException("No tienes permiso para eliminar este comentario");

            return await _commentRepository.DeleteAsync(commentId);
        }
    }
}

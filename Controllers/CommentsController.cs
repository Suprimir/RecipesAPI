using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipesAPI.DTOs;
using RecipesAPI.Services;
using System.Security.Claims;

namespace RecipesAPI.Controllers
{
    [ApiController]
    [Route("api/recipes/{recipeId:guid}/comments")]
    [Produces("application/json")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly ILogger<CommentsController> _logger;

        public CommentsController(ICommentService commentService, ILogger<CommentsController> logger)
        {
            _commentService = commentService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetComments(
            Guid recipeId,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 20,
            [FromQuery] string sort = "recent")
        {
            try
            {
                var result = await _commentService.GetCommentsAsync(recipeId, page, limit, sort);
                return Ok(new
                {
                    items = result.Comments,
                    page,
                    limit,
                    total = result.Total
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener comentarios");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(CommentDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateComment(Guid recipeId, [FromBody] CreateCommentDTO dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var comment = await _commentService.CreateCommentAsync(Guid.Parse(userId), recipeId, dto);
                return StatusCode(StatusCodes.Status201Created, comment);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear comentario");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPut("{commentId:guid}")]
        [Authorize]
        [ProducesResponseType(typeof(CommentDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateComment(Guid recipeId, Guid commentId, [FromBody] UpdateCommentDTO dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var comment = await _commentService.UpdateCommentAsync(Guid.Parse(userId), recipeId, commentId, dto);
                return Ok(comment);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar comentario {CommentId}", commentId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpDelete("{commentId:guid}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteComment(Guid recipeId, Guid commentId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var deleted = await _commentService.DeleteCommentAsync(Guid.Parse(userId), recipeId, commentId);
                if (!deleted)
                    return NotFound(new { message = "Comentario no encontrado" });

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar comentario {CommentId}", commentId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}

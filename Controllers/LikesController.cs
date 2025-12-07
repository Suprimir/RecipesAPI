using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipesAPI.DTOs;
using RecipesAPI.Services;
using System.Security.Claims;

namespace RecipesAPI.Controllers
{
    [ApiController]
    [Route("api/recipes/{recipeId:guid}/likes")]
    [Produces("application/json")]
    public class LikesController : ControllerBase
    {
        private readonly ILikeService _likeService;
        private readonly ILogger<LikesController> _logger;

        public LikesController(ILikeService likeService, ILogger<LikesController> logger)
        {
            _likeService = likeService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(LikeResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLikes(Guid recipeId)
        {
            try
            {
                Guid? currentUserId = null;
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userId) && Guid.TryParse(userId, out var parsed))
                {
                    currentUserId = parsed;
                }

                var response = await _likeService.GetLikesAsync(recipeId, currentUserId);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener likes");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMyLike(Guid recipeId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var liked = await _likeService.GetLikesAsync(recipeId, Guid.Parse(userId));
                return Ok(new { liked = liked.Liked });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar like");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(LikeResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AddLike(Guid recipeId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var response = await _likeService.AddLikeAsync(Guid.Parse(userId), recipeId);
                return StatusCode(StatusCodes.Status201Created, response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar like");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpDelete]
        [Authorize]
        [ProducesResponseType(typeof(LikeResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveLike(Guid recipeId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var response = await _likeService.RemoveLikeAsync(Guid.Parse(userId), recipeId);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar like");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipesAPI.DTOs;
using RecipesAPI.Services;
using System.Security.Claims;

namespace RecipesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;

        public FavoritesController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        /// <summary>
        /// Obtiene todas las recetas favoritas del usuario autenticado con paginación
        /// </summary>
        [HttpGet("me/favorites")]
        [ProducesResponseType(typeof(IEnumerable<FavoriteDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<FavoriteDTO>>> GetMyFavorites(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 20)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var favorites = await _favoriteService.GetUserFavoritesAsync(Guid.Parse(userId), page, limit);
            return Ok(favorites);
        }

        /// <summary>
        /// Agrega una receta a favoritos del usuario autenticado
        /// </summary>
        [HttpPost("recipes/{recipeId}/favorite")]
        [ProducesResponseType(typeof(FavoriteDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FavoriteDTO>> AddFavorite(Guid recipeId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var favorite = await _favoriteService.AddFavoriteAsync(Guid.Parse(userId), recipeId);
                return CreatedAtAction(nameof(GetMyFavorites), new { }, favorite);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("no encontrada"))
                    return NotFound(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Elimina una receta de favoritos del usuario autenticado
        /// </summary>
        [HttpDelete("recipes/{recipeId}/favorite")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveFavorite(Guid recipeId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var removed = await _favoriteService.RemoveFavoriteAsync(Guid.Parse(userId), recipeId);
            if (!removed)
                return NotFound("Favorito no encontrado");

            return NoContent();
        }

        /// <summary>
        /// Verifica si una receta está en favoritos del usuario autenticado
        /// </summary>
        [HttpGet("recipes/{recipeId}/is-favorited")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> IsFavorite(Guid recipeId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var isFavorite = await _favoriteService.IsFavoriteAsync(Guid.Parse(userId), recipeId);
            return Ok(new { isFavorited = isFavorite });
        }
    }
}

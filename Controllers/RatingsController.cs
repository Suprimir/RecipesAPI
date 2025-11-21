using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipesAPI.DTOs;
using RecipesAPI.Services;
using System.Security.Claims;

namespace RecipesAPI.Controllers
{
    [ApiController]
    [Route("api/recipes")]
    [Produces("application/json")]
    public class RatingsController : ControllerBase
    {
        private readonly IRatingService _ratingService;

        public RatingsController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        /// <summary>
        /// Obtiene todas las calificaciones de una receta con paginación y ordenamiento
        /// </summary>
        [HttpGet("{recipeId}/ratings")]
        [ProducesResponseType(typeof(IEnumerable<RatingDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RatingDTO>>> GetRecipeRatings(
            Guid recipeId,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 20,
            [FromQuery] string sort = "recent")
        {
            var ratings = await _ratingService.GetRecipeRatingsAsync(recipeId, page, limit, sort);
            return Ok(ratings);
        }

        /// <summary>
        /// Crea una nueva calificación para una receta
        /// </summary>
        [HttpPost("{recipeId}/ratings")]
        [Authorize]
        [ProducesResponseType(typeof(RatingDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RatingDTO>> CreateRating(
            Guid recipeId,
            [FromBody] CreateRatingDTO dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var rating = await _ratingService.CreateRatingAsync(Guid.Parse(userId), recipeId, dto);
                return CreatedAtAction(nameof(GetRecipeRatings), new { recipeId }, rating);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("no encontrada"))
                    return NotFound(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Actualiza una calificación existente del usuario autenticado
        /// </summary>
        [HttpPut("{recipeId}/ratings")]
        [Authorize]
        [ProducesResponseType(typeof(RatingDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RatingDTO>> UpdateRating(
            Guid recipeId,
            [FromBody] UpdateRatingDTO dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var rating = await _ratingService.UpdateRatingAsync(Guid.Parse(userId), recipeId, dto);
                return Ok(rating);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("No has calificado"))
                    return NotFound(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Elimina la calificación del usuario autenticado para una receta
        /// </summary>
        [HttpDelete("{recipeId}/ratings")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteRating(Guid recipeId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var deleted = await _ratingService.DeleteRatingAsync(Guid.Parse(userId), recipeId);
            if (!deleted)
                return NotFound("Calificación no encontrada");

            return NoContent();
        }

        /// <summary>
        /// Obtiene la calificación del usuario autenticado para una receta específica
        /// </summary>
        [HttpGet("{recipeId}/ratings/me")]
        [Authorize]
        [ProducesResponseType(typeof(RatingDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RatingDTO>> GetMyRating(Guid recipeId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var rating = await _ratingService.GetMyRatingForRecipeAsync(Guid.Parse(userId), recipeId);
            if (rating == null)
                return NotFound("No has calificado esta receta");

            return Ok(rating);
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipesAPI.DTOs;
using RecipesAPI.Services;
using System.Security.Claims;

namespace RecipesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipesController : ControllerBase
    {
        private readonly IRecipeService _recipeService;
        private readonly ILogger<RecipesController> _logger;

        public RecipesController(IRecipeService recipeService, ILogger<RecipesController> logger)
        {
            _recipeService = recipeService;
            _logger = logger;
        }

        /// <summary>
        /// Listar recetas públicas con filtros y paginación
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RecipeDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<RecipeDTO>>> GetAllRecipes(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 20,
            [FromQuery] string? sort = "newest",
            [FromQuery] int? category = null,
            [FromQuery] string? difficulty = null,
            [FromQuery] int? max_time = null,
            [FromQuery] string? search = null)
        {
            try
            {
                var recipes = await _recipeService.GetAllRecipesAsync(
                    page,
                    limit,
                    sort,
                    sort == "newest",
                    category,
                    difficulty,
                    max_time,
                    search);

                return Ok(recipes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener recetas");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtener receta completa con pasos, ingredientes y tags
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RecipeDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RecipeDTO>> GetRecipeById(
            Guid id,
            [FromQuery] bool include = true)
        {
            try
            {
                var recipe = await _recipeService.GetRecipeByIdAsync(id, include);
                if (recipe == null)
                {
                    return NotFound(new { message = $"Receta con ID {id} no encontrada" });
                }
                return Ok(recipe);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener receta {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Crear nueva receta
        /// </summary>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(RecipeDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RecipeDTO>> CreateRecipe([FromBody] CreateRecipeDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new { message = "Usuario no válido" });
                }

                var createdRecipe = await _recipeService.CreateRecipeAsync(dto, userId);
                return CreatedAtAction(
                    nameof(GetRecipeById),
                    new { id = createdRecipe.Id },
                    createdRecipe);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear receta");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Actualizar receta existente
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateRecipe(Guid id, [FromBody] UpdateRecipeDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new { message = "Usuario no válido" });
                }

                var updated = await _recipeService.UpdateRecipeAsync(id, dto, userId);
                if (!updated)
                {
                    return NotFound(new { message = $"Receta con ID {id} no encontrada o no tienes permisos" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar receta {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Eliminar receta
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteRecipe(Guid id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new { message = "Usuario no válido" });
                }

                var deleted = await _recipeService.DeleteRecipeAsync(id, userId);
                if (!deleted)
                {
                    return NotFound(new { message = $"Receta con ID {id} no encontrada o no tienes permisos" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar receta {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Publicar receta (cambiar is_public a true)
        /// </summary>
        [HttpPatch("{id}/publish")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PublishRecipe(Guid id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new { message = "Usuario no válido" });
                }

                var published = await _recipeService.PublishRecipeAsync(id, userId);
                if (!published)
                {
                    return NotFound(new { message = $"Receta con ID {id} no encontrada o no tienes permisos" });
                }

                return Ok(new { message = "Receta publicada correctamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al publicar receta {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Estadísticas de receta
        /// </summary>
        [HttpGet("{id}/stats")]
        [ProducesResponseType(typeof(RecipeStatsDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RecipeStatsDTO>> GetRecipeStats(Guid id)
        {
            try
            {
                var stats = await _recipeService.GetRecipeStatsAsync(id);
                if (stats == null)
                {
                    return NotFound(new { message = $"Receta con ID {id} no encontrada" });
                }
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas de receta {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}

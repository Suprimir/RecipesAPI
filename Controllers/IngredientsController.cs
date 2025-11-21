using Microsoft.AspNetCore.Mvc;
using RecipesAPI.DTOs;
using RecipesAPI.Models;
using RecipesAPI.Services;

namespace RecipesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IngredientsController : ControllerBase
    {
        private readonly IIngredientService _service;
        private readonly ILogger<IngredientsController> _logger;

        public IngredientsController(
            IIngredientService service,
            ILogger<IngredientsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<IngredientDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<IngredientDTO>>> GetAllIngredients()
        {
            try
            {
                var ingredients = await _service.GetAllIngredientsAsync();
                return Ok(ingredients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener ingredientes");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(IngredientDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IngredientDTO>> GetIngredientById(int id)
        {
            try
            {
                var ingredient = await _service.GetIngredientByIdAsync(id);
                if (ingredient == null)
                {
                    return NotFound(new { message = $"Ingrediente con ID {id} no encontrado" });
                }
                return Ok(ingredient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener ingrediente {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(IngredientDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IngredientDTO>> CreateIngredient([FromBody] CreateIngredientDTO ingredient)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdIngredient = await _service.CreateIngredientAsync(ingredient);
                return CreatedAtAction(
                    nameof(GetIngredientById),
                    new { id = createdIngredient.Id },
                    createdIngredient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear ingrediente");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateIngredient(int id, [FromBody] UpdateIngredientDTO ingredient)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updated = await _service.UpdateIngredientAsync(id, ingredient);
                if (!updated)
                {
                    return NotFound(new { message = $"Ingrediente con ID {id} no encontrado" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar ingrediente {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteIngredient(int id)
        {
            try
            {
                var deleted = await _service.DeleteIngredientAsync(id);
                if (!deleted)
                {
                    return NotFound(new { message = $"Ingrediente con ID {id} no encontrado" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar ingrediente {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}

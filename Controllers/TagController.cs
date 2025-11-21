using Microsoft.AspNetCore.Mvc;
using RecipesAPI.DTOs;
using RecipesAPI.Services;

namespace RecipesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagController : ControllerBase
    {
        private readonly ITagService _service;
        private readonly ILogger<TagController> _logger;

        public TagController(ITagService service, ILogger<TagController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TagDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<TagDTO>>> GetAllTags()
        {
            try
            {
                var tags = await _service.GetAllTagsAsync();
                return Ok(tags);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener etiquetas");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TagDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TagDTO>> GetTagById(int id)
        {
            try
            {
                var tag = await _service.GetTagByIdAsync(id);
                if (tag == null)
                {
                    return NotFound(new { message = $"Etiqueta con ID {id} no encontrada" });
                }
                return Ok(tag);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener etiqueta {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(TagDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TagDTO>> CreateTag([FromBody] CreateTagDTO tag)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdTag = await _service.CreateTagAsync(tag);
                return CreatedAtAction(
                    nameof(GetTagById),
                    new { id = createdTag.Id },
                    createdTag);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear etiqueta");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateTag(int id, [FromBody] UpdateTagDTO tag)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var exists = await _service.TagExistsAsync(id);
                if (!exists)
                {
                    return NotFound(new { message = $"Etiqueta con ID {id} no encontrada" });
                }

                var updated = await _service.UpdateTagAsync(id, tag);
                if (updated)
                {
                    return NoContent();
                }
                else
                {
                    return StatusCode(500, new { message = "Error al actualizar la etiqueta" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar etiqueta {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteTag(int id)
        {
            try
            {
                var exists = await _service.TagExistsAsync(id);
                if (!exists)
                {
                    return NotFound(new { message = $"Etiqueta con ID {id} no encontrada" });
                }

                var deleted = await _service.DeleteTagAsync(id);
                if (deleted)
                {
                    return NoContent();
                }
                else
                {
                    return StatusCode(500, new { message = "Error al eliminar la etiqueta" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar etiqueta {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

    }
}
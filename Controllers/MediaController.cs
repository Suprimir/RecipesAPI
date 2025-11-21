using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipesAPI.DTOs;
using RecipesAPI.Services;

namespace RecipesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class MediaController : ControllerBase
    {
        private readonly IFileStorageService _fileStorageService;

        public MediaController(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }

        /// <summary>
        /// Sube una imagen al servidor
        /// </summary>
        /// <param name="request">Datos del archivo a subir</param>
        /// <returns>URL de la imagen subida</returns>
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequestSizeLimit(5 * 1024 * 1024)] // 5MB
        public async Task<ActionResult> UploadImage([FromForm] UploadImageRequest request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest(new { message = "No se proporcionó ningún archivo" });

            if (!_fileStorageService.IsValidImageFile(request.File))
                return BadRequest(new { message = "Archivo inválido. Solo se permiten imágenes JPG, JPEG, PNG, GIF, WEBP de máximo 5MB" });

            // Validar tipo
            var validTypes = new[] { "recipe", "profile", "step" };
            if (!validTypes.Contains(request.Type.ToLower()))
                return BadRequest(new { message = "Tipo inválido. Usa 'recipe', 'profile' o 'step'" });

            try
            {
                var imageUrl = await _fileStorageService.SaveImageAsync(request.File, request.Type.ToLower());
                return Ok(new
                {
                    url = imageUrl,
                    type = request.Type.ToLower(),
                    fileName = Path.GetFileName(imageUrl)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina una imagen del servidor
        /// </summary>
        /// <param name="mediaId">ID/nombre del archivo o URL completa de la imagen</param>
        [HttpDelete("{mediaId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteImage(string mediaId)
        {
            if (string.IsNullOrEmpty(mediaId))
                return BadRequest(new { message = "mediaId no puede estar vacío" });

            // Si mediaId contiene '/', asumimos que es una URL completa o ruta
            // Si no, construimos la ruta con diferentes carpetas posibles
            var imageUrl = mediaId;

            if (!mediaId.Contains('/') && !mediaId.Contains('\\'))
            {
                // Intentar eliminar de las carpetas conocidas
                var folders = new[] { "recipe", "profile", "step" };
                foreach (var folder in folders)
                {
                    imageUrl = $"/uploads/{folder}/{mediaId}";
                    var deleted = await _fileStorageService.DeleteImageAsync(imageUrl);
                    if (deleted)
                        return NoContent();
                }

                return NotFound(new { message = "Imagen no encontrada" });
            }

            var result = await _fileStorageService.DeleteImageAsync(imageUrl);
            if (!result)
                return NotFound(new { message = "Imagen no encontrada" });

            return NoContent();
        }
    }
}

namespace RecipesAPI.DTOs
{
    /// <summary>
    /// DTO para la subida de imágenes
    /// </summary>
    public class UploadImageRequest
    {
        /// <summary>
        /// Archivo de imagen
        /// </summary>
        public IFormFile File { get; set; } = null!;

        /// <summary>
        /// Tipo de imagen: 'recipe', 'profile' o 'step'
        /// </summary>
        public string Type { get; set; } = "recipe";
    }
}

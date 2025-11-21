using System.ComponentModel.DataAnnotations;

namespace RecipesAPI.DTOs
{
    public class UpdateProfileDTO
    {
        [StringLength(500, ErrorMessage = "La biografía no puede exceder 500 caracteres")]
        public string? Bio { get; set; }

        [Url(ErrorMessage = "La URL de la imagen de perfil no es válida")]
        [StringLength(500, ErrorMessage = "La URL no puede exceder 500 caracteres")]
        public string? ProfileImageUrl { get; set; }
    }
}

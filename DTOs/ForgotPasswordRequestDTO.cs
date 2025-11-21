using System.ComponentModel.DataAnnotations;

namespace RecipesAPI.DTOs
{
    public class ForgotPasswordRequestDTO
    {
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        public string Email { get; set; } = string.Empty;
    }
}

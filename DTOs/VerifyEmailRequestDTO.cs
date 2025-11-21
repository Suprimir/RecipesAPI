using System.ComponentModel.DataAnnotations;

namespace RecipesAPI.DTOs
{
    public class VerifyEmailRequestDTO
    {
        [Required(ErrorMessage = "El token es requerido")]
        public string Token { get; set; } = string.Empty;
    }
}

using System.ComponentModel.DataAnnotations;

namespace RecipesAPI.DTOs
{
    public class RefreshTokenRequestDTO
    {
        [Required(ErrorMessage = "El refresh token es requerido")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}

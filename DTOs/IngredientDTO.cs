using System.ComponentModel.DataAnnotations;

namespace RecipesAPI.DTOs
{
    public class CreateIngredientDTO
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [MaxLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50, ErrorMessage = "La categoría no puede exceder 50 caracteres")]
        public string? Category { get; set; }
    }

    public class UpdateIngredientDTO
    {
        [MaxLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50, ErrorMessage = "La categoría no puede exceder 50 caracteres")]
        public string? Category { get; set; }
    }

    public class IngredientDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Category { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

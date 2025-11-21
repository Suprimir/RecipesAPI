using System.ComponentModel.DataAnnotations;

namespace RecipesAPI.DTOs
{
    public class CreateRecipeDTO
    {
        public int? CategoryId { get; set; }

        [Required(ErrorMessage = "El título es requerido")]
        [StringLength(200, ErrorMessage = "El título no puede exceder 200 caracteres")]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }
        public string? CoverImageUrl { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El tiempo de preparación debe ser positivo")]
        public int? PrepTimeMinutes { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El tiempo de cocción debe ser positivo")]
        public int? CookTimeMinutes { get; set; }

        [RegularExpression("^(easy|medium|hard)$", ErrorMessage = "La dificultad debe ser: easy, medium o hard")]
        public string DifficultyLevel { get; set; } = "easy";

        public bool IsPublic { get; set; } = false;

        public List<CreateRecipeStepDTO>? Steps { get; set; }
        public List<CreateRecipeIngredientDTO>? Ingredients { get; set; }
        public List<int>? TagIds { get; set; }
    }
}

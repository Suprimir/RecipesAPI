using System.ComponentModel.DataAnnotations;

namespace RecipesAPI.DTOs
{
    public class CreateRecipeStepDTO
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "El número de paso debe ser mayor a 0")]
        public int StepNumber { get; set; }

        [Required]
        public string StepType { get; set; } = "preparation";

        public string? Title { get; set; }

        [Required(ErrorMessage = "La descripción del paso es requerida")]
        public string Description { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "La duración debe ser positiva")]
        public int? DurationMinutes { get; set; }
    }
}

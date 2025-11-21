using System.ComponentModel.DataAnnotations;

namespace RecipesAPI.DTOs
{
    public class CreateRecipeIngredientDTO
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del ingrediente es requerido")]
        public int IngredientId { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public decimal? Quantity { get; set; }

        [StringLength(50, ErrorMessage = "La unidad no puede exceder 50 caracteres")]
        public string? Unit { get; set; }

        public string? Notes { get; set; }
    }
}

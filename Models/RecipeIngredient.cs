namespace RecipesAPI.Models
{
    public class RecipeIngredient
    {
        public Guid Id { get; set; }
        public Guid RecipeId { get; set; }
        public int IngredientId { get; set; }

        public decimal? Quantity { get; set; }
        public string? Unit { get; set; }
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public Recipe? Recipe { get; set; }
        public Ingredient? Ingredient { get; set; }
    }
}

namespace RecipesAPI.DTOs
{
    public class RecipeIngredientDTO
    {
        public Guid Id { get; set; }
        public int IngredientId { get; set; }
        public string IngredientName { get; set; } = string.Empty;
        public decimal? Quantity { get; set; }
        public string? Unit { get; set; }
        public string? Notes { get; set; }
    }
}

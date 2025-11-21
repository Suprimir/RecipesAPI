namespace RecipesAPI.Models
{
    public class RecipeStep
    {
        public Guid Id { get; set; }
        public Guid RecipeId { get; set; }

        public int StepNumber { get; set; }
        public string StepType { get; set; } = "preparation";
        public string? Title { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int? DurationMinutes { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation property
        public Recipe? Recipe { get; set; }
    }
}

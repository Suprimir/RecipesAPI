namespace RecipesAPI.Models
{
    public class Recipe
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int? CategoryId { get; set; }

        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? CoverImageUrl { get; set; }

        public int? PrepTimeMinutes { get; set; }
        public int? CookTimeMinutes { get; set; }
        public int? TotalTimeMinutes { get; set; }

        public string DifficultyLevel { get; set; } = "easy";
        public bool IsPublic { get; set; }
        public int FavoritesCount { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }

        // Navigation properties
        public User? User { get; set; }
        public Category? Category { get; set; }
        public ICollection<RecipeStep>? Steps { get; set; }
        public ICollection<RecipeIngredient>? RecipeIngredients { get; set; }
        public ICollection<RecipeTag>? RecipeTags { get; set; }
    }
}

namespace RecipesAPI.DTOs
{
    public class RecipeDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }

        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? CoverImageUrl { get; set; }

        public int? PrepTimeMinutes { get; set; }
        public int? CookTimeMinutes { get; set; }
        public int? TotalTimeMinutes { get; set; }

        public string DifficultyLevel { get; set; } = "easy";
        public bool IsPublic { get; set; }
        public int FavoritesCount { get; set; }
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }

        public List<RecipeStepDTO>? Steps { get; set; }
        public List<RecipeIngredientDTO>? Ingredients { get; set; }
        public List<TagDTO>? Tags { get; set; }
    }
}

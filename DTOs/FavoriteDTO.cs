namespace RecipesAPI.DTOs
{
    /// <summary>
    /// DTO para representar un favorito con información de la receta
    /// </summary>
    public class FavoriteDTO
    {
        public Guid Id { get; set; }
        public Guid RecipeId { get; set; }
        public string RecipeTitle { get; set; } = string.Empty;
        public string? RecipeDescription { get; set; }
        public string? CoverImageUrl { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? CategoryName { get; set; }
        public string DifficultyLevel { get; set; } = string.Empty;
        public int? TotalTimeMinutes { get; set; }
        public int FavoritesCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

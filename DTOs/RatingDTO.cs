namespace RecipesAPI.DTOs
{
    /// <summary>
    /// DTO para representar una calificación
    /// </summary>
    public class RatingDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public Guid RecipeId { get; set; }
        public int Rating { get; set; }
        public string? Review { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

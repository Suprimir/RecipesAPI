namespace RecipesAPI.Models
{
    public class Rating
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid RecipeId { get; set; }
        public int RatingValue { get; set; }
        public string? Review { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public User? User { get; set; }
        public Recipe? Recipe { get; set; }
    }
}

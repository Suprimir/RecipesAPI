namespace RecipesAPI.Models
{
    public class Favorite
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid RecipeId { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public User? User { get; set; }
        public Recipe? Recipe { get; set; }
    }
}

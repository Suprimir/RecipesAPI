namespace RecipesAPI.Models
{
    public class RecipeLike
    {
        public Guid Id { get; set; }
        public Guid RecipeId { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public Recipe? Recipe { get; set; }
        public User? User { get; set; }
    }
}

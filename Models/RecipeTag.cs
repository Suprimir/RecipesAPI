namespace RecipesAPI.Models
{
    public class RecipeTag
    {
        public Guid RecipeId { get; set; }
        public int TagId { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public Recipe? Recipe { get; set; }
        public Tag? Tag { get; set; }
    }
}

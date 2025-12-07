namespace RecipesAPI.Models
{
    public class RecipeComment
    {
        public Guid Id { get; set; }
        public Guid RecipeId { get; set; }
        public Guid UserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? ParentCommentId { get; set; }
        public bool IsEdited { get; set; }

        // Navigation properties
        public Recipe? Recipe { get; set; }
        public User? User { get; set; }
        public RecipeComment? ParentComment { get; set; }
        public ICollection<RecipeComment>? Replies { get; set; }
    }
}

namespace RecipesAPI.DTOs
{
    public class CommentDTO
    {
        public Guid Id { get; set; }
        public Guid RecipeId { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsEdited { get; set; }
        public Guid? ParentCommentId { get; set; }
        public int RepliesCount { get; set; }
    }
}

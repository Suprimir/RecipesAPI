namespace RecipesAPI.DTOs
{
    /// <summary>
    /// DTO para representar información básica de un seguidor o seguido
    /// </summary>
    public class FollowUserDTO
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? ProfileImageUrl { get; set; }
        public DateTime FollowedAt { get; set; }
    }
}

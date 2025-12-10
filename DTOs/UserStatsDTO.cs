namespace RecipesAPI.DTOs
{
    public class UserStatsDTO
    {
        public Guid UserId { get; set; }
        public int RecipesCount { get; set; }
        public int PublicRecipesCount { get; set; }
        public int PrivateRecipesCount { get; set; }
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
        public int TotalFavoritesReceived { get; set; }
    }
}

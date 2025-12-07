namespace RecipesAPI.DTOs
{
    public class RecipeStatsDTO
    {
        public Guid RecipeId { get; set; }
        public int ViewsCount { get; set; }
        public int FavoritesCount { get; set; }
        public double AverageRating { get; set; }
        public int RatingsCount { get; set; }
        public int CommentsCount { get; set; }
        public int LikesCount { get; set; }
    }
}

namespace RecipesAPI.DTOs
{
    public class RecipeStepDTO
    {
        public Guid Id { get; set; }
        public int StepNumber { get; set; }
        public string StepType { get; set; } = "preparation";
        public string? Title { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int? DurationMinutes { get; set; }
    }
}

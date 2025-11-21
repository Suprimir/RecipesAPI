namespace RecipesAPI.Services
{
    public interface IFileStorageService
    {
        Task<string> SaveImageAsync(IFormFile file, string folder);
        Task<bool> DeleteImageAsync(string imageUrl);
        string GetImageUrl(string fileName, string folder);
        bool IsValidImageFile(IFormFile file);
    }
}

using RecipesAPI.DTOs;

namespace RecipesAPI.Services
{
    public interface ITagService
    {
        Task<IEnumerable<TagDTO>> GetAllTagsAsync();
        Task<TagDTO?> GetTagByIdAsync(int id);
        Task<TagDTO> CreateTagAsync(CreateTagDTO createDto);
        Task<bool> UpdateTagAsync(int id, UpdateTagDTO updateDto);
        Task<bool> DeleteTagAsync(int id);
        Task<bool> TagExistsAsync(int id);
    }
}
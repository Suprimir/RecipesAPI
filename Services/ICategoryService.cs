using RecipesAPI.DTOs;
using RecipesAPI.Models;

namespace RecipesAPI.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync();
        Task<CategoryDTO?> GetCategoryByIdAsync(int id);
        Task<CategoryDTO> CreateCategoryAsync(CreateCategoryDTO createDto);
        Task<bool> UpdateCategoryAsync(int id, UpdateCategoryDTO updateDto);
        Task<bool> DeleteCategoryAsync(int id);
    }
}
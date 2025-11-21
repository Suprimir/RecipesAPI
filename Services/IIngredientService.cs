using RecipesAPI.DTOs;

namespace RecipesAPI.Services
{
    public interface IIngredientService
    {
        Task<IEnumerable<IngredientDTO>> GetAllIngredientsAsync();
        Task<IngredientDTO?> GetIngredientByIdAsync(int id);
        Task<IngredientDTO> CreateIngredientAsync(CreateIngredientDTO createIngredientDTO);
        Task<bool> UpdateIngredientAsync(int id, UpdateIngredientDTO updateIngredientDTO);
        Task<bool> DeleteIngredientAsync(int id);
    }
}

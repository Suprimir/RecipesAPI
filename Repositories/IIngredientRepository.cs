using RecipesAPI.Models;

namespace RecipesAPI.Repositories
{
    public interface IIngredientRepository
    {
        Task<IEnumerable<Ingredient>> GetAllAsync();
        Task<Ingredient?> GetByIdAsync(int id);
        Task<Ingredient> AddAsync(Ingredient ingredient);
        Task<Ingredient> UpdateAsync(Ingredient ingredient);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}

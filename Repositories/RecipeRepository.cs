using Microsoft.EntityFrameworkCore;
using RecipesAPI.Data;
using RecipesAPI.Models;

namespace RecipesAPI.Repositories
{
    public class RecipeRepository : IRecipeRepository
    {
        private readonly RecipesDbContext _context;

        public RecipeRepository(RecipesDbContext context)
        {
            _context = context;
        }

        public async Task<Recipe?> GetByIdAsync(Guid id, bool includeDetails = false)
        {
            var query = _context.Recipes.AsQueryable();

            if (includeDetails)
            {
                query = query
                    .Include(r => r.User)
                    .Include(r => r.Category)
                    .Include(r => r.Steps!.OrderBy(s => s.StepNumber))
                    .Include(r => r.RecipeIngredients!)
                        .ThenInclude(ri => ri.Ingredient)
                    .Include(r => r.RecipeTags!)
                        .ThenInclude(rt => rt.Tag);
            }

            return await query.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Recipe>> GetAllAsync(
            int page,
            int limit,
            string? sort,
            bool newest,
            int? categoryId,
            string? difficulty,
            int? maxTime,
            string? search)
        {
            var query = _context.Recipes
                .Include(r => r.User)
                .Include(r => r.Category)
                .Where(r => r.IsPublic);

            if (categoryId.HasValue)
            {
                query = query.Where(r => r.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrEmpty(difficulty))
            {
                query = query.Where(r => r.DifficultyLevel == difficulty);
            }

            if (maxTime.HasValue)
            {
                query = query.Where(r => r.TotalTimeMinutes <= maxTime.Value);
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(r => EF.Functions.ILike(r.Title, $"%{search}%") ||
                                        EF.Functions.ILike(r.Description ?? "", $"%{search}%"));
            }

            query = sort?.ToLower() switch
            {
                "newest" => query.OrderByDescending(r => r.CreatedAt),
                "oldest" => query.OrderBy(r => r.CreatedAt),
                "favorites" => query.OrderByDescending(r => r.FavoritesCount),
                _ => newest ? query.OrderByDescending(r => r.CreatedAt) : query.OrderBy(r => r.CreatedAt)
            };

            return await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<Recipe>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Recipes
                .Include(r => r.Category)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<Recipe> CreateAsync(Recipe recipe)
        {
            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();
            return recipe;
        }

        public async Task<Recipe> UpdateAsync(Recipe recipe)
        {
            recipe.UpdatedAt = DateTime.UtcNow;
            _context.Recipes.Update(recipe);
            await _context.SaveChangesAsync();
            return recipe;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null) return false;

            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Recipes.AnyAsync(r => r.Id == id);
        }

        public async Task PublishAsync(Guid id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe != null)
            {
                recipe.IsPublic = true;
                recipe.PublishedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<RecipeStep>> GetStepsByRecipeIdAsync(Guid recipeId)
        {
            return await _context.RecipeSteps
                .Where(s => s.RecipeId == recipeId)
                .OrderBy(s => s.StepNumber)
                .ToListAsync();
        }

        public async Task AddStepsAsync(IEnumerable<RecipeStep> steps)
        {
            _context.RecipeSteps.AddRange(steps);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteStepsByRecipeIdAsync(Guid recipeId)
        {
            var steps = await _context.RecipeSteps
                .Where(s => s.RecipeId == recipeId)
                .ToListAsync();

            _context.RecipeSteps.RemoveRange(steps);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<RecipeIngredient>> GetIngredientsByRecipeIdAsync(Guid recipeId)
        {
            return await _context.RecipeIngredients
                .Include(ri => ri.Ingredient)
                .Where(ri => ri.RecipeId == recipeId)
                .ToListAsync();
        }

        public async Task AddIngredientsAsync(IEnumerable<RecipeIngredient> ingredients)
        {
            _context.RecipeIngredients.AddRange(ingredients);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteIngredientsByRecipeIdAsync(Guid recipeId)
        {
            var ingredients = await _context.RecipeIngredients
                .Where(ri => ri.RecipeId == recipeId)
                .ToListAsync();

            _context.RecipeIngredients.RemoveRange(ingredients);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<RecipeTag>> GetTagsByRecipeIdAsync(Guid recipeId)
        {
            return await _context.RecipeTags
                .Include(rt => rt.Tag)
                .Where(rt => rt.RecipeId == recipeId)
                .ToListAsync();
        }

        public async Task AddTagsAsync(IEnumerable<RecipeTag> tags)
        {
            _context.RecipeTags.AddRange(tags);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTagsByRecipeIdAsync(Guid recipeId)
        {
            var tags = await _context.RecipeTags
                .Where(rt => rt.RecipeId == recipeId)
                .ToListAsync();

            _context.RecipeTags.RemoveRange(tags);
            await _context.SaveChangesAsync();
        }
    }
}

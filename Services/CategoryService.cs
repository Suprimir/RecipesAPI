using AutoMapper;
using RecipesAPI.DTOs;
using RecipesAPI.Models;
using RecipesAPI.Repositories;

namespace RecipesAPI.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(ICategoryRepository repository, IMapper mapper, ILogger<CategoryService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync()
        {
            _logger.LogInformation("Obteniendo todas las categorías.");
            var categories = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoryDTO>>(categories);
        }

        public async Task<CategoryDTO?> GetCategoryByIdAsync(int id)
        {
            _logger.LogInformation("Obteniendo categoría con ID: {CategoryId}", id);
            var category = await _repository.GetByIdAsync(id);
            return _mapper.Map<CategoryDTO>(category);
        }

        public async Task<CategoryDTO> CreateCategoryAsync(CreateCategoryDTO createDto)
        {
            _logger.LogInformation("Creando una nueva categoría.");

            var category = _mapper.Map<Category>(createDto);

            var createdCategory = await _repository.AddAsync(category);

            return _mapper.Map<CategoryDTO>(createdCategory);
        }

        public async Task<bool> UpdateCategoryAsync(int id, UpdateCategoryDTO updateDto)
        {
            _logger.LogInformation("Actualizando categoría con ID: {CategoryId}", id);

            var category = await _repository.GetByIdAsync(id);
            if (category == null)
            {
                return false;
            }

            _mapper.Map(updateDto, category);
            category.Id = id;

            await _repository.UpdateAsync(category);
            return true;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            _logger.LogInformation("Eliminando categoría con ID: {CategoryId}", id);
            if (!await _repository.ExistsAsync(id))
            {
                return false;
            }
            return await _repository.DeleteAsync(id);
        }
    }
}
using AutoMapper;
using RecipesAPI.DTOs;
using RecipesAPI.Models;
using RecipesAPI.Repositories;

namespace RecipesAPI.Services
{
    public class IngredientService : IIngredientService
    {
        private readonly IIngredientRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<IngredientService> _logger;

        public IngredientService(
            IIngredientRepository repository,
            IMapper mapper,
            ILogger<IngredientService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<IngredientDTO>> GetAllIngredientsAsync()
        {
            _logger.LogInformation("Obteniendo todos los ingredientes");
            var ingredients = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<IngredientDTO>>(ingredients);
        }

        public async Task<IngredientDTO?> GetIngredientByIdAsync(int id)
        {
            _logger.LogInformation("Obteniendo ingrediente con ID: {Id}", id);
            var ingredient = await _repository.GetByIdAsync(id);
            return ingredient != null ? _mapper.Map<IngredientDTO>(ingredient) : null;
        }

        public async Task<IngredientDTO> CreateIngredientAsync(CreateIngredientDTO createDto)
        {
            _logger.LogInformation("Creando nuevo ingrediente: {Name}", createDto.Name);

            var ingredient = _mapper.Map<Ingredient>(createDto);

            var createdIngredient = await _repository.AddAsync(ingredient);

            return _mapper.Map<IngredientDTO>(createdIngredient);
        }

        public async Task<bool> UpdateIngredientAsync(int id, UpdateIngredientDTO updateDto)
        {
            _logger.LogInformation("Actualizando ingrediente con ID: {Id}", id);

            var ingredient = await _repository.GetByIdAsync(id);
            if (ingredient == null)
            {
                return false;
            }

            _mapper.Map(updateDto, ingredient);
            ingredient.Id = id;

            await _repository.UpdateAsync(ingredient);
            return true;
        }

        public async Task<bool> DeleteIngredientAsync(int id)
        {
            _logger.LogInformation("Eliminando ingrediente con ID: {Id}", id);
            return await _repository.DeleteAsync(id);
        }
    }
}

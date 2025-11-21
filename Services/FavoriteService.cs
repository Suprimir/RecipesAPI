using AutoMapper;
using RecipesAPI.DTOs;
using RecipesAPI.Models;
using RecipesAPI.Repositories;

namespace RecipesAPI.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IFavoriteRepository _favoriteRepository;
        private readonly IRecipeRepository _recipeRepository;
        private readonly IMapper _mapper;

        public FavoriteService(
            IFavoriteRepository favoriteRepository,
            IRecipeRepository recipeRepository,
            IMapper mapper)
        {
            _favoriteRepository = favoriteRepository;
            _recipeRepository = recipeRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<FavoriteDTO>> GetUserFavoritesAsync(Guid userId, int page, int limit)
        {
            var favorites = await _favoriteRepository.GetUserFavoritesAsync(userId, page, limit);

            var favoriteDTOs = favorites.Select(f => new FavoriteDTO
            {
                Id = f.Id,
                RecipeId = f.RecipeId,
                RecipeTitle = f.Recipe?.Title ?? string.Empty,
                RecipeDescription = f.Recipe?.Description,
                CoverImageUrl = f.Recipe?.CoverImageUrl,
                Username = f.Recipe?.User?.Username ?? string.Empty,
                CategoryName = f.Recipe?.Category?.Name,
                DifficultyLevel = f.Recipe?.DifficultyLevel ?? string.Empty,
                TotalTimeMinutes = f.Recipe?.TotalTimeMinutes,
                FavoritesCount = f.Recipe?.FavoritesCount ?? 0,
                CreatedAt = f.CreatedAt
            });

            return favoriteDTOs;
        }

        public async Task<FavoriteDTO> AddFavoriteAsync(Guid userId, Guid recipeId)
        {
            // Verificar que la receta existe
            var recipe = await _recipeRepository.GetByIdAsync(recipeId);
            if (recipe == null)
                throw new Exception("Receta no encontrada");

            // Verificar que no esté ya en favoritos
            var existingFavorite = await _favoriteRepository.GetFavoriteAsync(userId, recipeId);
            if (existingFavorite != null)
                throw new Exception("La receta ya está en favoritos");

            var favorite = new Favorite
            {
                UserId = userId,
                RecipeId = recipeId
            };

            var createdFavorite = await _favoriteRepository.AddFavoriteAsync(favorite);

            // Obtener el favorito completo con la receta
            var favorites = await _favoriteRepository.GetUserFavoritesAsync(userId, 1, 1000);
            var fullFavorite = favorites.FirstOrDefault(f => f.Id == createdFavorite.Id);

            return new FavoriteDTO
            {
                Id = fullFavorite!.Id,
                RecipeId = fullFavorite.RecipeId,
                RecipeTitle = fullFavorite.Recipe?.Title ?? string.Empty,
                RecipeDescription = fullFavorite.Recipe?.Description,
                CoverImageUrl = fullFavorite.Recipe?.CoverImageUrl,
                Username = fullFavorite.Recipe?.User?.Username ?? string.Empty,
                CategoryName = fullFavorite.Recipe?.Category?.Name,
                DifficultyLevel = fullFavorite.Recipe?.DifficultyLevel ?? string.Empty,
                TotalTimeMinutes = fullFavorite.Recipe?.TotalTimeMinutes,
                FavoritesCount = fullFavorite.Recipe?.FavoritesCount ?? 0,
                CreatedAt = fullFavorite.CreatedAt
            };
        }

        public async Task<bool> RemoveFavoriteAsync(Guid userId, Guid recipeId)
        {
            return await _favoriteRepository.RemoveFavoriteAsync(userId, recipeId);
        }

        public async Task<bool> IsFavoriteAsync(Guid userId, Guid recipeId)
        {
            return await _favoriteRepository.IsFavoriteAsync(userId, recipeId);
        }
    }
}

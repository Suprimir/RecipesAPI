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
        private readonly ILikeRepository _likeRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IMapper _mapper;

        public FavoriteService(
            IFavoriteRepository favoriteRepository,
            IRecipeRepository recipeRepository,
            ILikeRepository likeRepository,
            ICommentRepository commentRepository,
            IMapper mapper)
        {
            _favoriteRepository = favoriteRepository;
            _recipeRepository = recipeRepository;
            _likeRepository = likeRepository;
            _commentRepository = commentRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<FavoriteDTO>> GetUserFavoritesAsync(Guid userId, int page, int limit)
        {
            var favorites = (await _favoriteRepository.GetUserFavoritesAsync(userId, page, limit)).ToList();

            if (!favorites.Any())
            {
                return Enumerable.Empty<FavoriteDTO>();
            }

            var recipeIds = favorites.Select(f => f.RecipeId).Distinct().ToList();

            // Serializamos consultas para evitar concurrencia sobre el mismo DbContext
            var likes = await _likeRepository.GetLikesCountAsync(recipeIds);
            var comments = await _commentRepository.CountByRecipesAsync(recipeIds);

            return favorites.Select(f => new FavoriteDTO
            {
                Id = f.Id,
                UserId = f.Recipe?.UserId ?? Guid.Empty,
                RecipeId = f.RecipeId,
                RecipeTitle = f.Recipe?.Title ?? string.Empty,
                RecipeDescription = f.Recipe?.Description,
                CoverImageUrl = f.Recipe?.CoverImageUrl,
                Username = f.Recipe?.User?.Username ?? string.Empty,
                CategoryId = f.Recipe?.CategoryId,
                CategoryName = f.Recipe?.Category?.Name,
                DifficultyLevel = f.Recipe?.DifficultyLevel ?? string.Empty,
                TotalTimeMinutes = f.Recipe?.TotalTimeMinutes,
                FavoritesCount = f.Recipe?.FavoritesCount ?? 0,
                LikesCount = likes.TryGetValue(f.RecipeId, out var l) ? l : 0,
                CommentsCount = comments.TryGetValue(f.RecipeId, out var c) ? c : 0,
                CreatedAt = f.CreatedAt,
                PublishedAt = f.Recipe?.PublishedAt
            });
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

            var likesCount = await _likeRepository.GetLikesCountAsync(recipeId);
            var commentsCount = await _commentRepository.CountByRecipeAsync(recipeId);

            return new FavoriteDTO
            {
                Id = fullFavorite!.Id,
                UserId = fullFavorite.Recipe?.UserId ?? Guid.Empty,
                RecipeId = fullFavorite.RecipeId,
                RecipeTitle = fullFavorite.Recipe?.Title ?? string.Empty,
                RecipeDescription = fullFavorite.Recipe?.Description,
                CoverImageUrl = fullFavorite.Recipe?.CoverImageUrl,
                Username = fullFavorite.Recipe?.User?.Username ?? string.Empty,
                CategoryId = fullFavorite.Recipe?.CategoryId,
                CategoryName = fullFavorite.Recipe?.Category?.Name,
                DifficultyLevel = fullFavorite.Recipe?.DifficultyLevel ?? string.Empty,
                TotalTimeMinutes = fullFavorite.Recipe?.TotalTimeMinutes,
                FavoritesCount = fullFavorite.Recipe?.FavoritesCount ?? 0,
                LikesCount = likesCount,
                CommentsCount = commentsCount,
                CreatedAt = fullFavorite.CreatedAt,
                PublishedAt = fullFavorite.Recipe?.PublishedAt
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

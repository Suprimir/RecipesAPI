using RecipesAPI.DTOs;
using RecipesAPI.Models;
using RecipesAPI.Repositories;

namespace RecipesAPI.Services
{
    public class FollowService : IFollowService
    {
        private readonly IFollowRepository _followRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRecipeRepository _recipeRepository;

        public FollowService(
            IFollowRepository followRepository,
            IUserRepository userRepository,
            IRecipeRepository recipeRepository)
        {
            _followRepository = followRepository;
            _userRepository = userRepository;
            _recipeRepository = recipeRepository;
        }

        public async Task<IEnumerable<FollowUserDTO>> GetFollowersAsync(Guid userId, int page, int limit)
        {
            var followers = await _followRepository.GetFollowersAsync(userId, page, limit);
            return followers.Select(f => new FollowUserDTO
            {
                UserId = f.FollowerId,
                Username = f.Follower?.Username ?? string.Empty,
                Bio = f.Follower?.Bio,
                ProfileImageUrl = f.Follower?.ProfileImageUrl,
                FollowedAt = f.CreatedAt
            });
        }

        public async Task<IEnumerable<FollowUserDTO>> GetFollowingAsync(Guid userId, int page, int limit)
        {
            var following = await _followRepository.GetFollowingAsync(userId, page, limit);
            return following.Select(f => new FollowUserDTO
            {
                UserId = f.FollowingId,
                Username = f.Following?.Username ?? string.Empty,
                Bio = f.Following?.Bio,
                ProfileImageUrl = f.Following?.ProfileImageUrl,
                FollowedAt = f.CreatedAt
            });
        }

        public async Task<FollowUserDTO> FollowUserAsync(Guid followerId, Guid followingId)
        {
            // Validar que no sea el mismo usuario
            if (followerId == followingId)
                throw new Exception("No puedes seguirte a ti mismo");

            // Validar que el usuario a seguir existe
            var userToFollow = await _userRepository.GetByIdAsync(followingId);
            if (userToFollow == null)
                throw new Exception("Usuario no encontrado");

            // Validar que no esté ya siguiendo
            var existingFollow = await _followRepository.GetFollowAsync(followerId, followingId);
            if (existingFollow != null)
                throw new Exception("Ya sigues a este usuario");

            var follow = new Follow
            {
                FollowerId = followerId,
                FollowingId = followingId
            };

            var createdFollow = await _followRepository.CreateFollowAsync(follow);

            return new FollowUserDTO
            {
                UserId = createdFollow.FollowingId,
                Username = createdFollow.Following?.Username ?? string.Empty,
                Bio = createdFollow.Following?.Bio,
                ProfileImageUrl = createdFollow.Following?.ProfileImageUrl,
                FollowedAt = createdFollow.CreatedAt
            };
        }

        public async Task<bool> UnfollowUserAsync(Guid followerId, Guid followingId)
        {
            return await _followRepository.DeleteFollowAsync(followerId, followingId);
        }

        public async Task<bool> IsFollowingAsync(Guid followerId, Guid followingId)
        {
            return await _followRepository.IsFollowingAsync(followerId, followingId);
        }

        public async Task<IEnumerable<RecipeDTO>> GetFeedAsync(Guid userId, int page, int limit)
        {
            // Obtener los usuarios que sigue
            var following = await _followRepository.GetFollowingAsync(userId, 1, 1000);
            var followingIds = following.Select(f => f.FollowingId).ToList();

            if (!followingIds.Any())
                return new List<RecipeDTO>();

            // Obtener recetas públicas de los usuarios seguidos, ordenadas por fecha
            var allRecipes = new List<Recipe>();

            foreach (var followingId in followingIds)
            {
                var recipes = await _recipeRepository.GetByUserIdAsync(followingId);
                allRecipes.AddRange(recipes.Where(r => r.IsPublic));
            }

            // Ordenar por fecha de publicación (más reciente primero) y paginar
            var paginatedRecipes = allRecipes
                .OrderByDescending(r => r.PublishedAt ?? r.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToList();

            // Convertir a DTOs
            return paginatedRecipes.Select(r => new RecipeDTO
            {
                Id = r.Id,
                UserId = r.UserId,
                Username = r.User?.Username ?? string.Empty,
                CategoryId = r.CategoryId,
                CategoryName = r.Category?.Name,
                Title = r.Title,
                Description = r.Description,
                CoverImageUrl = r.CoverImageUrl,
                PrepTimeMinutes = r.PrepTimeMinutes,
                CookTimeMinutes = r.CookTimeMinutes,
                TotalTimeMinutes = r.TotalTimeMinutes,
                DifficultyLevel = r.DifficultyLevel,
                IsPublic = r.IsPublic,
                FavoritesCount = r.FavoritesCount,
                CreatedAt = r.CreatedAt,
                PublishedAt = r.PublishedAt
            });
        }
    }
}

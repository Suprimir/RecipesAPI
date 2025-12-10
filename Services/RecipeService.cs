using AutoMapper;
using RecipesAPI.DTOs;
using RecipesAPI.Models;
using RecipesAPI.Repositories;

namespace RecipesAPI.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly ILikeRepository _likeRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<RecipeService> _logger;

        public RecipeService(
            IRecipeRepository recipeRepository,
            ILikeRepository likeRepository,
            ICommentRepository commentRepository,
            IMapper mapper,
            ILogger<RecipeService> logger)
        {
            _recipeRepository = recipeRepository;
            _likeRepository = likeRepository;
            _commentRepository = commentRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<RecipeDTO?> GetRecipeByIdAsync(Guid id, bool includeDetails = false)
        {
            var recipe = await _recipeRepository.GetByIdAsync(id, includeDetails);
            if (recipe == null) return null;

            var dto = _mapper.Map<RecipeDTO>(recipe);

            dto.LikesCount = await _likeRepository.GetLikesCountAsync(recipe.Id);
            dto.CommentsCount = await _commentRepository.CountByRecipeAsync(recipe.Id);

            if (includeDetails)
            {
                dto.Steps = recipe.Steps?.OrderBy(s => s.StepNumber).Select(_mapper.Map<RecipeStepDTO>).ToList();
                dto.Ingredients = recipe.RecipeIngredients?.Select(_mapper.Map<RecipeIngredientDTO>).ToList();
                dto.Tags = recipe.RecipeTags?.Select(rt => _mapper.Map<TagDTO>(rt.Tag)).ToList();
            }

            return dto;
        }

        public async Task<IEnumerable<RecipeDTO>> GetAllRecipesAsync(
            int page,
            int limit,
            string? sort,
            bool newest,
            int? categoryId,
            string? difficulty,
            int? maxTime,
            string? search)
        {
            var recipes = await _recipeRepository.GetAllAsync(page, limit, sort, newest, categoryId, difficulty, maxTime, search);
            var list = recipes.ToList();

            if (!list.Any())
            {
                return Enumerable.Empty<RecipeDTO>();
            }

            var ids = list.Select(r => r.Id).ToList();

            // Ejecutar en secuencia para evitar concurrencia sobre el mismo DbContext
            var likes = await _likeRepository.GetLikesCountAsync(ids);
            var comments = await _commentRepository.CountByRecipesAsync(ids);

            return list.Select(r =>
            {
                var dto = _mapper.Map<RecipeDTO>(r);
                dto.LikesCount = likes.TryGetValue(r.Id, out var l) ? l : 0;
                dto.CommentsCount = comments.TryGetValue(r.Id, out var c) ? c : 0;
                return dto;
            });
        }

        public async Task<IEnumerable<RecipeDTO>> GetRecipesByUserIdAsync(Guid userId)
        {
            var recipes = (await _recipeRepository.GetByUserIdAsync(userId)).ToList();

            if (!recipes.Any())
            {
                return Enumerable.Empty<RecipeDTO>();
            }

            var ids = recipes.Select(r => r.Id).ToList();

            // Serializamos las consultas para evitar concurrencia en el mismo DbContext
            var likes = await _likeRepository.GetLikesCountAsync(ids);
            var comments = await _commentRepository.CountByRecipesAsync(ids);

            return recipes.Select(r =>
            {
                var dto = _mapper.Map<RecipeDTO>(r);
                dto.LikesCount = likes.TryGetValue(r.Id, out var l) ? l : 0;
                dto.CommentsCount = comments.TryGetValue(r.Id, out var c) ? c : 0;
                return dto;
            });
        }

        public async Task<RecipeDTO> CreateRecipeAsync(CreateRecipeDTO dto, Guid userId)
        {
            var recipe = new Recipe
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CategoryId = dto.CategoryId,
                Title = dto.Title,
                Description = dto.Description,
                CoverImageUrl = dto.CoverImageUrl,
                PrepTimeMinutes = dto.PrepTimeMinutes,
                CookTimeMinutes = dto.CookTimeMinutes,
                DifficultyLevel = dto.DifficultyLevel,
                IsPublic = dto.IsPublic,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            if (dto.IsPublic)
            {
                recipe.PublishedAt = DateTime.UtcNow;
            }

            var createdRecipe = await _recipeRepository.CreateAsync(recipe);

            // Add steps
            if (dto.Steps != null && dto.Steps.Any())
            {
                var steps = dto.Steps.Select(s => new RecipeStep
                {
                    Id = Guid.NewGuid(),
                    RecipeId = createdRecipe.Id,
                    StepNumber = s.StepNumber,
                    StepType = s.StepType,
                    Title = s.Title,
                    Description = s.Description,
                    ImageUrl = s.ImageUrl,
                    DurationMinutes = s.DurationMinutes,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
                await _recipeRepository.AddStepsAsync(steps);
            }

            // Add ingredients
            if (dto.Ingredients != null && dto.Ingredients.Any())
            {
                var ingredients = dto.Ingredients.Select(i => new RecipeIngredient
                {
                    Id = Guid.NewGuid(),
                    RecipeId = createdRecipe.Id,
                    IngredientId = i.IngredientId,
                    Quantity = i.Quantity,
                    Unit = i.Unit,
                    Notes = i.Notes,
                    CreatedAt = DateTime.UtcNow
                });
                await _recipeRepository.AddIngredientsAsync(ingredients);
            }

            // Add tags
            if (dto.TagIds != null && dto.TagIds.Any())
            {
                var tags = dto.TagIds.Select(tagId => new RecipeTag
                {
                    RecipeId = createdRecipe.Id,
                    TagId = tagId,
                    CreatedAt = DateTime.UtcNow
                });
                await _recipeRepository.AddTagsAsync(tags);
            }

            return await GetRecipeByIdAsync(createdRecipe.Id, true) ?? throw new InvalidOperationException("Error al crear la receta");
        }

        public async Task<bool> UpdateRecipeAsync(Guid id, UpdateRecipeDTO dto, Guid userId)
        {
            var recipe = await _recipeRepository.GetByIdAsync(id);
            if (recipe == null || recipe.UserId != userId) return false;

            recipe.CategoryId = dto.CategoryId;
            recipe.Title = dto.Title;
            recipe.Description = dto.Description;
            recipe.CoverImageUrl = dto.CoverImageUrl;
            recipe.PrepTimeMinutes = dto.PrepTimeMinutes;
            recipe.CookTimeMinutes = dto.CookTimeMinutes;
            recipe.DifficultyLevel = dto.DifficultyLevel;

            await _recipeRepository.UpdateAsync(recipe);

            // Update steps
            if (dto.Steps != null)
            {
                await _recipeRepository.DeleteStepsByRecipeIdAsync(id);
                if (dto.Steps.Any())
                {
                    var steps = dto.Steps.Select(s => new RecipeStep
                    {
                        Id = Guid.NewGuid(),
                        RecipeId = id,
                        StepNumber = s.StepNumber,
                        StepType = s.StepType,
                        Title = s.Title,
                        Description = s.Description,
                        ImageUrl = s.ImageUrl,
                        DurationMinutes = s.DurationMinutes,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                    await _recipeRepository.AddStepsAsync(steps);
                }
            }

            // Update ingredients
            if (dto.Ingredients != null)
            {
                await _recipeRepository.DeleteIngredientsByRecipeIdAsync(id);
                if (dto.Ingredients.Any())
                {
                    var ingredients = dto.Ingredients.Select(i => new RecipeIngredient
                    {
                        Id = Guid.NewGuid(),
                        RecipeId = id,
                        IngredientId = i.IngredientId,
                        Quantity = i.Quantity,
                        Unit = i.Unit,
                        Notes = i.Notes,
                        CreatedAt = DateTime.UtcNow
                    });
                    await _recipeRepository.AddIngredientsAsync(ingredients);
                }
            }

            // Update tags
            if (dto.TagIds != null)
            {
                await _recipeRepository.DeleteTagsByRecipeIdAsync(id);
                if (dto.TagIds.Any())
                {
                    var tags = dto.TagIds.Select(tagId => new RecipeTag
                    {
                        RecipeId = id,
                        TagId = tagId,
                        CreatedAt = DateTime.UtcNow
                    });
                    await _recipeRepository.AddTagsAsync(tags);
                }
            }

            return true;
        }

        public async Task<bool> DeleteRecipeAsync(Guid id, Guid userId)
        {
            var recipe = await _recipeRepository.GetByIdAsync(id);
            if (recipe == null || recipe.UserId != userId) return false;

            return await _recipeRepository.DeleteAsync(id);
        }

        public async Task<bool> PublishRecipeAsync(Guid id, Guid userId)
        {
            var recipe = await _recipeRepository.GetByIdAsync(id);
            if (recipe == null || recipe.UserId != userId) return false;

            await _recipeRepository.PublishAsync(id);
            return true;
        }

        public async Task<RecipeStatsDTO?> GetRecipeStatsAsync(Guid id)
        {
            var recipe = await _recipeRepository.GetByIdAsync(id);
            if (recipe == null) return null;

            var likesCount = await _likeRepository.GetLikesCountAsync(id);
            var commentsCount = await _commentRepository.CountByRecipeAsync(id);

            return new RecipeStatsDTO
            {
                RecipeId = id,
                ViewsCount = 0,
                FavoritesCount = recipe.FavoritesCount,
                LikesCount = likesCount,
                CommentsCount = commentsCount
            };
        }
    }
}

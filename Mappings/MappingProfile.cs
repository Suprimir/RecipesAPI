using AutoMapper;
using RecipesAPI.DTOs;
using RecipesAPI.Models;

namespace RecipesAPI.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Ingredient, IngredientDTO>();

            CreateMap<CreateIngredientDTO, Ingredient>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow)); // Establecer fecha actual

            CreateMap<UpdateIngredientDTO, Ingredient>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()); // No modificar CreatedAt

            CreateMap<Category, CategoryDTO>();

            CreateMap<CreateCategoryDTO, Category>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<UpdateCategoryDTO, Category>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            CreateMap<Tag, TagDTO>();

            CreateMap<CreateTagDTO, Tag>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<UpdateTagDTO, Tag>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            CreateMap<User, UserDTO>();

            CreateMap<User, UserProfileDTO>()
                .ForMember(dest => dest.RecipesCount, opt => opt.Ignore())
                .ForMember(dest => dest.FollowersCount, opt => opt.Ignore())
                .ForMember(dest => dest.FollowingCount, opt => opt.Ignore());

            CreateMap<Recipe, RecipeDTO>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User != null ? src.User.Username : string.Empty))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
                .ForMember(dest => dest.Steps, opt => opt.Ignore())
                .ForMember(dest => dest.Ingredients, opt => opt.Ignore())
                .ForMember(dest => dest.Tags, opt => opt.Ignore());

            CreateMap<RecipeStep, RecipeStepDTO>();

            CreateMap<RecipeIngredient, RecipeIngredientDTO>()
                .ForMember(dest => dest.IngredientName, opt => opt.MapFrom(src => src.Ingredient != null ? src.Ingredient.Name : string.Empty));
        }
    }
}

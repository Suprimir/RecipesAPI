using System.ComponentModel.DataAnnotations;

namespace RecipesAPI.DTOs
{
    public class CreateCategoryDTO
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [MaxLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        [MaxLength(50, ErrorMessage = "El nombre del ícono no puede exceder 50 caracteres")]
        public string? IconName { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdateCategoryDTO
    {
        [MaxLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        [MaxLength(50, ErrorMessage = "El nombre del ícono no puede exceder 50 caracteres")]
        public string? IconName { get; set; }
        public bool IsActive { get; set; }
    }

    public class CategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? IconName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
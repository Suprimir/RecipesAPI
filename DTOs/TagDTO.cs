using System.ComponentModel.DataAnnotations;

namespace RecipesAPI.DTOs
{
    public class CreateTagDTO
    {
        [Required(ErrorMessage = "El campo 'Name' es obligatorio.")]
        [MaxLength(50, ErrorMessage = "El campo 'Name' no puede exceder 50 caracteres.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo 'Slug' es obligatorio.")]
        [MaxLength(50, ErrorMessage = "El campo 'Slug' no puede exceder 50 caracteres.")]
        public string Slug { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo 'Type' es obligatorio.")]
        [MaxLength(30, ErrorMessage = "El campo 'Type' no puede exceder 30 caracteres.")]
        public string Type { get; set; } = string.Empty;

        [MaxLength(7, ErrorMessage = "El campo 'Color' no puede exceder 7 caracteres.")]
        public string Color { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class UpdateTagDTO
    {
        [MaxLength(50, ErrorMessage = "El campo 'Name' no puede exceder 50 caracteres.")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50, ErrorMessage = "El campo 'Slug' no puede exceder 50 caracteres.")]
        public string Slug { get; set; } = string.Empty;

        [MaxLength(30, ErrorMessage = "El campo 'Type' no puede exceder 30 caracteres.")]
        public string Type { get; set; } = string.Empty;

        [MaxLength(7, ErrorMessage = "El campo 'Color' no puede exceder 7 caracteres.")]
        public string Color { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class TagDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace RecipesAPI.DTOs
{
    public class CreateCommentDTO
    {
        [Required(ErrorMessage = "El contenido es requerido")]
        [MaxLength(1000, ErrorMessage = "El contenido no puede exceder 1000 caracteres")]
        public string Content { get; set; } = string.Empty;

        public Guid? ParentCommentId { get; set; }
    }
}

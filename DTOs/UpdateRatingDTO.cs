using System.ComponentModel.DataAnnotations;

namespace RecipesAPI.DTOs
{
    /// <summary>
    /// DTO para actualizar una calificación existente
    /// </summary>
    public class UpdateRatingDTO
    {
        /// <summary>
        /// Calificación de 1 a 5 estrellas
        /// </summary>
        [Required(ErrorMessage = "La calificación es requerida")]
        [Range(1, 5, ErrorMessage = "La calificación debe estar entre 1 y 5")]
        public int Rating { get; set; }

        /// <summary>
        /// Comentario opcional sobre la receta
        /// </summary>
        [MaxLength(1000, ErrorMessage = "El comentario no puede exceder los 1000 caracteres")]
        public string? Review { get; set; }
    }
}

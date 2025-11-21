using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipesAPI.DTOs;
using RecipesAPI.Services;
using System.Security.Claims;

namespace RecipesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRecipeService _recipeService;

        public UsersController(IUserService userService, IRecipeService recipeService)
        {
            _userService = userService;
            _recipeService = recipeService;
        }

        /// <summary>
        /// Obtiene el perfil del usuario autenticado
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(UserProfileDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserProfileDTO>> GetMyProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var profile = await _userService.GetUserProfileAsync(Guid.Parse(userId));
            if (profile == null)
                return NotFound("Usuario no encontrado");

            return Ok(profile);
        }

        /// <summary>
        /// Actualiza el perfil del usuario autenticado
        /// </summary>
        [HttpPut("me")]
        [Authorize]
        [ProducesResponseType(typeof(UserProfileDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserProfileDTO>> UpdateMyProfile([FromBody] UpdateProfileDTO dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var profile = await _userService.UpdateProfileAsync(Guid.Parse(userId), dto);
            if (profile == null)
                return NotFound("Usuario no encontrado");

            return Ok(profile);
        }

        /// <summary>
        /// Elimina la cuenta del usuario autenticado
        /// </summary>
        [HttpDelete("me")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMyAccount()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var deleted = await _userService.DeleteAccountAsync(Guid.Parse(userId));
            if (!deleted)
                return NotFound("Usuario no encontrado");

            return NoContent();
        }

        /// <summary>
        /// Obtiene el perfil público de un usuario
        /// </summary>
        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(UserProfileDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserProfileDTO>> GetUserProfile(Guid userId)
        {
            var profile = await _userService.GetUserProfileAsync(userId);
            if (profile == null)
                return NotFound("Usuario no encontrado");

            return Ok(profile);
        }

        /// <summary>
        /// Obtiene las recetas públicas de un usuario con paginación
        /// </summary>
        [HttpGet("{userId}/recipes")]
        [ProducesResponseType(typeof(IEnumerable<RecipeDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<RecipeDTO>>> GetUserRecipes(Guid userId)
        {
            var profile = await _userService.GetUserProfileAsync(userId);
            if (profile == null)
                return NotFound("Usuario no encontrado");

            var recipes = await _recipeService.GetRecipesByUserIdAsync(userId);

            return Ok(recipes);
        }

        /// <summary>
        /// Obtiene las estadísticas de un usuario
        /// </summary>
        [HttpGet("{userId}/stats")]
        [ProducesResponseType(typeof(UserStatsDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserStatsDTO>> GetUserStats(Guid userId)
        {
            var stats = await _userService.GetUserStatsAsync(userId);
            if (stats == null)
                return NotFound("Usuario no encontrado");

            return Ok(stats);
        }
    }
}

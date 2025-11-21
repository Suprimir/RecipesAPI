using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipesAPI.DTOs;
using RecipesAPI.Services;
using System.Security.Claims;

namespace RecipesAPI.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Produces("application/json")]
    public class SocialController : ControllerBase
    {
        private readonly IFollowService _followService;

        public SocialController(IFollowService followService)
        {
            _followService = followService;
        }

        /// <summary>
        /// Obtiene la lista de seguidores de un usuario
        /// </summary>
        [HttpGet("{userId}/followers")]
        [ProducesResponseType(typeof(IEnumerable<FollowUserDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<FollowUserDTO>>> GetFollowers(
            Guid userId,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 20)
        {
            var followers = await _followService.GetFollowersAsync(userId, page, limit);
            return Ok(followers);
        }

        /// <summary>
        /// Obtiene la lista de usuarios que sigue un usuario
        /// </summary>
        [HttpGet("{userId}/following")]
        [ProducesResponseType(typeof(IEnumerable<FollowUserDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<FollowUserDTO>>> GetFollowing(
            Guid userId,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 20)
        {
            var following = await _followService.GetFollowingAsync(userId, page, limit);
            return Ok(following);
        }

        /// <summary>
        /// Seguir a un usuario
        /// </summary>
        [HttpPost("{userId}/follow")]
        [Authorize]
        [ProducesResponseType(typeof(FollowUserDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FollowUserDTO>> FollowUser(Guid userId)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            try
            {
                var follow = await _followService.FollowUserAsync(Guid.Parse(currentUserId), userId);
                return CreatedAtAction(nameof(GetFollowing), new { userId = currentUserId }, follow);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("no encontrado"))
                    return NotFound(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Dejar de seguir a un usuario
        /// </summary>
        [HttpDelete("{userId}/follow")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UnfollowUser(Guid userId)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            var unfollowed = await _followService.UnfollowUserAsync(Guid.Parse(currentUserId), userId);
            if (!unfollowed)
                return NotFound("No sigues a este usuario");

            return NoContent();
        }

        /// <summary>
        /// Verificar si el usuario autenticado sigue a otro usuario
        /// </summary>
        [HttpGet("{userId}/is-following")]
        [Authorize]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> IsFollowing(Guid userId)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            var isFollowing = await _followService.IsFollowingAsync(Guid.Parse(currentUserId), userId);
            return Ok(new { isFollowing });
        }

        /// <summary>
        /// Obtiene el feed personalizado del usuario autenticado (recetas de usuarios seguidos)
        /// </summary>
        [HttpGet("me/feed")]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<RecipeDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<RecipeDTO>>> GetFeed(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 20)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            var feed = await _followService.GetFeedAsync(Guid.Parse(currentUserId), page, limit);
            return Ok(feed);
        }
    }
}

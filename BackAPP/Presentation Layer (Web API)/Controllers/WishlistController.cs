using Business_Logic_Layer.DTOs;
using Business_Logic_Layer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AirbnbClone.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WishlistController : ControllerBase
    {
        private readonly WishlistService _wishlistService;

        public WishlistController()
        {
            _wishlistService = new WishlistService();
        }

        [HttpPost("toggle")]
        [AllowAnonymous]
        public IActionResult ToggleWishlist([FromBody] WishlistDto dto)
        {
            var result = _wishlistService.ToggleWishlist(dto);
            if (result.Contains("Error"))
                return BadRequest(new { Message = result });

            return Ok(new { Message = result });
        }

        [HttpGet("user/{userId}")]
        [AllowAnonymous]
        public IActionResult GetUserWishlist(Guid userId)
        {

            var wishlist = _wishlistService.GetWishlistByUserId(userId);
            return Ok(wishlist);
        }
    }
}
using Business_Logic_Layer.DTOs;
using Business_Logic_Layer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AirbnbClone.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly ReviewService _reviewService;
        private readonly IAuthorizationService _authorizationService;

        public ReviewsController(IAuthorizationService authorizationService)
        {
            _reviewService = new ReviewService();
            _authorizationService = authorizationService;
        }

        [HttpPost("add")]
       
       
        public IActionResult AddReview([FromBody] ReviewCreateDto dto)
        {
            if (dto == null) return BadRequest();

            try
            {
                // استدعاء الخدمة والحصول على النتائج المفككة
                var (review, message, userImage, userName) = _reviewService.AddReview(dto);

                if (review == null)
                {
                    return BadRequest(new { Message = message });
                }

                // إرجاع الكائن الكامل للفرونت إند ليتم عرضه فوراً
                return Ok(new
                {
                    id = review.Id,
                    listingId = review.ListingId,
                    rating = review.Rating,
                    comment = review.Comment,
                    createdAt = review.CreatedAtUtc,
                    userImage = userImage ?? "", // الصورة التي جلبناها
                    userName = userName ?? "Anonymous", // الاسم لكي لا تظهر البطاقة فارغة
                    message = message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Internal Server Error: " + ex.Message });
            }
        }

        [HttpGet("listing/{listingId}")]
        [AllowAnonymous]
        public IActionResult GetListingReviews(Guid listingId)
        {
            var reviews = _reviewService.GetListingReviews(listingId);
            if (reviews == null) return NotFound();

            return Ok(reviews);
        }

        [HttpDelete("delete/{id}")]
        
        public async Task<IActionResult> DeleteReviewAsync(Guid id)
        {
            var success = _reviewService.DeleteReview(id);
            if (!success)
                return BadRequest(new { Message = "Failed to delete review." });
            var result = await _authorizationService.AuthorizeAsync(User, id, "ListingOwner");
            if (!result.Succeeded) return Forbid();

            return Ok(new { Message = "Review deleted successfully." });
        }
    }
}
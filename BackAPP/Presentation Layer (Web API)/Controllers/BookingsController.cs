using Business_Logic_Layer.DTOs;
using Business_Logic_Layer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly BookingService _bookingService;
    private readonly UserService _currentUser; // This will be set in the constructor or a method to get the current user from the token
    private readonly IAuthorizationService _authorizationService;

    public BookingsController(IAuthorizationService authorizationService)
    {
        _bookingService = new BookingService();
        _authorizationService = authorizationService;
        _currentUser = new UserService(); // Implement this method to extract user info from the token
    }

    [HttpPost("create")]
    [EnableRateLimiting("fixed")]
    public IActionResult CreateBooking([FromBody] BookingCreateDto dto)
    {
        var result = _bookingService.CreateBooking(dto);
        if (!result.IsSuccess)
        {
            return BadRequest(new { result.Message });
        }

        return Ok(new { Message = "Booking created successfully!", BookingId = result });
    }

    [HttpGet("user/{userId}")]

    public async Task<IActionResult> GetMyBookingsAsync(Guid userId)
    {
        var bookings = _bookingService.GetUserBookings(userId);
        var result = await _authorizationService.AuthorizeAsync(User, userId, "AccountOwner");
        if (!result.Succeeded) return Forbid();
        return Ok(bookings);
    }
    [HttpGet("{id}")]
    
    public async Task<IActionResult> GetBookingByIdAsync(Guid id)
    {
        // 1. جلب الحجز من الخدمة
        var booking = _bookingService.GetBookingById(id);

        // 2. التحقق من وجود الحجز
        if (booking == null)
        {
            return NotFound(new { Message = "Booking not found" });
        }

        // 3. التحقق من الصلاحية: هل المستخدم الحالي هو صاحب هذا الحجز؟
        // نستخدم GuestId الموجود داخل الحجز للمقارنة
        var result = await _authorizationService.AuthorizeAsync(User, booking.GuestId, "AccountOwner");

        if (!result.Succeeded)
        {
            return Forbid();
        }

        // 4. إرجاع بيانات الحجز (يفضل أن تكون DTO كما فعلنا سابقاً)
        return Ok(booking);
    }

    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllBookingsAsync()
    {
        var bookings = _bookingService.GetAllBookings();
        
        return Ok(bookings);
    }

    // إلغاء حجز معين
    [HttpDelete("cancel/{id}")]
    public async Task<IActionResult> CancelBookingAsync(Guid id)
    {
        var currentUserIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (currentUserIdClaim == null) return Unauthorized();
        Guid currentUserId = Guid.Parse(currentUserIdClaim);
        var resulte = await _authorizationService.AuthorizeAsync(User, currentUserId, "AccountOwner");
        if (!resulte.Succeeded) return Forbid();

        bool result = _bookingService.CancelBooking(id);
        if (!result)
            return BadRequest(new { Message = "Error: Booking cancellation failed." });
        
        return Ok(new { Message = "Booking cancelled successfully." });
    }
}
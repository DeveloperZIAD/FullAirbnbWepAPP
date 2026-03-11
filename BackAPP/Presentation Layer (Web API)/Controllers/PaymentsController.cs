using Business_Logic_Layer.Interfaces;
using Business_Logic_Layer.DTOs; // تأكد من استيراد الـ DTOs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly IPaymentService _paymentService;
    private readonly IConfiguration _config;

    public PaymentsController(
        IBookingService bookingService,
        IPaymentService paymentService,
        IConfiguration config)
    {
        _bookingService = bookingService;
        _paymentService = paymentService;
        _config = config;
        // مفتاح Stripe السري
        StripeConfiguration.ApiKey = _config["Stripe:SecretKey"] ?? Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY");
    }
    

    [HttpPost("process")]
     //pm_card_visa
    public async Task<IActionResult> ProcessPayment([FromQuery] Guid bookingId, [FromQuery] string paymentMethodId)
    {
        // 1. الفلديشن الأساسي للمدخلات
        if (string.IsNullOrEmpty(paymentMethodId) || paymentMethodId == "stripe")
        {
            return BadRequest(new { Message = "Valid PaymentMethod ID is required (e.g., pm_card_visa)" });
        }

        // 2. جلب بيانات الحجز للتأكد من وجوده والحصول على السعر
        var booking = _bookingService.GetBookingById(bookingId);
        if (booking == null) return NotFound(new { Message = "Booking not found" });

        try
        {
            // 3. تنفيذ عملية الدفع في سيرفرات Stripe
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(booking.TotalPrice * 100), // تحويل الدولار إلى سنتات
                Currency = "usd",
                PaymentMethod = paymentMethodId,
                Confirm = true,
                Description = $"Payment for Booking ID: {bookingId}",
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                    AllowRedirects = "never"
                }
            };

            var service = new PaymentIntentService();
            PaymentIntent intent = await service.CreateAsync(options);

            // 4. إذا نجح الدفع في Stripe، نستخدم الـ Service والـ UnitOfWork لحفظ البيانات
            if (intent.Status == "succeeded")
            {
                // تجهيز الـ DTO المطلوب للميثود الخاصة بك
                var paymentDto = new PaymentProcessDto
                {
                    BookingId = bookingId,
                    UserId = (Guid)booking.GuestId,
                    Amount = booking.TotalPrice,
                    Currency = "USD",
                    Provider = "Stripe",
                    TransactionId = intent.Id // المعرف القادم من Stripe
                };

                // استدعاء الميثود التي تستخدم الـ UnitOfWork داخلياً
                string resultMessage = _paymentService.ProcessPayment(paymentDto);

                if (resultMessage.Contains("Error"))
                {
                    return StatusCode(500, new { Message = resultMessage });
                }

                return Ok(new
                {
                    Success = true,
                    TransactionId = intent.Id,
                    Message = resultMessage
                });
            }

            return BadRequest(new { Message = "Payment failed with status: " + intent.Status });
        }
        catch (StripeException e)
        {
            // في حالة رفض البطاقة أو أي خطأ من طرف Stripe
            return BadRequest(new { Message = e.StripeError.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An internal error occurred", Details = ex.Message });
        }
    }
}
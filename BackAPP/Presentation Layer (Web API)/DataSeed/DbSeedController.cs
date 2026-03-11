using Business_Logic_Layer.Interfaces;
using Business_Logic_Layer.Interfaces.GenData;
using Business_Logic_Layer.Service;
using Business_Logic_Layer.Services;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace Presentation_Layer.Controllers
{
    [Authorize(Roles = "Admin")] // تأكد أن المستخدم هو Admin عشان يقدر ينادي العملية دي
    [Route("api/[controller]")]
    [ApiController]
    
    public class GenDataController : ControllerBase
    {
        private readonly IDatabaseSeederService _seederService;
        private readonly UnitOfWork _unitOfWork;

        public GenDataController(IDatabaseSeederService seederService)
        {
            _seederService = seederService;
            _unitOfWork = new UnitOfWork();

        }

        [HttpPost("seed-database")]
        // تأكد أن العملية دي مش محمية عشان ممكن تحتاج تتعمل في بيئة التطوير أو الاختبار
        public async Task<IActionResult> SeedDatabase([FromQuery] int userCount = 10)
        {
            // تأمين بسيط عشان العملية دي "تقيلة" ومش أي حد يناديها
            if (userCount <= 0 || userCount > 100)
            {
                return BadRequest("Please provide a user count between 1 and 100.");
            }

            try
            {
                var result = await _seederService.SeedSystemDataAsync(userCount);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                // الـ Exception هنا هيتم تسجيله في الـ Logger داخل السيرفس
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost("seed-by-category")]
       
        public async Task<IActionResult> SeedByCategory([FromQuery] int count, Guid categoryId)
        {
            var category =  _unitOfWork.Categories.GetById(categoryId);

            if (category == null)
                throw new Exception("Category not found!");

            // استدعاء الخدمة بالفئة الصحيحة
            var result = await _seederService.SeedDataByCategoryAsync(count, category.Name);

            return Ok(new
            {
                status = "Success",
                details = result
            });
        }

        [HttpGet("available-categories")]
       
        public async Task<IActionResult> GetAvailableCategories()
        {
            // جلب كل التصنيفات الموجودة في قاعدة البيانات
            var categories = await _unitOfWork.Categories.GetAllAsync();
            return Ok(categories.Select(c => new { c.Id, c.Name }));
        }

        [HttpPost("seed-reviews/{listingId}")]
      
        public async Task<IActionResult> SeedReviews(Guid listingId, [FromQuery] int count = 5)
        {
            var result = await _seederService.SeedReviewsAsync(listingId, count);
            return Ok(result);
        }

        [HttpPost("seed-by-country")]
       
        public async Task<IActionResult> SeedByCountry([FromQuery] int count, [FromQuery] string countryCode)
        {
            if (string.IsNullOrEmpty(countryCode) || count <= 0)
                return BadRequest("Invalid count or country code.");

            var message = await _seederService.SeedListingsByCountryAsync(count, countryCode.ToUpper());
            return Ok(new { message });
        }
    }
}
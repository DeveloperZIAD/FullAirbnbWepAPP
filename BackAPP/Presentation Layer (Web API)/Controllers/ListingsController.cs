using AirbnbClone.Api.Controllers.Authentication;
using Business_Logic_Layer.DTOs;
using Business_Logic_Layer.DTOs.API;
using Business_Logic_Layer.Services;
using Data_Access_Layer.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace AirbnbClone.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // تأكد أن المستخدم مسجل دخول
    public class ListingsController : ControllerBase
    {
        private readonly ListingService _listingService;
        private readonly IAuthorizationService _authorizationService;
        private readonly UnitOfWork _unitOfWork = new UnitOfWork(); // للوصول إلى الـ Users Repository
        public ListingsController(IAuthorizationService authorizationService)
        {
            _listingService = new ListingService();
            _authorizationService = authorizationService;
        }
     
        // 1. إضافة عقار جديد
        [HttpPost("create")][EnableRateLimiting("fixed")]
        public IActionResult CreateListing([FromBody] SimpleListingCreateDto listingDto)
        {
            if (listingDto == null) return BadRequest("Invalid listing data.");

            var result = _listingService.AddListing(listingDto);

            if (!result.IsSuccess)
            {
                return BadRequest(new { result.Message });
            }

            return Ok(new { Message = "Listing created successfully!", Data = result });
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetListings(int page = 1, int pageSize = 10)
        {
            var listings = _listingService.GetListingsPaged(page, pageSize);
            return Ok(listings);
        }   
            


        // 7. البحث المتقدم والفلترة
        [HttpGet("search")]
        [AllowAnonymous]
        //[EnableRateLimiting("fixed")]
        public IActionResult SearchListings([FromQuery] ListingSearchParams searchParams)
        {
            // سنقوم بإنشاء هذه الميثود في الـ Service الآن
            var results = _listingService.SearchListings(searchParams);
            return Ok(results);
        }
        // 2. جلب كل العقارات (للعرض في الصفحة الرئيسية)
        [HttpGet("all")]
        [AllowAnonymous]
        public IActionResult GetAllListings()
        {
            var listings = _listingService.GetAllListings();
            return Ok(listings);
        }

        // 3. جلب تفاصيل عقار واحد بالـ Id
        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult GetListingById(Guid id)
        {
            var listing = _listingService.GetListingById(id);
            if (listing == null) return NotFound(new { Message = "Listing not found." });
            return Ok(listing);
        }

        // 4. جلب عقارات مضيف معين
        [HttpGet("host/{hostId}")]
        [AllowAnonymous]
        public IActionResult GetListingsByHost(Guid hostId)
        {
            var hostListings = _listingService.GetListingsByHostId(hostId);
            return Ok(hostListings);
        }

        // 5. تحديث بيانات عقار
        [HttpPut("update/{id}")]
        [Authorize(Roles = UserRoles.Host)]
        public async Task<IActionResult> Update(Guid id, ListingCreateDto dto)
        {
            var result = await _authorizationService.AuthorizeAsync(User, id, "ListingOwner");
            if (!result.Succeeded) return Forbid();

            var success = _listingService.UpdateListing(id, dto);
            if (!success) return BadRequest("Update failed.");

            return Ok("Updated successfully.");
        }
        // 6. حذف عقار (Soft Delete)
        [HttpDelete("delete/{id}")]
        [Authorize(Roles = UserRoles.Host)]
        public async Task<IActionResult> DeleteListingAsync(Guid id)
        {
            
            // 2. التحقق من الصلاحية عبر AuthorizationService
            // نمرر الـ id (listingId) كـ Resource
            var result = await _authorizationService.AuthorizeAsync(User, id, "ListingOwner");

            if (!result.Succeeded) return Forbid();

            // 3. التنفيذ
            var success = _listingService.DeleteListing(id);

            if (!success) return BadRequest(new { Message = "Delete failed." });

            return Ok(new { Message = "Listing deleted successfully." });
        }
    }
}
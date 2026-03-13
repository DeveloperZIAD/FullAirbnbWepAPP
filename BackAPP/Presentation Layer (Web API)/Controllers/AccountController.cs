using Business_Logic_Layer.DTOs;
using Business_Logic_Layer.Interfaces;
using Business_Logic_Layer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System;
using static Business_Logic_Layer.DTOs.UserRegisterDto;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace AirbnbClone.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // تأكد أن المستخدم مسجل دخول
    public class AccountController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly HostProfileService _hostService;
        private readonly IAuthorizationService _authorizationService;

        public AccountController(IAuthorizationService authorizationService)
        {
            _userService = new UserService();
            _hostService = new HostProfileService();
            _authorizationService = authorizationService;
        }


        [AllowAnonymous]
        [HttpPost("register/guest")]
        [EnableRateLimiting("fixed")]
        public IActionResult RegisterGuest([FromBody] GuestRegisterDto dto)
        {
         
            return ProcessRegistration(dto);
        }
        [HttpGet("role/{userId}")]
        [AllowAnonymous]
        public IActionResult GetUserRole (Guid userId)
        {
            var role = _userService.GetUserRole(userId);
            if (role == null) return NotFound();
            return Ok(role);
        }

        [HttpPost("register/host")]
        [AllowAnonymous]
        [EnableRateLimiting("fixed")]
        public IActionResult RegisterHost([FromBody] HostRegisterDto dto)
        {
            return ProcessRegistration(dto);
        }
        [HttpPost("upgrade-to-host/{userId}")]
        [AllowAnonymous]
        public IActionResult UpgradeToHost(Guid userId)
        {
            var result = _userService.MakeUserAHost(userId);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result.Message);
        }

        private IActionResult ProcessRegistration(UserRegisterDto dto)
        {
           
           
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = _userService.RegisterUser(dto);

            // يفضل التحقق من النتيجة بناءً على كود أو كائن وليس مجرد String يحتوي على كلمة Error
            if (!result.IsSuccess)
            {
                return BadRequest(new {  result.Message });
            }

            return Ok(new
            {
                result.Message,
                 result.UserId
            });
        }


        [HttpGet("users")]
        [Authorize(Roles = "Admin")]
        
        public IActionResult GetAllUsers()
        {
            var users = _userService.GetAllUsers();
            return Ok(users);
        }
        
        [HttpGet("users/{id}")]
        //[Authorize(Roles = "Admin")]
      
        public async Task<IActionResult> GetUserAsync(Guid id)
        {
            var user = _userService.GetUserById(id);
            if (user == null) return NotFound();
                var authResult = await _authorizationService.AuthorizeAsync(User, id, "AccountOwner");
            if (!authResult.Succeeded) return Forbid();
            return Ok(user);
        }

        [HttpPost("host/profile")]

        public async Task<IActionResult> CreateOrUpdateHostProfileAsync([FromBody] HostProfileDto profileDto)
        {
            if (profileDto == null) return BadRequest();

            var result = _hostService.CreateOrUpdateProfile(profileDto);

            if (result.Contains("Error"))
                return BadRequest(new { Message = result });
            var authResult = await _authorizationService.AuthorizeAsync(User, profileDto.UserId, "ListingOwner");
            if (!authResult.Succeeded) return Forbid();

            return Ok(new { Message = result });
        }

        [HttpGet("host/{userId}")]
        [AllowAnonymous]
        
        public async Task<IActionResult> GetHostProfileAsync(Guid userId)
        {
            var profile = _hostService.GetProfileByUserId(userId);
            if (profile == null) return NotFound();
            //var authResult = await _authorizationService.AuthorizeAsync(User, profile.UserId, "ListingOwner");
            //if (!authResult.Succeeded) return Forbid();
            return Ok(profile);
        }

        [HttpPut("host/verify/{userId}")]
        
        public IActionResult VerifyHost(Guid userId)
        {
            var success = _hostService.VerifyHost(userId);
            if (!success) return BadRequest();
            var authResult = _authorizationService.AuthorizeAsync(User, userId, "Admin").Result;
            return Ok(new { Message = "Host verified successfully" });
        }


        [HttpGet("statsHost")]
        [AllowAnonymous]
        public async Task<IActionResult> GetStats(Guid hostId)
        {
           
            var stats = await _hostService.GetHostDashboardStats(hostId);
            if (stats == null) return NotFound();
            //var authResult = await _authorizationService.AuthorizeAsync(User, hostId, "ListingOwner");
            //if (!authResult.Succeeded) return Forbid();
            return Ok(stats);
        }
        [HttpGet("listingsHost")]
        [AllowAnonymous]
        public async Task<IActionResult> GetListings(Guid hostId)
        {
           
            var listings = await _hostService.GetHostListings(hostId);
            if (listings == null) return NotFound();
            //var authResult = await _authorizationService.AuthorizeAsync(User, hostId, "ListingOwner");
            //if (!authResult.Succeeded) return Forbid();
            return Ok(listings);
        }


        //[HttpPost("migrate-passwords")]
        //[AllowAnonymous]
        //public IActionResult RunPasswordMigration()
        //{
        //    try
        //    {
        //        _userService.MigratePasswords();
        //        return Ok(new { Message = "Passwords migrated successfully to BCrypt Hash!" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { Message = "Migration failed: " + ex.Message });
        //    }
        //}
    }
}
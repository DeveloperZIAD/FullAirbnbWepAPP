using AirbnbClone.Api.Controllers.Authentication;
using Business_Logic_Layer.DTOs;
using Business_Logic_Layer.DTOs.API;
using Business_Logic_Layer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System;
using System.Collections.Generic;


namespace AirbnbClone.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    
    public class AuthController : ControllerBase
    {
        private readonly AccountService _accountService;

        public AuthController(AccountService accountService)
        {
            _accountService = accountService;
        }

        // 1. تسجيل الدخول - Login
        [HttpPost("login")]
        [AllowAnonymous] // السماح بالوصول بدون توكن لأن هذا هو نقطة الدخول للحصول على التوكن
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _accountService.Login(loginDto);

            if (result == null)
                // غيرنا Forbid لـ Unauthorized لأن البيانات خطأ
                return Unauthorized(new { Message = "Invalid Email or Password" });

            return Ok(result);
        }

        // 2. تجديد التوكن - Refresh Token
        [HttpPost("refresh-token")]
        [AllowAnonymous]
       
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDto tokenRequest)
        {
            // 1. التحقق من أن الجسم (Body) ليس فارغاً
            if (tokenRequest == null || string.IsNullOrEmpty(tokenRequest.RefreshToken))
            {
                return BadRequest(new { Message = "Access Token and Refresh Token are required." });
            }

            // 2. استدعاء الخدمة المحدثة التي تقوم بالتحقق وإصدار توكن جديد
            var result = await _accountService.RefreshToken(tokenRequest.AccessToken, tokenRequest.RefreshToken);

            // 3. إذا فشلت العملية (التوكن ملغي، منتهي، أو لا يخص المستخدم)
            if (result == null)
            {
                // نرجع 401 Unauthorized لكي يفهم الفرونت-إند أنه يجب توجيه المستخدم لـ Login
                return Unauthorized(new { Message = "Session expired. Please login again." });
            }

            // 4. نجاح العملية وإرسال الـ AuthResponseDto الجديد (Token + User Data)
            return Ok(result);
        }
        // 3. تسجيل الخروج (إبطال التوكن) - Revoke Token
        // يفضل أن يكون [Authorize] لضمان أن صاحب التوكن هو من يلغيه
        [HttpPost("revoke/{refreshToken}")]
        public async Task<IActionResult> Revoke(string refreshToken)
        {
            var result = await _accountService.RevokeToken(refreshToken);

            if (!result)
                return BadRequest(new { Message = "Token not found or already revoked" });

            return Ok(new { Message = "Token revoked successfully" });
        }
    }
}
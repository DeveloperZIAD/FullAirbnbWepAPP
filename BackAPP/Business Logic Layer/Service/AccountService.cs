using Business_Logic_Layer.DTOs;
using Business_Logic_Layer.Interfaces.API;
using Data_Access_Layer.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Business_Logic_Layer.Service.util;

namespace Business_Logic_Layer.Services
{
    public class AccountService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private readonly ITokenService _tokenService;
        public AccountService(UnitOfWork unitOfWork, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _config = config;
            _tokenService = new TokenService(config); // إنشاء خدمة التوكن داخل الكونستركتور
        }
        public async Task<AuthResponseDto> Login(LoginDto loginDto)
        {
            // 1. البحث عن المستخدم مع جلب بيانات الاتصال (Contact)
            // نستخدم الـ Repository المحدث الذي يدعم الـ Includes
            var contact = _unitOfWork.Contacts // تأكد من وجود Repository للـ Contacts
        .GetAll(c => c.User) // عمل Join مع جدول اليوزر
        .FirstOrDefault(c => c.PrimaryEmail == loginDto.Email);

            if (contact == null || contact.User == null || !PasswordHelper.VerifyPassword(loginDto.Password, contact.User.Password))
            {
                return null; // فشل تسجيل الدخول
            }

            var user = contact.User;

            // 2. توليد التوكنات باستخدام الخدمة المنفصلة (TokenService)
            var accessToken = _tokenService.CreateToken(user);
            var refreshTokenValue = _tokenService.GenerateRefreshToken();
            var expiration = DateTime.UtcNow.AddMinutes(30);

            // 3. إنشاء كائن الـ Refresh Token وحفظه
            var refreshTokenEntity = new UserRefreshToken
            {
                Id = Guid.NewGuid(),
                Token = refreshTokenValue,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false
            };

            _unitOfWork.UserRefreshTokens.Add(refreshTokenEntity);
            _unitOfWork.Complete();

            // 4. إرجاع الرد كامل البيانات للـ Frontend
            return new AuthResponseDto
            {
                Token = accessToken,
                RefreshToken = refreshTokenValue,
                Expiration = expiration,
                UserType = user.UserType,
                User = new UserDataDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    ImageSrc = user.AvatarUrl,
                    Email = user.Contact.PrimaryEmail
                }
            };
        }
        public async Task<AuthResponseDto> RefreshToken(string expiredAccessToken, string refreshToken)
        {
            // 1. فك التوكن المنتهي واستخراج الـ Claims
            // نستخدم TokenService للقيام بهذه المهمة بدلاً من كتابتها هنا
            var principal = _tokenService.GetPrincipalFromExpiredToken(expiredAccessToken);
            if (principal == null) return null;

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId)) return null;

            // 2. البحث عن الـ Refresh Token في قاعدة البيانات
            // الأفضل البحث بـ Filter مباشرة في الداتابيز بدلاً من جلب كل البيانات بـ GetAllAsync
            var storedRefreshToken = _unitOfWork.UserRefreshTokens
                        .GetAll() // ستجلب IEnumerable
                        .FirstOrDefault(t =>
                            t.Token == refreshToken &&
                            t.UserId == userId &&
                            !t.IsRevoked);

            // 3. التحقق من وجود التوكن وصلاحيته الزمنية
            if (storedRefreshToken == null || storedRefreshToken.ExpiresAt <= DateTime.UtcNow)
            {
                return null; // الجلسة انتهت ويجب تسجيل الدخول مرة أخرى
            }

            // 4. جلب بيانات المستخدم مع الـ Contact لتوليد التوكن الجديد
            var user = _unitOfWork.Users
                .GetAll(u => u.Contact)
                .FirstOrDefault(u => u.Id == userId);

            if (user == null || user.Contact == null) return null;

            // 5. توليد التوكنات الجديدة (Rotation)
            // نستخدم الـ TokenService التي أنشأناها لتوحيد منطق الإصدار
            var newAccessToken = _tokenService.CreateToken(user);
            var newRefreshTokenValue = _tokenService.GenerateRefreshToken();

            // 6. تحديث قاعدة البيانات (إبطال التوكن الحالي وإنشاء جديد)
            // هذا الأسلوب يسمى "Refresh Token Rotation" وهو الأكثر أماناً
            storedRefreshToken.IsRevoked = true;

            var newRefreshTokenEntity = new UserRefreshToken
            {
                Id = Guid.NewGuid(),
                Token = newRefreshTokenValue,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false
            };

            _unitOfWork.UserRefreshTokens.Add(newRefreshTokenEntity);
            _unitOfWork.Complete();

            // 7. إرجاع الرد بنفس تنسيق الـ Login لضمان توافق الفرونت-إند
            return new AuthResponseDto
            {
                Token = newAccessToken,
                RefreshToken = newRefreshTokenValue,
                Expiration = DateTime.UtcNow.AddMinutes(30),
                UserType = user.UserType,
                User = new UserDataDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    ImageSrc = user.AvatarUrl,
                    Email = user.Contact.PrimaryEmail
                }
            };
        }
       
        public async Task<bool> RevokeToken(string refreshToken)
        {
            var tokenEntity = (await _unitOfWork.UserRefreshTokens.GetAllAsync())
                .FirstOrDefault(t => t.Token == refreshToken);

            if (tokenEntity == null || tokenEntity.IsRevoked) return false;

            tokenEntity.IsRevoked = true;
            _unitOfWork.Complete();
            return true;
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
using Business_Logic_Layer.DTOs;
using Business_Logic_Layer.Interfaces;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;

namespace Business_Logic_Layer.Services
{
    public class HostProfileService : IHostProfileService
    {
        private readonly UnitOfWork _unitOfWork;

        public HostProfileService()
        {
            _unitOfWork = new UnitOfWork();
        }

        public async Task<object> GetHostDashboardStats(Guid hostId)
        {
            // جلب المضيف مع عقاراته والحجوزات المرتبطة بها
            var host = _unitOfWork.HostProfiles.GetById(hostId, h => h.Listings);
            if(host == null) return null;

            // حساب الإحصائيات (يمكن تطوير هذا لاحقاً بعمل Query احترافي)
            var listings = host.Listings.ToList();

            return new
            {
                revenue = listings.Sum(l => l.BasePrice), // مثال بسيط للحساب
                listings = listings.Count,
                bookings = 45 // هذا الرقم سيتم جلبه من جدول الحجوزات لاحقاً
            };
        }
        public async Task<IEnumerable<ListingDto>> GetHostListings(Guid hostId)
        {
            // 1. استخدام GetQueryable بدلاً من GetAll لتحسين الأداء وتطبيق الفلترة في قاعدة البيانات
            var query = _unitOfWork.Listings.GetQueryable(
                l => l.Images,
                l => l.Location
            );

            // 2. إضافة شرط !l.IsDeleted لاستبعاد المحذوفات
            var listings = query.Where(l => l.HostId == hostId && !l.IsDeleted)
                                .OrderByDescending(l => l.CreatedAtUtc); // ترتيب الأحدث أولاً

            return listings.Select(l => new ListingDto
            {
                Id = l.Id,
                Title = l.Title,
                BasePrice = l.BasePrice,
                // تنظيف روابط الصور من القيم الفارغة أو النصوص الافتراضية "string"
                ImageUrls = l.Images
                             .Where(i => !string.IsNullOrEmpty(i.Url) && i.Url != "string")
                             .Select(i => i.Url)
                             .ToList(),
                // التأكد من عدم وجود قيم "string" في المدينة
                City = (l.Location != null && l.Location.City != "string") ? l.Location.City : "Unknown"
            }).ToList();
        }

        public string CreateOrUpdateProfile(HostProfileDto dto)
        {
            var existingProfile = _unitOfWork.HostProfiles.GetAll()
                .FirstOrDefault(p => p.UserId == dto.UserId);

            if (existingProfile != null)
            {
                // Update
                existingProfile.TaxId = dto.TaxId;
                _unitOfWork.HostProfiles.Update(existingProfile);
            }
            else
            {
                // Create
                var newProfile = new HostProfile
                {
                    Id = Guid.NewGuid(),
                    UserId = dto.UserId,
                    TaxId = dto.TaxId,
                    IsVerified = false, // يبدأ غير موثق دائماً
                    CreatedAtUtc = DateTime.UtcNow
                };
                _unitOfWork.HostProfiles.Add(newProfile);
            }

            return _unitOfWork.Complete() > 0 ? "Host profile saved!" : "Error: Failed to save profile.";
        }

        public HostProfileDto GetProfileByUserId(Guid userId)
        {
            var profile = _unitOfWork.HostProfiles.GetAll()
                .FirstOrDefault(p => p.UserId == userId);

            if (profile == null) return null;

            return new HostProfileDto
            {
                id = profile.Id,
                UserId = profile.UserId,
                TaxId = profile.TaxId,
                IsVerified = profile.IsVerified
            };
        }

        public bool VerifyHost(Guid userId)
        {
            var profile = _unitOfWork.HostProfiles.GetAll()
                .FirstOrDefault(p => p.UserId == userId);

            if (profile != null)
            {
                profile.IsVerified = true;
                _unitOfWork.HostProfiles.Update(profile);
                return _unitOfWork.Complete() > 0;
            }
            return false;
        }
    }
}
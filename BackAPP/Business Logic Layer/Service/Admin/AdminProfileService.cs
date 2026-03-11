using Business_Logic_Layer.DTOs;
using Business_Logic_Layer.DTOs;
using Business_Logic_Layer.DTOs.Admin;
using Business_Logic_Layer.Interfaces;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Business_Logic_Layer.Service.Admin
{
    public class AdminService : IAdminProfileService
    {
        private readonly UnitOfWork _unitOfWork;

        public AdminService()
        {
            _unitOfWork = new UnitOfWork();
        }

        public IEnumerable<object> GetAllListingsDetailed()
        {
            // 1. دلوقتي تقدر تعمل Include للـ Location وهتشتغل "نار" لأن EF عرف الـ Key خلاص
            var listings = _unitOfWork.Listings.GetAll(l => l.Host, l => l.Category, l => l.Location).ToList();

            return listings.Select(l =>
            {
                var user = _unitOfWork.Users.GetById(l.Host.UserId);
                var contact = _unitOfWork.Contacts.GetAll().FirstOrDefault(c => c.UserId == l.Host.UserId);

                return new
                {
                    l.Id,
                    l.Title,
                    Price = l.BasePrice,
                    CategoryName = l.Category?.Name ?? "General",
                    Status = l.Status,
                    CreatedAt = l.CreatedAtUtc,

                    HostDetails = new
                    {
                        l.Host.Id,
                        l.Host.UserId,
                        FullName = user?.FullName ?? "Unknown",
                        Email = contact?.PrimaryEmail ?? "No Email"
                    },

                    // ✅ دلوقتي l.Location مستحيل ترجع Null طالما ليها سجل
                    Location = l.Location != null ? new
                    {
                        l.Location.City,
                        l.Location.CountryCode,
                        l.Location.StreetAddress,
                        l.Location.Latitude,
                        l.Location.Longitude
                    } : null
                };
            }).ToList();
        }
        public IEnumerable<object> GetAllReservationsDetailed()
        {
            // 1. جلب الحجوزات مع العقار والمضيف المرتبط به
            var bookings = _unitOfWork.Bookings.GetAll(
                b => b.Listing,
                b => b.Listing.Host
            ).ToList();

            return bookings.Select(b =>
            {
                // --- جلب بيانات الضيف (الذي قام بالحجز) ---
                // بما أن b.GuestId هو معرف المستخدم الذي حجز
                var guestUser = _unitOfWork.Users.GetById(b.GuestId);
                var guestContact = _unitOfWork.Contacts.GetAll()
                    .FirstOrDefault(c => c.UserId == b.GuestId);

                // --- جلب بيانات المضيف (صاحب العقار) ---
                // نستخدم UserId الموجود داخل كائن الـ Host المرتبط بالعقار
                var hostUser = _unitOfWork.Users.GetById(b.Listing.Host.UserId);
                var hostContact = _unitOfWork.Contacts.GetAll()
                    .FirstOrDefault(c => c.UserId == b.Listing.Host.UserId);

                return new
                {
                    Id = b.Id,
                    ListingTitle = b.Listing?.Title ?? "Deleted Listing",

                    // تفاصيل الضيف
                    GuestDetails = new
                    {
                        UserId = b.GuestId,
                        FullName = guestUser?.FullName ?? "Unknown Guest",
                        Email = guestContact?.PrimaryEmail ?? "No Email"
                    },

                    // تفاصيل المضيف
                    HostDetails = new
                    {
                        HostId = b.Listing.HostId,
                        UserId = b.Listing.Host.UserId,
                        FullName = hostUser?.FullName ?? "Unknown Host",
                        Email = hostContact?.PrimaryEmail ?? "No Email"
                    },

                    CheckIn = b.CheckIn,
                    CheckOut = b.CheckOut,
                    PricePerNight = b.PricePerNightAtBooking,
                    TotalPrice = b.TotalPrice,
                    Status = b.Status,
                    CreatedAt = b.CreatedAtUtc
                };
            }).ToList();
        }
        public AdminDashboardDto GetDashboardStats()
        {
            return new AdminDashboardDto
            {
                TotalUsers = _unitOfWork.Users.GetAll().Count(),
                TotalListings = _unitOfWork.Listings.GetAll().Count(l => !l.IsDeleted),
                ActiveBookings = _unitOfWork.Bookings.GetAll().Count(b => b.Status == "Confirmed"),
                TotalRevenue = _unitOfWork.Bookings.GetAll().Sum(b => b.TotalPrice)
            };
        }
        public IEnumerable<object> GetAllUsersDetailed()
        {
            // 1. جلب المستخدمين الأساسيين من قاعدة البيانات
            var users = _unitOfWork.Users.GetAll().ToList();

            // 2. جلب جهات الاتصال والأدوار لمرة واحدة لتقليل الكويريز (أداء أفضل)
            var allContacts = _unitOfWork.Contacts.GetAll().ToList();

            // ملاحظة: افترضنا وجود جدول أو طريقة لجلب الأدوار (Roles)
            // لو بتستخدم ASP.NET Identity ممكن تجيبها من الـ UserManager

            return users.Select(u =>
            {
                // البحث عن الإيميل في جدول الـ Contacts بناءً على الـ UserId
                var contact = allContacts.FirstOrDefault(c => c.UserId == u.Id);

                return new
                {
                    u.Id,
                    FullName = u.FullName ?? "N/A",
                    // 1. اسم البريد (من جدول الـ Contacts)
                    Email = contact?.PrimaryEmail ?? "No Email",

                    // 2. الرول (سواء كان Admin, Host, أو Guest)
                    // هنا نفترض أن الحقل موجود في جدول الـ User أو يتم استنتاجه
                    Role = u.UserType ?? "Guest",

                    // 3. تاريخ الإنشاء
                    CreatedAt = u.CreatedAtUtc
                };
            }).ToList();
        }

        public string CreateAdminProfile(AdminProfileDto dto)
        {
            var user = _unitOfWork.Users.GetById(dto.UserId);
            if (user == null) return "Error: User does not exist.";

            var adminEntity = new Data_Access_Layer.Entities.Admin
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                Role = dto.Role,
                Permissions = dto.Permissions,
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow
            };

            _unitOfWork.Admins.Add(adminEntity);
            return _unitOfWork.Complete() > 0 ? "Admin created successfully!" : "Error: Failed to save.";
        }

        // هذه الميثود كانت ناقصة وهي سبب الخطأ الأساسي
        public bool UpdatePermissions(Guid userId, string newPermissions)
        {
            var admin = _unitOfWork.Admins.GetAll().FirstOrDefault(a => a.UserId == userId);
            if (admin == null) return false;

            admin.Permissions = newPermissions;
            _unitOfWork.Admins.Update(admin);
            return _unitOfWork.Complete() > 0;
        }

        public AdminProfileDto GetAdminDetails(Guid userId)
        {
            var admin = _unitOfWork.Admins.GetAll().FirstOrDefault(a => a.UserId == userId);
            if (admin == null) return null;

            return new AdminProfileDto
            {
                UserId = admin.UserId,
                Role = admin.Role,
                Permissions = admin.Permissions,
                IsActive = admin.IsActive,
                LastLogin = admin.LastLogin
            };
        }
        public bool ToggleUserBlockStatus(Guid userId, bool isBlocked)
        {
            var user = _unitOfWork.Users.GetById(userId);
            if (user == null) return false;

            user.IsDeleted = isBlocked; // In this context, soft delete acts as a block
            return _unitOfWork.Complete() > 0;
        }

        public IEnumerable<AdminActivityLog> GetSystemLogs()
        {
            return _unitOfWork.AdminActivityLogs.GetAll()
                .OrderByDescending(log => log.CreatedAtUtc)
                .Take(100); // Return the last 100 activities
        }
        public bool DeactivateAdmin(Guid userId)
        {
            var admin = _unitOfWork.Admins.GetAll().FirstOrDefault(a => a.UserId == userId);
            if (admin == null) return false;

            admin.IsActive = false;
            _unitOfWork.Admins.Update(admin);
            return _unitOfWork.Complete() > 0;
        }
    }
}
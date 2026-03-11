using Business_Logic_Layer.DTOs;
using Business_Logic_Layer.Interfaces;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;

namespace Business_Logic_Layer.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly UnitOfWork _unitOfWork;

        public WishlistService()
        {
            _unitOfWork = new UnitOfWork();
        }
        public IEnumerable<object> GetWishlistByUserId(Guid userId)
        {
            // نستخدم النسخة التي تقبل Includes لجلب الجداول المرتبطة بالترتيب
            // لاحظ كيف نصل للجداول العميقة (مثل Location داخل Listing)
            var wishlists = _unitOfWork.Wishlists.GetAll(
                w => w.Listing,
                w => w.Listing.Location,
                w => w.Listing.Category,
                w => w.Listing.Images,
                w => w.User // هذا اليوزر صاحب المفضلات
            )
            .Where(w => w.UserId == userId && !w.IsDeleted)
            .ToList();

            return wishlists.Select(w => new
            {
                id = w.ListingId,
                title = w.Listing?.Title,
                description = w.Listing?.Description,
                basePrice = w.Listing?.BasePrice,

                // الآن ستظهر البيانات لأننا عملنا لها Include
                city = w.Listing?.Location?.City,
                countryCode = w.Listing?.Location?.CountryCode,
                addressLine = w.Listing?.Location?.StreetAddress,

                categoryName = w.Listing?.Category?.Name,

                // جلب اسم صاحب العقار (Host)
                // إذا كان الـ Listing نفسه يحتوي على User (صاحب العقار)
                hostName = w.User?.FullName ?? "Host Name",

                imageUrls = w.Listing?.Images?
                    .Where(img => !img.IsDeleted)
                    .Select(img => img.Url)
                    .ToList() ?? new List<string>()
            });
        }
        public string ToggleWishlist(WishlistDto wishlistDto)
        {
            var userExists = _unitOfWork.Users.GetById(wishlistDto.UserId) != null;
            var listingExists = _unitOfWork.Listings.GetById(wishlistDto.ListingId) != null;
            if (!userExists || !listingExists)
            {
                return "User or Listing not found";
            }

            // البحث عما إذا كان العقار موجوداً بالفعل في قائمة أمنيات المستخدم
            var existing = _unitOfWork.Wishlists.GetAll()
                .FirstOrDefault(w => w.UserId == wishlistDto.UserId && w.ListingId == wishlistDto.ListingId);

            if (existing != null)
            {
                // إذا كان موجوداً، نقوم بإزالته (Un-favorite)
                _unitOfWork.Wishlists.Delete(existing.UserId); // ملاحظة: تعتمد على كيفية تعريف الـ Delete في الـ Generic Repo عندك
                _unitOfWork.Complete();
                return "Removed from wishlist";
            }
            else
            {
                // إذا لم يكن موجوداً، نقوم بإضافته
                var wishlistItem = new Wishlist
                {
                    UserId = wishlistDto.UserId,
                    ListingId = wishlistDto.ListingId,
                    CreatedAtUtc = DateTime.UtcNow
                };

                _unitOfWork.Wishlists.Add(wishlistItem);
                _unitOfWork.Complete();
                return "Added to wishlist";
            }
        }

        public IEnumerable<Guid> GetUserWishlist(Guid userId)
        {
            return _unitOfWork.Wishlists.GetAll()
                .Where(w => w.UserId == userId)
                .Select(w => w.ListingId)
                .ToList();
        }
    }
}
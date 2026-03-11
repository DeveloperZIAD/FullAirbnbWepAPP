using Business_Logic_Layer.DTOs;
using Business_Logic_Layer.DTOs.API;
using Business_Logic_Layer.Interfaces;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;

namespace Business_Logic_Layer.Services
{
    public class ReviewService : IReviewService
    {
        private readonly UnitOfWork _unitOfWork;

        public ReviewService()
        {
            _unitOfWork = new UnitOfWork();
        }

        public (Review review, string message, string userImage, string userName) AddReview(ReviewCreateDto reviewDto)
        {
            // 1. التحقق من التقييم
            if (reviewDto.Rating < 1 || reviewDto.Rating > 5)
                return (null, "Error: Rating must be between 1 and 5.", null, null);

            // 2. التحقق من وجود العقار
            var listingExists = _unitOfWork.Listings.GetById(reviewDto.ListingId);
            if (listingExists == null)
                return (null, "Error: Listing not found.", null, null);

            // 3. التحقق من وجود المستخدم وجلب بياناته (الصورة والاسم)
            var user = _unitOfWork.Users.GetById(reviewDto.GuestId);
            if (user == null)
                return (null, "Error: User not found.", null, null);

            // 4. إنشاء كيان التقييم
            var reviewEntity = new Review
            {
                ListingId = reviewDto.ListingId,
                UserID = reviewDto.GuestId,
                Rating = reviewDto.Rating,
                Comment = reviewDto.Comment,
                CreatedAtUtc = DateTime.UtcNow,
                IsDeleted = false
            };

            // 5. الحفظ
            _unitOfWork.Reviews.Add(reviewEntity);
            int result = _unitOfWork.Complete();

            if (result > 0)
            {
                // نُعيد الكيان مع بيانات المستخدم التي جلبناها بالأعلى
                return (reviewEntity, "Review added successfully!", user.AvatarUrl, user.FullName);
            }

            return (null, "Error: Failed to save review.", null, null);
        }
        // غير النوع هنا من IEnumerable<Review> إلى IEnumerable<string>
        public IEnumerable<string> GetCommentsByListing(Guid listingId)
        {
            // 1. نجلب المراجعات المرتبطة بالعقار
            return _unitOfWork.Reviews.GetAll()
                .Where(r => r.ListingId == listingId && !r.IsDeleted)
                // 2. نختار حقل الـ Comment فقط (سلسلة نصية)
                .Select(r => r.Comment)
                // 3. التنفيذ الفعلي
                .ToList();
        }
        public bool DeleteReview(Guid id)
        {
            var review = _unitOfWork.Reviews.GetById(id);
            if (review == null) return false;

            review.IsDeleted = true;
            return _unitOfWork.Complete() > 0;
        }
        public IEnumerable<ReviewReadDto> GetListingReviews(Guid listingId)
        {
            // 1. جلب التقييمات الخاصة بهذا العقار فقط
            var reviews = _unitOfWork.Reviews.GetAll()
                .Where(r => r.ListingId == listingId && !r.IsDeleted)
                .ToList();

            // 2. تحويل التقييمات إلى DTO مع جلب بيانات المستخدم لكل تقييم يدوياً
            var reviewDtos = reviews.Select(r =>
            {
                // جلب بيانات المستخدم بناءً على الـ UserID الموجود في التقييم
                var user = _unitOfWork.Users.GetById(r.UserID);

                return new ReviewReadDto
                {
                    Id = r.Id,
                    ListingId = r.ListingId,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAtUtc,
                    // إذا وجدنا المستخدم نأخذ بياناته، وإلا نضع قيم افتراضية
                    UserName = user?.FullName ?? "User",
                    UserImage = user?.AvatarUrl ?? ""
                };
            }).ToList();

            return reviewDtos;
        }

        public double GetAverageRating(Guid listingId)
        {
            var reviews = _unitOfWork.Reviews.GetAll()
                .Where(r => r.ListingId == listingId && !r.IsDeleted);

            if (!reviews.Any()) return 0;

            return reviews.Average(r => r.Rating);
        }
    }
}
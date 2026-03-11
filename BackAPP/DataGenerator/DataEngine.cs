using Bogus;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Models;
using DataGenerator.Generators;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Data_Access_Layer.Data;

namespace DataGenerator
{
    public class DataEngine
    {
        private readonly ApplicationDbContext _context;

        // نحتاج الـ Context هنا لنسحب التصنيفات الحقيقية
        public DataEngine(ApplicationDbContext context)
        {
            _context = context;
        }

        public class DataGeneratorResult
        {
            public List<User> Users { get; set; } = new();
            public List<Listing> Listings { get; set; } = new();
        }


        public async Task<DataGeneratorResult> GenerateSystemDataAsync(int userCount)
        {
            // 1. جلب التصنيفات (AsNoTracking مهم جداً هنا لتقليل حمل الذاكرة ومنع التضارب)
            var categories = await _context.Categories
                .AsNoTracking()   // ← شيل ده أو غيّره لـ AsTracking()
                .ToListAsync(); await UserGenerator.InitializeAvatarsAsync(userCount);

            var result = new DataGeneratorResult();
            var userFaker = UserGenerator.CreateUserFaker();
            var resultUsers = userFaker.Generate(userCount);

            foreach (var user in resultUsers)
            {
                if (user.UserType == "Host")
                {
                    var hProfile = new HostProfile
                    {
                        UserId = user.Id,
                        CreatedAtUtc = DateTime.UtcNow
                    };
                    user.HostProfile = hProfile;

                    var randomCategory = categories[new Random().Next(categories.Count)];

                    // التعديل هنا: نمرر الـ IDs فقط لضمان عدم حدوث Tracking Conflict
                    var listingFaker = ListingGenerator.CreateListingFaker(hProfile, randomCategory);
                   // Console.WriteLine($"HostId: {hProfile.Id}, CategoryId: {randomCategory.Id}");
                    var hostListings = listingFaker.Generate(new Random().Next(1, 4));

                    foreach (var listing in hostListings)
                    {
                        // جلب الصور
                        var imageUrls = await ImageGenerator.GenerateImageUrlsAsync(3);

                        foreach (var url in imageUrls)
                        {
                            listing.Images.Add(new ListingImage
                            {
                                Url = url,
                                IsMain = (listing.Images.Count == 0),
                                CreatedAtUtc = DateTime.UtcNow
                            });
                        }

                        // تأكد أن العلاقات الكبيرة NULL لكي لا يحاول EF حفظ نسخ جديدة منها
                        listing.Host = hProfile;
                        listing.Category = null;

                        result.Listings.Add(listing);
                    }
                }
                result.Users.Add(user);
            }
            return result;
        }

        public async Task<DataGeneratorResult> GenerateDataByCountryAsync(int listingCount, string countryCode)
        {
            var result = new DataGeneratorResult();
            var random = new Random();

            // جلب مستخدمين عشوائيين للربط بهم
            var hosts = await _context.HostProfiles.Include(h => h.User).ToListAsync();
            var categories = await _context.Categories.ToListAsync();
            var locations = await _context.Locations.Where(l => l.CountryCode == countryCode).ToListAsync();

            for (int i = 0; i < listingCount; i++)
            {
                var host = hosts[random.Next(hosts.Count)];
                var category = categories[random.Next(categories.Count)];

                // استخدام الـ Faker المطور
                var listingFaker = ListingGenerator.CreateCountrySpecificFaker(host, category, countryCode, locations);
                var listing = listingFaker.Generate();

                result.Listings.Add(listing);
            }
            return result;
        }

        public async Task<DataGeneratorResult> GenerateDataByCategoryAsync(int userCount, string categoryName)
        {
            // 1. جلب الفئة المحددة فقط من قاعدة البيانات
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Name == categoryName && !c.IsDeleted);

            if (category == null)
            {
                throw new Exception($"Category '{categoryName}' not found in database.");
            }

            await UserGenerator.InitializeAvatarsAsync(userCount);
            var result = new DataGeneratorResult();
            var userFaker = UserGenerator.CreateUserFaker();
            var resultUsers = userFaker.Generate(userCount);

            foreach (var user in resultUsers)
            {
                if (user.UserType == "Host")
                {
                    var hProfile = new HostProfile
                    {
                        UserId = user.Id,
                        CreatedAtUtc = DateTime.UtcNow
                    };
                    user.HostProfile = hProfile;
                    // نستخدم الكتجري الممرر في الباراميتر بدلاً من العشوائي
                    var listingFaker = ListingGenerator.CreateListingFaker(hProfile, category);

                    // توليد عدد عشوائي من العقارات لهذا المضيف داخل هذه الفئة
                    var hostListings = listingFaker.Generate(new Random().Next(1, 4));

                    foreach (var listing in hostListings)
                    {
                        var imageUrls = await ImageGenerator.GenerateImageUrlsAsync(3);
                        foreach (var url in imageUrls)
                        {
                            listing.Images.Add(new ListingImage
                            {
                                Url = url,
                                IsMain = (listing.Images.Count == 0),
                                CreatedAtUtc = DateTime.UtcNow
                            });
                        }

                        // نربط بالـ ID لضمان عدم حدوث Tracking Conflict
                        listing.CategoryId = category.Id;
                        listing.Category = null;
                        listing.Host = hProfile;

                        result.Listings.Add(listing);
                    }
                }
                result.Users.Add(user);
            }
            return result;
        }


    public async Task<List<Review>> GenerateReviewsForListingAsync(Guid listingId, int count)
    {
        var reviews = new List<Review>();
        var random = new Random();

        // 1. تعريف الـ Faker لتوليد تعليقات واقعية
        var reviewFaker = new Faker<Review>()
            .RuleFor(r => r.Id, f => Guid.NewGuid())
            .RuleFor(r => r.ListingId, f => listingId)
            .RuleFor(r => r.Rating, f => f.Random.Int(1, 5))
            // توليد تعليقات متنوعة بناءً على التقييم
            .RuleFor(r => r.Comment, f => f.PickRandom(new[] {
            "Amazing stay! The host was very helpful.",
            "Great location, but the place was a bit noisy.",
            "Beautiful design and very clean.",
            "Perfect for a weekend getaway. Highly recommended!",
            "It was okay, not exactly as pictured.",
            "Absolutely stunning view! We had a wonderful time.",
            "Basic amenities, but the price is unbeatable.",
            "Very cozy and comfortable, felt just like home."
            }))
            .RuleFor(r => r.CreatedAtUtc, f => f.Date.Past(1)); // تاريخ عشوائي خلال السنة الماضية

        // 2. جلب مستخدمين عشوائيين لربطهم بالتعليقات
        var users = await _context.Users
            .OrderBy(x => Guid.NewGuid())
            .Take(count)
            .ToListAsync();

        // 3. إنشاء التقييمات
        foreach (var user in users)
        {
            var review = reviewFaker.Generate();
            review.UserID = user.Id;
            reviews.Add(review);
        }

        return reviews;
    }
    // قمت بتحويلها لـ Synchronous لأن Faker لا يحتاج Async داخلياً
    public List<Listing> GenerateListingsForHost(Guid hostProfileId, int count, List<Category> availableCategories)
        {
            var listingFaker = new Faker<Listing>()
               // .RuleFor(l => l.Id, f => Guid.NewGuid())
                .RuleFor(l => l.HostId, hostProfileId)
                // التعديل السحري: اختر تصنيف عشوائي من القائمة الحقيقية
                .RuleFor(l => l.CategoryId, f => f.PickRandom(availableCategories).Id)
                .RuleFor(l => l.Title, f => f.Commerce.ProductName())
                .RuleFor(l => l.Description, f => f.Lorem.Paragraph())
                .RuleFor(l => l.BasePrice, f => f.Random.Decimal(100, 1000))
                .RuleFor(l => l.Status, f => "Published")
                .RuleFor(l => l.CreatedAtUtc, f => DateTime.UtcNow)

                // تجاهل العلاقات المعقدة لمنع الـ Shadow Properties والـ Conflicts
                .Ignore(l => l.Host)
                .Ignore(l => l.Category)
                .Ignore(l => l.Location)
                .Ignore(l => l.Images)
                .Ignore(l => l.Amenities);

            return listingFaker.Generate(count);
        }
    }
}
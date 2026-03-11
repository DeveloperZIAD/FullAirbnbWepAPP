using Bogus;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Models;
using Microsoft.EntityFrameworkCore;

namespace DataGenerator.Generators
{
    public static class ListingGenerator
    {
        public static Faker<Listing> CreateListingFaker(HostProfile hostProfile, Category category)
        {
            var addressFaker = LocationGenerator.CreateAddressFaker();

            return new Faker<Listing>()
                // 1. الربط باستخدام الكائنات (Navigation Properties)
                // تأكد أن أسماء الحقول (HostProfile / Category) تطابق الـ Entities عندك
                .RuleFor(l => l.Host, f => hostProfile)
                .RuleFor(l => l.CategoryId, f => category.Id)

                .RuleFor(l => l.Title, f => f.Commerce.ProductName() + " in " + f.Address.City())
                .RuleFor(l => l.Description, f => f.Lorem.Paragraphs(2))
                .RuleFor(l => l.BasePrice, f => f.Random.Decimal(50, 300))
                .RuleFor(l => l.Status, f => "Published")
                .RuleFor(l => l.CreatedAtUtc, f => f.Date.Past(1))

                // 2. ربط الـ Location (بدون تعيين IDs يدوياً)
                .RuleFor(l => l.Location, (f, l) => {
                    var addr = addressFaker.Generate();
                    return new Location
                    {
                        CountryCode = addr.Country,
                        City = addr.City,
                        StreetAddress = addr.Street,
                        Latitude = (decimal)addr.Latitude,
                        Longitude = (decimal)addr.Longitude
                        // EF سيقوم بربط الـ ListingId تلقائياً عند الحفظ
                    };
                })

                // 3. إضافة الصور باستخدام FinishWith
                .FinishWith((f, l) =>
                {
                    // جلب الصور بشكل متزامن للـ Seeding
                    var imageUrls = ImageGenerator.GenerateImageUrlsAsync(5).GetAwaiter().GetResult();

                    foreach (var url in imageUrls)
                    {
                        l.Images.Add(new ListingImage
                        {
                            Url = url,
                            IsMain = (l.Images.Count == 0) // أول صورة هي الأساسية
                        });
                    }
                });
        }

        public static Faker<Listing> CreateCountrySpecificFaker(HostProfile hostProfile, Category category, string countryCode, List<Location> locations)
        {
            var addressFaker = LocationGenerator.CreateAddressFaker();
            var random = new Random();

            return new Faker<Listing>()
                .RuleFor(l => l.HostId, f => hostProfile.Id)
                .RuleFor(l => l.CategoryId, f => category.Id)
                .RuleFor(l => l.Title, f => f.Commerce.ProductName() + " in " + f.Address.City())
                .RuleFor(l => l.Description, f => f.Lorem.Paragraphs(2))
                .RuleFor(l => l.BasePrice, f => f.Random.Decimal(50, 300))
                .RuleFor(l => l.Status, f => "Published")
                .RuleFor(l => l.CreatedAtUtc, f => f.Date.Past(1))

                // 2. ربط الـ Location باستخدام القائمة الممررة (منع التكرار بأداء عالٍ)
                .RuleFor(l => l.Location, (f, l) => {

                    // اختيار موقع عشوائي من القائمة المتاحة إذا وجدت
                    if (locations != null && locations.Any() && f.Random.Bool(0.7f))
                    {
                        return locations[random.Next(locations.Count)];
                    }

                    // إنشاء موقع جديد إذا لم تتوفر مواقع أو فشل الاحتمال
                    var addr = addressFaker.Generate();
                    return new Location
                    {
                        CountryCode = countryCode,
                        City = addr.City,
                        StreetAddress = addr.Street,
                        Latitude = (decimal)addr.Latitude,
                        Longitude = (decimal)addr.Longitude
                    };
                })

                // 3. إضافة الصور
                .FinishWith((f, l) =>
                {
                    var imageUrls = ImageGenerator.GenerateImageUrlsAsync(5).GetAwaiter().GetResult();

                    foreach (var url in imageUrls)
                    {
                        l.Images.Add(new ListingImage
                        {
                            Url = url,
                            IsMain = (l.Images.Count == 0)
                        });
                    }
                });
        }
    }
}
using Business_Logic_Layer.Interfaces.GenData;
using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;
using DataGenerator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Business_Logic_Layer.Service
{
    public class DatabaseSeederService : IDatabaseSeederService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly ILogger<DatabaseSeederService> _logger;
        private readonly ApplicationDbContext Context = new ApplicationDbContext(); 
        public DatabaseSeederService(UnitOfWork unitOfWork, ILogger<DatabaseSeederService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<string> SeedSystemDataAsync(int userCount)
        {
            try
            {
                // 1. تأمين التصنيفات (استخدام GetAll بدلاً من AnyAsync لعدم توفرها)
                var existingCategories = await _unitOfWork.Categories.GetAllAsync();

                if (existingCategories == null || !existingCategories.Any())
                {
                    var defaultCategories = new List<Category>
                    {
                        new Category {  Name = "Amazing Pools", CreatedAtUtc = DateTime.UtcNow },
                        new Category {  Name = "Beachfront", CreatedAtUtc = DateTime.UtcNow },
                        new Category { Name = "Cabins", CreatedAtUtc = DateTime.UtcNow },
                        new Category { Name = "Countryside", CreatedAtUtc = DateTime.UtcNow },
                        new Category { Name = "Mansions", CreatedAtUtc = DateTime.UtcNow }
                    };

                    foreach (var cat in defaultCategories)
                    {
                        _unitOfWork.Categories.Add(cat);
                    }
                    _unitOfWork.Complete(); // حفظ التصنيفات أولاً لتكون متاحة للعقارات
                }

                // 2. توليد البيانات (بدون تمرير Context)
                var engine = new DataEngine(Context);
                var result = await engine.GenerateSystemDataAsync(userCount);

                // 3. إضافة المستخدمين فرادى
                if (result.Users != null)
                {
                    foreach (var user in result.Users)
                    {
                        _unitOfWork.Users.Add(user);
                    }
                }

                // 4. إضافة العقارات فرادى
                if (result.Listings != null)
                {
                    foreach (var listing in result.Listings)
                    {
                        _unitOfWork.Listings.Add(listing);
                    }
                }

                // 5. الحفظ النهائي لكل شيء
                int rows = _unitOfWork.Complete();

                return $"Success! {rows} rows affected. Users: {result.Users?.Count ?? 0}, Listings: {result.Listings?.Count ?? 0}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during seeding process.");
                throw;
            }
        }

        public async Task<string> SeedDataByCategoryAsync(int userCount, string categoryName)
        {
            try
            {
                // استخدام الميثود الجديدة في الـ Engine
                var engine = new DataEngine(Context);
                var result = await engine.GenerateDataByCategoryAsync(userCount, categoryName);

                // إضافة البيانات للـ Unit of Work
                if (result.Users != null)
                {
                    foreach (var user in result.Users) _unitOfWork.Users.Add(user);
                }

                if (result.Listings != null)
                {
                    foreach (var listing in result.Listings) _unitOfWork.Listings.Add(listing);
                }

                int rows = _unitOfWork.Complete();
                return $"Success! Generated {result.Listings.Count} listings for category: {categoryName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Seeding by category failed");
                throw;
            }
        }

        public async Task<string> SeedReviewsAsync(Guid listingId, int count)
        {
            try
            {
                var engine = new DataEngine(Context);
                var reviews = await engine.GenerateReviewsForListingAsync(listingId, count);

                foreach (var review in reviews)
                {
                    _unitOfWork.Reviews.Add(review);
                }

                int rows = _unitOfWork.Complete();
                return $"Successfully added {rows} reviews to listing {listingId}.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error seeding reviews.");
                throw;
            }
        }

        public async Task<string> SeedListingsByCountryAsync(int count, string countryCode)
        {
            try {
            var engine = new DataEngine(Context);

            // 1. توليد البيانات عبر الـ Engine
            var result = await engine.GenerateDataByCountryAsync(count, countryCode);

            // 2. الحفظ في قاعدة البيانات
            foreach (var listing in result.Listings)
            {
                _unitOfWork.Listings.Add(listing);
            }

            int savedRows = _unitOfWork.Complete();
            return $"Successfully seeded {savedRows} listings for country: {countryCode}";
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error seeding reviews.");
                throw;

            }
        }
    }
}

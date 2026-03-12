using Business_Logic_Layer.DTOs;
using Business_Logic_Layer.DTOs.API;
using Business_Logic_Layer.Interfaces;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Business_Logic_Layer.Services
{
    public class ListingService : IListingService
    {
        private readonly UnitOfWork _unitOfWork;

        public ListingService()
        {
            _unitOfWork = new UnitOfWork();
        }
        public bool DeleteListing(Guid id)
        {
            // 1. جلب العقار من قاعدة البيانات
            var listing = _unitOfWork.Listings.GetById(id);

            // 2. التحقق من الوجود
            if (listing == null) return false;

            // 3. تحديث الحالة (Soft Delete)
            listing.IsDeleted = true;

            // 4. تنفيذ الحفظ الفعلي في قاعدة البيانات وإرجاع النتيجة
            // Complete() ترجع عدد الأسطر المتأثرة (يجب أن يكون أكبر من 0 للنجاح)
            return _unitOfWork.Complete() > 0;
        }
        public bool IsUserOwnerOfListing(Guid userId, Guid listingId)
        {
            // 1. البحث عن ملف المضيف (HostProfile) الذي يمتلك هذا الـ UserId
            var hostProfile = _unitOfWork.HostProfiles
                .GetQueryable()
                .FirstOrDefault(h => h.UserId == userId);

            // إذا لم يكن للمستخدم ملف مضيف، فهو بالتأكيد لا يملك العقار
            if (hostProfile == null) return false;

            // 2. جلب العقار للتأكد من وجوده ومن أن صاحب العقار هو المضيف الحالي
            var listing = _unitOfWork.Listings.GetById(listingId);

            // المقارنة الآن تتم بين HostProfileId الموجود في جدول العقارات 
            // وبين الـ Id الخاص بجدول الـ HostProfiles
            return listing != null && listing.HostId == hostProfile.Id;
        }
        public bool UpdateListing(Guid id, ListingCreateDto dto)
        {
            // 1. جلب العقار مع تضمين بيانات الموقع (Location) لتحديثها
            // نستخدم GetQueryable لعمل Include للموقع
            var listing = _unitOfWork.Listings
                .GetQueryable(l => l.Location)
                .FirstOrDefault(l => l.Id == id);

            if (listing == null || listing.IsDeleted) return false;

            // 2. تحديث البيانات الأساسية
            listing.Title = dto.Title;
            listing.Description = dto.Description;
            listing.BasePrice = dto.BasePrice;
            listing.CategoryId = dto.CategoryId;

            // 3. تحديث الصور (مسح القديم وإضافة الجديد أو الاستبدال)
            // بما أن Cloudinary يعطينا روابط، نقوم بتحديث القائمة
            listing.Images = (ICollection<ListingImage>)(dto.ImageUrls ?? new List<string>());

            // 4. تحديث بيانات الموقع (Location)
            if (listing.Location != null)
            {
                listing.Location.City = dto.City;
                listing.Location.CountryCode = dto.CountryCode;
                listing.Location.StreetAddress = dto.StreetAddress;
            }
            else
            {
                // في حال لم يكن للعقار موقع (حالة نادرة)، نقوم بإنشائه
                listing.Location = new Location
                {
                    City = dto.City,
                    CountryCode = dto.CountryCode,
                    StreetAddress = dto.StreetAddress
                };
            }

            // 5. حفظ التغييرات في قاعدة البيانات
            // نتحقق من > 0 أو نرجع true إذا لم تتغير البيانات لكن العملية لم تفشل
            try
            {
                _unitOfWork.Complete();
                return true;
            }
            catch (Exception ex)
            {
                // تسجيل الخطأ هنا (Logging)
                Console.WriteLine($"Update failed: {ex.Message}");
                return false;
            }
        }
        public IEnumerable<Listing> GetListingsByHostId(Guid hostId)
        {
            return _unitOfWork.Listings.GetAll().Where(l => l.HostId == hostId && !l.IsDeleted);
        }
        public ListingCreateDto GetListingById(Guid id)
        {
            var listing = _unitOfWork.Listings.GetById(
                    id,
                    l => l.Location,
                    l => l.Images,
                    l => l.Category,
                    l => l.Host,
                    l => l.Host.User,
                    l => l.Host.User.Contact // تأكد من تضمين الـ Contact لو رقم الهاتف داخله
                );

            if (listing == null) return null;

            // توليد أرقام عشوائية للعرض فقط حتى يتم إضافتها للداتابيز
            var random = new Random(listing.Id.GetHashCode()); // ثابت لنفس العقار

            return new ListingCreateDto
            {
                Id = listing.Id,
                Title = listing.Title,
                Description = listing.Description,
                BasePrice = listing.BasePrice,
                CategoryId = listing.CategoryId,
                CategoryName = listing.Category?.Name ?? "General",

                // جلب بيانات المضيف 
                Host = new HostDetailsDto
                {
                    Id = listing.HostId,
                    Name = listing.Host?.User?.FullName ?? "Unknown Host",
                    ImageUrl = listing.Host?.User?.AvatarUrl ?? "https://via.placeholder.com/150",
                    // تأكد من المسار الصحيح للرقم حسب الـ Entity عندك
                    PhoneNumber = listing.Host?.User?.Contact?.PhoneNumber ?? "No phone available"
                },

                // بيانات الموقع (منظمة ككائن)
                Location = new LocationDto
                {
                    City = listing.Location?.City ?? "N/A",
                    CountryCode = listing.Location?.CountryCode ?? "N/A",
                    StreetAddress = listing.Location?.StreetAddress ?? "N/A",
                    Latitude = listing.Location?.Latitude ?? 0,
                    Longitude = listing.Location?.Longitude ?? 0
                },

                ImageUrls = listing.Images?.Select(img => img.Url).ToList() ?? new List<string>(),

                // إضافة قيم افتراضية/راندوم لضمان ظهور الـ UI بشكل مكتمل
                GuestCount = random.Next(2, 10),    // من 2 لـ 10 ضيوف
                RoomCount = random.Next(1, 5),     // من 1 لـ 5 غرف
                BathroomCount = random.Next(1, 3)  // من 1 لـ 3 حمامات
            };
        }

        public RegistrationResult AddListing(SimpleListingCreateDto dto)
        {
            try
            {
                // 1. التحقق من الهوست
                var hostProfile = _unitOfWork.HostProfiles
                    .GetAll(h => h.User)
                    .FirstOrDefault(h => h.UserId == dto.HostId);

                if (hostProfile == null || hostProfile.User?.UserType != "Host")
                    return new RegistrationResult { IsSuccess = false, Message = "Invalid Host." };

                // 2. البحث عن التصنيف بالاسم (نحول الاسم لـ lowercase لتجنب مشاكل الحروف)
                var category = _unitOfWork.Categories.GetAll()
                    .FirstOrDefault(c => c.Name.ToLower() == dto.CategoryName.ToLower());

                if (category == null)
                    return new RegistrationResult { IsSuccess = false, Message = "Category not found." };

                // 3. إنشاء العقار
                var listingEntity = new Listing
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    BasePrice = dto.BasePrice,
                    CategoryId = category.Id, // نربط بالـ ID الذي وجدناه
                    HostId = hostProfile.Id,
                    Status = "Active",
                    CreatedAtUtc = DateTime.UtcNow,
                    //GuestCount = dto.GuestCount,
                    //RoomCount = dto.RoomCount,
                    //BathroomCount = dto.BathroomCount,
                    Location = new Location
                    {
                        City = dto.City,
                        CountryCode = dto.CountryCode,
                        StreetAddress = dto.StreetAddress,
                        CreatedAtUtc = DateTime.UtcNow
                    },
                    Images = dto.ImageUrls.Select(url => new ListingImage
                    {
                        Url = url,
                        CreatedAtUtc = DateTime.UtcNow
                    }).ToList()
                };

                _unitOfWork.Listings.Add(listingEntity);
                _unitOfWork.Complete();

                return new RegistrationResult { IsSuccess = true, Message = "Success!", UserId = listingEntity.Id };
            }
            catch (Exception ex)
            {
                return new RegistrationResult { IsSuccess = false, Message = ex.Message };
            }
        }
        public RegistrationResult AddListing(ListingCreateDto dto)
        {
            try
            {
                // 1. Validation أساسي
                if (dto.BasePrice <= 0)
                    return new RegistrationResult { IsSuccess = false, Message = "Error: Price must be greater than zero." };

                if (dto.HostId == Guid.Empty)
                    return new RegistrationResult { IsSuccess = false, Message = "Error: HostId is required." };

                // التأكد من وجود الـ Host فعلياً في قاعدة البيانات
                var hostProfile = _unitOfWork.HostProfiles
            .GetAll(h => h.User) // نضمن تحميل بيانات اليوزر
            .FirstOrDefault(h => h.UserId == dto.HostId);

                // 2. التحقق من وجود البروفايل
                if (hostProfile == null)
                {
                    return new RegistrationResult { IsSuccess = false, Message = "Error: Host profile not found for this user." };
                }

                // 3. التحقق من أن الرول "Host" فعلاً
                if (hostProfile.User?.UserType != "Host")
                {
                    return new RegistrationResult { IsSuccess = false, Message = "Error: User is not registered as a Host." };
                }

                // 💡 النقطة الجوهرية: نأخذ الـ Id الحقيقي لجدول HostProfiles 
                // ونضعه في الـ HostId الخاص بالعقار

                var category = _unitOfWork.Categories.GetById(dto.CategoryId);
                if (category == null)
                    return new RegistrationResult { IsSuccess = false, Message = "Error: Category not found." };

                // 2. إنشاء معرف العقار أولاً لربط باقي الجداول به
                var listingEntity = new Listing
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    BasePrice = dto.BasePrice,
                    CategoryId = dto.CategoryId,
                    Status = "Active",
                    CreatedAtUtc = DateTime.UtcNow,

                    // الربط عن طريق الكائن (Navigation Property)
                    // هذا يضمن أن EF سيأخذ الـ Id الصحيح (PK) من الكائن المجلوب
                    HostId = hostProfile.Id,

                    Location = new Location
                    {
                        City = dto.City,
                        CountryCode = dto.CountryCode,
                        StreetAddress = dto.StreetAddress,
                        CreatedAtUtc = DateTime.UtcNow
                    },

                    Images = dto.ImageUrls.Select(url => new ListingImage
                    {
                        Url = url,
                        CreatedAtUtc = DateTime.UtcNow
                    }).ToList()
                };

                // 3. الإضافة والحفظ
                _unitOfWork.Listings.Add(listingEntity);

                int result = _unitOfWork.Complete();

                return result > 0
                    ? new RegistrationResult { IsSuccess = true, Message = "Success!", UserId = listingEntity.Id }
                    : new RegistrationResult { IsSuccess = false, Message = "No changes saved." };

            }
            catch (Exception ex)
            {

                return new RegistrationResult { IsSuccess = false, Message = ex.InnerException?.Message ?? ex.Message };
            }
        }

        //public IEnumerable<ListingCreateDto> SearchListings(ListingSearchParams searchParams)
        //{
        //    // 1. جلب العقارات مع كل الجداول المرتبطة (Location, Category, Host, Images)
        //    // نستخدم Include لضمان عدم حدوث N+1 query problem
        //    var query = _unitOfWork.Listings.GetAll(
        //        l => l.Location,
        //        l => l.Category,
        //        l => l.Host,
        //        l => l.Images // بفرض أن جدول الصور اسمه Images
        //    ).Where(l => l.Status == "Published" && !l.IsDeleted);

        //    // 2. فلترة حسب المكان (البحث في جدول Location المرتبط)
        //    // 2. فلترة حسب المكان مع حماية من الـ Null
        //    if (!string.IsNullOrEmpty(searchParams.Location))
        //    {
        //        query = query.Where(l => l.Location != null && (
        //                                 l.Location.City.Contains(searchParams.Location) ||
        //                                 l.Location.StreetAddress.Contains(searchParams.Location) ||
        //                                 l.Location.CountryCode.Contains(searchParams.Location)));
        //    }

        //    // 3. فلترة حسب التصنيف
        //    if (!string.IsNullOrEmpty(searchParams.Category))
        //    {
        //        query = query.Where(l => l.Category.Name == searchParams.Category);
        //    }

        //    // 4. فلترة حسب السعر
        //    if (searchParams.MinPrice.HasValue)
        //        query = query.Where(l => l.BasePrice >= searchParams.MinPrice.Value);

        //    if (searchParams.MaxPrice.HasValue)
        //        query = query.Where(l => l.BasePrice <= searchParams.MaxPrice.Value);

        //    // 5. فلترة التواريخ (استبعاد العقارات التي لديها حجز متداخل)
        //    if (searchParams.CheckIn.HasValue && searchParams.CheckOut.HasValue)
        //    {
        //        var reservedListingIds = _unitOfWork.Bookings.GetAll()
        //            .Where(b => (searchParams.CheckIn < b.CheckOut && searchParams.CheckOut > b.CheckIn))
        //            .Select(b => b.ListingId)
        //            .ToList();

        //        query = query.Where(l => !reservedListingIds.Contains(l.Id));
        //    }

        //    // 6. التحويل إلى DTO مع جلب بيانات الهوست من جدول المستخدمين
        //    return query.Select(l =>
        //    {
        //        // جلب بيانات المستخدم (FullName) المرتبط بالهوست
        //        // ملاحظة: إذا كان Host يملك علاقة Navigation مع User، استخدم l.Host.User.FullName
        //        var hostUser = _unitOfWork.Users.GetById(l.Host.UserId);

        //        return new ListingCreateDto
        //        {
        //            Id = l.Id,
        //            Title = l.Title,
        //            Description = l.Description,
        //            BasePrice = l.BasePrice,
        //            HostId = l.HostId,
        //            HostName = l.Host.User.FullName,
        //            // إضافة حقل جديد في الـ DTO أو استخدامه إذا كان موجوداً لعرض اسم المضيف
        //            CategoryName = l.Category?.Name ?? "General",
        //            CategoryId = l.CategoryId,

        //            // بيانات الموقع من جدول Location
        //            City = l.Location?.City ?? "N/A",
        //            CountryCode = l.Location?.CountryCode ?? "N/A",
        //            StreetAddress = l.Location?.StreetAddress ?? "N/A",

        //            // جلب روابط الصور من جدول الصور المرتبط
        //            ImageUrls = l.Images.Select(img => img.Url).ToList()
        //        };
        //    }).ToList();
        //}

        public IEnumerable<ListingCreateDto> SearchListings(ListingSearchParams searchParams)
        {
            // 1. جلب البيانات باستخدام Include لكل الجداول المطلوبة بما فيها Host.User
            // هذا يضمن أن البيانات تأتي في استعلام SQL واحد (JOIN)
            var query = _unitOfWork.Listings.GetQueryable(
                l => l.Location,
                l => l.Category,
                l => l.Host.User,  // جلب بيانات المستخدم المرتبط بالمضيف هنا
                l => l.Images
            ).Where(l => l.Status == "Published" && !l.IsDeleted);

            // 2. فلترة حسب المكان
            if (!string.IsNullOrEmpty(searchParams.Location))
            {
                query = query.Where(l => l.Location != null && (
                    l.Location.City.Contains(searchParams.Location) ||
                    l.Location.StreetAddress.Contains(searchParams.Location) ||
                    l.Location.CountryCode.Contains(searchParams.Location)));
            }

            // 3. فلترة حسب التصنيف
            if (!string.IsNullOrEmpty(searchParams.Category))
            {
                query = query.Where(l => l.Category.Name == searchParams.Category);
            }

            // 4. فلترة حسب السعر
            if (searchParams.MinPrice.HasValue)
                query = query.Where(l => l.BasePrice >= searchParams.MinPrice.Value);

            if (searchParams.MaxPrice.HasValue)
                query = query.Where(l => l.BasePrice <= searchParams.MaxPrice.Value);

            // 5. فلترة التواريخ (تم تحويلها لـ IQueryable لتنفيذها داخل الـ SQL)
            if (searchParams.CheckIn.HasValue && searchParams.CheckOut.HasValue)
            {
                var reservedListingIds = _unitOfWork.Bookings.GetQueryable()
                    .Where(b => (searchParams.CheckIn < b.CheckOut && searchParams.CheckOut > b.CheckIn))
                    .Select(b => b.ListingId); // بدون .ToList() لتبقى داخل الـ Query

                query = query.Where(l => !reservedListingIds.Contains(l.Id));
            }

            // 6. التحويل إلى DTO (الآن كل البيانات جاهزة في الذاكرة بفضل الـ Includes)
            return query.Select(l => new ListingCreateDto
            {
                Id = l.Id,
                Title = l.Title,
                Description = l.Description,
                BasePrice = l.BasePrice,
                HostId = l.HostId,
                // الوصول المباشر للبيانات بدون طلبات SQL إضافية
                HostName = l.Host.User != null ? l.Host.User.FullName : "Unknown",
                CategoryName = l.Category != null ? l.Category.Name : "General",
                CategoryId = l.CategoryId,
                City = l.Location != null ? l.Location.City : "N/A",
                CountryCode = l.Location != null ? l.Location.CountryCode : "N/A",
                StreetAddress = l.Location != null ? l.Location.StreetAddress : "N/A",
                ImageUrls = l.Images.Select(img => img.Url).ToList()
            }).ToList();
        }

        public IEnumerable<ListingCreateDto> GetAllListings()
        {
            var listings = _unitOfWork.Listings.GetAll(
                l => l.Location,
                l => l.Images
            );

            return listings.Select(l => new ListingCreateDto
            {
                Id = l.Id, // <--- السطر السحري اللي هيحل مشكلة الـ undefined
                Title = l.Title,
                Description = l.Description,
                BasePrice = l.BasePrice,
                HostId = l.HostId,
                CategoryId = l.CategoryId,

                City = l.Location?.City ?? "N/A",
                CountryCode = l.Location?.CountryCode ?? "N/A",
                StreetAddress = l.Location?.StreetAddress ?? "N/A",

                ImageUrls = l.Images.Select(img => img.Url).ToList(),
            }).ToList();
        }
        public IEnumerable<ListingCardDto> GetListingsByCategory(Guid categoryId)
        {
            // 1. استخدام GetQueryable للتنفيذ داخل قاعدة البيانات (SQL side)
            return _unitOfWork.Listings.GetQueryable(
                l => l.Location,
                l => l.Images,
                l => l.Category,
                l => l.Host.User
            )
            // 2. تطبيق شروط الفلترة الصارمة
            .Where(l => l.CategoryId == categoryId &&
                        !l.IsDeleted &&
                        l.Status == "Published" &&
                        l.Location != null)

            // 3. ترتيب النتائج (الأحدث أولاً)
            .OrderByDescending(l => l.CreatedAtUtc)

            // 4. تحويل البيانات إلى ListingCardDto (أو ListingCreateDto حسب حاجتك)
            .Select(l => new ListingCardDto
            {
                Id = l.Id,
                Title = l.Title,
                Description = l.Description,
                BasePrice = l.BasePrice,
                HostName = l.Host != null && l.Host.User != null ? l.Host.User.FullName : "Unknown Host",

                // تنظيف البيانات من نصوص "string" الافتراضية
                CategoryName = (l.Category != null && l.Category.Name != "string") ? l.Category.Name : "General",
                City = (l.Location != null && l.Location.City != "string") ? l.Location.City : "Unknown",
                CountryCode = (l.Location != null && l.Location.CountryCode != "string") ? l.Location.CountryCode : "N/A",

                // جلب روابط الصور المتاحة فقط
                ImageUrls = l.Images
                             .Where(img => !string.IsNullOrEmpty(img.Url) && img.Url != "string")
                             .Select(img => img.Url)
                             .ToList()
            })
            .ToList();
        }

        public IEnumerable<ListingCardDto> GetListingsPaged(int page, int pageSize)
        {
            // 1. بناء الاستعلام مع تضمين كافة الجداول المرتبطة
            var query = _unitOfWork.Listings.GetQueryable(
                l => l.Location,
                l => l.Images, // بفرض استمرار استخدام جدول الصور المنفصل
                l => l.Category,
                l => l.Host.User
            );

            return query
                // 2. تصفية المصدر: استبعاد المحذوف، غير المنشور، ومن ليس لديه صور أو موقع
                .Where(l => !l.IsDeleted &&
                            l.Status == "Published" &&
                            l.Images.Any() &&
                            l.Location != null)

                // 3. الترتيب والتقسيم (Pagination)
                .OrderByDescending(l => l.CreatedAtUtc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)

                // 4. تحويل البيانات (Projection) وتنظيفها
                .Select(l => new ListingCardDto
                {
                    Id = l.Id,
                    Title = l.Title,
                    Description = l.Description,
                    BasePrice = l.BasePrice,
                    HostName = l.Host != null && l.Host.User != null ? l.Host.User.FullName : "Unknown Host",

                    // تنظيف بيانات الفئة (Category)
                    CategoryName = (l.Category != null && l.Category.Name != "string") ? l.Category.Name : "General",

                    // تنظيف بيانات الموقع (Location)
                    City = (l.Location != null && l.Location.City != "string") ? l.Location.City : "Unknown",
                    CountryCode = (l.Location != null && l.Location.CountryCode != "string") ? l.Location.CountryCode : "N/A",

                    // فلترة الصور والتأكد من صحة الروابط
                    ImageUrls = l.Images
                                 .Where(img => !string.IsNullOrEmpty(img.Url) && img.Url != "string")
                                 .Select(img => img.Url)
                                 .ToList()
                })
                .ToList();
        }
    }
}
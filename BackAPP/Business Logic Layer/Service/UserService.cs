using Business_Logic_Layer.DTOs;
using Business_Logic_Layer.DTOs.API;
using Business_Logic_Layer.Interfaces;
using Business_Logic_Layer.Service.util;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;

namespace Business_Logic_Layer.Services
{
    public class UserService : IUserService
    {
        private readonly UnitOfWork _unitOfWork;

        public UserService()
        {
            _unitOfWork = new UnitOfWork();
        }
        public string GetUserRole(Guid userId)
        {
            var user = _unitOfWork.Users.GetById(userId);
            return user?.UserType; // إرجاع نوع المستخدم (Admin/Host/Guest) أو null إذا لم يتم العثور عليه
        }
        public RegistrationResult MakeUserAHost(Guid userId)
        {
            // 1. التأكد أن المستخدم موجود أصلاً
            var user = _unitOfWork.Users.GetById(userId);
            if (user == null)
                return new RegistrationResult { IsSuccess = false, Message = "User not found" };

            // 2. التحقق ما إذا كان هوست بالفعل لتجنب التكرار
            // التعديل هنا: نستخدم GetAll() بدون باراميترات ثم نستخدم .Where من LINQ للفلترة
            var existingHost = _unitOfWork.HostProfiles
                .GetAll()
                .FirstOrDefault(h => h.UserId == userId);

            if (existingHost != null)
                return new RegistrationResult { IsSuccess = true, Message = "User is already a host", UserId = existingHost.Id };

            // 3. إنشاء الـ HostProfile
            var hostProfile = new HostProfile
            {
                Id = Guid.NewGuid(), // يفضل توليده يدوياً إذا كنت ستستخدمه فوراً في النتيجة
                UserId = userId,
                CreatedAtUtc = DateTime.UtcNow
            };

            // 4. تحديث حالة المستخدم
            user.UserType = "Host";

            _unitOfWork.HostProfiles.Add(hostProfile);
            _unitOfWork.Users.Update(user);

            int result = _unitOfWork.Complete();

            if (result > 0)
            {
                return new RegistrationResult
                {
                    IsSuccess = true,
                    Message = "You are now a host!",
                    UserId = hostProfile.Id
                };
            }

            return new RegistrationResult { IsSuccess = false, Message = "Registration failed." };
        }
        public RegistrationResult RegisterUser(UserRegisterDto userDto)
        {
            // 1. التحقق من الإيميل (كما هو في كودك)
            var existingUsers = _unitOfWork.Users.GetAll(u => u.Contact);
            if (existingUsers.Any(u => u.Contact?.PrimaryEmail == userDto.Email))
                return new RegistrationResult { IsSuccess = false, Message = "Error: Email is already registered." };


            // 2. إنشاء اليوزر (User Entity)
            var userEntity = new User
            {
                Id = Guid.NewGuid(),
                FullName = userDto.FullName,
                UserType = userDto.UserType,
                AvatarUrl = userDto.AvatarUrl ?? "https://img.icons8.com/?size=100&id=HEBTcR9O3uzR&format=png&color=000000", // صوره افتراضيه لو مبعتش
                CreatedAtUtc = DateTime.UtcNow,
                IsDeleted = false,
                Password = PasswordHelper.HashPassword(userDto.Password)

            };

            // 3. إنشاء بيانات الاتصال (Contact Entity)
            var contactEntity = new Contact
            {
                Id = Guid.NewGuid(),
                UserId = userEntity.Id,
                PrimaryEmail = userDto.Email,
                PhoneNumber = userDto.PhoneNumber
            };

            // --- الخطوة المفقودة: إنشاء الـ HostProfile ---
            if (userDto.UserType == "Host") // تأكد من مطابقة الكلمة Admin/Host/Guest
            {
                var hostProfileEntity = new HostProfile
                {
                    UserId = userEntity.Id, // نستخدم نفس الـ Id لليوزر ليكون الربط 1:1
                    //Bio = "Welcome to my profile!", // بيانات افتراضية
                    CreatedAtUtc = DateTime.UtcNow,
                    // أي حقول إضافية في جدول الـ HostProfile عندك
                };
                _unitOfWork.HostProfiles.Add(hostProfileEntity);
            }
            // --------------------------------------------

            // 4. الحفظ الكل في وقت واحد (Atomic Transaction)
            _unitOfWork.Users.Add(userEntity);
            _unitOfWork.Contacts.Add(contactEntity);

            int result = _unitOfWork.Complete();

            if (result > 0)
            {
                return new RegistrationResult
                {
                    IsSuccess = true,
                    Message = userDto.UserType == "Host" ? "Host registered successfully with profile!" : "User registered successfully!",
                    UserId = userEntity.Id
                };
            }

            return new RegistrationResult { IsSuccess = false, Message = "Registration failed." };
        }
        //public IEnumerable<UserRegisterDto> GetAllUsers()
        //{
        //    return _unitOfWork.Users.GetAll().Select(u => new UserRegisterDto
        //    {
        //        FullName = u.FullName,
        //        UserType = u.UserType
        //        // Note: Email and Phone need "Include" in Repository to show up here
        //    }).ToList();
        //}
        public IEnumerable<UserRegisterDto> GetAllUsers()
        {
            // 1. نستخدم الميثود التي تدعم الـ includes وندخل اسم جدول الـ Contact
            var users = _unitOfWork.Users.GetAll(u => u.Contact);

            // 2. نقوم بعمل الـ Mapping الآن
            return users.Select(u => new UserRegisterDto
            {
                FullName = u.FullName,
                UserType = u.UserType,
                AvatarUrl = u.AvatarUrl,
                // الآن u.Contact لن تكون null لأننا عملنا لها Include
                Email = u.Contact?.PrimaryEmail,
                PhoneNumber = u.Contact?.PhoneNumber
            }).ToList();
        }
        public UserProfileDto GetUserById(Guid id)
        {
            // جلب المستخدم مع بيانات التواصل
            var user = _unitOfWork.Users.GetById(id, u => u.Contact);
            if (user == null) return null;

            return new UserProfileDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Contact?.PrimaryEmail,
                PhoneNumber = user.Contact?.PhoneNumber,
                Role = user.UserType, // إرسال الرول للمساعدة في حماية الـ Frontend
                AvatarUrl = user.AvatarUrl // لو متاح عندك في الموديل
            };
        }

        //public void MigratePasswords()
        //{
        //    // 1. جلب المستخدمين الذين لديهم كلمة المرور P@ssword123 تحديداً
        //    // هذا سيجعل العملية أسرع بكثير
        //    var usersToMigrate = _unitOfWork.Users.GetAll()
        //        .Where(u => u.Password == "P@ssword123");

        //    int count = 0;
        //    foreach (var user in usersToMigrate)
        //    {
        //        // 2. التشفير
        //        string hashed = PasswordHelper.HashPassword("P@ssword123");

        //        // 3. التحديث
        //        user.Password = hashed;
        //        _unitOfWork.Users.Update(user);

        //        count++;
        //        // طباعة في الـ Output window لمعرفة التقدم
        //        System.Diagnostics.Debug.WriteLine($"Migrated user {user.Id}: {count} users processed.");
        //    }

        //    // 4. الحفظ مرة واحدة فقط خارج الحلقة
        //    _unitOfWork.Complete();
        //    System.Diagnostics.Debug.WriteLine("Migration Finished successfully!");
        //}
    }
}
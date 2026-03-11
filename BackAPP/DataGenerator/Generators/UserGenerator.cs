using Bogus;
using Data_Access_Layer.Models;

namespace DataGenerator.Generators
{
    public static class UserGenerator
    {
        // قائمة لتخزين الروابط محلياً ليتمكن Faker من سحبها بشكل متزامن
        private static List<string> _cachedAvatars = new();
        private static int _avatarCounter = 0;

        /// <summary>
        /// يجب مناداة هذه الميثود مرة واحدة فقط في بداية الـ Seeding لتعبئة الصور
        /// </summary>
        public static async Task InitializeAvatarsAsync(int count)
        {
            _cachedAvatars = await ImageGenerator.GenerateHostAvatarUrlsAsync(count);
            _avatarCounter = 0;
        }

        public static Faker<User> CreateUserFaker()
        {
            return new Faker<User>()
                //.RuleFor(u => u.Id, f => Guid.NewGuid())
                .RuleFor(u => u.FullName, f => f.Name.FullName())
                .RuleFor(u => u.Password, f => "P@ssword123")
                .RuleFor(u => u.UserType, f => f.PickRandom("Host", "Guest"))

                // سحب الصورة من الكاش المحمل مسبقاً
                .RuleFor(u => u.AvatarUrl, f =>
                {
                    if (_cachedAvatars == null || !_cachedAvatars.Any())
                        return f.Internet.Avatar(); // Fallback في حال لم يتم التهيئة

                    var url = _cachedAvatars[_avatarCounter % _cachedAvatars.Count];
                    _avatarCounter++;
                    return url;
                })

                .RuleFor(u => u.Contact, (f, u) => new Contact
                {
                   // Id = Guid.NewGuid(),
                    //UserId = u.Id,
                    PrimaryEmail = f.Internet.Email(),
                    PhoneNumber = f.Phone.PhoneNumber("###-###-####")
                })
                .RuleFor(u => u.CreatedAtUtc, f => f.Date.Past(1));
        }
    }
}
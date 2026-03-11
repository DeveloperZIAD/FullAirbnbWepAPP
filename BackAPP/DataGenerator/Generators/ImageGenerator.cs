using Bogus;
using System.Net.Http.Json;

namespace DataGenerator.Generators
{
    public static class ImageGenerator
    {
        // 1. ضع مفتاح الـ API الخاص بك هنا
        private static readonly string PexelsApiKey = Environment.GetEnvironmentVariable("PEXELS_API_KEY");
        private static readonly HttpClient _httpClient = new();
        private static readonly Faker _faker = new();

        private static readonly string[] SearchQueries =
{
    // دافئة ومريحة
    "cozy modern living room fireplace",
    "warm scandinavian bedroom evening light",
    "cozy cabin interior wood beams",
    "inviting living room plush sofa",
    "cozy reading nook window seat",
    // دافئة ومريحة (Warm & Cozy)
    "cozy fireplace living room evening glow",
    "soft lighting bedroom wooden accents",
    "warm cottage interior candlelight",
    "inviting sofa corner blanket throw",
    "cozy window seat rainy day",

    // عصرية وبسيطة (Modern & Minimal)
    "clean minimalist apartment daylight",
    "modern kitchen island white cabinets",
    "minimalist bedroom grey linen bedding",
    "open loft space concrete walls",
    "scandi style living room oak furniture",

    // فاخرة وأنيقة (Luxury & Elegant)
    "luxury penthouse marble floors",
    "elegant dining room chandelier",
    "high-end bedroom velvet headboard",
    "sophisticated villa interior sunset",
    "luxury hotel suite panoramic view",

    // طبيعية ومفتوحة (Natural & Open)
    "bright open living room skylights",
    "natural wood bedroom indoor plants",
    "airy home with large glass doors",
    "rustic modern interior stone fireplace",
    "sunlit countryside bedroom flowers",

    // شتوية وجبلية (Winter & Mountain)
    "cozy mountain cabin snow outside",
    "alpine chalet wood paneling",
    "winter bedroom thick duvet",
    "ski chalet living room warm rug",
    "snowy modern cabin glass walls",

    // بحرية وشاطئية (Beach & Coastal)
    "beachfront living room sea breeze",
    "coastal bedroom seashell decor",
    "tropical villa open air design",
    "seaside apartment white shutters",
    "hamptons coastal home blue tones",

    // ريفية ومزرعة (Farmhouse & Rural)
    "farmhouse kitchen butcher block",
    "rustic barn home reclaimed wood",
    "country cottage bedroom quilt",
    "vintage farmhouse living room",
    "barn conversion open kitchen",

    // إضافات متنوعة عالية الجودة (Diverse Premium)
    "neutral palette luxury bedroom",
    "boho living room rattan furniture",
    "industrial loft exposed brick",
    "japanese zen bedroom low bed",
    "mid-century modern walnut furniture",
    "scandinavian dining wood table",
    "california modern indoor outdoor",
    "mediterranean home terracotta tiles",
    "desert retreat adobe walls",
    "jungle bungalow thatched roof interior",

    // عصرية وبسيطة
    "minimalist luxury apartment interior",
    "modern open plan living kitchen",
    "contemporary white bedroom natural light",
    "sleek modern loft high ceilings",
    "minimal boho living room neutral tones",

    // فاخرة وأنيقة
    "luxury vacation villa interior",
    "elegant marble kitchen island",
    "high-end modern penthouse living room",
    "luxurious master bedroom ocean view",
    "chic boutique hotel style suite",

    // طبيعية ومفتوحة
    "bright airy living room large windows",
    "natural light bedroom plants decor",
    "open concept home forest view",
    "rustic modern interior wooden floors",
    "light-filled countryside living space",

    // شتوية وجبلية
    "winter cozy cabin interior snow view",
    "mountain chalet living room fireplace",
    "arctic style bedroom warm blankets",
    "ski lodge interior wood fireplace",
    "snowy cabin modern rustic design",

    // بحرية وشاطئية
    "beach house living room ocean view",
    "coastal bedroom blue white decor",
    "tropical vacation rental interior",
    "seaside modern apartment balcony",
    "hamptons style beach house living",

    // ريفية ومزرعة
    "farmhouse kitchen open shelves",
    "rustic barn conversion living room",
    "countryside cottage bedroom flowers",
    "vintage farmhouse interior cozy",
    "barn style home exposed beams",

    // إضافات متنوعة عالية الجودة
    "neutral toned luxury living room",
    "boho chic bedroom macrame decor",
    "industrial loft apartment interior",
    "japanese minimal bedroom tatami",
    "mid century modern living room",
    "scandinavian kitchen wood accents",
    "californian coastal home interior",
    "mediterranean villa living space",
    "desert modern adobe style home",
    "tropical jungle bungalow interior",
};

        private static readonly string[] HostAvatarKeywords =
        {
    "smiling friendly host portrait",
    "welcoming man headshot natural light",
    "warm smile woman professional portrait",
    "confident host face close-up",
    "cheerful man friendly smile",
    "happy female host natural smile",
    "trustworthy man relaxed portrait",
    "elegant woman gentle expression",
    "approachable host warm headshot",
    "genuine smile man casual portrait",
    "man",
    "reliable host confident expression",
    "friendly man golden hour portrait",
    "joyful woman soft lighting face",
    "charismatic host natural smile",
    "kind man welcoming expression",
    "relaxed host outdoor close-up",
    "professional woman bright smile",
    "warm-hearted man portrait",
    "inviting female host headshot"
};
        /// <summary>
        /// يجلب روابط صور حقيقية من Pexels API بناءً على الكلمات الدلالية.
        /// </summary>
        public static async Task<List<string>> GenerateImageUrlsAsync(int count)
        {
            var imageUrls = new HashSet<string>(); // استخدام HashSet لمنع أي تكرار برمجياً
            int retryCount = 0;

            // سنحاول ملء القائمة حتى نصل للعدد المطلوب
            while (imageUrls.Count < count && retryCount < 5)
            {
                // 1. تنويع كلمات البحث في كل دورة
                var query = _faker.PickRandom(SearchQueries);

                // 2. طلب الصور بصفحة عشوائية (page) لضمان الحصول على صور جديدة في كل مرة
                int randomPage = _faker.Random.Int(1, 50);
                var requestUrl = $"https://api.pexels.com/v1/search?query={Uri.EscapeDataString(query)}&per_page={count}&page={randomPage}";

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", PexelsApiKey);

                try
                {
                    var response = await _httpClient.GetFromJsonAsync<PexelsResponse>(requestUrl);
                    if (response?.Photos != null)
                    {
                        foreach (var photo in response.Photos)
                        {
                            imageUrls.Add(photo.Src.Large); // HashSet يمنع إضافة الرابط إذا كان موجوداً
                            if (imageUrls.Count >= count) break;
                        }
                    }
                }
                catch { /* تجاهل الأخطاء المؤقتة للـ API */ }

                retryCount++;
            }

            // 3. إذا لم نصل للعدد المطلوب بعد محاولات، نكمل بـ Fallback متنوع جداً (Seed فريد)
            while (imageUrls.Count < count)
            {
                var seed = Guid.NewGuid().ToString(); // استخدام GUID كـ Seed لضمان عدم التكرار نهائياً
                imageUrls.Add($"https://picsum.photos/seed/{seed}/1200/800");
            }

            return imageUrls.ToList();
        }


        public static async Task<List<string>> GenerateHostAvatarUrlsAsync(int count)
        {
            var avatarUrls = new List<string>();

            // 1. اختيار كلمة بحث عشوائية لوصف المضيفين
            var query = _faker.PickRandom(HostAvatarKeywords);

            // 2. نطلب عدد صور يساوي 'count' لضمان أن كل مستخدم يحصل على صورة فريدة
            var requestUrl = $"https://api.pexels.com/v1/search?query={Uri.EscapeDataString(query)}&per_page={count}";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", PexelsApiKey);

            try
            {
                var response = await _httpClient.GetFromJsonAsync<PexelsResponse>(requestUrl);

                if (response?.Photos != null && response.Photos.Any())
                {
                    // 3. نقوم باستخراج الروابط من الصور المرجعة
                    foreach (var photo in response.Photos)
                    {
                        // نفضل جودة Large2x أو Large للـ Avatar لضمان الوضوح
                        var url = photo.Src.Large;
                        avatarUrls.Add(url);
                    }

                    // 4. في حالة كان عدد الصور من الـ API أقل من المطلوب، نكمل بـ Fallback
                    while (avatarUrls.Count < count)
                    {
                        var seed = _faker.Random.Int(1, 10000);
                        avatarUrls.Add($"https://picsum.photos/seed/host{seed}/400/400");
                    }
                }
                else
                {
                    throw new Exception("Pexels returned empty photo list.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Pexels API Error for Host avatars: {ex.Message}. Switching to fallback.");

                // 5. Fallback الاحترافي في حالة فشل الـ API أو الـ Key
                for (int i = 0; i < count; i++)
                {
                    var seed = _faker.Random.Int(1, 10000);
                    // نستخدم picsum مع تحديد مقاس مربع 400x400 لأنه مثالي للـ Avatar
                    avatarUrls.Add($"https://picsum.photos/seed/h{seed}/400/400");
                }
            }

            return avatarUrls;
        }        // كلاسات مساعدة لترجمة رد الـ API (DTOs)
        private class PexelsResponse
        {
            public List<PexelsPhoto> Photos { get; set; }
        }

        private class PexelsPhoto
        {
            public PexelsSrc Src { get; set; }
        }

        private class PexelsSrc
        {
            public string Large { get; set; }
        }
    }
}
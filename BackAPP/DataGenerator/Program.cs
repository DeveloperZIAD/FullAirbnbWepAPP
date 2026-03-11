//using Data_Access_Layer.Data;
//using Data_Access_Layer.Entities;
//using DataGenerator;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;

//class Program
//{
//    static async Task Main(string[] args)
//    {
//        var connectionString = "Data Source=.;Initial Catalog=AirbnbClone;Integrated Security=True;TrustServerCertificate=True;";

//        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
//             .UseSqlServer(connectionString)
//             // إعادة تفعيل الـ Log لمشاهدة الكويري التي تفشل بالتفصيل
//             .LogTo(Console.WriteLine, LogLevel.Information)
//             .EnableSensitiveDataLogging()
//             .Options;

//        using var context = new ApplicationDbContext(options);

//        // نمرر الـ context للمحرك لكي يسحب التصنيفات
//        var engine = new DataEngine(context);

//        try
//        {
//            // --- [الخطوة 0] تأمين التصنيفات الأساسية (Seeding) ---
//            Console.WriteLine("\n=== [Step 0] Checking/Seeding Categories ===");

//            if (!await context.Categories.AnyAsync())
//            {
//                var seedCategories = new List<Category>
//                {
//                    new Category { Id = Guid.NewGuid(), Name = "Amazing Pools", CreatedAtUtc = DateTime.UtcNow },
//                    new Category { Id = Guid.NewGuid(), Name = "Beachfront", CreatedAtUtc = DateTime.UtcNow },
//                    new Category { Id = Guid.NewGuid(), Name = "Cabins", CreatedAtUtc = DateTime.UtcNow },
//                    new Category { Id = Guid.NewGuid(), Name = "Countryside", CreatedAtUtc = DateTime.UtcNow },
//                    new Category { Id = Guid.NewGuid(), Name = "Mansions", CreatedAtUtc = DateTime.UtcNow },
//                    new Category { Id = Guid.NewGuid(), Name = "Castles", CreatedAtUtc = DateTime.UtcNow }
//                };

//                await context.Categories.AddRangeAsync(seedCategories);
//                await context.SaveChangesAsync();
//                Console.WriteLine($"Successfully seeded {seedCategories.Count} categories.");
//            }
//            else
//            {
//                Console.WriteLine("Categories already exist. Skipping seed...");
//            }

//            // --- [الخطوة 1] توليد البيانات في الذاكرة ---
//            Console.WriteLine("\n=== [Step 1] Generating Data in Memory ===");
//            var result = await engine.GenerateSystemDataAsync(5);

//            // --- [الخطوة 2] تتبع الكيانات ---
//            Console.WriteLine("\n=== [Step 2] Tracking Entities ===");
//            foreach (var user in result.Users)
//            {
//                context.Users.Add(user);
//            }
//            foreach (var listing in result.Listings)
//            {
//                context.Listings.Add(listing);
//            }

//            // --- [الخطوة 3] الحفظ في قاعدة البيانات ---
//            Console.WriteLine("\n=== [Step 3] Saving to Database ===");
//            int rowsAffected = await context.SaveChangesAsync();

//            Console.ForegroundColor = ConsoleColor.Green;
//            Console.WriteLine($"\n[SUCCESS] Done! {rowsAffected} rows affected.");
//            Console.ResetColor();
//        }
//        catch (DbUpdateException dbEx)
//        {
//            Console.ForegroundColor = ConsoleColor.Red;
//            Console.WriteLine("\n--- [DETAILED DATABASE ERROR] ---");

//            var message = dbEx.InnerException?.Message ?? dbEx.Message;
//            Console.WriteLine($"Actual SQL Error: {message}");

//            // تحليل المشكلة: هل السبب هو IconUrl؟
//            if (message.Contains("IconUrl"))
//            {
//                Console.WriteLine("\n[HINT]:  'IconUrl' .");
//                Console.WriteLine("Category.cs  [NotMapped]  IconUrl.");
//            }

//            Console.ResetColor();
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"\n[GENERAL ERROR]: {ex.Message}");
//        }

//        Console.WriteLine("\nPress any key to exit...");
//        Console.ReadKey();
//    }
//}
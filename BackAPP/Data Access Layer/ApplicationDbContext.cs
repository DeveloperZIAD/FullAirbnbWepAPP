using Data_Access_Layer.Entities;
using Data_Access_Layer.Models;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Xml.Linq;

namespace Data_Access_Layer.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() { }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // --- الجداول (DbSets) ---
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<HostProfile> HostProfiles { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Listing> Listings { get; set; }
        public DbSet<ListingImage> ListingImages { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Amenity> Amenities { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<AdminActivityLog> AdminActivityLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // 1. محاولة جلب الـ ConnectionString من متغيرات البيئة أولاً (الأولوية للإنتاج)
                string connString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

                // 2. إذا لم نجدها في المتغيرات، نذهب لقراءتها من ملف الـ XML (للتطوير المحلي)
                if (string.IsNullOrWhiteSpace(connString))
                {
                    string xmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configurations.xml");

                    if (!File.Exists(xmlPath))
                        throw new FileNotFoundException("File Configurations.xml not found", xmlPath);

                    XElement xml = XElement.Load(xmlPath);
                    connString = xml.Element("ConnectionString")?.Value;

                    if (string.IsNullOrWhiteSpace(connString))
                        throw new Exception("ConnectionString is missing in Configurations.xml");
                }

                // 3. تمرير السلسلة (سواء كانت من البيئة أو من الملف) لـ Entity Framework
                optionsBuilder.UseSqlServer(connString);

                // ملاحظة: يمكنك ترك هذه الأسطر للـ Debugging فقط ولا تستخدمها في الـ Production
                // .LogTo(message => System.Diagnostics.Debug.WriteLine(message), Microsoft.Extensions.Logging.LogLevel.Information)
                // .EnableSensitiveDataLogging();
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1. ربط الـ Listing بالـ HostProfile كما هو في الواقع (UserId)
            modelBuilder.Entity<Listing>(entity =>
            {
                entity.HasOne(l => l.Host)
                      .WithMany(h => h.Listings)
                      .HasForeignKey(l => l.HostId); // الربط الطبيعي على الـ Primary Key (Id)
            });

            // 2. تعريف جدول الهوست بروفايل كما هو في الـ DB الآن
            modelBuilder.Entity<HostProfile>()
            .Property(h => h.Id)
            .ValueGeneratedOnAdd(); // يخبر EF بتوليد القيمة فقط للسجلات الجديدة

            // 3. تعريف الـ Location
            modelBuilder.Entity<Location>(entity =>
            {
                entity.ToTable("Locations");
                entity.HasKey(e => e.ListingId);
                entity.Property(e => e.ListingId).ValueGeneratedNever();
                entity.Ignore(e => e.Id);
            });

            // 4. بقية الجداول بأسماءها الصريحة لتجنب الجمع (Pluralization) التلقائي
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Contact>().ToTable("Contacts");
            modelBuilder.Entity<Booking>().ToTable("Bookings");
            modelBuilder.Entity<Category>().ToTable("Categories");
            modelBuilder.Entity<Amenity>().ToTable("Amenities");
            modelBuilder.Entity<ListingImage>().ToTable("ListingImages");

            // messages system
            modelBuilder.Entity<Conversation>()
             .HasOne(c => c.Listing)
             .WithMany() // أو أضف مجموعة Conversations في كلاس Listing إذا أردت
             .HasForeignKey(c => c.ListingId)
             .OnDelete(DeleteBehavior.Cascade);

            // ضبط علاقة المحادثة مع الضيف (User)
            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.Guest)
                .WithMany()
                .HasForeignKey(c => c.GuestId)
                .OnDelete(DeleteBehavior.Restrict);

            // ضبط علاقة المحادثة مع المضيف (HostProfile)
            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.HostProfile)
                .WithMany()
                .HasForeignKey(c => c.HostProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            // ضبط علاقة الرسالة مع المحادثة
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
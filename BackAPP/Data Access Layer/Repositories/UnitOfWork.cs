using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using System;

namespace Data_Access_Layer.Repositories
{
    public class UnitOfWork : IDisposable
    {
        private readonly ApplicationDbContext _context;

        // 1. تعريف الـ Repositories لكل الجداول (14 جدول)
        public IGenericRepository<UserRefreshToken> UserRefreshTokens { get; private set; }
        public IGenericRepository<Conversation> Conversations { get; private set; }
        public IGenericRepository<Message> Messages { get; private set; }
        public IGenericRepository<User> Users { get; private set; }
        public IGenericRepository<Contact> Contacts { get; private set; }
        public IGenericRepository<HostProfile> HostProfiles { get; private set; }
        public IGenericRepository<Admin> Admins { get; private set; }
        public IGenericRepository<Listing> Listings { get; private set; }
        public IGenericRepository<ListingImage> ListingImages { get; private set; }
        public IGenericRepository<Location> Locations { get; private set; }
        public IGenericRepository<Category> Categories { get; private set; }
        public IGenericRepository<Amenity> Amenities { get; private set; }
        public IGenericRepository<Booking> Bookings { get; private set; }
        public IGenericRepository<Payment> Payments { get; private set; }
        public IGenericRepository<Review> Reviews { get; private set; }
        public IGenericRepository<Wishlist> Wishlists { get; private set; }
        public IGenericRepository<AdminActivityLog> AdminActivityLogs { get; private set; }

        public UnitOfWork()
        {
            _context = new ApplicationDbContext();

            // 2. تهيئة الـ Repositories وربطها بنفس الـ
            Conversations = new GenericRepository<Conversation>(_context);
            Messages = new GenericRepository<Message>(_context);
            UserRefreshTokens = new GenericRepository<UserRefreshToken>(_context);
            Users = new GenericRepository<User>(_context);
            Contacts = new GenericRepository<Contact>(_context);
            HostProfiles = new GenericRepository<HostProfile>(_context);
            Admins = new GenericRepository<Admin>(_context);
            Listings = new GenericRepository<Listing>(_context);
            ListingImages = new GenericRepository<ListingImage>(_context);
            Locations = new GenericRepository<Location>(_context);
            Categories = new GenericRepository<Category>(_context);
            Amenities = new GenericRepository<Amenity>(_context);
            Bookings = new GenericRepository<Booking>(_context);
            Payments = new GenericRepository<Payment>(_context);
            Reviews = new GenericRepository<Review>(_context);
            Wishlists = new GenericRepository<Wishlist>(_context);
            AdminActivityLogs = new GenericRepository<AdminActivityLog>(_context);
        }

        // 3. حفظ كل التغييرات التي تمت في الـ Repositories دفعة واحدة
        public int Complete()
        {
            return _context.SaveChanges();
        }

        // 4. تنظيف الـ Context من الذاكرة بعد الانتهاء
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}


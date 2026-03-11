using Data_Access_Layer.Entities;

public class User : BaseEntity
{
    public string FullName { get; set; } = null!;
    public string? UserType { get; set; } = "Guest"; // Guest, Host, Admin
    public string? AvatarUrl { get; set; }

    // Relationships
    public virtual Contact Contact { get; set; } = null!;
    public virtual HostProfile? HostProfile { get; set; }
    public virtual Admin? AdminProfile { get; set; }
    public virtual ICollection<Booking> Bookings { get; set; } = new HashSet<Booking>();
    public virtual ICollection<Wishlist> Wishlists { get; set; } = new HashSet<Wishlist>();
    public string? Password { get; set; }
   // public virtual ICollection<Listing> Listings { get; set; } = new HashSet<Listing>();
}
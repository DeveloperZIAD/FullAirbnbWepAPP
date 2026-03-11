using Data_Access_Layer.Entities;

public class Booking : BaseEntity
{


    public Guid ListingId { get; set; }
    public Guid GuestId { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public decimal PricePerNightAtBooking { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = "Pending";

    public virtual Listing? Listing { get; set; } = null!;
    public virtual User? Guest { get; set; } = null!;
    public virtual Payment? Payment { get; set; }
}
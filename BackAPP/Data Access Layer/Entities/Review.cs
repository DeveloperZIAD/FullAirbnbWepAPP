namespace Data_Access_Layer.Entities
{
    public class Review : BaseEntity
    {
       // public Guid? BookingId { get; set; }
        public Guid ListingId { get; set; }
        public Guid UserID { get; set; }
        public int Rating { get; set; } // من 1 لـ 5
        public string? Comment { get; set; }

       // public virtual Booking Booking { get; set; } = null!;
        public virtual Listing Listing { get; set; } = null!;
        public virtual User Author { get; set; } = null!;
    }
}
namespace Data_Access_Layer.Entities
{
    public class Wishlist: BaseEntity
    {
        // Composite Key (UserId + ListingId)
        public Guid UserId { get; set; }
        public Guid ListingId { get; set; }
        public DateTime CreatedAtUtc { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual Listing Listing { get; set; } = null!;
    }
}
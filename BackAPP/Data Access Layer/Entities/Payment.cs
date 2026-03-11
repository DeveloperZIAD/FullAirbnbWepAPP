namespace Data_Access_Layer.Entities
{
    public class Payment : BaseEntity
    {
        public Guid BookingId { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string Provider { get; set; } = null!; // Stripe, PayPal
        public string? TransactionId { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Completed, Failed

        public virtual Booking Booking { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
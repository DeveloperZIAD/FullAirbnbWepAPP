namespace Business_Logic_Layer.DTOs
{
    public class PaymentProcessDto
    {
        public Guid BookingId { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } // USD, EGP, etc.
        public string Provider { get; set; } // Stripe, PayPal, Visa
        public string TransactionId { get; set; } // الرقم القادم من بوابة الدفع
    }
}
namespace Business_Logic_Layer.DTOs
{
    public class BookingCreateDto
    {
        public Guid ListingId { get; set; }
        public Guid GuestId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
    }
    public class BookingDto
    {
        public Guid Id { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        // بيانات الـ Listing اللي محتاجها في الفرونت إند بس
        public string ListingTitle { get; set; }
        public string ListingImage { get; set; }
        public string City { get; set; }
        public string? CountryCode { get; set; }
        public Guid? GuestId { get; set; }
    }
}
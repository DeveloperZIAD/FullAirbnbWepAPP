namespace Business_Logic_Layer.DTOs
{
    public class ReviewCreateDto
    {
        public Guid ListingId { get; set; }
        public Guid GuestId { get; set; }
        public int Rating { get; set; } // من 1 إلى 5
        public string Comment { get; set; }
    }
}
namespace Business_Logic_Layer.DTOs
{
    public class HostProfileDto
    {
        public Guid? id { get; set; }
        public Guid UserId { get; set; }
        public string TaxId { get; set; }
        public bool IsVerified { get; set; }
        // يمكن إضافة نبذة عن المضيف أو لغات يتحدثها هنا مستقبلاً
    }
    public class ListingDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public decimal BasePrice { get; set; }
        // لا تضع المضيف (Host) هنا لتجنب الحلقة المرجعية
        public List<string> ImageUrls { get; set; }
        public string City { get; set; }
    }
}
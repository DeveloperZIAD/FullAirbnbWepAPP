namespace Business_Logic_Layer.DTOs
{
    public class ListingImageDto
    {
        public Guid ListingId { get; set; }
        public string Url { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsMain { get; set; } // لتحديد الصورة التي تظهر كغلاف للعقار
    }
}
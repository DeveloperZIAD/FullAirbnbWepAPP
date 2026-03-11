namespace Data_Access_Layer.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = null!;
       // public string? IconUrl { get; set; }

        // العلاقة مع العقارات
        public virtual ICollection<Listing> Listings { get; set; } = new HashSet<Listing>();
    }
}
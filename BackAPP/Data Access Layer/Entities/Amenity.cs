namespace Data_Access_Layer.Entities
{
    public class Amenity : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string? IconClass { get; set; }

        // علاقة Many-to-Many مع العقارات
       // public virtual ICollection<Listing> Listings { get; set; } = new HashSet<Listing>();
    }
}
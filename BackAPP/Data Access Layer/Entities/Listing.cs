using Data_Access_Layer.Entities;
using System.ComponentModel.DataAnnotations.Schema;

public class Listing : BaseEntity
{
    [ForeignKey("HostId")]
    public Guid HostId { get; set; }
    public Guid CategoryId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public decimal BasePrice { get; set; }
    public string Status { get; set; } = "Published";


    
    public virtual HostProfile Host { get; set; } = null!;
    public virtual Category? Category { get; set; } = null!;
    public virtual Location Location { get; set; } = null!;
    public virtual ICollection<ListingImage> Images { get; set; } = new HashSet<ListingImage>();
    public virtual ICollection<Amenity>? Amenities { get; set; } = //  no relation so it can be null add ? to make it nullable
        new HashSet<Amenity>();
}
using Data_Access_Layer.Entities;

public class ListingImage : BaseEntity
{
    public Guid ListingId { get; set; }
    public string? Url { get; set; } = null!;
    public int DisplayOrder { get; set; }
    public bool IsMain { get; set; }

    public virtual Listing Listing { get; set; } = null!;
}
using Data_Access_Layer.Entities;
using System.ComponentModel.DataAnnotations.Schema;

public class HostProfile : BaseEntity
{   [ForeignKey("UserId")]
    public Guid UserId { get; set; }
    public string? TaxId { get; set; }
    public bool IsVerified { get; set; }
    
    public virtual User User { get; set; } = null!;
    public virtual ICollection<Listing> Listings { get; set; } = new HashSet<Listing>();
}
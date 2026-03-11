using Data_Access_Layer.Entities;
using System.ComponentModel.DataAnnotations.Schema;

public class Contact : BaseEntity
{
    public Guid UserId { get; set; }
    public string PrimaryEmail { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string? EmergencyContact { get; set; }
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
}
using Data_Access_Layer.Entities;

public class AdminActivityLog:BaseEntity
{
    public Guid Id { get; set; }
    public Guid? AdminId { get; set; }
    public string AdminName { get; set; } = null!;
    public string Action { get; set; } = null!;
    public string EntityName { get; set; } = null!;
    public Guid EntityId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
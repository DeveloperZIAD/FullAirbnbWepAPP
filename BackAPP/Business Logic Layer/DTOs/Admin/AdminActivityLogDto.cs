public class AdminActivityLogDto
{
    public Guid AdminId { get; set; }
    public string Action { get; set; } // e.g., "Deleted Listing", "Verified Host"
    public string EntityName { get; set; }
    public DateTime Timestamp { get; set; }
}



namespace Business_Logic_Layer.DTOs.Admin
{
    public class AdminProfileDto
    {
        public Guid UserId { get; set; }
        public string Role { get; set; } // SuperAdmin, ContentModerator, Support
        public string Permissions { get; set; } // JSON string or comma-separated roles
        public bool IsActive { get; set; }
        public DateTime? LastLogin { get; set; }
    }
}
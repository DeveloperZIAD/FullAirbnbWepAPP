namespace Data_Access_Layer.Entities
{
    public class Admin : BaseEntity
    {
        public Guid UserId { get; set; }
        public string Role { get; set; } = "Moderator"; // SuperAdmin, Moderator
        public string? Permissions { get; set; } // يتم تخزينها كـ JSON
        public bool IsActive { get; set; } = true;
        public DateTime? LastLogin { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
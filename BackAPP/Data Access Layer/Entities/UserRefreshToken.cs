// في ملف Entities
using Data_Access_Layer.Entities;
public class UserRefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsRevoked { get; set; } = false;

    // Foreign Key
    public Guid UserId { get; set; }
    public User User { get; set; } // Navigation Property
}
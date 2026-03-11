using Data_Access_Layer.Entities;

namespace Data_Access_Layer.Models
{
    public class UserToken : BaseEntity
    {
        public string AccessToken { get; set; } // اختياري: لتتبع آخر توكن
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public bool IsRevoked { get; set; } // لإلغاء التوكن يدوياً عند الضرورة

        // الربط مع المستخدم
        public Guid UserId { get; set; }
        public User User { get; set; }
        public virtual ICollection<UserToken> UserTokens { get; set; } = new HashSet<UserToken>();
    }
}
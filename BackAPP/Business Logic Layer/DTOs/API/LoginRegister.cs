namespace Business_Logic_Layer.DTOs
{
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class AuthResponseDto
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime Expiration { get; set; }
        public string UserType { get; set; }
        public UserDataDto User { get; set; } // البيانات المطلوبة للـ Navbar
    }
    public class UserDataDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string ImageSrc { get; set; }
        public string Email { get; set; }
    }
}
using System.Text.Json.Serialization;
   using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;
namespace Business_Logic_Layer.DTOs
{
 

    public class UserRegisterDto
    {
        [Required(ErrorMessage = "Full Name is required")]
        [StringLength(100, MinimumLength = 3)]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]

        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        [JsonIgnore]
        public string UserType { get; set; }

        public string? AvatarUrl { get; set; } 
    }

    public class GuestRegisterDto : UserRegisterDto
    {
        public GuestRegisterDto()
        {
            UserType = "Guest"; // القيمة ثابتة لهذا الـ DTO دائماً
        }
    }

    public class HostRegisterDto : UserRegisterDto
    {
        public HostRegisterDto()
        {
            UserType = "Host";
        }
    }

    public class UserProfileDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; } // هنا هنبعت الـ Role (Admin/User/Host)
        public string? AvatarUrl { get; set; }
    }
}

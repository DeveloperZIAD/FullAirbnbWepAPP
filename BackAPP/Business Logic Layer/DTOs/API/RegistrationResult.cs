using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.DTOs.API
{
    public class RegistrationResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public Guid? UserId { get; set; } // أو int حسب نوع الـ ID عندك
    }

    public class RegistrationResultBooking
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public Guid? bookingId { get; set; } // أو int حسب نوع الـ ID عندك
    }

}

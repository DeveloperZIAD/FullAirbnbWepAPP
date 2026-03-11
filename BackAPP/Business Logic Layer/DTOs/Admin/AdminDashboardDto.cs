namespace Business_Logic_Layer.DTOs
{
    public class AdminDashboardDto
    {
        public int TotalUsers { get; set; }
        public int TotalListings { get; set; }
        public int ActiveBookings { get; set; }
        public decimal TotalRevenue { get; set; }

        public IEnumerable<UserDto> RecentUsers { get; set; }
        public IEnumerable<BookingDto> RecentBookings { get; set; }
    }

    public class UserDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime RegisteredAt { get; set; }

        public bool IsBlocked { get; set; }

    }
}
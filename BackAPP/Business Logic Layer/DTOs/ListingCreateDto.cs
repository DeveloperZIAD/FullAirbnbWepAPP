namespace Business_Logic_Layer.DTOs
{
    public class ListingCreateDto
    {
        public Guid? Id { get; set; } 
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }
        public Guid HostId { get; set; }
        public Guid CategoryId { get; set; }
        public string? CategoryName { get; set; } // To be used in React for dropdown display

        // Location Info (To be saved in Location Table)
        public string City { get; set; }
        public string CountryCode { get; set; }
        public string StreetAddress { get; set; }

        public List<string> ImageUrls { get; set; } = new List<string>();

        public string? HostName { get; set; } // أضف هذا لعرض اسم المضيف في نتائج البحث

        public LocationDto? Location { get; set; } = null!;
        //public List<Guid> AmenityIds { get; set; } = new List<Guid>();
        public HostDetailsDto? Host { get; set; } = null!;

        public int? GuestCount { get; set; }
        public int? RoomCount { get; set; }
        public int? BathroomCount { get; set; }
    }
    public class LocationDto
    {
        public string CountryCode { get; set; } = null!;
        public string City { get; set; } = null!;
        public string StreetAddress { get; set; } = null!;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
    public class HostDetailsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public string? PhoneNumber { get; set; }
    }

    public class SimpleListingCreateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }
        public Guid HostId { get; set; } // الـ UserId الخاص بالمضيف
        public string CategoryName { get; set; } // سنبحث عنها بالاسم
        public string City { get; set; }
        public string CountryCode { get; set; }
        public string StreetAddress { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();
        public int GuestCount { get; set; }
        public int RoomCount { get; set; }
        public int BathroomCount { get; set; }
    }

    public class ListingCardDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }
        public string HostName { get; set; }
        public string CategoryName { get; set; }
        public string City { get; set; }
        public string CountryCode { get; set; }
        public List<string> ImageUrls { get; set; }
    }
}
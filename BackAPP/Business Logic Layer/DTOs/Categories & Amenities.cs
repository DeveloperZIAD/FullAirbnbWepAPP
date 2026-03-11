// CategoryDto.cs
namespace Business_Logic_Layer.DTOs
{
    public class CategoryDto
    {
        public Guid Id { get; set; } // نحتاجه عند العرض والاختيار
        public string Name { get; set; }
        public string IconUrl { get; set; }
    }
}

// AmenityDto.cs
namespace Business_Logic_Layer.DTOs
{
    public class AmenityDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string IconClass { get; set; }
    }
}
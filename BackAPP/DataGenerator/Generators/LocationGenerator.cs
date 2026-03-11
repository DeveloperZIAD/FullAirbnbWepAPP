using Bogus;

namespace DataGenerator.Generators
{
    // DTO محلي خاص بمشروع الـ Generator فقط
    public class AddressGeneratorDto
    {
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public static class LocationGenerator
    {
        public static Faker<AddressGeneratorDto> CreateAddressFaker()
        {
            return new Faker<AddressGeneratorDto>()
                .RuleFor(a => a.Country, f => f.Address.Country())
                .RuleFor(a => a.City, f => f.Address.City())
                .RuleFor(a => a.Street, f => f.Address.StreetAddress())
                .RuleFor(a => a.Latitude, f => f.Address.Latitude())
                .RuleFor(a => a.Longitude, f => f.Address.Longitude());
        }
    }
}
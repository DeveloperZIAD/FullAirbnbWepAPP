using System.ComponentModel.DataAnnotations;

namespace Data_Access_Layer.Entities
{
    public class Location:BaseEntity
    {

        
        public Guid ListingId { get; set; }
        public string CountryCode { get; set; } = null!; // مثال: EGY, USA
        public string City { get; set; } = null!;
        public string StreetAddress { get; set; } = null!;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }


        public virtual Listing Listing { get; set; } = null!;

    }
}
using Business_Logic_Layer.DTOs;
using System.Collections.Generic;

namespace Business_Logic_Layer.Interfaces
{
    public interface IListingAmenityService
    {
        string AssignAmenitiesToListing(Guid listingId, List<Guid> amenityIds);
        IEnumerable<AmenityDto> GetAmenitiesByListing(Guid listingId);
    }
}
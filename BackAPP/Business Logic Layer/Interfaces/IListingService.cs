using Business_Logic_Layer.DTOs;
using Business_Logic_Layer.DTOs.API;
using System.Collections.Generic;

namespace Business_Logic_Layer.Interfaces
{
    public interface IListingService
    {
        RegistrationResult AddListing(ListingCreateDto listingDto);
        IEnumerable<ListingCreateDto> GetAllListings();
        IEnumerable<ListingCardDto> GetListingsByCategory(Guid categoryId);
    }
}
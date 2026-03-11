using Business_Logic_Layer.DTOs;
using System.Collections.Generic;

namespace Business_Logic_Layer.Interfaces
{
    public interface IListingImageService
    {
        string AddImages(List<ListingImageDto> imageDtos);
        IEnumerable<ListingImageDto> GetImagesByListingId(Guid listingId);
        bool DeleteImage(Guid imageId);
    }
}
using Business_Logic_Layer.DTOs;
using Business_Logic_Layer.DTOs.API;
using Data_Access_Layer.Entities;

namespace Business_Logic_Layer.Interfaces
{
    public interface IReviewService
    {
        (Review review, string message, string userImage, string userName) AddReview(ReviewCreateDto reviewDto);
        IEnumerable<ReviewReadDto> GetListingReviews(Guid listingId);
        double GetAverageRating(Guid listingId);
    }
}
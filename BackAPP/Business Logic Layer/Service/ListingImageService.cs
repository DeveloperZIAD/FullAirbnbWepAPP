using Business_Logic_Layer.DTOs;
using Business_Logic_Layer.Interfaces;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;

namespace Business_Logic_Layer.Services
{
    public class ListingImageService : IListingImageService
    {
        private readonly UnitOfWork _unitOfWork;

        public ListingImageService()
        {
            _unitOfWork = new UnitOfWork();
        }

        public string AddImages(List<ListingImageDto> imageDtos)
        {
            if (imageDtos == null || !imageDtos.Any())
                return "Error: No images provided.";

            foreach (var dto in imageDtos)
            {
                var imageEntity = new ListingImage
                {
                    Id = Guid.NewGuid(),
                    ListingId = dto.ListingId,
                    Url = dto.Url,
                    DisplayOrder = dto.DisplayOrder,
                    IsMain = dto.IsMain,
                    CreatedAtUtc = DateTime.UtcNow
                };
                _unitOfWork.ListingImages.Add(imageEntity);
            }

            int result = _unitOfWork.Complete();
            return result > 0 ? "Images added successfully!" : "Error: Failed to save images.";
        }

        public IEnumerable<ListingImageDto> GetImagesByListingId(Guid listingId)
        {
            return _unitOfWork.ListingImages.GetAll()
                .Where(img => img.ListingId == listingId)
                .OrderBy(img => img.DisplayOrder)
                .Select(img => new ListingImageDto
                {
                    ListingId = img.ListingId,
                    Url = img.Url,
                    DisplayOrder = img.DisplayOrder,
                    IsMain = img.IsMain
                }).ToList();
        }

        public bool DeleteImage(Guid imageId)
        {
            var image = _unitOfWork.ListingImages.GetById(imageId);
            if (image == null) return false;

            // In some systems we do soft delete, here we'll follow your DAL pattern
            image.IsDeleted = true;
            _unitOfWork.ListingImages.Update(image);
            return _unitOfWork.Complete() > 0;
        }
    }
}
using Business_Logic_Layer.DTOs;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;

public class AmenityService : IAmenityService
{
    private readonly UnitOfWork _unitOfWork;
    public AmenityService() => _unitOfWork = new UnitOfWork();

    public IEnumerable<AmenityDto> GetAllAmenities()
    {
        return _unitOfWork.Amenities.GetAll()
            .Select(a => new AmenityDto
            {
                Id = a.Id,
                Name = a.Name,
                IconClass = a.IconClass
            }).ToList();
    }

    public string AddAmenity(AmenityDto dto)
    {
        var amenity = new Amenity
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            IconClass = dto.IconClass
        };
        _unitOfWork.Amenities.Add(amenity);
        return _unitOfWork.Complete() > 0 ? "Amenity added" : "Error";
    }
}
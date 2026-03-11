using Business_Logic_Layer.DTOs;

public interface ICategoryService
{
    IEnumerable<CategoryDto> GetAllCategories();
    string AddCategory(CategoryDto categoryDto);
}

public interface IAmenityService
{
    IEnumerable<AmenityDto> GetAllAmenities();
    string AddAmenity(AmenityDto amenityDto);
}
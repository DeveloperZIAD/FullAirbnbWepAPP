using Business_Logic_Layer.DTOs;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;

public class CategoryService : ICategoryService
{
    private readonly UnitOfWork _unitOfWork;
    public CategoryService() => _unitOfWork = new UnitOfWork();

    public IEnumerable<CategoryDto> GetAllCategories()
    {
        return _unitOfWork.Categories.GetAll()
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                //IconUrl = c.IconUrl
            }).ToList();
    }

    public string AddCategory(CategoryDto dto)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            //IconUrl = dto.IconUrl
        };
        _unitOfWork.Categories.Add(category);
        return _unitOfWork.Complete() > 0 ? "Category added" : "Error";
    }
}
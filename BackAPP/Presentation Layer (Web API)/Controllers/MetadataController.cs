using Business_Logic_Layer.Services;
using Data_Access_Layer.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AirbnbClone.Api.Controllers
{//. MetadataController.cs (Categories & Amenities)
    [Route("api/[controller]")]
    [ApiController]
    public class MetadataController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public MetadataController()
        {
            _unitOfWork = new UnitOfWork();
        }

        // Get all categories for the filter bar
        [HttpGet("categories")]
        public IActionResult GetCategories()
        {
            var categories = _unitOfWork.Categories.GetAll().Where(c => !c.IsDeleted);
            return Ok(categories);
        }

        // Get all amenities for the creation form
        //[HttpGet("amenities")]
        //public IActionResult GetAmenities()
        //{
        //    var amenities = _unitOfWork.Amenities.GetAll().Where(a => !a.IsDeleted);
        //    return Ok(amenities);
        //}
    }
}
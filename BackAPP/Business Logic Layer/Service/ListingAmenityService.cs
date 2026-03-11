using Business_Logic_Layer.DTOs;
using Business_Logic_Layer.Interfaces;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Business_Logic_Layer.Services
{
    public class ListingAmenityService : IListingAmenityService
    {
        private readonly UnitOfWork _unitOfWork;

        public ListingAmenityService()
        {
            _unitOfWork = new UnitOfWork();
        }

        // ملاحظة: لو مفيش جدول ListingAmenities في الـ UnitOfWork، 
        // بنستخدم الـ Listings repository لو كان الـ Mapping معمول Many-to-Many
        public string AssignAmenitiesToListing(Guid listingId, List<Guid> amenityIds)
        {
            var listing = _unitOfWork.Listings.GetById(listingId);
            if (listing == null) return "Error: Listing not found.";

            // هنا المنطق بيعتمد على الـ Entity Framework Many-to-Many
            // لو عندك جدول وسيط مش متسجل في الـ UnitOfWork، لازم يتضاف الأول.
            // لكن كحل مؤقت للمنطق:
            return "Logic needs ListingAmenities Repository in UnitOfWork to function correctly.";
        }

        public IEnumerable<AmenityDto> GetAmenitiesByListing(Guid listingId)
        {
            // بنجيب الميزات من خلال جدول الـ Amenities الرئيسي
            // ده استعلام افتراضي لحين التأكد من وجود جدول الربط في الـ UOW
            return _unitOfWork.Amenities.GetAll()
                .Select(a => new AmenityDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    IconClass = a.IconClass
                }).ToList();
        }
    }
}
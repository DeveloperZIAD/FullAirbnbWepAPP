using Business_Logic_Layer.DTOs;
using Business_Logic_Layer.DTOs.API;
using Business_Logic_Layer.Interfaces;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Business_Logic_Layer.Services
{
    public class BookingService : IBookingService
    {
        private readonly UnitOfWork _unitOfWork;

        public BookingService()
        {
            _unitOfWork = new UnitOfWork();
        }//eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIyNzNhZDFkNC0zNDkyLTQyNDEtYmQ1Ny1iNjM5MjgyMzNjZGMiLCJlbWFpbCI6InVzYXcyZXJAZXhhbXBsZS5jb20iLCJyb2xlIjoiR3Vlc3QiLCJuYmYiOjE3NzIzMzUxNDMsImV4cCI6MTc3MjMzNjk0MywiaWF0IjoxNzcyMzM1MTQzfQ.eJQ7XTJN_Mi5fG5U2bgOYluSDI9ItgUksiYmNRK_Zck
        public IEnumerable<BookingDto> GetUserBookings(Guid userId)
        {
            return _unitOfWork.Bookings
                .GetAll(b => b.Listing, b => b.Listing.Location, b => b.Listing.Images)
                .Where(b => b.GuestId == userId)
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    CheckIn = b.CheckIn,
                    CheckOut = b.CheckOut,
                    TotalPrice = b.TotalPrice,
                    Status = b.Status,
                    ListingTitle = b.Listing.Title,
                    // الوصول للصور: نتأكد من وجود القائمة أولاً
                    ListingImage = b.Listing.Images.OrderBy(img => img.DisplayOrder)
                                                  .Select(img => img.Url)
                                                  .FirstOrDefault(),
                    // الوصول للمدينة: من خلال الـ Navigation Property "Location"
                    City = b.Listing.Location != null ? b.Listing.Location.City : "Unknown",
                    CountryCode = b.Listing.Location != null ? b.Listing.Location.CountryCode : ""
                })
                .ToList();
        }
        public RegistrationResultBooking CreateBooking(BookingCreateDto bookingDto)
        {
            // 1. Validate Dates
            if (bookingDto.CheckIn >= bookingDto.CheckOut)
            {
                return new RegistrationResultBooking
                {
                    IsSuccess = false,
                    Message = "Error: Check-out date must be after check-in date."
                };
            }

            if (bookingDto.CheckIn < DateTime.Now.Date)
            {
                return new RegistrationResultBooking
                {
                    IsSuccess = false,
                    Message = "Error: Cannot book a date in the past."
                };
            }

            // 2. Verify Guest Existence
            var guest = _unitOfWork.Users.GetById(bookingDto.GuestId);
            if (guest == null)
            {
                return new RegistrationResultBooking
                {
                    IsSuccess = false,
                    Message = "Error: Guest not found."
                };
            }

            // 3. Verify Listing Existence
            var listing = _unitOfWork.Listings.GetById(bookingDto.ListingId);
            if (listing == null)
            {
                return new RegistrationResultBooking
                {
                    IsSuccess = false,
                    Message = "Error: Listing not found."
                };
            }

            // 4. Map DTO to Entity
            var bookingEntity = new Booking
            {
                Id = Guid.NewGuid(),
                ListingId = bookingDto.ListingId,
                GuestId = bookingDto.GuestId,
                CheckIn = bookingDto.CheckIn,
                CheckOut = bookingDto.CheckOut,
                CreatedAtUtc = DateTime.UtcNow,
                IsDeleted = false,
                Status = "Pending",
                PricePerNightAtBooking = listing.BasePrice
            };

            // 5. Calculate total price
            int days = (bookingDto.CheckOut - bookingDto.CheckIn).Days;
            if (days <= 0) days = 1;

            bookingEntity.TotalPrice = days * listing.BasePrice;

            // 6. Save via UnitOfWork
            _unitOfWork.Bookings.Add(bookingEntity);
            int result = _unitOfWork.Complete();

            // 7. Final Result
            if (result > 0)
            {
                return new RegistrationResultBooking
                {
                    IsSuccess = true,
                    Message = "Booking created successfully!",
                    bookingId = bookingEntity.Id
                };
            }

            return new RegistrationResultBooking
            {
                IsSuccess = false,
                Message = "Error: Database save failed."
            };
        }
        public IEnumerable<BookingCreateDto> GetAllBookings()
        {
            // Mapping from Entity list to DTO list to keep the API layer clean
            var bookings = _unitOfWork.Bookings.GetAll();

            return bookings.Select(b => new BookingCreateDto
            {
                ListingId = b.ListingId,
                GuestId = b.GuestId,
                CheckIn = b.CheckIn,
                CheckOut = b.CheckOut
            }).ToList();
        }
        public bool UpdateBookingStatus(Guid bookingId, string status)
        {
            var booking = _unitOfWork.Bookings.GetById(bookingId);
            if (booking == null) return false;

            booking.Status = status; // تغيير من Pending لـ Confirmed
            _unitOfWork.Complete();
            return true;
        }
        public BookingDto GetBookingById(Guid bookingId)
        {
            return _unitOfWork.Bookings
                .GetAll(b => b.Listing, b => b.Listing.Location, b => b.Listing.Images)
                .Where(b => b.Id == bookingId)
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    CheckIn = b.CheckIn,
                    CheckOut = b.CheckOut,
                    TotalPrice = b.TotalPrice,
                    Status = b.Status,
                    ListingTitle = b.Listing.Title,
                    ListingImage = b.Listing.Images.OrderBy(i => i.DisplayOrder).Select(i => i.Url).FirstOrDefault(),
                    City = b.Listing.Location.City,
                    CountryCode = b.Listing.Location.CountryCode,
                    GuestId = b.GuestId // مهم جداً لعمل الـ Authorization في الكنترولر
                })
                .FirstOrDefault();
        }

        public bool CancelBooking(Guid bookingId)
        {
            var booking = _unitOfWork.Bookings.GetById(bookingId);
            if (booking != null)
            {
                booking.Status = "Cancelled";
                _unitOfWork.Bookings.Update(booking);
                return _unitOfWork.Complete() > 0;
            }
            return false;
        }
    }
}
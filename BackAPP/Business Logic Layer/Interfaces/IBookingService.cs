using Business_Logic_Layer.DTOs;
using Business_Logic_Layer.DTOs.API;
using System;
using System.Collections.Generic;

namespace Business_Logic_Layer.Interfaces
{
    public interface IBookingService
    {
        // Now it accepts BookingCreateDto instead of the heavy Booking Entity
        RegistrationResultBooking CreateBooking(BookingCreateDto bookingDto);

        // We can return a list of simplified DTOs for the view (optional) 
        // but for now, let's keep it as is or use a BookingReadDto
        BookingDto GetBookingById(Guid bookingId);
        IEnumerable<BookingCreateDto> GetAllBookings();

        bool CancelBooking(Guid bookingId);
        bool UpdateBookingStatus(Guid bookingId, string status);
    }
}
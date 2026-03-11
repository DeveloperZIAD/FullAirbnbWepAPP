using Business_Logic_Layer.DTOs;
using Data_Access_Layer.Entities;

namespace Business_Logic_Layer.Interfaces
{
    public interface IPaymentService
    {
        string ProcessPayment(PaymentProcessDto paymentDto);
        PaymentProcessDto GetPaymentDetails(Guid bookingId);
        Task<bool> CreatePaymentAsync(Payment payment);
    }
}
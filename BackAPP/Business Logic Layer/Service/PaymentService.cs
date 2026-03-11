using Business_Logic_Layer.DTOs;
using Business_Logic_Layer.Interfaces;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;

namespace Business_Logic_Layer.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly UnitOfWork _unitOfWork;

        public PaymentService()
        {
            _unitOfWork = new UnitOfWork();
        }
        public async Task<bool> CreatePaymentAsync(Payment payment)
        {
            try
            {
                // 1. إضافة سجل الدفع
                _unitOfWork.Payments.Add(payment);

                // 2. حفظ التغييرات في قاعدة البيانات
                int result = _unitOfWork.Complete();

                return result > 0;
            }
            catch (Exception ex)
            {
                // يمكنك تسجيل الخطأ هنا (Logging)
                Console.WriteLine($"Error saving payment: {ex.Message}");
                return false;
            }
        }
        public string ProcessPayment(PaymentProcessDto paymentDto)
        {
            // 1. التأكد من وجود الحجز
            var booking = _unitOfWork.Bookings.GetById(paymentDto.BookingId);
            if (booking == null) return "Error: Booking not found.";

            // 2. فحص ما إذا كان هناك دفع مسجل بالفعل لهذا الحجز أو لهذا الـ TransactionId
            // هذا يمنع خطأ الـ Duplicate Key تماماً
            var existingPayment = _unitOfWork.Payments.GetAll()
                .FirstOrDefault(p => p.BookingId == paymentDto.BookingId || p.TransactionId == paymentDto.TransactionId);

            if (existingPayment != null)
            {
                return "Payment already processed for this booking.";
            }

            // 3. إنشاء كائن الدفع (Payment Entity)
            var paymentEntity = new Payment
            {
               // Id = Guid.NewGuid(),
                BookingId = paymentDto.BookingId,
                UserId = paymentDto.UserId,
                Amount = paymentDto.Amount,
                Currency = paymentDto.Currency,
                Provider = paymentDto.Provider,
                TransactionId = paymentDto.TransactionId,
                Status = "Completed",
                CreatedAtUtc = DateTime.UtcNow
            };

            // 4. تحديث حالة الحجز
            booking.Status = "Confirmed";
            _unitOfWork.Bookings.Update(booking);

            // 5. إضافة السجل الجديد
            _unitOfWork.Payments.Add(paymentEntity);

            try
            {
                int result = _unitOfWork.Complete();
                return result > 0 ? "Payment processed and booking confirmed!" : "Error: No changes saved.";
            }
            catch (Exception ex)
            {
                // هنا ستمسك بأي خطأ قاعدة بيانات وتعرف تفاصيله
                return $"Error: Database update failed. {ex.InnerException?.Message ?? ex.Message}";
            }
        }
        public PaymentProcessDto GetPaymentDetails(Guid bookingId)
        {
            var payment = _unitOfWork.Payments.GetAll()
                .FirstOrDefault(p => p.BookingId == bookingId);

            if (payment == null) return null;

            return new PaymentProcessDto
            {
                BookingId = payment.BookingId,
                Amount = payment.Amount,
                Currency = payment.Currency,
                TransactionId = payment.TransactionId,
                Provider = payment.Provider
            };
        }
    }
}
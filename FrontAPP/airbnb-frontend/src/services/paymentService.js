import api from "./apiClient";

const paymentService = {
  processPayment: async (bookingId, paymentMethodId) => {
    // إرسال البيانات كـ Query Parameters بأسماء مطابقة للـ Swagger
    const response = await api.post(
      `/Payments/process`,
      null, // الـ Body فارغ لأننا نستخدم Query Params
      {
        params: {
          bookingId: bookingId, // تأكد أن الاسم يطابق الـ Parameter في .NET
          paymentMethodId: paymentMethodId,
        },
      },
    );
    return response.data;
  },

  // إذا كنت ستحتاج جلب تاريخ المدفوعات لاحقاً
  getPaymentDetails: async (paymentId) => {
    try {
      const response = await api.get(`/Payments/${paymentId}`);
      return response.data;
    } catch (error) {
      throw error;
    }
  },
};

export default paymentService;

import { get } from "react-hook-form";
import api from "./apiClient";
import authService from "./authService";

const bookingService = {
  // إنشاء حجز جديد
  createBooking: async (bookingData) => {
    // console.log("Booking Data Sent to API:", bookingData); // Debugging log
    const response = await api.post("/bookings/create", bookingData);
    return response.data;
  },
  getBookingById: async (id) => {
    try {
      // console.log("Fetching booking with ID:", id); // Debugging log
      const response = await api.get(`/Bookings/${id}`);
      return response.data;
    } catch (error) {
      console.error("Error fetching booking by ID:", error);
      throw error;
    }
  },
  // جلب حجوزات المستخدم الحالي (لصفحة Trips)
  getBookingByUserId: async () => {
    try {
      const user = authService.getCurrentUser();
      const userId = user?.id;

      if (!userId) {
        console.error("No User ID found in localStorage");
        return [];
      }

      console.log("Requesting bookings for ID:", userId);

      // تأكد من الـ Endpoint في الـ Swagger أولاً
      const response = await api.get(`/bookings/user/${userId}`);

      //console.log("API Response Data:", response.data);
      return response.data;
    } catch (error) {
      // هذا الجزء سيطبع لك تفاصيل الخطأ في الـ Console
      //console.error("Detailed API Error:", error.response || error);
      throw error; // لنسمح لـ TripsPage بإظهار الـ Toast
    }
  },
  // إلغاء حجز
  cancelBooking: async (bookingId) => {
    console.log("Cancelling Booking ID:", bookingId); // Debugging log
    const response = await api.delete(`Bookings/cancel/${bookingId}`);
    return response.data;
  },
};

export default bookingService;

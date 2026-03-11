// services/adminService.js
import api from "./apiClient";

const adminService = {
  // جلب كل المستخدمين (مع صلاحية الحذف)
  getAllUsers: async () => {
    const response = await api.get("/admin/users");
    return response.data;
  },

  // جلب كل العقارات (للمراجعة أو الحذف)
  getAllListings: async () => {
    const response = await api.get("/admin/listings");
    return response.data;
  },

  BlockUser: async (userId) => {
    const response = await api.post(`/admin/users/${userId}/block`);
    return response.data;
  },

  getActivityLogs: async () => {
    const response = await api.get("/admin/activity-logs");
    return response.data;
  },

  // جلب كل الحجوزات (الـ Reservations)
  getAllReservations: async () => {
    const response = await api.get("/admin/reservations");
    return response.data;
  },

  // إحصائيات سريعة للـ Dashboard
  getAllStats: async () => {
    const response = await api.get("/Admin/dashboard-stats");
    return response.data;
  },
};

export default adminService;

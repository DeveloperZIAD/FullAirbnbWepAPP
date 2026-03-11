// src/services/hostService.js
import api from "./apiClient";

const hostService = {
  // جلب إحصائيات المضيف
  getDashboardStats: async (userId) => {
    try {
      // 1. جلب بيانات الهوست للحصول على الـ hostId
      const hostRes = await api.get(`/Account/host/${userId}`);
      const hostId = hostRes.data.id;

      // 2. التعديل هنا: استخدام ?hostId= بدلاً من المائل /
      // الرابط الناتج: /Account/statsHost?hostId=0b913352...
      const response = await api.get(`/Account/statsHost`, {
        params: { hostId: hostId },
      });

      return response.data;
    } catch (error) {
      console.error("Error in getDashboardStats:", error);
      throw error;
    }
  },

  // جلب العقارات الخاصة بالمضيف
  getMyListings: async (userId) => {
    try {
      // 1. جلب بيانات الهوست للحصول على الـ hostId
      const hostRes = await api.get(`/Account/host/${userId}`);
      const hostId = hostRes.data.id;

      // 2. التعديل هنا أيضاً: استخدام نظام الـ Params
      // الرابط الناتج: /Account/listingsHost?hostId=0b913352...
      const response = await api.get(`/Account/listingsHost`, {
        params: { hostId: hostId },
      });

      return response.data;
    } catch (error) {
      console.error("Error in getMyListings:", error);
      throw error;
    }
  },
  upgradeToHost: async (userId) => {
    try {
      const response = await api.post(`/Account/upgrade-to-host/${userId}`);
      console.log("Host registration response:", response.data);
      return response.data;
    } catch (error) {
      console.error("Error registering host:", error);
      throw error;
    }
  },

  // حذف قائمة عقار حسب ID
};

export default hostService;

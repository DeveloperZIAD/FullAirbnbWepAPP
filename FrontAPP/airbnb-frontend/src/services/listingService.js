import api from "./apiClient";

const listingService = {
  // 1. جلب كل العقارات (للسياح والزوار)
  getAllListings: async () => {
    try {
      const response = await api.get("/Listings/all");
      return response.data;
    } catch (error) {
      console.error("Error fetching listings:", error);
      throw error;
    }
  },

  getAllListingsWithDetails: async () => {
    try {
      const response = await api.get("/Listings/all"); // تأكد من المسار الصحيح في الباك-إند
      return response.data;
    } catch (error) {
      console.error("Error fetching listings with details:", error);
      throw error;
    }
  },
  // 2. جلب تفاصيل عقار معين (بما فيها الـ Images والـ Location)
  getListingById: async (id) => {
    try {
      const response = await api.get(`/Listings/${id}`);
      return response.data;
    } catch (error) {
      console.error(`Error fetching listing ${id}:`, error);
      throw error;
    }
  },

  // 3. جلب عقارات "الهوست" الحالي فقط
  // هنا الباك-إند هيستخدم التوكن عشان يعرف مين اليوزر ويجيب حاجته
  getMyListings: async () => {
    try {
      const response = await api.get("/listings/my-listings");
      return response.data;
    } catch (error) {
      console.error("Error fetching my listings:", error);
      throw error;
    }
  },

  // 4. إضافة عقار جديد (اللي جربناها في سواجر ونجحت!)
  // داخل listingService.js
  createListing: async (listingData) => {
    try {
      // Axios سيرسل هذا كـ JSON تلقائياً وبضبط Content-Type: application/json
      const response = await api.post("/Listings/create", listingData);
      return response.data;
    } catch (error) {
      console.error("Error creating listing:", error.response?.data || error); // طباعة تفاصيل الخطأ من السيرفر
      throw error;
    }
  },

  // داخل listingService.js
  updateListing: async (id, data) => {
    const response = await api.put(`/Listings/update/${id}`, data);
    return response.data;
  },
  searchListings: async (query) => {
    try {
      // التصحيح: نستخدم مفتاح params لتمرير المتغيرات في طلب GET
      const response = await api.get("/Listings/search", {
        params: query,
      });

      // console.log("Search results:", response.data);
      return response.data;
    } catch (error) {
      console.error("Error searching listings:", error);
      throw error;
    }
  },
  getListingsPaged: async (page, pageSize) => {
    const response = await api.get(
      `/listings?page=${page}&pageSize=${pageSize}`,
    );
    return response.data;
  },

  createReservation: async (reservationData) => {
    try {
      // سيتم إرسال التوكن تلقائياً عبر الـ Interceptor الذي أعددناه
      const response = await api.post("/listings/create", reservationData);
      return response.data;
    } catch (error) {
      console.error("Error creating reservation:", error);
      throw error;
    }
  },
  deleteListing: async (listingId) => {
    try {
      const response = await api.delete(`/listings/delete/${listingId}`);
      console.log("Delete response:", response.data);
      return response.data;
    } catch (error) {
      console.error("Error deleting listing:", error);
      throw error;
    }
  },
};

export default listingService;

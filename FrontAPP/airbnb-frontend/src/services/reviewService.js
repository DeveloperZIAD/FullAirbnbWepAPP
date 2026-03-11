import api from "./apiClient";
import authService from "./authService";

const reviewService = {
  // إضافة تقييم جديد
  addReview: async (reviewData) => {
    // التحقق من أن جميع الحقول المطلوبة موجودة
    const currentUser = authService.getCurrentUser(); // تأكد من أن المستخدم مسجل الدخول
    if (!currentUser) {
      throw new Error("User is not authenticated");
    }

    const payload = {
      listingId: reviewData.listingId,
      guestId: currentUser.id, // استخدم ID المستخدم الحالي
      rating: reviewData.rating,
      comment: reviewData.comment,
    };

    const response = await api.post("/Reviews/add", payload);
    return response.data;
  },

  // جلب جميع التقييمات لعقار معين
  getReviewsByListing: async (listingId) => {
    const response = await api.get(`/Reviews/listing/${listingId}`);
    return response.data;
  },

  // حذف تقييم (لصاحب التقييم فقط)
  deleteReview: async (reviewId) => {
    const response = await api.delete(`/Reviews/delete/${reviewId}`);
    return response.data;
  },
};

export default reviewService;

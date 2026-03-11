import axios from "axios";

const api = axios.create({
  baseURL: process.env.REACT_APP_API_URL || "http://localhost:5000/api",
  withCredentials: true, // مهم جداً للتعامل مع الـ CORS والـ Sessions
});

// إضافة التوكن للطلبات الصادرة
api.interceptors.request.use((config) => {
  const token = localStorage.getItem("accessToken");
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// معالجة الردود (الاستجابة لخطأ 401)
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    // إذا كان الخطأ 401 (Unauthorized) ولم نقم بالمحاولة مسبقاً
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true; // نضع علامة لعدم الدخول في حلقة مفرغة

      try {
        const refreshToken = localStorage.getItem("refreshToken");
        const accessToken = localStorage.getItem("accessToken");

        if (!refreshToken) throw new Error("No refresh token available");

        console.log(
          "🔄 Access token expired. Attempting refresh..." +
            " the refreshToken " +
            refreshToken +
            " the accessToken " +
            accessToken,
        );

        // نستخدم axios العادي هنا وليس api (لتجنب الـ interceptors الحالية)
        const response = await axios.post(
          "http://localhost:5000/api/Auth/refresh-token",
          {
            accessToken: accessToken,
            refreshToken: refreshToken,
          },
        );

        if (response.data.token) {
          console.log("✅ Token refreshed successfully!");

          // تحديث البيانات الجديدة (تأكد من المسميات القادمة من الباك اند)
          localStorage.setItem("accessToken", response.data.token);
          if (response.data.refreshToken) {
            localStorage.setItem("refreshToken", response.data.refreshToken);
          }

          // إعادة إرسال الطلب الأصلي بالتوكن الجديد
          originalRequest.headers.Authorization = `Bearer ${response.data.token}`;
          return api(originalRequest);
        }
      } catch (refreshError) {
        console.error("❌ Refresh token expired or invalid. Logging out...");
        localStorage.clear();
        // window.location.href = "/login"; // اختياري: توجيه المستخدم لصفحة الدخول
        return Promise.reject(refreshError);
      }
    }

    return Promise.reject(error);
  },
);

export default api;

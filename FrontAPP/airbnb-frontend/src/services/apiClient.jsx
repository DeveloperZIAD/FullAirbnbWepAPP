import axios from "axios";

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || "http://localhost:5000/api",
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

    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      try {
        const refreshToken = localStorage.getItem("refreshToken");
        const accessToken = localStorage.getItem("accessToken");

        if (!refreshToken) throw new Error("No refresh token available");

        // استخدام api.post بدلاً من axios.post لضمان استخدام الـ baseURL الصحيح
        // ملاحظة: الرابط هنا مكمل للـ baseURL (أي نكتب /Auth/refresh-token فقط)
        const response = await api.post("/Auth/refresh-token", {
          accessToken: accessToken,
          refreshToken: refreshToken,
        });

        if (response.data.token) {
          localStorage.setItem("accessToken", response.data.token);
          if (response.data.refreshToken) {
            localStorage.setItem("refreshToken", response.data.refreshToken);
          }

          originalRequest.headers.Authorization = `Bearer ${response.data.token}`;
          return api(originalRequest);
        }
      } catch (refreshError) {
        console.error("❌ Refresh token expired. Logging out...");
        localStorage.clear();
        window.location.reload(); // لإعادة توجيه المستخدم لصفحة تسجيل الدخول
        return Promise.reject(refreshError);
      }
    }
    return Promise.reject(error);
  },
);

export default api;

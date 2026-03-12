import api from "./apiClient";

const authService = {
  // 1. التسجيل + تسجيل دخول تلقائي
  register: async (userData) => {
    const response = await api.post("/Account/register/guest", userData);

    // الباك-إند يرسل userId
    const userId = response.data?.userId;

    if (userId) {
      const userToSave = {
        id: userId, // التزمنا بالاسم id كما طلبت
        email: userData.email,
        fullName: userData.fullName || "User",
        imageSrc: userData.imageSrc || "",
      };

      localStorage.setItem("user", JSON.stringify(userToSave));
      console.log("User saved to storage:", userToSave);

      // ثم تسجيل الدخول
      return await authService.login({
        email: userData.email,
        password: userData.password,
      });
    }
    return response.data;
  },
  login: async (credentials) => {
    try {
      const response = await api.post("/Auth/login", credentials);
      const data = response.data; // هنا يوجد الكائن: { token, refreshToken, user: { id, fullName, email, imageSrc } }

      const token = data.token || data.accessToken;
      const refreshToken = data.refreshToken;

      if (token) {
        localStorage.setItem("accessToken", token);
        if (refreshToken) {
          localStorage.setItem("refreshToken", refreshToken);
        }

        // حفظ كائن المستخدم القادم من الباك-إند مباشرة
        if (data.user) {
          const userToSave = {
            id: data.user.id,
            fullName: data.user.fullName,
            email: data.user.email,
            imageSrc: data.user.imageSrc, // التأكد من جلب الصورة أيضاً
          };

          localStorage.setItem("user", JSON.stringify(userToSave));
          console.log("User saved during login:", userToSave);
        }
      }
      return response.data;
    } catch (error) {
      throw error;
    }
  },
  getUserperID: async (userId) => {
    try {
      const response = await api.get(`/Account/users/${userId}`);
      return response.data;
    } catch (error) {
      throw error;
    }
  },

  refreshToken: async () => {
    const refreshToken = localStorage.getItem("refreshToken");
    const response = await api.post("/auth/refresh-token", { refreshToken });

    if (response.data.token) {
      localStorage.setItem("accessToken", response.data.token);
    }
    return response.data;
  },

  logout: () => {
    localStorage.removeItem("accessToken");
    localStorage.removeItem("refreshToken");
    localStorage.removeItem("user");
    window.location.href = "/";
  },

  getCurrentUser: () => {
    const userStr = localStorage.getItem("user");
    if (userStr) return JSON.parse(userStr);
    return null;
  },

  // في ملف الـ service الخاص بك
  getUserRole: async (UserId) => {
    const response = await api.get(`/Account/role/${UserId}`);
    return response.data; // تأكد أن الـ API يرجع "Host" أو "User" مباشرة
  },
};

export default authService;

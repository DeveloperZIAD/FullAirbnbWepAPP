import api from "./apiClient";

const authService = {
  // 1. التسجيل + تسجيل دخول تلقائي
  register: async (userData) => {
    const response = await api.post("/Account/register/guest", userData);

    // إذا نجح التسجيل (الباك اند رجع userId)
    if (response.data && response.data.userId) {
      await new Promise((resolve) => setTimeout(resolve, 1500));
      console.log(
        "Registration successful, proceeding to login...",
        userData.email,
        userData.password,
      );
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
            const data = response.data;

            const token = data.token || data.accessToken;
            const refreshToken = data.refreshToken;

            if (token) {
                localStorage.setItem("accessToken", token);
                if (refreshToken) {
                    localStorage.setItem("refreshToken", refreshToken);
                }

                // هنا التعديل: نضمن دمج الـ ID في كائن المستخدم
                // الباك-إند غالباً يرسل الـ ID إما داخل data.user أو كـ data.userId
                const userId = data.userId || (data.user && data.user.id);

                const userToSave = data.user || {
                    email: credentials.email,
                    fullName: data.fullName || "User",
                };

                // دمج الـ ID في كائن المستخدم المحفوظ
                if (userId) {
                    userToSave.id = userId;
                }

                localStorage.setItem("user", JSON.stringify(userToSave));
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

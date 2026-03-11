import api from "./apiClient.jsx";

const wishlistService = {
  toggleFavorite: async (listingId) => {
    // 1. جلب كائن اليوزر بالكامل من الستورج
    const userStorage = localStorage.getItem("user");

    if (!userStorage) {
      throw new Error("User not found in storage");
    }

    const userData = JSON.parse(userStorage);

    // 2. استخراج الـ id (تأكدنا من الصورة أن اسمه id)
    const userId = userData.id;

    if (!userId) {
      console.error("User Object found but ID is missing:", userData);
      throw new Error("User ID is missing");
    }

    // 3. إرسال الطلب بنفس الصيغة التي يتوقعها السيرفر (JSON Body)
    const response = await api.post(`/Wishlist/toggle`, {
      userId: userId, // القيمة المستخرجة: "03d4b8af-..."
      listingId: listingId,
    });

    return response.data;
  },

  getFavorites: async () => {
    const userStorage = localStorage.getItem("user");
    if (!userStorage) return [];

    const userData = JSON.parse(userStorage);
    const userId = userData.id;

    const response = await api.get(`/Wishlist/user/${userId}`);
    return response.data;
  },
};

export default wishlistService;

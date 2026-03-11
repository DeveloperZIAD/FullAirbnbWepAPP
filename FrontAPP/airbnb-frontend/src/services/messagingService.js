import api from "./apiClient";

const messagingService = {
  // جلب محادثات المستخدم
  getUserConversations: async (userId) => {
    try {
      const response = await api.get(`/Messaging/user/${userId}`);
      console.log("Service Response:", response.data);
      return response.data;
    } catch (error) {
      console.error("Service Error:", error);
      throw error;
    }
  },
  // جلب تفاصيل محادثة معينة
  getConversationById: (conversationId) =>
    api
      .get(`/messaging/conversation/${conversationId}`)
      .then((res) => res.data),

  // إرسال رسالة
  sendMessage: (data) =>
    console.log("Sending message with data:", data) ||
    api.post("/messaging/send", data).then((res) => res.data),

  // بدء محادثة جديدة
  startConversation: (data) =>
    console.log("Starting conversation with data:", data) ||
    api.post("/Messaging/start", data).then((res) => res.data),

  // تمييز كمقروءة
  markAsRead: (conversationId) =>
    api.post(`/messaging/mark-read/${conversationId}`).then((res) => res.data),
};

export default messagingService;

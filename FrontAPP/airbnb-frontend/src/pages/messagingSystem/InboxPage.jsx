import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import messagingService from "./../../services/messagingService.js";
import { Loader, MessageCircle, AlertCircle } from "lucide-react";

const InboxPage = () => {
  const [conversations, setConversations] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const navigate = useNavigate();

  useEffect(() => {
    let isMounted = true; // لمنع تحديث الحالة إذا تم إلغاء الـ Component

    const fetchConversations = async () => {
      const user = localStorage.getItem("user");
      const userid = JSON.parse(user)?.id;

      if (!userid) {
        setError("User session expired. Please login again.");
        setLoading(false);
        return;
      }

      try {
        setLoading(true);
        console.log("Sending request for userid:", userid);
        const data = await messagingService.getUserConversations(userid);

        // التحقق من أن البيانات مصفوفة قبل التعيين
        if (isMounted) {
          if (Array.isArray(data)) {
            // الترتيب حسب الأحدث
            const sorted = data.sort(
              (a, b) => new Date(b.createdAt) - new Date(a.createdAt),
            );
            setConversations(sorted);
          } else {
            setConversations([]);
          }
        }
      } catch (err) {
        console.error("Fetch Error:", err);
        if (isMounted)
          setError("Could not load conversations. Please try again.");
      } finally {
        if (isMounted) setLoading(false);
      }
    };

    fetchConversations();
    return () => {
      isMounted = false;
    };
  }, []);

  if (loading)
    return (
      <div className="flex h-64 items-center justify-center">
        <Loader className="animate-spin text-rose-500" size={40} />
      </div>
    );

  if (error)
    return (
      <div className="flex flex-col items-center justify-center h-64 text-red-500">
        <AlertCircle size={40} className="mb-2" />
        <p>{error}</p>
      </div>
    );

  return (
    <div className="max-w-2xl mx-auto p-4">
      <h2 className="text-2xl font-bold mb-6">Inbox</h2>

      {conversations.length === 0 ? (
        <div className="text-center mt-20 text-gray-500">
          <MessageCircle size={48} className="mx-auto mb-4 opacity-50" />
          <p>No conversations yet.</p>
        </div>
      ) : (
        <div className="flex flex-col gap-3">
          {conversations.map((conv) => (
            <div
              key={conv.id}
              onClick={() => navigate(`/messages/${conv.id}`)}
              className="flex items-center p-4 border border-gray-200 rounded-xl cursor-pointer hover:bg-gray-50 transition-all"
            >
              <div className="w-12 h-12 rounded-full bg-rose-100 flex items-center justify-center font-bold text-rose-600 mr-4">
                {conv.hostName?.charAt(0).toUpperCase() || "?"}
              </div>

              <div className="flex-1">
                <div className="flex justify-between items-center">
                  <h4 className="font-semibold text-gray-900">
                    {conv.hostName || "Unknown"}
                  </h4>
                  <small className="text-gray-400">
                    {new Date(conv.createdAt).toLocaleTimeString([], {
                      hour: "2-digit",
                      minute: "2-digit",
                    })}
                  </small>
                </div>
                <p className="text-gray-600 text-sm truncate">
                  {conv.listingTitle || "No title"}
                </p>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default InboxPage;

import React, { useState, useEffect, useRef } from "react";
import { Loader, Send } from "lucide-react";
import { useParams } from "react-router-dom";
import { useChat } from "../../hooks/useChat";
import messagingService from "./../../services/messagingService.js";

const ChatPage = () => {
  const { conversationId } = useParams();
  const { messages, isLoading } = useChat(conversationId);
  const [input, setInput] = useState("");
  const scrollRef = useRef(null);

  // --- سحب معرف المستخدم من LocalStorage ---
  // افترضنا أنك تخزنه تحت مفتاح "userId" أو داخل كائن "user"
  const user = JSON.parse(localStorage.getItem("user"));
  const currentUserId = user?.id || localStorage.getItem("userId");

  useEffect(() => {
    scrollRef.current?.scrollIntoView({ behavior: "smooth" });
  }, [messages]);

  const onSend = async (e) => {
    e.preventDefault();
    if (!input.trim() || !currentUserId) {
      console.error("Missing Message Content or User ID");
      return;
    }

    try {
      await messagingService.sendMessage({
        conversationId: conversationId,
        senderId: currentUserId, // الآن نرسل المعرف المسحوب
        content: input,
      });
      setInput("");
    } catch (err) {
      console.error("Send failed", err);
    }
  };

  if (isLoading)
    return (
      <div className="flex h-screen items-center justify-center">
        <Loader className="animate-spin text-rose-500" size={40} />
      </div>
    );

  return (
    <div className="flex flex-col h-[85vh] max-w-3xl mx-auto border rounded-2xl shadow-2xl bg-white overflow-hidden mt-6 mb-6">
      {/* Header */}
      <div className="p-4 border-b bg-white flex items-center justify-between">
        <h2 className="font-bold text-gray-800">Messages</h2>
      </div>

      {/* Messages Area */}
      <div className="flex-1 overflow-y-auto p-6 space-y-4 bg-gray-50">
        {messages.length === 0 ? (
          <div className="text-center text-gray-400 mt-10">
            No messages yet. Start the conversation!
          </div>
        ) : (
          messages.map((msg, i) => (
            <div
              key={i}
              className={`flex ${msg.senderId === currentUserId ? "justify-end" : "justify-start"}`}
            >
              <div
                className={`max-w-[75%] p-3 px-4 rounded-2xl text-[15px] shadow-sm ${
                  msg.senderId === currentUserId
                    ? "bg-rose-500 text-white rounded-br-none"
                    : "bg-white border border-gray-200 text-gray-700 rounded-bl-none"
                }`}
              >
                {msg.content}
                <div
                  className={`text-[10px] mt-1 flex ${msg.senderId === currentUserId ? "justify-end text-rose-100" : "text-gray-400"}`}
                >
                  {new Date(msg.createdAt).toLocaleTimeString([], {
                    hour: "2-digit",
                    minute: "2-digit",
                  })}
                </div>
              </div>
            </div>
          ))
        )}
        <div ref={scrollRef} />
      </div>

      {/* Input Area */}
      <form
        onSubmit={onSend}
        className="p-4 bg-white border-t flex gap-3 items-center"
      >
        <input
          className="flex-1 border border-gray-300 rounded-full px-5 py-3 focus:outline-none focus:ring-2 focus:ring-rose-500 focus:border-transparent transition-all"
          placeholder="Write a message..."
          value={input}
          onChange={(e) => setInput(e.target.value)}
        />
        <button
          type="submit"
          disabled={!input.trim()}
          className="bg-rose-500 text-white p-3 rounded-full hover:bg-rose-600 disabled:bg-gray-300 disabled:cursor-not-allowed transition-all shadow-md"
        >
          <Send size={22} />
        </button>
      </form>
    </div>
  );
};

export default ChatPage;

import { useState, useEffect, useRef, useCallback } from "react";
import * as signalR from "@microsoft/signalr";
import messagingService from "../services/messagingService";

export const useChat = (conversationId) => {
  const [messages, setMessages] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [connectionState, setConnectionState] = useState("disconnected");
  const [error, setError] = useState(null);
  const connectionRef = useRef(null);

  const joinConversation = useCallback(
    async (connection) => {
      try {
        await connection.invoke("JoinConversation", conversationId);
        console.log("Joined conversation:", conversationId);
      } catch (err) {
        console.error("Failed to join conversation:", err);
      }
    },
    [conversationId],
  );

  const initConnection = useCallback(async () => {
    if (!conversationId || connectionRef.current) return;

    setConnectionState("connecting");

    const baseUrl = import.meta.env.VITE_API_URL
      ? import.meta.env.VITE_API_URL.replace("/api", "") // يحول الرابط من /api إلى الرابط الأساسي
      : "https://airbnb-backend-production-7ed2.up.railway.app";

    const connection = new signalR.HubConnectionBuilder()
      .withUrl(`${baseUrl}/chathub`)
      .withAutomaticReconnect()
      .build();

    connectionRef.current = connection;

    // معالجة الرسائل الواردة
    connection.on("ReceiveMessage", (newMessage) => {
      setMessages((prev) => [...prev, newMessage]);
    });

    // التعامل مع إغلاق الاتصال (بديل صحيح لـ onerror)
    connection.onclose((err) => {
      setConnectionState("disconnected");
      if (err) setError(`Connection closed: ${err.message}`);
    });

    try {
      await connection.start();
      setConnectionState("connected");
      setError(null);
      await joinConversation(connection);
    } catch (err) {
      setConnectionState("error");
      setError("Failed to connect to chat server");
      console.error("SignalR start error:", err);
    }
  }, [conversationId, joinConversation]);

  const sendMessage = async (messageData) => {
    if (!connectionRef.current || connectionState !== "connected") {
      throw new Error("Not connected to chat");
    }
    // تأكد أن الاسم يطابق الـ Hub (مثلاً SendMessage)
    await connectionRef.current.invoke(
      "SendMessage",
      conversationId,
      messageData,
    );
  };

  useEffect(() => {
    let isMounted = true;

    const setup = async () => {
      setIsLoading(true);
      try {
        const data = await messagingService.getConversationById(conversationId);
        if (isMounted) {
          setMessages(data.messages || []);
          setIsLoading(false);
          await initConnection();
        }
      } catch (err) {
        if (isMounted) setIsLoading(false);
      }
    };

    setup();

    return () => {
      isMounted = false;
      if (connectionRef.current) {
        connectionRef.current.stop();
        connectionRef.current = null;
      }
    };
  }, [conversationId, initConnection]);

  return { messages, isLoading, connectionState, error, sendMessage };
};

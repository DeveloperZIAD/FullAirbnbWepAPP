import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";
import basicSsl from "@vitejs/plugin-basic-ssl";
import path from "path"; // لابد من استيراد path للتعامل مع المسارات

export default defineConfig({
  plugins: [react(), basicSsl()],
  resolve: {
    alias: {
      // هذا الجزء يحل مشكلة الـ @ التي ظهرت في رسالة الخطأ
      "@": path.resolve(__dirname, "./src"),
    },
  },
  server: {
    https: true, // تشغيل السيرفر بأمان
    port: 5173,
    host: true, // للسماح بالوصول عبر الشبكة كما يظهر في صورتك
  },
});

import { useEffect, useState } from "react";
import { Outlet, Navigate } from "react-router-dom";
import AdminSidebar from "./AdminSidebar";
import adminService from "./../../services/adminService.js";
import authService from "./../../services/authService.jsx";

const AdminLayout = () => {
  const [stats, setStats] = useState(null);
  const [loading, setLoading] = useState(true);
  const [isAdmin, setIsAdmin] = useState(false);
  const [authChecking, setAuthChecking] = useState(true);

  useEffect(() => {
    const checkAuthAndFetchStats = async () => {
      try {
        // 1. قراءة بيانات المستخدم من الـ Storage
        const userDataString = localStorage.getItem("user");

        if (!userDataString) {
          setIsAdmin(false);
          setAuthChecking(false);
          return;
        }

        const userData = JSON.parse(userDataString);
        const userId = userData.id;

        // 2. التحقق من صلاحيات المستخدم من الـ API
        const userProfile = await authService.getUserperID(userId);

        // التحقق لو كان "Admin" (تأكد من مسمى الرول في الـ API بتاعك)
        if (userProfile && userProfile.role === "Admin") {
          setIsAdmin(true);

          // 3. لو أدمن، نجيب إحصائيات الداشبورد
          const statsData = await adminService.getAllStats();
          setStats(statsData);
        } else {
          setIsAdmin(false);
        }
      } catch (error) {
        console.error("Auth/Stats Verification Failed:", error);
        setIsAdmin(false);
      } finally {
        setAuthChecking(false);
        setLoading(false);
      }
    };

    checkAuthAndFetchStats();
  }, []);

  // حالة التحميل الأولية للتأكد من الهوية
  if (authChecking) {
    return (
      <div className="h-screen w-full flex items-center justify-center bg-neutral-100">
        <div className="text-rose-500 font-bold animate-pulse">
          Verifying Permissions...
        </div>
      </div>
    );
  }

  // لو مش أدمن، يرجعه للرئيسية
  if (!isAdmin) {
    return <Navigate to="/" replace />;
  }

  return (
    <div className="flex flex-row min-h-screen bg-neutral-100">
      <AdminSidebar />
      <main className="flex-1 p-8 overflow-y-auto">
        {/* نمرر الـ stats كـ context لجميع الصفحات الداخلية */}
        <Outlet context={{ stats, setStats }} />
      </main>
    </div>
  );
};

export default AdminLayout;

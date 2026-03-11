import { useOutletContext } from "react-router-dom";

// مكون الكارت الموحد
const DashboardCard = ({ title, value, color, isCurrency = false }) => (
  <div className="bg-white p-6 rounded-xl border border-neutral-200 shadow-sm hover:shadow-md transition-all duration-300">
    <div className="text-neutral-500 text-sm font-medium uppercase tracking-wider">
      {title}
    </div>
    <div className={`text-3xl font-bold mt-2 ${color}`}>
      {/* عرض "0" إذا كانت القيمة غير موجودة لمنع الـ Null errors */}
      {isCurrency && "$"}
      {value !== undefined && value !== null ? value.toLocaleString() : 0}
    </div>
  </div>
);

const AdminDashboard = () => {
  // استلام البيانات من الـ AdminLayout عبر الـ context
  // stats المتوفرة الآن: totalUsers, totalListings, activeBookings, totalRevenue
  const { stats } = useOutletContext();

  return (
    <div className="p-2 animate-in fade-in duration-500">
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-neutral-800">
          Dashboard Overview
        </h1>
        <p className="text-neutral-500 mt-1">
          Real-time statistics from your platform.
        </p>
      </div>

      {/* شبكة الكروت الإحصائية */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <DashboardCard
          title="Total Users"
          value={stats?.totalUsers}
          color="text-blue-600"
        />
        <DashboardCard
          title="Active Listings"
          value={stats?.totalListings}
          color="text-rose-600"
        />
        <DashboardCard
          title="Active Bookings" // تم تعديل المسمى هنا ليطابق الـ JSON
          value={stats?.activeBookings}
          color="text-green-600"
        />
        <DashboardCard
          title="Total Revenue"
          value={stats?.totalRevenue}
          color="text-amber-600"
          isCurrency={true} // لتنظيم عرض العملة بشكل أفضل
        />
      </div>

      {/* قسم الأنشطة الأخيرة */}
      <div className="mt-12 bg-white rounded-2xl border border-neutral-200 p-8 shadow-sm">
        <div className="flex items-center justify-between mb-6">
          <h2 className="text-xl font-bold text-neutral-800">
            Recent Activities
          </h2>
          <span className="text-xs font-semibold bg-neutral-100 text-neutral-500 px-3 py-1 rounded-full uppercase">
            Live Updates
          </span>
        </div>

        <div className="flex flex-col items-center justify-center py-12 border-2 border-dashed border-neutral-100 rounded-xl">
          <div className="text-neutral-300 mb-2">
            <svg
              className="w-12 h-12"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth="2"
                d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"
              />
            </svg>
          </div>
          <div className="text-neutral-400 italic text-sm">
            Everything is up to date. No recent activities found.
          </div>
        </div>
      </div>
    </div>
  );
};

export default AdminDashboard;

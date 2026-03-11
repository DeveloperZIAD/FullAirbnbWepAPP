import {
  LayoutDashboard,
  Users,
  Home,
  ClipboardList,
  Settings,
} from "lucide-react";
import { Link, useLocation } from "react-router-dom";

const AdminSidebar = () => {
  const { pathname } = useLocation();

  const menuItems = [
    { label: "Overview", icon: LayoutDashboard, path: "/admin" },
    { label: "Manage Users", icon: Users, path: "/admin/users" },
    { label: "Manage Listings", icon: Home, path: "/admin/listings" },
    { label: "Reservations", icon: ClipboardList, path: "/admin/reservations" },
  ];

  return (
    <div className="w-64 bg-neutral-900 h-screen sticky top-0 text-white p-6 flex flex-col gap-8">
      <div className="text-2xl font-bold text-rose-500 border-b border-neutral-800 pb-4">
        Admin Panel
      </div>
      <nav className="flex flex-col gap-2">
        {menuItems.map((item) => {
          const isActive = pathname === item.path;
          return (
            <Link
              key={item.label}
              to={item.path}
              className={`flex items-center gap-3 p-3 rounded-lg transition ${
                isActive
                  ? "bg-rose-500 text-white"
                  : "hover:bg-neutral-800 text-neutral-400"
              }`}
            >
              <item.icon size={20} />
              <span className="font-medium">{item.label}</span>
            </Link>
          );
        })}
      </nav>
    </div>
  );
};

export default AdminSidebar;

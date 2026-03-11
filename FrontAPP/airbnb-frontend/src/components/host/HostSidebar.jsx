import {
  LayoutDashboard,
  Building,
  CalendarDays,
  Settings,
} from "lucide-react";
import { NavLink } from "react-router-dom";
import { MessageCircle } from "lucide-react";
const HostSidebar = () => {
  const routes = [
    { label: "Dashboard", icon: LayoutDashboard, href: "/host/dashboard" },
    { label: "My Listings", icon: Building, href: "/host" },
    { label: "Inbox", icon: MessageCircle, href: "/host/inbox" },
    { label: "Reservations", icon: CalendarDays, href: "/host/reservations" },
    { label: "Settings", icon: Settings, href: "/host/settings" },
  ];

  return (
    <aside className="w-64 h-screen border-r bg-white p-6 flex flex-col gap-2 sticky top-0">
      {/* الشعار أو اسم الموقع */}
      <div className="mb-8 font-bold text-xl text-rose-500">Host Portal</div>

      <nav className="flex flex-col gap-1">
        {routes.map((route) => (
          <NavLink
            key={route.href}
            to={route.href}
            // الخاصية end مهمة جداً للرابط الرئيسي /host لكي لا يكون نشطاً عند الدخول للـ dashboard
            end={route.href === "/host"}
            className={({ isActive }) => `
              flex items-center gap-3 px-4 py-3 rounded-xl transition-all duration-200
              ${
                isActive
                  ? "bg-rose-50 text-rose-600 font-semibold shadow-sm"
                  : "text-neutral-500 hover:bg-neutral-100 hover:text-neutral-800"
              }
            `}
          >
            <route.icon size={20} />
            {route.label}
          </NavLink>
        ))}
      </nav>
    </aside>
  );
};

export default HostSidebar;

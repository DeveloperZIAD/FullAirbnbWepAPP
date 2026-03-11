import { Outlet } from "react-router-dom";
import HostSidebar from "./../HostSidebar.jsx";

export default function Hostpage() {
  return (
    <div className="flex bg-neutral-50 min-h-screen">
      <HostSidebar />
      <main className="flex-1 p-8">
        {/* هنا المكان الذي ستظهر فيه الـ Dashboard أو الـ Reservations */}
        <Outlet />
      </main>
    </div>
  );
}

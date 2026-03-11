import { useEffect, useState } from "react";
import adminService from "./../../services/adminService.js";

const AdminReservations = () => {
  const [reservations, setReservations] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    adminService
      .getAllReservations()
      .then((res) => {
        const data = res?.data || res;
        setReservations(Array.isArray(data) ? data : []);
      })
      .catch((err) => {
        console.error("Error fetching reservations:", err);
      })
      .finally(() => setLoading(false));
  }, []);

  // دالة مساعدة لتحديد لون الـ Status بناءً على القيمة
  const getStatusStyles = (status) => {
    switch (status) {
      case "Confirmed":
        return "bg-green-100 text-green-700";
      case "Cancelled":
        return "bg-rose-100 text-rose-700";
      case "Pending":
        return "bg-amber-100 text-amber-700";
      default:
        return "bg-neutral-100 text-neutral-700";
    }
  };

  if (loading)
    return (
      <div className="p-10 text-center text-neutral-500 animate-pulse">
        Loading Reservations...
      </div>
    );

  return (
    <div className="bg-white rounded-xl shadow-sm border border-neutral-200 overflow-hidden">
      <div className="p-6 border-b flex justify-between items-center">
        <h2 className="text-xl font-bold text-neutral-800">
          Manage Reservations
        </h2>
        <span className="text-sm bg-neutral-100 px-3 py-1 rounded-full text-neutral-600 font-medium">
          {reservations?.length || 0} Bookings
        </span>
      </div>

      <div className="overflow-x-auto">
        <table className="w-full text-left">
          <thead className="bg-neutral-50 text-neutral-500 text-xs uppercase tracking-wider">
            <tr>
              <th className="p-4 font-semibold">Listing</th>
              <th className="p-4 font-semibold">Guest</th>
              <th className="p-4 font-semibold">Host</th>
              <th className="p-4 font-semibold">Stay Dates</th>
              <th className="p-4 font-semibold">Pricing</th>
              <th className="p-4 font-semibold">Status</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-neutral-100">
            {reservations?.length > 0 ? (
              reservations.map((res) => (
                <tr key={res.id} className="hover:bg-neutral-50/50 transition">
                  {/* عنوان العقار */}
                  <td className="p-4 text-sm font-medium text-neutral-700 max-w-[200px] truncate">
                    {res?.listingTitle || "Deleted Listing"}
                  </td>

                  {/* تفاصيل الضيف */}
                  <td className="p-4">
                    <div className="text-sm font-bold text-neutral-800">
                      {res?.guestDetails?.fullName || "Unknown Guest"}
                    </div>
                    <div className="text-[10px] text-neutral-400 truncate max-w-[150px]">
                      {res?.guestDetails?.email}
                    </div>
                  </td>

                  {/* تفاصيل المضيف */}
                  <td className="p-4">
                    <div className="text-sm font-medium text-neutral-700">
                      {res?.hostDetails?.fullName || "Unknown Host"}
                    </div>
                    <div className="text-[10px] text-neutral-400 truncate max-w-[150px]">
                      {res?.hostDetails?.email}
                    </div>
                  </td>

                  {/* التواريخ */}
                  <td className="p-4">
                    <div className="text-[11px] font-medium text-neutral-600">
                      {res?.checkIn
                        ? new Date(res.checkIn).toLocaleDateString()
                        : "N/A"}
                    </div>
                    <div className="text-[10px] text-neutral-400">
                      to{" "}
                      {res?.checkOut
                        ? new Date(res.checkOut).toLocaleDateString()
                        : "N/A"}
                    </div>
                  </td>

                  {/* السعر الإجمالي */}
                  <td className="p-4">
                    <div className="text-sm font-bold text-green-600">
                      ${res?.totalPrice?.toFixed(2) || "0.00"}
                    </div>
                    <div className="text-[9px] text-neutral-400 italic">
                      ${res?.pricePerNight}/night
                    </div>
                  </td>

                  {/* الحالة */}
                  <td className="p-4">
                    <span
                      className={`px-2.5 py-1 rounded-md text-[10px] font-bold uppercase tracking-tight ${getStatusStyles(res?.status)}`}
                    >
                      {res?.status || "Unknown"}
                    </span>
                  </td>
                </tr>
              ))
            ) : (
              <tr>
                <td colSpan="6" className="p-16 text-center text-neutral-400">
                  <p className="italic text-sm">
                    No reservations found in the records.
                  </p>
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default AdminReservations;

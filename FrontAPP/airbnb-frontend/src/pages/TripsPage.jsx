import React, { useEffect, useState } from "react";
import { toast } from "react-hot-toast";
import bookingService from "../services/bookingService";
import TripCard from "../features/trips/TripCard";
import Swal from "sweetalert2";
const TripsPage = () => {
  const [bookings, setBookings] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [deletingId, setDeletingId] = useState("");

  // جلب الحجوزات عند تحميل الصفحة
  useEffect(() => {
    const fetchTrips = async () => {
      try {
        setIsLoading(true);
        const data = await bookingService.getBookingByUserId();
        console.log("Data in Component:", data); // تأكد أن البيانات وصلت هنا
        setBookings(data || []);
      } catch (error) {
        // لا تكتفي بـ Toast، انظر للكونسول
        toast.error(
          "Failed to load: " + (error.response?.statusText || "Check Console"),
        );
      } finally {
        setIsLoading(false);
      }
    };
    fetchTrips();
  }, []);
  // وظيفة إلغاء الحجز
  const onCancel = async (id) => {
    // 1. إظهار نافذة التأكيد بشكل عصري
    const result = await Swal.fire({
      title: "Are you sure?",
      text: "You won't be able to revert this reservation!",
      icon: "warning",
      showCancelButton: true,
      confirmButtonColor: "#f43f5e", // لون الـ Rose الخاص بـ Airbnb
      cancelButtonColor: "#737373",
      confirmButtonText: "Yes, cancel it!",
      cancelButtonText: "Keep it",
      reverseButtons: true, // لجعل زر الإلغاء على اليسار وزر التأكيد على اليمين
      background: "#ffffff",
      borderRadius: "15px",
      customClass: {
        popup: "rounded-2xl shadow-xl border-none",
        confirmButton: "px-6 py-2 rounded-lg font-semibold",
        cancelButton: "px-6 py-2 rounded-lg font-semibold",
      },
    });

    // 2. إذا وافق المستخدم
    if (result.isConfirmed) {
      try {
        setDeletingId(id);

        // إظهار Loading بسيط داخل الـ Toast أو الـ Button
        await bookingService.cancelBooking(id);

        // 3. نجاح العملية
        Swal.fire({
          title: "Cancelled!",
          text: "Your reservation has been successfully removed.",
          icon: "success",
          confirmButtonColor: "#f43f5e",
          timer: 2000, // تغلق تلقائياً بعد ثانيتين
          showConfirmButton: false,
        });

        // تحديث القائمة محلياً
        setBookings((prev) => prev.filter((item) => item.id !== id));
      } catch (error) {
        // 4. فشل العملية
        Swal.fire({
          title: "Error!",
          text:
            error.response?.data?.message ||
            "Something went wrong while cancelling.",
          icon: "error",
          confirmButtonColor: "#f43f5e",
        });
      } finally {
        setDeletingId("");
      }
    }
  };

  if (isLoading)
    return <div className="pt-24 text-center">Loading your trips...</div>;

  if (bookings.length === 0) {
    return (
      <div className="pt-24 flex flex-col items-center gap-4">
        <h1 className="text-2xl font-bold">No trips booked... yet!</h1>
        <p className="text-neutral-500">
          Time to dust off your bags and start planning your next adventure.
        </p>
      </div>
    );
  }

  return (
    <div className="max-w-[2520px] mx-auto xl:px-20 md:px-10 sm:px-2 px-4 pt-24 pb-10">
      <h1 className="text-3xl font-bold mb-8">Trips</h1>
      <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 2xl:grid-cols-6 gap-8">
        {bookings.map((booking) => (
          <TripCard
            key={booking.id}
            booking={booking}
            onCancel={onCancel}
            disabled={deletingId === booking.id}
          />
        ))}
      </div>
    </div>
  );
};

export default TripsPage;

import React from "react";
import { format, isValid } from "date-fns";

const TripCard = ({ booking, onCancel, disabled }) => {
  // استخراج البيانات بما فيها الـ status الجديد
  const {
    id,
    checkIn,
    checkOut,
    totalPrice,
    listingTitle,
    listingImage,
    city,
    countryCode,
    status, // تأكد أن الباك إند يرسل هذه القيمة (مثلاً: "Pending", "Confirmed", "Cancelled")
  } = booking;

  // التحقق مما إذا كان الحجز ملغياً
  const isCancelled = status === "Cancelled";

  // تحويل التواريخ
  const startDate = new Date(checkIn);
  const endDate = new Date(checkOut);

  const formattedDate =
    isValid(startDate) && isValid(endDate)
      ? `${format(startDate, "PP")} - ${format(endDate, "PP")}`
      : "Date not available";

  return (
    <div
      className={`col-span-1 group border-[1px] border-neutral-200 rounded-xl overflow-hidden shadow-sm transition bg-white 
      ${isCancelled ? "opacity-80" : "hover:shadow-md"}`}
    >
      <div className="flex flex-col gap-2 w-full">
        {/* الحاوية النسبية للصورة */}
        <div className="aspect-square w-full relative overflow-hidden bg-neutral-100">
          <img
            src={listingImage || "/images/placeholder.jpg"}
            alt={listingTitle}
            className={`object-cover h-full w-full transition duration-300 
            ${isCancelled ? "grayscale" : "group-hover:scale-110"}`}
          />

          {/* شارة الحالة (Badge) تظهر فقط إذا كان ملغياً */}
          {isCancelled && (
            <div className="absolute top-3 left-3 bg-white/90 backdrop-blur-sm px-3 py-1 rounded-full shadow-md z-10">
              <span className="text-rose-600 text-xs font-bold uppercase tracking-tighter">
                Cancelled
              </span>
            </div>
          )}
        </div>

        <div className="p-4 flex flex-col gap-1">
          {/* المدينة والدولة */}
          <div
            className={`font-bold text-lg truncate ${isCancelled ? "text-neutral-500" : "text-black"}`}
          >
            {city}, {countryCode}
          </div>

          {/* عنوان العقار */}
          <div className="font-light text-neutral-500 text-sm truncate">
            {listingTitle}
          </div>

          {/* التواريخ */}
          <div className="font-normal text-neutral-600 text-sm mt-1">
            {formattedDate}
          </div>

          {/* السعر الإجمالي */}
          <div className="flex flex-row items-center gap-1 mt-2">
            <div
              className={`font-bold ${isCancelled ? "text-neutral-400 line-through" : "text-rose-600"}`}
            >
              $ {totalPrice}
            </div>
            <div className="font-light text-sm text-neutral-500">
              {isCancelled ? "was total" : "total trip"}
            </div>
          </div>

          {/* زر الإلغاء أو رسالة الحالة */}
          {!isCancelled ? (
            <button
              disabled={disabled}
              onClick={(e) => {
                e.stopPropagation();
                onCancel(id);
              }}
              className="mt-4 w-full border-[1px] border-rose-500 text-rose-500 py-2 rounded-lg font-semibold hover:bg-rose-500 hover:text-white transition active:scale-95 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {disabled ? "Processing..." : "Cancel Reservation"}
            </button>
          ) : (
            <div className="mt-4 w-full bg-neutral-100 text-neutral-500 py-2 rounded-lg font-medium text-center text-sm border border-neutral-200 cursor-default">
              Reservation Inactive
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default TripCard;

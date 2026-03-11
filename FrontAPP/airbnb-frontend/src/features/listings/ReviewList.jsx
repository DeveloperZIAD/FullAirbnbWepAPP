import React from "react";
import { Star, Trash2 } from "lucide-react";

const ReviewList = ({ reviews, currentUser, onDelete }) => {
  if (!reviews || reviews.length === 0) {
    return (
      <p className="text-neutral-500 mt-4">No reviews yet for this property.</p>
    );
  }

  return (
    <div className="grid grid-cols-1 md:grid-cols-2 gap-8 mt-8">
      {reviews.map((review, index) => {
        // 1. استخراج بيانات المستخدم (سواء كانت مسطحة من الـ POST أو متداخلة من الـ GET)
        // الباك-إند في الـ GET غالباً يرسل كائن اسمه user أو guest
        const userData = review.user || review.guest || {};

        const displayName =
          review.userName || // من نتيجة الإضافة (POST)
          userData.fullName || // من نتيجة الجلب (GET)
          review.userEmail ||
          "Guest User";

        const displayImage =
          review.userImage || // من نتيجة الإضافة
          userData.imageSrc || // من نتيجة الجلب
          review.imageSrc;

        // 2. معالجة التاريخ
        const dateValue = review.createdAt || review.createdAtUtc;
        const formattedDate =
          dateValue && !isNaN(Date.parse(dateValue))
            ? new Date(dateValue).toLocaleDateString("en-US", {
                year: "numeric",
                month: "long",
                day: "numeric",
              })
            : "Recently";

        // 3. تحديد هل المستخدم الحالي هو صاحب التقييم؟
        // نستخدم الـ ID أو الـ Email للمقارنة لضمان الدقة
        const isOwner =
          currentUser?.id === review.userId ||
          currentUser?.id === userData.id ||
          currentUser?.email === (review.userEmail || userData.email);

        return (
          <div
            key={review.id || `review-${index}`}
            className="border p-4 rounded-xl shadow-sm hover:shadow-md transition"
          >
            <div className="flex justify-between items-start">
              <div className="flex items-center gap-3 mb-2">
                {/* عرض الصورة الشخصية */}
                {displayImage ? (
                  <img
                    src={displayImage}
                    className="w-10 h-10 rounded-full object-cover border border-neutral-100"
                    alt={displayName}
                  />
                ) : (
                  <div className="w-10 h-10 bg-neutral-200 rounded-full flex items-center justify-center font-bold text-neutral-600">
                    {displayName[0]?.toUpperCase()}
                  </div>
                )}

                <div>
                  <p className="font-semibold text-sm">{displayName}</p>
                  <p className="text-[11px] text-neutral-400">
                    {formattedDate}
                  </p>

                  {/* عرض النجوم */}
                  <div className="flex text-yellow-500 mt-1">
                    {[...Array(5)].map((_, i) => (
                      <Star
                        key={i}
                        size={12}
                        fill={i < review.rating ? "currentColor" : "none"}
                        className={
                          i < review.rating
                            ? "text-yellow-500"
                            : "text-neutral-300"
                        }
                      />
                    ))}
                  </div>
                </div>
              </div>

              {/* زر الحذف يظهر فقط للصاحب */}
              {isOwner && (
                <button
                  onClick={() => onDelete(review.id)}
                  className="text-neutral-400 hover:text-rose-500 transition p-1"
                >
                  <Trash2 size={16} />
                </button>
              )}
            </div>

            <p className="text-neutral-600 mt-3 text-sm leading-relaxed">
              {review.comment || "No comment provided."}
            </p>
          </div>
        );
      })}
    </div>
  );
};

export default ReviewList;

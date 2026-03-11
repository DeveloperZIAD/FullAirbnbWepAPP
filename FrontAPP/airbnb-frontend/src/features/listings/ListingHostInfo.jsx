import React from "react";
import {
  MdOutlineDoorFront,
  MdOutlineLocationOn,
  MdCalendarToday,
  MdOutlinePhone, // استيراد أيقونة الهاتف
  MdOutlineMessage, // أيقونة الرسالة
} from "react-icons/md";

const ListingHostInfo = ({
  hostName,
  avatar,
  hostSince,
  phoneNumber,
  onChatClick,
}) => {
  const highlights = [
    {
      id: 1,
      icon: <MdOutlineDoorFront size={28} className="text-gray-700" />,
      title: "Self check-in",
      desc: "Check yourself in with the keypad.",
    },
    {
      id: 2,
      icon: <MdOutlineLocationOn size={28} className="text-gray-700" />,
      title: "Great location",
      desc: "95% of recent guests gave the location a 5-star rating.",
    },
    {
      id: 3,
      icon: <MdCalendarToday size={26} className="text-gray-700" />,
      title: "Free cancellation for 48 hours",
      desc: "Get a full refund if you change your mind.",
    },
  ];

  return (
    <div className="flex flex-col gap-6">
      {/* قسم المضيف */}
      <div className="flex items-center gap-4">
        <div className="relative w-14 h-14">
          {" "}
          {/* كبرنا الحجم قليلاً */}
          <img
            src={
              avatar || "https://a0.muscache.com/defaults/user_pic-225x225.png"
            }
            alt={hostName || "Host"}
            className="w-full h-full rounded-full object-cover border border-neutral-200 shadow-sm"
            onError={(e) => {
              e.target.src =
                "https://a0.muscache.com/defaults/user_pic-225x225.png";
            }}
          />
        </div>
        <div className="flex flex-col">
          <h2 className="text-xl font-semibold text-neutral-800">
            Hosted by {hostName || "Mathilde"}
          </h2>
          <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-2 sm:gap-3">
            <div className="flex flex-col sm:flex-row sm:items-center gap-1 sm:gap-3">
              <p className="text-gray-500 text-sm">
                Host since {hostSince || "2024"}
              </p>
              {/* عرض رقم الهاتف إذا وجد */}
              {phoneNumber && (
                <div className="flex items-center gap-1 text-rose-500 text-sm font-medium">
                  <MdOutlinePhone size={16} />
                  <span>{phoneNumber}</span>
                </div>
              )}
            </div>
            {/* زر الدردشة */}
            {onChatClick && (
              <button
                onClick={onChatClick}
                className="flex items-center justify-center gap-2 bg-gradient-to-r from-rose-500 to-rose-600 text-white px-5 py-2.5 rounded-xl font-semibold text-sm shadow-lg hover:shadow-xl hover:from-rose-600 hover:to-rose-700 active:scale-95 transition-all duration-300 whitespace-nowrap"
              >
                <MdOutlineMessage size={18} />
                Chat with Host
              </button>
            )}
          </div>
        </div>
      </div>

      <hr className="border-neutral-200" />

      {/* قسم المميزات */}
      <div className="flex flex-col gap-6">
        {highlights.map((item) => (
          <div key={item.id} className="flex flex-row items-start gap-4">
            <div className="mt-1 p-1 bg-neutral-50 rounded-md">{item.icon}</div>
            <div className="flex flex-col">
              <div className="text-[17px] font-semibold text-neutral-800">
                {item.title}
              </div>
              <div className="text-neutral-500 font-light text-sm">
                {item.desc}
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};

export default ListingHostInfo;

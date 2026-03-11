import React from "react";

const CategoryView = ({ icon: Icon, label, description }) => {
  return (
    <div className="flex flex-col gap-6">
      <div className="flex flex-row items-center gap-4">
        {/* التحقق: هل Icon مكون صالح؟ */}
        {Icon ? (
          <Icon size={40} className="text-neutral-600" />
        ) : (
          /* أيقونة افتراضية في حال لم يجد تطابق */
          <div className="text-neutral-300">
            <MdOutlineVilla size={40} />
          </div>
        )}

        <div className="flex flex-col">
          <div className="text-lg font-semibold">{label}</div>
          <div className="text-neutral-500 font-light">
            {description || "Enjoy your stay in this unique location!"}
          </div>
        </div>
      </div>
    </div>
  );
};

export default CategoryView;

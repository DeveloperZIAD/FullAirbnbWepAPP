import React, { useMemo } from "react";
import ListingHostInfo from "./ListingHostInfo.jsx";
import ListingDescription from "./ListingDescription";
import ListingAmenities from "./ListingAmenities";
import CategoryView from "./CategoryView";
import ListingMap from "./ListingMap";
// افترضنا أنك تملك ملف الفئات هنا لجلب الأيقونات منه
import { categories } from "./../../components/layout/CategoriesData";

const ListingInfo = ({
  host,
  categoryName, // نستقبل الاسم هنا
  description,
  guestCount,
  roomCount,
  bathroomCount,
  location,
  onChatClick,
}) => {
  // منطق البحث عن بيانات الفئة (الأيقونة والوصف) بناءً على الاسم
  const categoryData = useMemo(() => {
    return categories.find((item) => item.label === categoryName);
  }, [categoryName]);

  return (
    <div className="col-span-4 flex flex-col gap-8">
      {/* 1. تفاصيل العقار */}
      <div className="flex flex-col gap-1">
        <div className="text-xl font-semibold">Property Details</div>
        <div className="flex flex-row items-center gap-4 font-light text-neutral-500">
          <div>{guestCount || 0} guests</div>
          <div>{roomCount || 0} rooms</div>
          <div>{bathroomCount || 0} bathrooms</div>
        </div>
      </div>

      <hr className="border-neutral-200" />

      {/* 2. عرض القسم (باستخدام البيانات التي وجدناها) */}
      {categoryData ? (
        <CategoryView
          icon={categoryData?.icon} // هنا سنمرر مكون الأيقونة فعلياً
          label={categoryName} // نعرض الاسم القادم من الـ JSON
          description={categoryData?.description}
        />
      ) : (
        // في حال لم نجد أيقونة، نعرض الاسم فقط بشكل بسيط
        <div className="flex flex-col gap-2">
          <div className="text-lg font-semibold">{categoryName}</div>
        </div>
      )}

      <hr className="border-neutral-200" />

      {/* 3. بيانات المضيف */}
      <ListingHostInfo
        hostName={host?.name}
        avatar={host?.imageUrl}
        hostSince="2024"
        phoneNumber={host?.phoneNumber} // تمرير الرقم هنا
        onChatClick={onChatClick}
      />

      <hr className="border-neutral-200" />

      <ListingDescription description={description} />

      <hr className="border-neutral-200" />

      <ListingAmenities />

      <hr className="border-neutral-200" />

      {/* 4. الخريطة */}
      <div className="flex flex-col gap-4">
        <ListingMap
          locationName={`${location?.city ?? ""}, ${location?.countryCode ?? ""}`}
          address={location?.streetAddress ?? ""}
        />
      </div>
    </div>
  );
};

export default ListingInfo;

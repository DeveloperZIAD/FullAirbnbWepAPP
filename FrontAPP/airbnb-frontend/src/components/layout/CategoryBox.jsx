import { useCallback } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import qs from "query-string";

const CategoryBox = ({ label, icon: Icon, selected }) => {
  const navigate = useNavigate();
  const [params] = useSearchParams();

  const handleClick = useCallback(() => {
    let currentQuery = {};

    // 1. استخراج الـ Query الحالي من الـ URL
    // if (params) {
    //   currentQuery = qs.parse(params.toString());
    // }

    // 2. تحديث التصنيف (إذا كان مضغوطاً مسبقاً، نقوم بإزالته - Toggle)
    const updatedQuery = {
      // ...currentQuery,
      Category: label, // نستخدم 'Category' بحرف كبير ليتوافق مع الـ API
    };

    if (params.get("Category") === label) {
      delete updatedQuery.Category;
    }

    // 3. توليد الرابط الجديد
    const url = qs.stringifyUrl(
      {
        url: "/",
        query: updatedQuery,
      },
      { skipNull: true },
    );

    navigate(url);
  }, [label, params, navigate]);

  return (
    <div
      onClick={handleClick}
      className={`
        flex 
        flex-col 
        items-center 
        justify-center 
        gap-2 
        p-3 
        border-b-2 
        hover:text-neutral-800 
        transition 
        cursor-pointer
        min-w-[80px]
        ${selected ? "border-b-neutral-800" : "border-transparent"}
        ${selected ? "text-neutral-800" : "text-neutral-500"}
      `}
    >
      <Icon size={26} />
      <div className="font-medium text-sm">{label}</div>
    </div>
  );
};

export default CategoryBox;

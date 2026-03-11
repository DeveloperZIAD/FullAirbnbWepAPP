import React, { useState } from "react";
import { IoIosArrowForward } from "react-icons/io";

const ListingDescription = ({ description }) => {
  const [isExpanded, setIsExpanded] = useState(false);

  // نحدد عدد الحروف المسموح بها قبل الإخفاء
  const TEXT_LIMIT = 250;
  const isLongDescription = description?.length > TEXT_LIMIT;

  const displayText = isExpanded
    ? description
    : description?.slice(0, TEXT_LIMIT);

  return (
    <div className="py-8 border-b border-gray-200">
      <div className="prose prose-sm max-w-none text-gray-700 leading-7">
        <p>
          {displayText}
          {!isExpanded && isLongDescription && "..."}
        </p>
      </div>

      {isLongDescription && (
        <button
          onClick={() => setIsExpanded(!isExpanded)}
          className="mt-4 flex items-center gap-1 font-semibold underline decoration-2 underline-offset-4 hover:text-black transition"
        >
          {isExpanded ? "Show less" : "Show more"}
          {!isExpanded && <IoIosArrowForward />}
        </button>
      )}
    </div>
  );
};

export default ListingDescription;

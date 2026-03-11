import React, { useState } from "react";
import { allAmenities } from "../../data/AmenitiesData";
import { IoMdClose } from "react-icons/io";

const ListingAmenities = () => {
  const [isOpen, setIsOpen] = useState(false);

  // عرض أول 6 مرافق فقط في الصفحة الرئيسية
  const displayedAmenities = allAmenities.slice(0, 6);

  return (
    <div className="py-8 border-b border-gray-200">
      <h3 className="text-xl font-semibold mb-6">What this place offers</h3>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        {displayedAmenities.map((item, index) => (
          <div key={index} className="flex items-center gap-4 text-gray-700">
            <item.icon size={24} className="text-gray-600" />
            <span className="text-[16px]">{item.label}</span>
          </div>
        ))}
      </div>

      {allAmenities.length > 6 && (
        <button
          onClick={() => setIsOpen(true)}
          className="mt-8 px-6 py-3 border border-black rounded-lg font-semibold hover:bg-neutral-100 transition"
        >
          Show all {allAmenities.length} amenities
        </button>
      )}

      {/* --- Modal Window --- */}
      {isOpen && (
        <div className="fixed inset-0 z-[100] flex items-center justify-center bg-black/50 backdrop-blur-sm p-4">
          <div className="bg-white w-full max-w-2xl max-h-[90vh] rounded-2xl overflow-hidden flex flex-col shadow-2xl animate-in fade-in zoom-in duration-300">
            {/* Modal Header */}
            <div className="p-6 border-b sticky top-0 bg-white z-10 flex items-center">
              <button
                onClick={() => setIsOpen(false)}
                className="p-2 hover:bg-neutral-100 rounded-full transition"
              >
                <IoMdClose size={24} />
              </button>
            </div>

            {/* Modal Body */}
            <div className="p-8 overflow-y-auto">
              <h2 className="text-2xl font-semibold mb-8">
                What this place offers
              </h2>
              <div className="flex flex-col gap-6">
                {allAmenities.map((item, index) => (
                  <div key={index}>
                    <div className="flex items-center gap-4 py-2 text-gray-800">
                      <item.icon size={28} />
                      <span className="text-lg">{item.label}</span>
                    </div>
                    <hr className="mt-4 border-gray-100" />
                  </div>
                ))}
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default ListingAmenities;

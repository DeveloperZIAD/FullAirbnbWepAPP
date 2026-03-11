import { useState, useCallback } from "react";
import { IoClose, IoChevronBack, IoChevronForward } from "react-icons/io5";
import HeartButton from "./../../components/shared/HeartButton.jsx";
import { HiOutlineViewGrid } from "react-icons/hi";
const ListingHeader = ({ title, imageUrls = [], locationValue, id, initialIsFavorite }) => {
  const [isOpen, setIsOpen] = useState(false);
  const [currentIndex, setCurrentIndex] = useState(0);

  // وظائف التنقل بين الصور
  const showNext = useCallback(
    (e) => {
      e.stopPropagation();
      setCurrentIndex((prev) => (prev + 1) % imageUrls.length);
    },
    [imageUrls.length],
  );

  const showPrev = useCallback(
    (e) => {
      e.stopPropagation();
      setCurrentIndex(
        (prev) => (prev - 1 + imageUrls.length) % imageUrls.length,
      );
    },
    [imageUrls.length],
  );

  return (
    <>
      {/* 1. قسم العناوين */}
      <div className="text-start">
        <h1 className="text-2xl font-bold text-neutral-800">{title}</h1>
        <div className="font-light text-neutral-500 mt-2">{locationValue}</div>
      </div>

      {/* 2. قسم الصور (Grid للـ PC و Carousel للموبايل) */}
      <div className="relative mt-6 rounded-xl overflow-hidden h-[40vh] md:h-[70vh] group">
        {/* عرض الموبايل: سلايدر أفقي */}
        <div className="flex md:hidden h-full overflow-x-auto snap-x snap-mandatory scrollbar-hide">
          {imageUrls.map((url, index) => (
            <div
              key={index}
              className="min-w-full h-full snap-center"
              onClick={() => {
                setIsOpen(true);
                setCurrentIndex(index);
              }}
            >
              <img
                src={url}
                className="object-cover w-full h-full"
                alt={`${title} ${index}`}
              />
            </div>
          ))}
        </div>

        {/* عرض الـ PC: الشبكة (Grid) */}
        <div className="hidden md:grid grid-cols-4 grid-rows-2 gap-2 h-full">
          {/* الصورة الكبيرة */}
          <div
            onClick={() => {
              setIsOpen(true);
              setCurrentIndex(0);
            }}
            className="col-span-2 row-span-2 relative cursor-pointer overflow-hidden"
          >
            <img
              src={imageUrls[0]}
              className="object-cover w-full h-full hover:scale-105 transition duration-500"
              alt={title}
            />
          </div>

          {/* الصور الصغيرة (تظهر فقط في الـ PC) */}
          {imageUrls.slice(1, 5).map((url, index) => (
            <div
              key={index}
              onClick={() => {
                setIsOpen(true);
                setCurrentIndex(index + 1);
              }}
              className="relative cursor-pointer overflow-hidden"
            >
              <img
                src={url}
                className="object-cover w-full h-full hover:scale-110 transition duration-500"
                alt={`${title} ${index}`}
              />
            </div>
          ))}
        </div>

        {/* زر القلب - معدل ليكون أوضح في الموبايل */}
        <div className="absolute top-3 right-3 md:top-5 md:right-5 z-10">
          <HeartButton listingId={id} initialIsFavorite={initialIsFavorite} />
        </div>

        {/* عداد الصور الصغير للموبايل فقط (مثلاً: 1/8) */}
        <div className="md:hidden absolute bottom-4 right-4 bg-black/60 text-white px-3 py-1 rounded-full text-xs font-medium">
          {currentIndex + 1} / {imageUrls.length}
        </div>

        {/* زر عرض الكل (يظهر فقط في الـ PC) */}
        <button
          onClick={() => setIsOpen(true)}
          className="
            hidden
            md:flex
            absolute 
            bottom-5 
            right-8 
            items-center 
            gap-2 
            bg-white 
            hover:bg-neutral-100 
            transition 
            text-neutral-800 
            border-[1px] 
            border-black 
            rounded-lg 
            px-4 
            py-2 
            text-sm 
            font-semibold 
            shadow-md 
            active:scale-95
          "
        >
          <HiOutlineViewGrid size={18} />
          <span>Show all {imageUrls.length} photos</span>
        </button>
      </div>

      {/* 3. الـ Modal (معرض الصور الكامل) */}
      {isOpen && (
        <div className="fixed inset-0 z-[100] bg-black flex flex-col items-center justify-center select-none">
          {/* زر الإغلاق */}
          <button
            onClick={() => setIsOpen(false)}
            className="absolute top-8 left-8 text-white hover:text-neutral-400 transition"
          >
            <IoClose size={35} />
          </button>

          {/* عداد الصور */}
          <div className="absolute top-8 text-white font-light">
            {currentIndex + 1} / {imageUrls.length}
          </div>

          {/* أزرار التحكم */}
          <button
            onClick={showPrev}
            className="absolute left-4 md:left-10 text-white p-2 hover:bg-white/10 rounded-full transition"
          >
            <IoChevronBack size={40} />
          </button>

          <div className="w-full max-w-5xl px-4 flex justify-center items-center">
            <img
              src={imageUrls[currentIndex]}
              className="max-h-[85vh] max-w-full object-contain transition-all duration-300"
              alt="Gallery"
            />
          </div>

          <button
            onClick={showNext}
            className="absolute right-4 md:right-10 text-white p-2 hover:bg-white/10 rounded-full transition"
          >
            <IoChevronForward size={40} />
          </button>
        </div>
      )}
    </>
  );
};

export default ListingHeader;

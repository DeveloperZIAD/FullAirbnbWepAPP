import { AiOutlineHeart, AiFillHeart } from "react-icons/ai";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
const ListingCard = ({ data }) => {
  const navigate = useNavigate();

  // 1. تجاهل العقارات بدون صور
  if (!data.imageUrls || data.imageUrls.length === 0) {
    return null;
  }

  // 2. حالة المفضلة (مؤقتاً لحين ربط الـ API)
  const [hasFavorited, setHasFavorited] = useState(false);

  // 3. دالة معالجة الضغط على القلب
  const toggleFavorite = (e) => {
    e.stopPropagation(); // لمنع فتح صفحة التفاصيل عند الضغط على القلب
    setHasFavorited(!hasFavorited);
    // هنا سنستدعي listingService.toggleFavorite(data.id) لاحقاً
  };

  return (
    <div
      className="col-span-1 cursor-pointer group"
      onClick={() => navigate(`/listings/${data.id}`)}
    >
      <div className="flex flex-col gap-2 w-full">
        {/* حاوية الصورة مع زر القلب بداخلها */}
        <div className="aspect-square w-full relative overflow-hidden rounded-xl">
          <img
            src={data.imageUrls[0]}
            alt={data.title}
            className="object-cover h-full w-full group-hover:scale-110 transition duration-300"
          />

          {/* زر المفضلة - مكانه الصحيح داخل الـ relative div */}
          <div
            onClick={toggleFavorite}
            className="absolute top-3 right-3 transition hover:opacity-80 cursor-pointer"
          >
            <div className="relative">
              <AiOutlineHeart
                size={28}
                className="fill-white absolute -top-[2px] -right-[2px]"
              />
              <AiFillHeart
                size={24}
                className={
                  hasFavorited ? "fill-rose-500" : "fill-neutral-500/70"
                }
              />
            </div>
          </div>
        </div>

        {/* تفاصيل العقار */}
        <div className="flex flex-col gap-1">
          <div className="font-semibold text-lg">
            {data.city}, {data.countryCode}
          </div>
          <div className="font-light text-neutral-500">
            {data.category || "Property"}
          </div>
          <div className="flex flex-row items-center gap-1">
            <div className="font-semibold">${data.basePrice}</div>
            <div className="font-light">night</div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ListingCard;

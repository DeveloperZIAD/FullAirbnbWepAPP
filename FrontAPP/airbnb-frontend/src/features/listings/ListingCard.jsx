import React, { useMemo } from "react";
import { useNavigate } from "react-router-dom";
import { Swiper, SwiperSlide } from "swiper/react";
import { Pagination } from "swiper/modules";
import HeartButton from "./../../components/shared/HeartButton";

import "swiper/css";
import "swiper/css/pagination";

const ListingCard = ({ data }) => {
  const navigate = useNavigate();

  const isFavorite = useMemo(() => {
    try {
      const userStorage = localStorage.getItem("user");
      if (!userStorage) return false;
      const userData = JSON.parse(userStorage);
      return userData.favoriteIds?.includes(data.id) ?? false;
    } catch {
      return false;
    }
  }, [data.id]);

  return (
    <div
      onClick={() => navigate(`/listings/${data.id}`)}
      className="col-span-1 cursor-pointer group"
    >
      <div className="flex flex-col gap-2 w-full">
        <div className="aspect-[20/19] w-full relative overflow-hidden rounded-xl bg-neutral-100">
          <Swiper
            modules={[Pagination]}
            pagination={{ clickable: true }}
            className="h-full w-full"
          >
            {data.imageUrls
              ?.filter(
                (url) =>
                  url &&
                  typeof url === "string" &&
                  url.trim() !== "" &&
                  url !== "string",
              )
              .map((url, index) => (
                <SwiperSlide key={index}>
                  <img
                    src={url}
                    alt="Listing"
                    className="object-cover h-full w-full group-hover:scale-105 transition duration-500"
                    onError={(e) => {
                      e.target.style.display = "none";
                    }}
                  />
                </SwiperSlide>
              ))}
          </Swiper>

          <div className="absolute top-3 right-3 z-[10]">
            <HeartButton listingId={data.id} initialIsFavorite={isFavorite} />
          </div>
        </div>

        {/* ... بقية الـ UI كما هي ... */}
        <div className="px-1 mt-1 flex flex-col gap-0.5">
          <div className="flex flex-row items-center justify-between">
            <div className="font-bold text-[15px] text-neutral-800 truncate">
              {data.city}, {data.countryCode}
            </div>
            <div className="flex flex-row items-center gap-1 text-sm">
              <span className="text-black text-[10px]">★</span>
              <span className="font-light text-neutral-600">5.0</span>
            </div>
          </div>
          <div className="font-light text-neutral-500 text-[14px]">
            Stay with {data.hostName || "Host"}
          </div>
          <div className="font-light text-neutral-400 text-[13px]">
            {data.categoryName}
          </div>
          <div className="flex flex-row items-center gap-1 mt-1">
            <div className="font-semibold text-neutral-900">
              ${data.basePrice?.toLocaleString()}
            </div>
            <div className="font-light text-neutral-600 text-[14px]">night</div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default React.memo(ListingCard, (prev, next) => {
  return (
    prev.data.id === next.data.id && prev.data.basePrice === next.data.basePrice
  );
});

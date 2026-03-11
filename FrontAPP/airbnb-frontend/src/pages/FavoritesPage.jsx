import React, { useEffect, useState, useCallback } from "react";
import ListingCard from "./../features/listings/ListingCard.jsx";
import wishlistService from "./../services/WishlistService.js";
import { toast } from "react-hot-toast";

const FavoritesPage = () => {
  const [favorites, setFavorites] = useState([]);
  const [isLoading, setIsLoading] = useState(true);

  // جلب بيانات المستخدم من localStorage
  const user = JSON.parse(localStorage.getItem("user"));
  const userId = user?.id;

  const fetchFavorites = useCallback(async () => {
    if (!userId) {
      setIsLoading(false);
      return;
    }

    try {
      setIsLoading(true);
      const data = await wishlistService.getFavorites(userId);

      /* تأكد من شكل البيانات القادمة من الباك اند:
         إذا كان السيرفر يعيد مصفوفة من الـ Wishlist Objects (التي تحتوي على userId و listingId وعنصر الـ listing)
         فنحن نحتاج لاستخراج الـ listings فقط منها.
      */
      const formattedData = data.map((item) =>
        item.listing ? item.listing : item,
      );

      setFavorites(formattedData);
    } catch (error) {
      console.error("Error fetching favorites:", error);
      toast.error("Could not load your favorites");
    } finally {
      setIsLoading(false);
    }
  }, [userId]);

  useEffect(() => {
    fetchFavorites();
  }, [fetchFavorites]);

  // دالة لتحديث القائمة محلياً فور إزالة عنصر (لحذف العنصر من الشاشة دون عمل ريلود)
  const handleFavoriteChange = (listingId) => {
    setFavorites((prev) => prev.filter((item) => item.id !== listingId));
  };

  if (isLoading) {
    return (
      <div className="pt-2 flex flex-col items-center justify-center gap-4">
        <div className="w-12 h-12 border-4 border-rose-500 border-t-transparent rounded-full animate-spin"></div>
        <p className="text-rose-500 font-medium">Loading your wishlist...</p>
      </div>
    );
  }

  if (!userId) {
    return (
      <div className="pt-28 text-center">
        <h1 className="text-2xl font-bold">Please login</h1>
        <p className="text-neutral-500">
          You need to be logged in to see your favorites.
        </p>
      </div>
    );
  }

  if (favorites.length === 0) {
    return (
      <div className="pt-48 text-center flex flex-col items-center gap-4 px-4">
        <h1 className="text-2xl font-bold">Your wishlist is empty</h1>
        <p className="text-neutral-500 text-lg max-w-md">
          Looks like you haven't saved any homes yet. Start exploring and click
          the heart icon to save your favorites!
        </p>
        <button
          onClick={() => (window.location.href = "/")}
          className="mt-4 px-10 py-3 bg-rose-500 text-white rounded-lg font-semibold hover:bg-rose-600 transition shadow-md"
        >
          Explore homes
        </button>
      </div>
    );
  }

  return (
    <div className="max-w-[2520px] mx-auto xl:px-20 md:px-10 sm:px-2 px-4 pt-48 pb-10">
      <div className="flex flex-col gap-2 mb-10 border-b pb-6">
        <h1 className="text-3xl font-bold text-neutral-800">Wishlist</h1>
        <p className="font-light text-neutral-500">
          You have {favorites.length}{" "}
          {favorites.length === 1 ? "place" : "places"} saved
        </p>
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 2xl:grid-cols-6 gap-8">
        {favorites.map((listing) => (
          <ListingCard
            key={listing.id}
            data={listing}
            // نرسل دالة التحديث لتعديل الـ UI فوراً عند الضغط على القلب
            onToggleFavorite={() => handleFavoriteChange(listing.id)}
          />
        ))}
      </div>
    </div>
  );
};

export default FavoritesPage;

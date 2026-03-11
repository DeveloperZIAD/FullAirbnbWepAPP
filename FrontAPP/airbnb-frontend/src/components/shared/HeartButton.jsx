import React, { useState, useEffect } from "react";
import { AiFillHeart, AiOutlineHeart } from "react-icons/ai";
import { toast } from "react-hot-toast";
import WishlistService from "./../../services/WishlistService.js";

const HeartButton = ({ listingId, initialIsFavorite, onToggle }) => {
  const [favorite, setFavorite] = useState(initialIsFavorite);

  useEffect(() => {
    setFavorite(initialIsFavorite);
  }, [initialIsFavorite]);

  const toggle = async (e) => {
    e.stopPropagation();

    const accessToken = localStorage.getItem("accessToken");
    const userStorage = localStorage.getItem("user");

    if (!accessToken || !userStorage) {
      return toast.error("Please login first to add favorites");
    }

    // 1. التحديث المتفائل للـ UI (Optimistic Update)
    const previousState = favorite;
    setFavorite(!previousState);

    try {
      // 2. إرسال الطلب للسيرفر
      await WishlistService.toggleFavorite(listingId);

      // 3. تحديث الـ LocalStorage لمزامنة الحالة في كل الموقع
      const userData = JSON.parse(userStorage);
      let updatedFavorites = [...(userData.favoriteIds || [])];

      if (!previousState) {
        // إضافة الـ ID إذا لم يكن موجوداً
        updatedFavorites.push(listingId);
      } else {
        // إزالة الـ ID إذا كان موجوداً
        updatedFavorites = updatedFavorites.filter((id) => id !== listingId);
      }

      // حفظ التعديل في الستورج
      userData.favoriteIds = updatedFavorites;
      localStorage.setItem("user", JSON.stringify(userData));
      window.dispatchEvent(new Event("wishlistUpdated"));
      if (onToggle) {
        onToggle(!previousState);
      }

      toast.success(
        !previousState ? "Added to favorites" : "Removed from favorites",
      );
    } catch (error) {
      // 4. التراجع في حال الفشل
      setFavorite(previousState);
      console.error("Wishlist toggle error:", error);
      toast.error("Failed to update wishlist");
    }
  };

  return (
    <div
      onClick={toggle}
      className="relative hover:opacity-80 transition cursor-pointer active:scale-90"
    >
      <AiOutlineHeart
        size={28}
        className="fill-white absolute -top-[2px] -right-[2px] drop-shadow-sm"
      />
      <AiFillHeart
        size={24}
        className={favorite ? "fill-rose-500" : "fill-black/20"}
      />
    </div>
  );
};

export default HeartButton;

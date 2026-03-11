import { useParams, useNavigate } from "react-router-dom";
import { useState, useEffect, useCallback } from "react";
import { differenceInCalendarDays } from "date-fns";
import { toast } from "react-hot-toast";
import { Star } from "lucide-react"; // icon used in reviews header

import ListingHeader from "../features/listings/ListingHeader.jsx";
import ListingInfo from "../features/listings/ListingInfo.jsx";
import { categories } from "../components/layout/Categories.jsx";
import Calendar from "../components/shared/Calendar.jsx";
import listingService from "../services/listingService";
import bookingService from "../services/bookingService";
import authService from "../services/authService.jsx";
import reviewService from "@/services/reviewService.js";
import ReviewForm from "@/components/inputs/ReviewForm.jsx";
import ReviewList from "@/features/listings/ReviewList.jsx";
import HeartButton from "../components/shared/HeartButton.jsx";
import messagingService from "../services/messagingService";

const getStartOfToday = () => {
  const date = new Date();
  date.setHours(0, 0, 0, 0); // تصفير الساعات، الدقائق، الثواني
  return date;
};

const initialDateRange = {
  startDate: getStartOfToday(),
  endDate: getStartOfToday(),
  key: "selection",
};

const ListingPage = () => {
  const { listingId } = useParams();
  const navigate = useNavigate();

  const currentUser = authService.getCurrentUser();

  const [listing, setListing] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isBooking, setIsBooking] = useState(false); // حالة خاصة بعملية الحجز
  const [dateRange, setDateRange] = useState(initialDateRange);
  const [totalPrice, setTotalPrice] = useState(0);
  const [reviews, setReviews] = useState([]); // تقييمات العقار
  const [isFavorite, setIsFavorite] = useState(false);

  // ميثود للتحقق من حالة المفضلات لهذا العقار تحديداً
  const checkFavoriteStatus = () => {
    const userStorage = localStorage.getItem("user");
    if (userStorage) {
      const userData = JSON.parse(userStorage);
      const favorited = userData.favoriteIds?.includes(listingId);
      setIsFavorite(!!favorited);
    }
  };

  useEffect(() => {
    // تشغيل الفحص عند تحميل الصفحة
    checkFavoriteStatus();

    // اختياري: مزامنة الحالة إذا تغير الستورج وأنت داخل الصفحة
    window.addEventListener("wishlistUpdated", checkFavoriteStatus);
    return () =>
      window.removeEventListener("wishlistUpdated", checkFavoriteStatus);
  }, [listingId]);

  // 1. جلب بيانات العقار
  useEffect(() => {
    const fetchListing = async () => {
      try {
        setIsLoading(true);
        const data = await listingService.getListingById(listingId);
        setListing(data);
        setTotalPrice(data.basePrice);
      } catch (error) {
        console.error("Error fetching listing details:", error);
        toast.error("Failed to load listing details");
      } finally {
        setIsLoading(false);
      }
    };
    fetchListing();
  }, [listingId]);

  // 2. حساب السعر الإجمالي تلقائياً عند تغيير التاريخ
  useEffect(() => {
    if (listing && dateRange.startDate && dateRange.endDate) {
      const dayCount = differenceInCalendarDays(
        dateRange.endDate,
        dateRange.startDate,
      );

      // إذا كان الفرق 0 أو أقل، نعتبرها ليلة واحدة (الحد الأدنى)
      const nights = dayCount <= 0 ? 1 : dayCount;
      setTotalPrice(nights * listing.basePrice);
    }
  }, [dateRange, listing]);
  // تحميل التقييمات عند تحميل الصفحة أو تغيير الـ listingId
  useEffect(() => {
    const fetchReviews = async () => {
      try {
        const data = await reviewService.getReviewsByListing(listingId);
        setReviews(data);
      } catch (error) {
        console.error("Failed fetching reviews:", error);
      }
    };

    if (listingId) {
      fetchReviews();
    }
  }, [listingId]);

  // 3. وظيفة الحجز (The Reserve Logic)
  const onCreateBooking = useCallback(async () => {
    const token = localStorage.getItem("accessToken");
    if (!token) {
      toast.error("Please login to reserve this home");
      return;
    }

    const guestId = authService.getCurrentUser()?.id;

    try {
      setIsBooking(true);

      // --- الجزء المعدل هنا ---
      const start = new Date(dateRange.startDate);
      start.setHours(12, 0, 0, 0); // نضبط الساعة 12 ظهراً لتجنب مشاكل المناطق الزمنية

      let end = new Date(dateRange.endDate);
      end.setHours(12, 0, 0, 0);

      // إذا اختار المستخدم يوماً واحداً (البداية تساوي النهاية)
      // نجعل النهاية تلقائياً في اليوم التالي
      if (start.getTime() === end.getTime()) {
        end.setDate(end.getDate() + 1);
      }
      // -----------------------

      const payload = {
        listingId: listing.id,
        checkIn: start.toISOString(),
        checkOut: end.toISOString(),
        guestId: guestId,
      };

      const response = await bookingService.createBooking(payload);
      const newId = response?.bookingId?.bookingId;

      if (newId) {
        toast.success("Reservation successful! Redirecting to payment...");
        navigate(`/payment/${newId}`);
      } else {
        toast.success("Reservation successful!");
        navigate("/trips");
      }
    } catch (error) {
      console.error("Booking Error:", error);
      const errorMsg = error.response?.data?.message || "Something went wrong";
      toast.error(errorMsg);
    } finally {
      setIsBooking(false);
    }
  }, [dateRange, listing, navigate]);
  // إضافة تقييم جديد
  const handleAddReview = async (reviewData) => {
    console.log("📝 Adding review with data:", { ...reviewData, listingId });
    try {
      const newReview = await reviewService.addReview({
        ...reviewData,
        listingId,
      });
      console.log("✅ Review added successfully:", newReview);
      setReviews((prev) => [newReview, ...prev]);
      toast.success("Review added!");
    } catch (err) {
      console.error(
        "❌ Error adding review:",
        err.response?.data || err.message,
      );
      toast.error(err.response?.data?.message || "Failed to add review");
    }
  };

  // حذف تقييم
  const handleDeleteReview = async (reviewId) => {
    try {
      await reviewService.deleteReview(reviewId);
      setReviews((prev) => prev.filter((r) => r.id !== reviewId));
      toast.success("Review deleted");
    } catch (err) {
      toast.error("Error deleting review");
    }
  };

  // معالج بدء محادثة مع المضيف
  const handleStartChat = async () => {
    try {
      const currentUserId = currentUser?.id;
      if (!currentUserId) {
        toast.error("Please login to start a conversation");
        return;
      }

      const conversation = await messagingService.startConversation({
        listingId: listing.id,
        guestId: currentUserId,
        hostProfileId: listing.host?.id,
      });

      if (conversation && conversation.id) {
        navigate(`/messages/${conversation.id}`);
      } else {
        toast.error("Failed to start conversation");
      }
    } catch (error) {
      console.error("Error starting conversation:", error);
      toast.error("Failed to start conversation");
    }
  };

  if (isLoading)
    return (
      <div className="pt-24 text-center animate-pulse text-rose-500 font-semibold">
        Loading home details...
      </div>
    );

  if (!listing)
    return (
      <div className="pt-24 text-center text-neutral-500">
        Listing not found.
      </div>
    );

  const category = categories.find((item) => item.label === listing.category);

  return (
    <div className="max-w-screen-xl mx-auto px-1 md:px-45 pt-1">
      <div className="flex flex-col gap-6">
        {/* الهيدر: الصور والعنوان واللوكيشن */}
        <ListingHeader
          title={listing.title}
          imageUrls={listing.imageUrls}
          // الوصول للبيانات من الكائن الجديد
          locationValue={`${listing.location?.city}, ${listing.location?.countryCode}`}
          id={listing.id}
          initialIsFavorite={isFavorite}
        />

        <div className="grid grid-cols-1 md:grid-cols-7 md:gap-10 mt-6">
          <ListingInfo
            host={listing.host}
            categoryName={listing.categoryName} // مرر الاسم هنا
            description={listing.description}
            guestCount={listing.guestCount}
            roomCount={listing.roomCount}
            bathroomCount={listing.bathroomCount}
            location={listing.location}
            onChatClick={handleStartChat}
          />

          {/* الجزء الأيمن: كارت الحجز (Sticky Booking Card) */}

          <div className="md:col-span-3 order-first mb-10 md:order-last relative">
            <div className="sticky top-24 h-fit">
              {" "}
              {/* h-fit تضمن أن الحاوية تتحرك بالكامل */}
              <div className="bg-white rounded-xl border-[1px] border-neutral-200 overflow-hidden shadow-lg">
                <div className="flex flex-row items-center gap-1 p-4">
                  <div className="text-2xl font-bold text-neutral-800">
                    $ {listing.basePrice}
                  </div>
                  <div className="font-light text-neutral-500">night</div>
                </div>
                <hr />

                {/* التقويم: تم تكبيره عبر إزالة scale-95 وزيادة الـ padding */}
                <div className="p-4 pl-0 md:pl-[75px] flex justify-center w-full bg-white">
                  <div className="w-full scale-100 md:scale-110 origin-center md:origin-top-left">
                    {" "}
                    {/* تكبير بسيط بنسبة 5% ليظهر بشكل أوضح */}
                    <Calendar
                      value={dateRange}
                      onChange={(value) => setDateRange(value.selection)}
                      minDate={getStartOfToday()} // منع اختيار الأيام الماضية
                    />
                  </div>
                </div>

                <hr />

                <div className="p-4">
                  <button
                    disabled={isBooking}
                    onClick={onCreateBooking}
                    className={`
            w-full bg-rose-500 text-white py-3 rounded-lg font-bold 
            transition active:scale-95 shadow-sm
            ${isBooking ? "opacity-70 cursor-not-allowed" : "hover:bg-rose-600"}
          `}
                  >
                    {isBooking ? "Reserving..." : "Reserve"}
                  </button>
                  <p className="text-center text-neutral-400 mt-3 text-[11px]">
                    You won't be charged yet
                  </p>
                </div>

                <hr />

                <div className="p-4 flex flex-row items-center justify-between font-bold text-lg text-neutral-800">
                  <div>Total</div>
                  <div>$ {totalPrice}</div>
                </div>
              </div>
            </div>
          </div>
        </div>

        {/* reviews */}
        <hr className="my-12 border-neutral-200" />
        <div className="max-w-screen-xl mx-auto md:px-45">
          <h2 className="text-2xl font-bold flex items-center gap-2">
            <Star fill="black" size={20} /> {reviews.length} reviews
          </h2>
          <ReviewList
            reviews={reviews}
            currentUser={currentUser}
            onDelete={handleDeleteReview}
          />
          {currentUser && <ReviewForm onAdd={handleAddReview} />}
        </div>
      </div>
    </div>
  );
};

export default ListingPage;

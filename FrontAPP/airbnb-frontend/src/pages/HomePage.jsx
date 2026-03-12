import { useEffect, useState, useRef, useCallback } from "react";
import { useSearchParams } from "react-router-dom";
import listingService from "@/services/listingService";
import ListingCard from "./../features/listings/ListingCard.jsx";
import EmptyState from "@/components/shared/EmptyState";
import Loader from "@/components/shared/Loader.jsx";

const Home = () => {
  const [listings, setListings] = useState([]);
  const [loading, setLoading] = useState(false);
  const [hasFetched, setHasFetched] = useState(false); // تم إضافة هذا المتغير للتحكم في ظهور EmptyState
  const [page, setPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);
  const [searchParams] = useSearchParams();
  const observer = useRef();

  const lastListingRef = useCallback(
    (node) => {
      if (loading) return;
      if (observer.current) observer.current.disconnect();
      observer.current = new IntersectionObserver((entries) => {
        if (entries[0].isIntersecting && hasMore) {
          setPage((prev) => prev + 1);
        }
      });
      if (node) observer.current.observe(node);
    },
    [loading, hasMore],
  );

  const fetchListings = useCallback(async () => {
    setLoading(true);
    try {
      const params = Object.fromEntries([...searchParams]);
      const isSearching = Object.keys(params).length > 0;

      if (isSearching) {
        const data = await listingService.searchListings(params);
        setListings(data.filter((l) => l.imageUrls?.length > 0));
        setHasMore(false);
      } else {
        const data = await listingService.getListingsPaged(page, 10);
        if (data.length < 10) {
          setHasMore(false);
        }
        setListings((prev) => (page === 1 ? data : [...prev, ...data]));
      }
    } catch (error) {
      console.error("Error fetching listings:", error);
    } finally {
      setLoading(false);
      setHasFetched(true); // تأكيد انتهاء أول عملية جلب
    }
  }, [searchParams, page]);

  // 1. إعادة التعيين عند تغيير الفلاتر
  useEffect(() => {
    setPage(1);
    setHasMore(true);
    setListings([]);
    setHasFetched(false); // إعادة تعيين الحالة عند تغيير البحث
  }, [searchParams]);

  // 2. تشغيل الـ fetch مع مراعاة الـ Debounce للبحث فقط
  useEffect(() => {
    const isSearching =
      Object.keys(Object.fromEntries([...searchParams])).length > 0;

    if (isSearching) {
      const handler = setTimeout(() => {
        fetchListings();
      }, 300);
      return () => clearTimeout(handler);
    } else {
      fetchListings();
    }
  }, [fetchListings]);

  // شرط إظهار EmptyState تم تعديله ليعتمد على hasFetched
  if (hasFetched && listings.length === 0 && !loading) {
    return (
      <EmptyState
        showReset
        title="No matches"
        subtitle="Try removing filters."
      />
    );
  }

  return (
    <div className="w-full px-4 md:px-10 lg:px-16 mx-auto mb-10 pt-24">
      <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 2xl:grid-cols-6 gap-6">
        {listings.map((listing, index) => {
          if (listings.length === index + 1 && hasMore) {
            return (
              <div ref={lastListingRef} key={listing.id}>
                <ListingCard data={listing} />
              </div>
            );
          }
          return <ListingCard key={listing.id} data={listing} />;
        })}
      </div>
      {loading && (
        <div className="mt-10">
          <Loader />
        </div>
      )}
    </div>
  );
};

export default Home;

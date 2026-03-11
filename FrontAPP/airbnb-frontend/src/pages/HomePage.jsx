import { useEffect, useState, useRef, useCallback } from "react";
import { useSearchParams } from "react-router-dom";
import listingService from "@/services/listingService";
import ListingCard from "./../features/listings/ListingCard.jsx";
import EmptyState from "@/components/shared/EmptyState";
import Loader from "@/components/shared/Loader.jsx";

const Home = () => {
  const [listings, setListings] = useState([]);
  const [loading, setLoading] = useState(false);
  const [page, setPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);
  const [searchParams] = useSearchParams();
  const observer = useRef();

  const lastListingRef = useCallback(
    (node) => {
      if (loading) return;
      if (observer.current) observer.current.disconnect();
      observer.current = new IntersectionObserver((entries) => {
        // لا نستخدم الـ observer في حالة البحث
        if (entries[0].isIntersecting && hasMore) {
          setPage((prev) => prev + 1);
        }
      });
      if (node) observer.current.observe(node);
    },
    [loading, hasMore],
  );

  const fetchListings = async () => {
    setLoading(true);
    try {
      const params = Object.fromEntries([...searchParams]);
      const isSearching = Object.keys(params).length > 0;

      if (isSearching) {
        // حالة البحث: جلب الكل مباشرة

        const data = await listingService.searchListings(params);
        setListings(data.filter((l) => l.imageUrls?.length > 0));
        setHasMore(false); // البحث لا يحتاج تحميل مستمر
      } else {
        // حالة التصفح: ترقيم 10 بـ 10
        const data = await listingService.getListingsPaged(page, 10);
        if (data.length === 0) {
          setHasMore(false);
        } else {
          setListings((prev) => (page === 1 ? data : [...prev, ...data]));
        }
      }
    } catch (error) {
      console.error(error);
    } finally {
      setLoading(false);
    }
  };

  // إعادة التعيين عند تغيير البحث
  useEffect(() => {
    setPage(1);
    setListings([]);
  }, [searchParams]);

  useEffect(() => {
    fetchListings();
  }, [page, searchParams]);

  if (listings.length === 0 && !loading) {
    return (
      <EmptyState
        showReset
        title="No matches"
        subtitle="Try removing filters."
      />
    );
  }

  return (
    // إضافة الـ Padding والتنسيق الأصلي
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
      {loading && <Loader />}
    </div>
  );
};

export default Home;

import { useEffect, useState } from "react";
import listingService from "../services/listingService";

const TestListings = () => {
  const [listings, setListings] = useState([]);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const data = await listingService.getAllListingsWithDetails();
        console.log("Data from API ✅:", data); // هنا هنشوف الـ JSON اللي كنا مستنيينه
        setListings(data);
      } catch (err) {
        console.log("API Error ❌:", err.message);
      }
    };
    fetchData();
  }, []);

  return (
    <div className="p-4">
      <h2 className="text-lg font-semibold">
        Listings Count: {listings.length}
      </h2>
      {/* هنا ممكن نعمل Map ونعرض العناوين للتأكد */}
    </div>
  );
};

export default TestListings;

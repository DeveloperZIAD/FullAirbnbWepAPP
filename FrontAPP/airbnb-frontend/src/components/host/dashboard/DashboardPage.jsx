"use client";
import { useEffect, useState, useCallback } from "react";
import hostService from "./../../../services/hostService.js";
import PropertyTable from "./../PropertyTable.jsx";
import StatCard from "./../StatCard.jsx";
import ListingForm from "./../ListingForm.jsx"; // generic form for create/edit
import { DollarSign, Home, CheckCircle, Loader2, X } from "lucide-react";
import listingService from "@/services/listingService.js";
import toast from "react-hot-toast"; // أضف هذا السطر في أعلى الملف
export default function DashboardPage() {
  const [listings, setListings] = useState([]);
  const [stats, setStats] = useState({
    revenue: 0,
    listingsCount: 0,
    bookings: 0,
  });
  const [loading, setLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false); // حالة المودال

  const fetchData = useCallback(async () => {
    try {
      const storedUser = localStorage.getItem("user");
      if (!storedUser) return;
      const hostId = JSON.parse(storedUser).id;

      const [listingsData, statsData] = await Promise.all([
        hostService.getMyListings(hostId),
        hostService.getDashboardStats(hostId),
      ]);

      setListings(listingsData);
      setStats(statsData);
    } catch (err) {
      console.error("Error fetching host data:", err);
    } finally {
      setLoading(false);
    }
  }, []);

  // دالة الحذف
  // داخل DashboardPage.jsx
  const handleDelete = async (id) => {
    console.log("Attempting to delete ID:", id); // تأكد أن الـ ID يصل هنا
    try {
      await listingService.deleteListing(id);
      console.log("Delete successful, fetching new data...");
      await fetchData(); // استخدم await هنا لضمان انتهاء التحديث
    } catch (err) {
      console.error("Delete failed:", err.response?.data || err);
      toast.error("Could not delete listing.");
    }
  };
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [selectedListing, setSelectedListing] = useState(null);
  const [editingId, setEditingId] = useState(null); // إضافة هذا الـ State
  const handleEdit = async (id) => {
    setEditingId(id); // نحدد الـ ID فوراً ليظهر مؤشر التحميل في الزر
    try {
      const data = await listingService.getListingById(id);
      setSelectedListing(data);
      setIsEditModalOpen(true);
    } catch (error) {
      toast.error("Failed to fetch property details.");
    } finally {
      setEditingId(null); // نلغي التحميل بعد انتهاء العملية
    }
  };

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  return (
    <div className="space-y-10 animate-in fade-in duration-500">
      <header className="flex justify-between items-center">
        <div>
          <h1 className="text-3xl font-extrabold text-neutral-800">
            Host Dashboard
          </h1>
          <p className="text-neutral-500 mt-1">Manage your properties here.</p>
        </div>
        <button
          onClick={() => setIsModalOpen(true)}
          className="bg-rose-500 text-white px-6 py-3 rounded-xl font-bold hover:bg-rose-600 transition shadow-md"
        >
          + Create New Listing
        </button>
      </header>

      {/* المودال الخاص بالإضافة */}
      {isModalOpen && (
        <div className="fixed inset-0 bg-black/50 z-50 flex items-center justify-center p-4">
          <div className="bg-white rounded-2xl w-full max-w-2xl max-h-[90vh] overflow-y-auto relative">
            <button
              onClick={() => setIsModalOpen(false)}
              className="absolute top-4 right-4 p-2 hover:bg-neutral-100 rounded-full"
            >
              <X size={20} />
            </button>
            <ListingForm
              onClose={() => setIsModalOpen(false)}
              onSave={() => {
                fetchData(); // إعادة جلب البيانات لتحديث الجدول
                setIsModalOpen(false);
              }}
            />
          </div>
        </div>
      )}

      {/* مودال التعديل */}
      {isEditModalOpen && selectedListing && (
        <div className="fixed inset-0 bg-black/50 z-50 flex items-center justify-center p-4">
          <div className="bg-white rounded-2xl w-full max-w-2xl max-h-[90vh] overflow-y-auto relative">
            <button
              onClick={() => setIsEditModalOpen(false)}
              className="absolute top-4 right-4 p-2 hover:bg-neutral-100 rounded-full"
            >
              <X size={20} />
            </button>
            <ListingForm
              initialData={selectedListing} // تمرير البيانات هنا
              onClose={() => {
                setIsEditModalOpen(false);
                setSelectedListing(null);
              }}
              onSave={() => {
                fetchData();
                setIsEditModalOpen(false);
                setSelectedListing(null);
              }}
            />
          </div>
        </div>
      )}

      {/* باقي محتوى الصفحة (StatCards & Table) كما هو */}
      {loading ? (
        <div className="flex justify-center h-64 items-center">
          <Loader2 className="animate-spin" />
        </div>
      ) : (
        <>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            <StatCard
              label="Total Revenue"
              value={`$${stats.revenue?.toLocaleString() || 0}`}
              icon={DollarSign}
            />
            <StatCard
              label="My Listings"
              value={listings.length || 0}
              icon={Home}
            />
            <StatCard
              label="Active Bookings"
              value={stats.bookings || 0}
              icon={CheckCircle}
            />
          </div>
          <section>
            <h2 className="text-xl font-bold mb-4">Your Properties</h2>
            <div className="bg-white border rounded-xl shadow-sm overflow-hidden">
              <PropertyTable
                listings={listings}
                onEdit={handleEdit}
                onDelete={handleDelete}
                loadingId={editingId} // الآن سيظهر المؤشر فوراً عند الضغط
              />
            </div>
          </section>
        </>
      )}
    </div>
  );
}

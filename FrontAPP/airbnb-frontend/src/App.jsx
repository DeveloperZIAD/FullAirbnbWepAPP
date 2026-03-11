import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import MainLayout from "./components/layout/MainLayout";
import ListingCard from "./components/shared/ListingCard";
import ListingPage from "./pages/ListingPage"; // تأكد من المسار
import RegisterModal from "./features/auth/RegisterModal.jsx";
import LoginModal from "./features/auth/LoginModal";
import useRentModal from "@/hooks/useRentModal";
import RentModal from "./components/modals/RentModal.jsx";
import FavoritesPage from "./pages/FavoritesPage";
import SearchModal from "./features/search/SearchModal";
import useSearchModal from "@/hooks/useSearchModal";
import PropertiesPage from "./pages/PropertiesPage";
import { Toaster } from "react-hot-toast";

import useRegisterModal from "./hooks/useRegisterModal";
import useLoginModal from "./hooks/useLoginModal";

import AdminLayout from "./pages/admin/AdminLayout";
import AdminUsers from "./pages/admin/Users";
import AdminListings from "./pages/admin/Listings";
import AdminDashboard from "./pages/admin/Dashboard";
import AdminReservations from "./pages/admin/Reservations";
import NotFound from "./pages/NotFound";
import listingService from "./services/listingService.js";
import TestListings from "./components/TestListings";
import Home from "./pages/HomePage";
import TripsPage from "./pages/TripsPage.jsx";
import PaymentPage from "./pages/PaymentPage.jsx";
import PaymentSuccess from "./pages/PaymentSuccessPage.jsx";
import Hostpage from "./components/host/dashboard/Hostpage.jsx";
import DashboardPage from "./components/host/dashboard/DashboardPage.jsx";
import ReservationsPage from "./components/host/dashboard/ReservationsPage.jsx";
import SettingsPage from "./components/host/dashboard/SettingsPage.jsx";
import ChatPage from "./pages/messagingSystem/ChatPage.jsx";
import InboxPage from "./pages/messagingSystem/InboxPage.jsx";

function App() {
  const rentModal = useRentModal();
  const registerModal = useRegisterModal();
  const loginModal = useLoginModal();
  const searchModal = useSearchModal();

  return (
    <Router>
      <Toaster position="top-center" reverseOrder={false} />

      {/* ربط المودالات بالحالة المركزية */}
      <RegisterModal
        isOpen={registerModal.isOpen}
        onClose={registerModal.onClose}
      />
      <LoginModal isOpen={loginModal.isOpen} onClose={loginModal.onClose} />
      <RentModal isOpen={rentModal.isOpen} onClose={rentModal.onClose} />
      <SearchModal isOpen={searchModal.isOpen} onClose={searchModal.onClose} />

      <MainLayout>
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/listings/:listingId" element={<ListingPage />} />
          <Route path="/favorites" element={<FavoritesPage />} />
          <Route path="/properties" element={<PropertiesPage />} />
          <Route path="/messages/:conversationId" element={<ChatPage />} />
          <Route path="/messages" element={<InboxPage />} />

          <Route path="/admin" element={<AdminLayout />}>
            <Route index element={<AdminDashboard />} />
            <Route path="users" element={<AdminUsers />} />
            <Route path="listings" element={<AdminListings />} />
            <Route path="reservations" element={<AdminReservations />} />
          </Route>

          <Route path="*" element={<NotFound />} />
          <Route path="trips" element={<TripsPage />} />
          <Route path="/payment/success" element={<PaymentSuccess />} />
          <Route path="/payment/:bookingId" element={<PaymentPage />} />

          <Route path="/host" element={<Hostpage />}>
            {/* هنا يتم عرض الكومبوننت داخل الـ Outlet الموجود في Hostpage */}
            <Route index element={<DashboardPage />} />{" "}
            {/* هذا سيظهر عند الدخول على /host مباشرة */}
            <Route path="dashboard" element={<DashboardPage />} />
            <Route path="reservations" element={<ReservationsPage />} />
            <Route path="settings" element={<SettingsPage />} />
            <Route path="inbox" element={<InboxPage />} />
          </Route>
        </Routes>
      </MainLayout>
    </Router>
  );
}

export default App;

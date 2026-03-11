import { Search, Menu, User, Globe } from "lucide-react";
import { useNavigate } from "react-router-dom";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import useRegisterModal from "@/hooks/useRegisterModal";
import useLoginModal from "@/hooks/useLoginModal";
import useRentModal from "@/hooks/useRentModal";
import useSearchModal from "@/hooks/useSearchModal";
import Categories from "./Categories.jsx";
import UserMenu from "./UserMenu.jsx";
import { useState, useEffect } from "react";
import authService from "@/services/authService";
import logo from "./../../assets/logo (2).png"; // تأكد من مسار الصورة واسمها (png أو svg)

const Navbar = () => {
  const navigate = useNavigate();
  const registerModal = useRegisterModal();
  const loginModal = useLoginModal();
  const rentModal = useRentModal();
  const searchModal = useSearchModal();

  // تعريف الحالة للمستخدم
  const [currentUser, setCurrentUser] = useState(null);
  const [userRole, setUserRole] = useState(null); // Host or User

  // دالة لجلب البيانات من الستورج
  const loadUser = () => {
    const userString = localStorage.getItem("user");
    if (userString) {
      try {
        setCurrentUser(JSON.parse(userString));
      } catch (e) {
        setCurrentUser(null);
      }
    } else {
      setCurrentUser(null);
    }
  };

  useEffect(() => {
    // تحميل المستخدم عند أول ظهور للمكون
    loadUser();

    // الاستماع لحدث 'storage' (مفيد للتبويبات المتعددة)
    window.addEventListener("storage", loadUser);

    // حدث مخصص (Custom Event) سنطلقه عند التسجيل/الدخول في نفس التبويب
    window.addEventListener("userLoginStatus", loadUser);

    return () => {
      window.removeEventListener("storage", loadUser);
      window.removeEventListener("userLoginStatus", loadUser);
    };
  }, []);

  // when currentUser changes we may need to fetch role
  useEffect(() => {
    if (currentUser?.id) {
      authService
        .getUserRole(currentUser.id)
        .then((role) => setUserRole(role))
        .catch((err) => console.error("Error getting role", err));
    } else {
      setUserRole(null);
    }
  }, [currentUser?.id]);

  const onLogout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("user");
    setCurrentUser(null);
    navigate("/");
  };

  return (
    <div className="fixed w-full bg-white z-10 shadow-sm">
      <nav className="py-4 border-b-[1px]">
        <div className="max-w-[2520px] mx-auto xl:px-20 md:px-10 sm:px-2 px-4">
          <div className="flex flex-row items-center justify-between gap-3 md:gap-0">
            {/* Logo */}
            <div
              onClick={() => (window.location.href = "/")}
              className="flex items-center gap-2 cursor-pointer hidden md:flex"
            >
              {/* أيقونة اللوجو */}
              <img
                src={logo}
                alt="Logo"
                className="w-8 h-8 object-contain" // يمكنك التحكم في الحجم هنا
              />

              {/* اسم البراند */}
              <span className="text-[#FF385C] font-bold text-2xl">airbnb</span>
            </div>

            {/* Search Bar */}
            <div
              onClick={searchModal.onOpen}
              className="border-[1px] w-full md:w-auto py-2 rounded-full shadow-sm hover:shadow-md transition cursor-pointer"
            >
              <div className="flex flex-row items-center justify-between">
                <div className="text-sm font-semibold px-6">Anywhere</div>
                <div className="hidden sm:block text-sm font-semibold px-6 border-x-[1px] flex-1 text-center">
                  Any Week
                </div>
                <div className="text-sm pl-6 pr-2 text-gray-600 flex flex-row items-center gap-3">
                  <div className="hidden sm:block">Add Guests</div>
                  <div className="p-2 bg-rose-500 rounded-full text-white">
                    <Search size={18} />
                  </div>
                </div>
              </div>
            </div>

            {/* User Actions */}
            <div className="flex flex-row items-center gap-3">
              {userRole !== "Host" && (
                <div
                  onClick={rentModal.onOpen}
                  className="hidden lg:block text-sm font-semibold py-3 px-4 rounded-full hover:bg-neutral-100 transition cursor-pointer"
                >
                  Become a host
                </div>
              )}

              <div className="p-2 border-none hover:bg-neutral-100 rounded-full cursor-pointer transition">
                <Globe size={18} />
              </div>

              <div className="px-2 py-1 border-[1px] border-neutral-200 flex flex-row items-center gap-3 rounded-full cursor-pointer hover:shadow-md transition">
                <DropdownMenu>
                  <DropdownMenuTrigger asChild>
                    <div className="flex items-center gap-3">
                      <Menu size={18} className="text-neutral-600" />
                      <Avatar className="h-8 w-8">
                        {/* عرض صورة المستخدم إذا وجِدَت، وإلا عرض الـ Fallback */}
                        <AvatarImage
                          src={currentUser?.imageSrc || ""}
                          alt="Profile"
                          className="h-full w-full object-cover"
                        />
                        <AvatarFallback className="bg-neutral-500 text-white text-xs">
                          {currentUser ? (
                            currentUser.fullName[0].toUpperCase()
                          ) : (
                            <User size={18} />
                          )}
                        </AvatarFallback>
                      </Avatar>
                    </div>
                  </DropdownMenuTrigger>

                  <DropdownMenuContent
                    align="end"
                    className="w-56 rounded-xl mt-2 p-2 shadow-xl border-none bg-white"
                  >
                    {currentUser ? (
                      <UserMenu onLogout={onLogout} />
                    ) : (
                      /* خيارات الزائر غير المسجل */
                      <>
                        <DropdownMenuItem
                          onClick={registerModal.onOpen}
                          className="font-semibold cursor-pointer py-3"
                        >
                          Sign up
                        </DropdownMenuItem>
                        <DropdownMenuItem
                          onClick={loginModal.onOpen}
                          className="py-3 cursor-pointer"
                        >
                          Log in
                        </DropdownMenuItem>
                        <DropdownMenuSeparator />
                        <DropdownMenuItem className="py-3 cursor-pointer">
                          Help Center
                        </DropdownMenuItem>
                      </>
                    )}
                  </DropdownMenuContent>
                </DropdownMenu>
              </div>
            </div>
          </div>
        </div>
      </nav>

      <Categories />
    </div>
  );
};

export default Navbar;

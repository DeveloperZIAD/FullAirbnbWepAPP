import Navbar from "./Navbar";
import Categories from "./Categories";
import Footer from "./Footer";

const MainLayout = ({ children }) => {
  return (
    <div className="relative flex flex-col min-h-screen">
      {/* القسم العلوي الثابت: يحتوي على الناف بار والتصنيفات */}
      <div className="fixed w-full bg-white z-20 shadow-sm">
        <Navbar />
        <div className="max-w-[2520px] mx-auto xl:px-20 md:px-10 sm:px-2 px-4">
          <Categories />
        </div>
      </div>

      {/* المحتوى الرئيسي: 
          تم تقليل pt إلى 40 للموبايل و 44 للشاشات الكبيرة 
          لأن الـ Navbar + Categories معاً يحتاجان مسافة متزنة
      */}
      <main className="flex-grow pt-36 md:pt-40">
        <div className="max-w-[2520px] mx-auto xl:px-20 md:px-10 sm:px-2 px-4 pb-20">
          {children}
        </div>
      </main>

      <Footer />
    </div>
  );
};

export default MainLayout;

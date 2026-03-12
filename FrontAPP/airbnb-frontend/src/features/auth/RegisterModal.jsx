import React, { useState, useCallback } from "react";
import { toast } from "react-hot-toast";
import Modal from "../../components/modals/Modal";
import authService from "../../services/authService";
import ImageUpload from "./../../components/inputs/ImageUpload.jsx"; // استيراد مكون الرفعimport name from './../../components/inputs/ImageUpload.jsx';

const RegisterModal = ({ isOpen, onClose, onSwitchToLogin }) => {
  const [isLoading, setIsLoading] = useState(false);
  const [formData, setFormData] = useState({
    fullName: "",
    email: "",
    password: "",
    phoneNumber: "",
    imageSrc: [],
  });

  const setCustomValue = (id, value) => {
    setFormData((prev) => ({ ...prev, [id]: value }));
  };

  const onSubmit = useCallback(async () => {
    setIsLoading(true);
    try {
      const imageUrl = Array.isArray(formData.imageSrc)
        ? formData.imageSrc[0]
        : formData.imageSrc;

      const dataToSubmit = {
        ...formData,
        imageSrc: imageUrl || "", // هنا نرسل النص الصافي (https://res.cloudinary.com/...)
      };
      // 1. تسجيل المستخدم في الباك اند
      const response = await authService.register(dataToSubmit);

      // 4. إطلاق حدث مخصص لتنبيه الـ Navbar (للتحديث بدون reload)
      window.dispatchEvent(new Event("storage"));

      toast.success("Success! Welcome to Airbnb.");
      onClose();

      // اختياري: إذا أردت التأكد بنسبة 100%، اترك الـ reload مؤقتاً
      // window.location.reload();
    } catch (error) {
      toast.error(error.response?.data?.message || "Registration failed");
    } finally {
      setIsLoading(false);
    }
  }, [formData, onClose]);

  const bodyContent = (
    <div className="flex flex-col gap-3 max-h-[45vh] overflow-y-auto px-2 custom-scrollbar">
      {" "}
      <div className="text-start">
        <h2 className="text-2xl font-bold">Welcome to Airbnb</h2>
        <p className="text-neutral-500 mt-1">Create an account!</p>
      </div>
      {/* قسم رفع صورة البروفايل */}
      <div className="flex flex-col gap-2">
        <label className="text-sm font-semibold">Profile Picture</label>
        <ImageUpload
          value={formData.imageSrc} // القيمة الحالية من الـ state
          onChange={(value) => setCustomValue("imageSrc", value)} // تحديث الـ state بالرابط الجديد
        />
      </div>
      <input
        disabled={isLoading}
        placeholder="Full Name"
        className="w-full p-4 border-2 border-neutral-200 rounded-xl outline-none focus:border-black transition disabled:opacity-70"
        onChange={(e) => setCustomValue("fullName", e.target.value)}
      />
      {/* إضافة رقم الهاتف */}
      <input
        disabled={isLoading}
        placeholder="Phone Number"
        type="tel"
        className="w-full p-4 border-2 border-neutral-200 rounded-xl outline-none focus:border-black transition disabled:opacity-70"
        onChange={(e) => setCustomValue("phoneNumber", e.target.value)}
      />
      <input
        disabled={isLoading}
        placeholder="Email"
        type="email"
        className="w-full p-4 border-2 border-neutral-200 rounded-xl outline-none focus:border-black transition disabled:opacity-70"
        onChange={(e) => setCustomValue("email", e.target.value)}
      />
      <input
        disabled={isLoading}
        placeholder="Password"
        type="password"
        className="w-full p-4 border-2 border-neutral-200 rounded-xl outline-none focus:border-black transition disabled:opacity-70"
        onChange={(e) => setCustomValue("password", e.target.value)}
      />
    </div>
  );

  const footerContent = (
    <div className="flex flex-col gap-4 mt-3">
      <hr />
      <div className="text-neutral-500 text-center mt-4 font-light">
        <p>
          Already have an account?
          <span
            onClick={onSwitchToLogin}
            className="text-neutral-800 cursor-pointer hover:underline font-semibold ml-1"
          >
            Log in
          </span>
        </p>
      </div>
    </div>
  );

  return (
    <Modal
      disabled={isLoading}
      isOpen={isOpen}
      title="Register"
      actionLabel="Continue"
      onClose={onClose}
      onSubmit={onSubmit}
      body={bodyContent}
      footer={footerContent}
    />
  );
};

export default RegisterModal;

import React, { useState, useCallback } from "react";
import { toast } from "react-hot-toast";
import Modal from "../../components/modals/Modal";
import authService from "../../services/authService";

const LoginModal = ({ isOpen, onClose, onSwitchToRegister }) => {
  const [isLoading, setIsLoading] = useState(false);
  const [formData, setFormData] = useState({ email: "", password: "" });

  const onSubmit = useCallback(async () => {
    setIsLoading(true);
    try {
      await authService.login(formData);
      toast.success("Logged in successfully!");
      onClose();
      // هنا بنعمل Reload أو نستخدم Context لتحديث الـ Navbar
      window.location.reload();
    } catch (error) {
      toast.error("Invalid email or password");
    } finally {
      setIsLoading(false);
    }
  }, [formData, onClose]);

  const bodyContent = (
    <div className="flex flex-col gap-4">
      <div className="text-start">
        <h2 className="text-2xl font-bold">Welcome back</h2>
        <p className="text-neutral-500 mt-2">Login to your account!</p>
      </div>
      <input
        disabled={isLoading}
        placeholder="Email"
        type="email"
        className="w-full p-4 border-2 border-neutral-200 rounded-xl outline-none focus:border-black transition disabled:opacity-70"
        onChange={(e) => setFormData({ ...formData, email: e.target.value })}
      />
      <input
        disabled={isLoading}
        placeholder="Password"
        type="password"
        className="w-full p-4 border-2 border-neutral-200 rounded-xl outline-none focus:border-black transition disabled:opacity-70"
        onChange={(e) => setFormData({ ...formData, password: e.target.value })}
      />
    </div>
  );

  const footerContent = (
    <div className="flex flex-col gap-4 mt-3">
      <hr />
      <div className="text-neutral-500 text-center mt-4 font-light">
        <p>
          First time using Airbnb?
          <span
            onClick={onSwitchToRegister}
            className="text-neutral-800 cursor-pointer hover:underline font-semibold ml-1"
          >
            Create an account
          </span>
        </p>
      </div>
    </div>
  );

  return (
    <Modal
      disabled={isLoading}
      isOpen={isOpen}
      title="Login"
      actionLabel="Continue"
      onClose={onClose}
      onSubmit={onSubmit}
      body={bodyContent}
      footer={footerContent}
    />
  );
};

export default LoginModal;

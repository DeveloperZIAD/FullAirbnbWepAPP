import { useState } from "react";
import authService from "./../services/authService.jsx";

import { toast } from "react-hot-toast";

export const useAuth = () => {
  const [isLoading, setIsLoading] = useState(false);

  const register = async (data, callback) => {
    setIsLoading(true);
    try {
      const result = await registerUser(data);
      toast.success("Account created successfully!");
      if (callback) callback(); // مثلاً لإغلاق المودال
      return result;
    } catch (error) {
      toast.error(error.response?.data?.message || "Registration failed");
    } finally {
      setIsLoading(false);
    }
  };

  return { register, isLoading };
};

import React from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { CheckCircle } from "lucide-react"; // تأكد من تثبيت lucide-react

const PaymentSuccess = () => {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const transactionId = searchParams.get("transactionId");

  return (
    <div className="flex flex-col items-center justify-center min-h-[80vh] px-4 text-center">
      <div className="bg-green-50 p-10 rounded-3xl shadow-sm border border-green-100 max-w-md w-full">
        <CheckCircle className="w-20 h-20 text-green-500 mx-auto mb-6 animate-bounce" />

        <h1 className="text-3xl font-bold text-neutral-800 mb-2">
          Payment Successful!
        </h1>
        <p className="text-neutral-500 mb-6">
          Thank you for your booking. Your reservation is now confirmed.
        </p>

        {transactionId && (
          <div className="bg-white p-3 rounded-lg border border-neutral-200 mb-8 text-sm">
            <span className="text-neutral-400 block mb-1 uppercase tracking-widest text-[10px] font-bold">
              Transaction ID
            </span>
            <code className="text-rose-500 font-mono">{transactionId}</code>
          </div>
        )}

        <div className="space-y-3">
          <button
            onClick={() => navigate("/trips")}
            className="w-full bg-rose-500 text-white py-3 rounded-xl font-semibold hover:bg-rose-600 transition shadow-md"
          >
            View My Trips
          </button>

          <button
            onClick={() => navigate("/")}
            className="w-full bg-white text-neutral-700 py-3 rounded-xl font-semibold border border-neutral-300 hover:bg-neutral-50 transition"
          >
            Back to Home
          </button>
        </div>
      </div>
    </div>
  );
};

export default PaymentSuccess;

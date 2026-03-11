import React, { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { loadStripe } from "@stripe/stripe-js";
import {
  Elements,
  CardElement,
  useStripe,
  useElements,
} from "@stripe/react-stripe-js";
import { toast } from "react-hot-toast";
import bookingService from "../services/bookingService";
import paymentService from "../services/paymentService";

// استبدل هذا بمفتاحك العام من Stripe Dashboard
const stripePromise = loadStripe(
  "pk_test_51SjOoeFOLcvqen74yR6znSfsqu8xJjUBZUC3LOHt1LBrVGwJchWgyIitOpCGrkEDRIqpB4u2prUdanAUHeVebqlg00J2OOfV7J",
);

// مكوّن فورم الدفع الداخلي
const CheckoutForm = ({ bookingDetails }) => {
  const stripe = useStripe();
  const elements = useElements();
  const navigate = useNavigate();
  const [isProcessing, setIsProcessing] = useState(false);

  const handleSubmit = async (event) => {
    event.preventDefault();

    if (!stripe || !elements) return;

    setIsProcessing(true);

    // 1. إنشاء Payment Method من خلال Stripe
    const cardElement = elements.getElement(CardElement);
    const { error, paymentMethod } = await stripe.createPaymentMethod({
      type: "card",
      card: cardElement,
    });

    if (error) {
      toast.error(error.message);
      setIsProcessing(false);
      return;
    }

    try {
      const response = await paymentService.processPayment(
        bookingDetails.id,
        paymentMethod.id,
      );

      if (response.success) {
        // التوجيه لصفحة النجاح مع تمرير رقم العملية
        navigate(`/payment/success?transactionId=${response.transactionId}`);
      }
    } catch (err) {
      toast.error("Payment failed at server");
    } finally {
      setIsProcessing(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-6">
      <div className="p-4 border rounded-xl bg-white shadow-sm border-neutral-300">
        <CardElement
          options={{
            style: {
              base: {
                fontSize: "16px",
                color: "#424770",
                "::placeholder": { color: "#aab7c4" },
              },
              invalid: { color: "#9e2146" },
            },
          }}
        />
      </div>
      <button
        type="submit"
        disabled={!stripe || isProcessing}
        className={`w-full bg-rose-500 text-white py-3 rounded-lg font-bold transition
          ${isProcessing ? "opacity-50 cursor-not-allowed" : "hover:bg-rose-600 shadow-md"}
        `}
      >
        {isProcessing ? "Processing..." : `Pay $${bookingDetails.totalPrice}`}
      </button>
    </form>
  );
};

// الصفحة الرئيسية التي تغلف الفورم بـ Elements
const PaymentPage = () => {
  const { bookingId } = useParams();
  const [bookingDetails, setBookingDetails] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchBooking = async () => {
      try {
        const data = await bookingService.getBookingById(bookingId);
        setBookingDetails(data);
      } catch (err) {
        toast.error("Booking not found");
      } finally {
        setLoading(false);
      }
    };
    fetchBooking();
  }, [bookingId]);

  if (loading)
    return <div className="p-10 text-center">Loading details...</div>;
  if (!bookingDetails)
    return <div className="p-10 text-center">Booking not found.</div>;

  return (
    <div className="max-w-screen-lg mx-auto p-8 pt-24">
      <h1 className="text-3xl font-bold mb-8">Confirm and Pay</h1>
      <div className="grid grid-cols-1 md:grid-cols-2 gap-12">
        <div className="space-y-6">
          <h2 className="text-xl font-semibold">Payment Method</h2>
          {/* هنا نقوم بتغليف الفورم بمزود Stripe */}
          <Elements stripe={stripePromise}>
            <CheckoutForm bookingDetails={bookingDetails} />
          </Elements>
        </div>

        {/* ملخص الحجز (الجزء اليمين) */}
        <div className="border p-6 rounded-2xl shadow-sm bg-white h-fit sticky top-24">
          <div className="flex gap-4 mb-4">
            <img
              src={bookingDetails.listingImage}
              className="w-20 h-20 object-cover rounded-lg"
              alt=""
            />
            <div>
              <p className="font-bold">{bookingDetails.listingTitle}</p>
              <p className="text-sm text-neutral-500">{bookingDetails.city}</p>
            </div>
          </div>
          <hr className="my-4" />
          <div className="flex justify-between font-bold text-lg">
            <span>Total (USD)</span>
            <span>${bookingDetails.totalPrice}</span>
          </div>
        </div>
      </div>
    </div>
  );
};

export default PaymentPage;

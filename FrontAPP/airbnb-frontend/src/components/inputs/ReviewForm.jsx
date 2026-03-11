import React, { useState } from "react";
import { Star } from "lucide-react";

const ReviewForm = ({ onAdd }) => {
  const [rating, setRating] = useState(5);
  const [comment, setComment] = useState("");

  const handleSubmit = (e) => {
    e.preventDefault();
    if (!comment.trim()) return;
    onAdd({ rating, comment });
    setComment("");
  };

  return (
    <form onSubmit={handleSubmit} className="mt-8 border-t pt-8">
      <h3 className="text-lg font-semibold mb-4">Add a review</h3>
      <div className="flex gap-2 mb-4">
        {[1, 2, 3, 4, 5].map((star) => (
          <Star
            key={star}
            size={24}
            className="cursor-pointer transition"
            fill={star <= rating ? "#fbbf24" : "none"}
            color={star <= rating ? "#fbbf24" : "#d1d5db"}
            onClick={() => setRating(star)}
          />
        ))}
      </div>
      <textarea
        className="w-full border rounded-xl p-4 focus:ring-rose-500 focus:border-rose-500 outline-none"
        rows="3"
        placeholder="How was your stay?"
        value={comment}
        onChange={(e) => setComment(e.target.value)}
      />
      <button
        type="submit"
        className="mt-4 bg-black text-white px-6 py-2 rounded-lg hover:opacity-80 transition"
      >
        Submit Review
      </button>
    </form>
  );
};

export default ReviewForm;

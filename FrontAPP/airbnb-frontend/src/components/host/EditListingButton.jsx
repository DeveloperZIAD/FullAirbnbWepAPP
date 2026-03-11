import { Loader2, Pencil } from "lucide-react";

export default function EditListingButton({ listingId, onClick, isLoading }) {
  return (
    <button
      onClick={() => onClick(listingId)}
      disabled={isLoading}
      className={`group flex items-center gap-2 text-sm font-medium transition-all duration-200 
        ${isLoading ? "text-neutral-400 cursor-not-allowed" : "text-blue-600 hover:text-blue-800"}`}
      aria-label="Edit listing"
    >
      {isLoading ? (
        <>
          <Loader2 size={16} className="animate-spin" />
          Loading...
        </>
      ) : (
        <>
          <Pencil
            size={16}
            className="transition-transform group-hover:scale-110"
          />
          Edit
        </>
      )}
    </button>
  );
}

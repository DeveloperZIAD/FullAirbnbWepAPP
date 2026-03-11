import { useState } from "react";
import listingService from "./../../services/listingService.js";
import ImageUpload from "./../inputs/ImageUpload.jsx";
import { toast } from "react-hot-toast";

export default function ListingForm({ onClose, onSave, initialData = null }) {
  const [formData, setFormData] = useState({
    title: initialData?.title || "",
    description: initialData?.description || "",
    basePrice: initialData?.basePrice || "",
    // location may be nested under initialData.location
    city: initialData?.city || initialData?.location?.city || "",
    countryCode:
      initialData?.countryCode || initialData?.location?.countryCode || "",
    categoryName: initialData?.categoryName || "",
    streetAddress:
      initialData?.streetAddress || initialData?.location?.streetAddress || "",
  });

  const [imageUrls, setImageUrls] = useState(initialData?.imageUrls || []);
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);

    const storedUser = localStorage.getItem("user");
    const hostId = storedUser ? JSON.parse(storedUser).id : null;

    const payload = {
      ...formData,
      basePrice: parseFloat(formData.basePrice),
      imageUrls,
      ...(initialData ? {} : { hostId }),
      // include location object for API compatibility
      location: {
        city: formData.city,
        countryCode: formData.countryCode,
        streetAddress: formData.streetAddress,
      },
    };

    try {
      if (initialData) {
        await listingService.updateListing(initialData.id, payload);
        toast.success("Listing updated successfully!");
      } else {
        await listingService.createListing(payload);
        toast.success("Listing created successfully!");
      }
      onSave();
      onClose();
    } catch (err) {
      toast.error(
        "Operation failed. Please check your inputs." +
          (err.response?.data?.message || ""),
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <form
      onSubmit={handleSubmit}
      className="space-y-4 p-6 bg-white rounded-xl shadow-lg border"
    >
      <h2 className="text-xl font-bold">
        {initialData ? "Edit Property" : "Add New Property"}
      </h2>

      <input
        value={formData.title}
        onChange={(e) => setFormData({ ...formData, title: e.target.value })}
        placeholder="Title"
        className="w-full p-3 border rounded-lg"
      />

      <textarea
        value={formData.description}
        onChange={(e) =>
          setFormData({ ...formData, description: e.target.value })
        }
        placeholder="Description"
        className="w-full p-3 border rounded-lg h-24"
      />

      <div className="grid grid-cols-2 gap-4">
        <input
          value={formData.basePrice}
          onChange={(e) =>
            setFormData({ ...formData, basePrice: e.target.value })
          }
          placeholder="Price"
          type="number"
          className="p-3 border rounded-lg"
        />
        <input
          value={formData.categoryName}
          onChange={(e) =>
            setFormData({ ...formData, categoryName: e.target.value })
          }
          placeholder="Category"
          className="p-3 border rounded-lg"
        />
      </div>

      <div className="grid grid-cols-2 gap-4">
        <input
          value={formData.city}
          onChange={(e) => setFormData({ ...formData, city: e.target.value })}
          placeholder="City"
          className="p-3 border rounded-lg"
        />
        <input
          value={formData.countryCode}
          onChange={(e) =>
            setFormData({ ...formData, countryCode: e.target.value })
          }
          placeholder="Country Code (e.g. EG)"
          className="p-3 border rounded-lg"
        />
      </div>

      <input
        value={formData.streetAddress}
        onChange={(e) =>
          setFormData({ ...formData, streetAddress: e.target.value })
        }
        placeholder="Street Address"
        className="w-full p-3 border rounded-lg"
      />

      <div className="mt-4">
        <label className="block text-sm font-medium mb-2 text-neutral-600">
          Property Images
        </label>
        <ImageUpload value={imageUrls} onChange={setImageUrls} />
      </div>

      <button
        type="submit"
        disabled={loading || imageUrls.length === 0}
        className="w-full bg-rose-500 text-white p-3 rounded-xl font-bold hover:bg-rose-600 disabled:bg-neutral-300"
      >
        {loading
          ? initialData
            ? "Saving..."
            : "Creating..."
          : initialData
            ? "Update Listing"
            : "Save Listing"}
      </button>
    </form>
  );
}

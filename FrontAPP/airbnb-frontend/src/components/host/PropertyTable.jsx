import React from "react";
import EditListingButton from "./EditListingButton";
import DeleteListingButton from "./DeleteListingButton";

const PropertyTable = ({ listings, onEdit, onDelete, loadingId }) => {
  if (!listings || listings.length === 0) {
    return (
      <div className="bg-white p-10 text-center border rounded-xl">
        <p className="text-neutral-500 italic">
          No listings found. Start by creating one!
        </p>
      </div>
    );
  }

  return (
    <div className="bg-white border rounded-xl overflow-x-auto shadow-sm">
      <table className="w-full text-left">
        <thead className="bg-neutral-50 border-b">
          <tr>
            <th className="p-4 font-semibold text-neutral-600">Property</th>
            <th className="p-4 font-semibold text-neutral-600">Price</th>
            <th className="p-4 font-semibold text-neutral-600">Location</th>
            <th className="p-4 font-semibold text-neutral-600">Actions</th>
          </tr>
        </thead>
        <tbody>
          {listings.map((listing) => (
            <tr
              key={listing.id}
              className="border-b hover:bg-neutral-50 transition"
            >
              <td className="p-4 flex items-center gap-3">
                <img
                  src={
                    listing.imageUrls?.[0] || "https://via.placeholder.com/150"
                  }
                  alt={listing.title}
                  className="w-12 h-12 rounded-lg object-cover"
                />
                <span className="font-medium text-neutral-800">
                  {listing.title}
                </span>
              </td>
              <td className="p-4 font-semibold text-rose-500">
                ${listing.basePrice}
              </td>
              <td className="p-4 text-neutral-500">{listing.city}</td>
              <td className="p-4">
                <div className="flex gap-3">
                  <EditListingButton
                    listingId={listing.id}
                    onClick={onEdit}
                    isLoading={loadingId === listing.id} // ربط حالة التحميل بهذا السطر تحديداً
                  />
                  <DeleteListingButton
                    listingId={listing.id}
                    onDelete={onDelete}
                  />
                </div>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default PropertyTable;

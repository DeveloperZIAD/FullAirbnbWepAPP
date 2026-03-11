import React, { useEffect, useState } from "react";
import adminService from "./../../services/adminService.js";
const AdminListings = () => {
  const [listings, setListings] = useState([]);

  useEffect(() => {
    adminService.getAllListings().then(setListings);
  }, []);

  return (
    <div className="p-6">
      <h1 className="text-2xl font-bold mb-4">Listings Management</h1>
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        {listings.map((item) => (
          <div
            key={item.id}
            className="bg-white p-4 rounded-lg shadow-sm border"
          >
            <h3 className="font-bold text-lg">{item.title}</h3>
            <p className="text-gray-500 text-sm">
              {item.categoryName} - ${item.price}
            </p>

            <div className="mt-3 pt-3 border-t">
              <p className="text-xs font-semibold text-gray-400 uppercase">
                Host
              </p>
              <p className="text-sm">
                {item.hostDetails.fullName} ({item.hostDetails.email})
              </p>
            </div>

            <div className="mt-2 pt-2 border-t text-gray-600">
              <p className="text-xs font-semibold text-gray-400 uppercase">
                Location
              </p>
              <p className="text-sm">
                {item.location?.city}, {item.location?.countryCode}
              </p>
              <p className="text-xs italic">{item.location?.streetAddress}</p>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};
export default AdminListings;

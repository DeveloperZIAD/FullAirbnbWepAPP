// src/app/host/components/StatCard.jsx
import React from "react";

/**
 * @param {{ label: string, value: string | number, icon: React.ElementType }} props
 */
const StatCard = ({ label, value, icon: Icon }) => {
  return (
    <div className="p-6 bg-white border border-neutral-200 rounded-2xl flex items-center gap-5 shadow-sm hover:shadow-md transition-shadow">
      <div className="p-4 bg-rose-50 text-rose-600 rounded-xl">
        <Icon size={28} />
      </div>
      <div>
        <p className="text-neutral-500 text-sm font-medium uppercase tracking-wider">
          {label}
        </p>
        <h3 className="text-3xl font-bold text-neutral-800">{value}</h3>
      </div>
    </div>
  );
};

export default StatCard;

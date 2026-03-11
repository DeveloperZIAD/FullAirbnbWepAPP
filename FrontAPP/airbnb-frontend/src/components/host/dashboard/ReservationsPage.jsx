export default function ReservationsPage() {
  return (
    <div className="space-y-6 animate-in fade-in duration-500">
      {/* Header */}
      <header>
        <h1 className="text-3xl font-extrabold text-neutral-800">
          Reservations
        </h1>
        <p className="text-neutral-500 mt-1">
          Manage your upcoming and past bookings.
        </p>
      </header>

      {/* Content */}
      <div className="bg-white p-8 rounded-xl border shadow-sm">
        <p className="text-neutral-600">
          List of all your reservations will appear here.
          {/* هنا يمكنك إضافة جدول ReservationsTable لاحقاً */}
        </p>
      </div>
    </div>
  );
}

import ListingCard from "@/components/shared/ListingCard";
import EmptyState from "@/components/shared/EmptyState";

const PropertiesPage = () => {
  // بيانات تجريبية لعقارات رفعها المستخدم
  const properties = [
    {
      id: "1",
      title: "Beach House",
      location: "Alexandria",
      country: "Egypt",
      category: "Beach",
      price: 150,
      image:
        "https://images.unsplash.com/photo-1507525428034-b723cf961d3e?auto=format&fit=crop&w=800",
    },
  ];

  if (properties.length === 0) {
    return (
      <EmptyState
        title="No properties found"
        subtitle="Looks like you have no properties."
      />
    );
  }

  return (
    <div className="max-w-[2520px] mx-auto xl:px-20 md:px-10 sm:px-2 px-4 pt-28">
      <div className="text-start">
        <h1 className="text-2xl font-bold">Properties</h1>
        <p className="font-light text-neutral-500 mt-2">
          List of your properties
        </p>
      </div>

      <div className="mt-10 grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 2xl:grid-cols-6 gap-8 pb-10">
        {properties.map((property) => (
          <ListingCard
            key={property.id}
            data={property}
            actionId={property.id}
            onAction={(id) => console.log("Delete Property:", id)}
            actionLabel="Delete property"
          />
        ))}
      </div>
    </div>
  );
};

export default PropertiesPage;

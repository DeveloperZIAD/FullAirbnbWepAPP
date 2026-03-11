const Loader = () => {
  return (
    <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-8 p-10">
      {[...Array(8)].map((_, i) => (
        <div key={i} className="flex flex-col gap-3 w-full">
          {/* الصورة */}
          <div className="aspect-square w-full relative overflow-hidden rounded-xl bg-neutral-200 animate-pulse" />
          {/* العنوان */}
          <div className="h-4 w-3/4 bg-neutral-200 rounded animate-pulse" />
          {/* الوصف */}
          <div className="h-3 w-1/2 bg-neutral-200 rounded animate-pulse" />
          {/* السعر */}
          <div className="h-4 w-1/4 bg-neutral-200 rounded animate-pulse" />
        </div>
      ))}
    </div>
  );
};

export default Loader;

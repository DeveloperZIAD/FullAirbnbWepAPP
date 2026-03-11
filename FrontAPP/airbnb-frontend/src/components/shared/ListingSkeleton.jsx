const ListingSkeleton = () => {
  return (
    <div className="col-span-1">
      <div className="flex flex-col gap-2 w-full animate-pulse">
        <div className="aspect-square w-full relative overflow-hidden rounded-xl bg-neutral-200"></div>
        <div className="h-4 bg-neutral-200 rounded w-3/4"></div>
        <div className="h-4 bg-neutral-200 rounded w-1/2"></div>
        <div className="h-4 bg-neutral-200 rounded w-1/4"></div>
      </div>
    </div>
  );
};

export default ListingSkeleton;

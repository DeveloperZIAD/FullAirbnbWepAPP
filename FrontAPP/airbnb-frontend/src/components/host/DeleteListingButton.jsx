import toast from "react-hot-toast";

export default function DeleteListingButton({ listingId, onDelete }) {
  const handleDelete = async () => {
    // 1. إظهار الإشعار مع زر التراجع
    toast((t) => (
      <div className="flex items-center gap-3">
        <p>Are you sure you want to delete this listing?</p>
        <button
          onClick={() => {
            toast.dismiss(t.id); // إغلاق الإشعار عند التراجع
          }}
          className="text-neutral-500 hover:text-neutral-800 font-medium"
        >
          Undo
        </button>
        <button
          onClick={async () => {
            toast.dismiss(t.id);
            await performDelete(); // تنفيذ الحذف الفعلي
          }}
          className="text-red-600 font-bold hover:text-red-700"
        >
          Confirm
        </button>
      </div>
    ), { duration: 5000 });
  };

  const performDelete = async () => {
    try {
      const loadingToast = toast.loading("Deleting...");
      // تأكد أن onDelete هي التي تقوم بالاتصال بالـ API
      await onDelete(listingId); 
      toast.dismiss(loadingToast);
      toast.success("Listing deleted successfully");
    } catch (error) {
      toast.dismiss(); // إغلاق أي toast مفتوح
      toast.error("Failed to delete listing");
      console.error("Delete Error:", error); // <-- هام جداً لتعرف لماذا يفشل الطلب
    }
};

  return (
    <button
      onClick={handleDelete}
      className="text-sm font-medium text-red-600 hover:text-red-800 transition"
    >
      Delete
    </button>
  );
}
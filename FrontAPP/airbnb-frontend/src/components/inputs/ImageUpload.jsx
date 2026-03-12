import { useCallback } from "react";
import { TbPhotoPlus, TbX } from "react-icons/tb";

const ImageUpload = ({ onChange, value = [] }) => {
  // التأكد دائماً أن القيمة مصفوفة لتجنب الانهيار (TypeError)
  const safeValue = Array.isArray(value) ? value : value ? [value] : [];

  const handleUpload = useCallback(() => {
    if (!window.cloudinary) {
      console.error("Cloudinary script not loaded");
      return;
    }

    const widget = window.cloudinary.createUploadWidget(
      {
        cloudName: "duwod2q7m",
        uploadPreset: "ml_default",
        multiple: true,
        maxFiles: 5,
      },
      (error, result) => {
        if (!error && result && result.event === "success") {
          const newImageUrl = result.info.secure_url;

          // تعديل هام: نرسل المصفوفة الجديدة كاملة للأب
          // لأن setCustomValue في الأب لا تقبل (prev => ...)
          onChange([...safeValue, newImageUrl]);
        }
      },
    );

    widget.open();
  }, [onChange, safeValue]); // أضفنا safeValue هنا لضمان تحديث الصور

  const handleRemove = (urlToRemove) => {
    onChange(safeValue.filter((url) => url !== urlToRemove));
  };

  return (
    <div className="space-y-4">
      <div
        onClick={handleUpload}
        className="relative cursor-pointer hover:opacity-70 transition border-dashed border-2 p-10 border-neutral-300 flex flex-col justify-center items-center gap-4 text-neutral-600 rounded-xl"
      >
        <TbPhotoPlus size={50} />
        <div className="font-semibold text-lg">Click to upload images</div>
      </div>

      {safeValue.length > 0 && (
        <div className="grid grid-cols-3 gap-4 mt-4">
          {safeValue.map((url) => (
            <div key={url} className="relative aspect-square w-full">
              <img
                alt="Uploaded"
                src={url}
                className="w-full h-full rounded-lg object-cover"
              />
              <button
                type="button"
                onClick={(e) => {
                  e.stopPropagation(); // منع فتح نافذة الرفع عند الحذف
                  handleRemove(url);
                }}
                className="absolute top-1 right-1 bg-rose-500 text-white p-1 rounded-full hover:bg-rose-600"
              >
                <TbX size={16} />
              </button>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default ImageUpload;

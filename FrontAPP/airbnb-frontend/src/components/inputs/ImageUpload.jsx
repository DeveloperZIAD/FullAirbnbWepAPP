import { useCallback, useState, useEffect } from "react";
import { TbPhotoPlus, TbX } from "react-icons/tb";

const ImageUpload = ({ onChange, value = [] }) => {
  // حالة داخلية لإدارة الصور محلياً
  const [internalValue, setInternalValue] = useState(value);

  // تحديث الحالة الداخلية إذا تغيرت الـ value من الأب
  useEffect(() => {
    setInternalValue(Array.isArray(value) ? value : []);
  }, [value]);

  const handleUpload = useCallback(() => {
    if (!window.cloudinary) return;

    const widget = window.cloudinary.createUploadWidget(
      {
        cloudName: "duwod2q7m",
        uploadPreset: "ml_default",
        multiple: true,
        maxFiles: 5,
      },
      (error, result) => {
        if (!error && result && result.event === "success") {
          const newUrl = result.info.secure_url;

          setInternalValue((prev) => {
            if (prev.includes(newUrl)) return prev;
            const updated = [...prev, newUrl];
            onChange(updated); // إرسال المصفوفة كاملة للأب
            return updated;
          });
        }
      },
    );

    widget.open();
  }, [onChange]);

  const handleRemove = (urlToRemove) => {
    const updated = internalValue.filter((url) => url !== urlToRemove);
    setInternalValue(updated);
    onChange(updated);
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

      {/* هنا تم استبدال safeValue بـ internalValue */}
      {internalValue.length > 0 && (
        <div className="grid grid-cols-3 gap-4 mt-4">
          {internalValue.map((url) => (
            <div key={url} className="relative aspect-square w-full">
              <img
                alt="Uploaded"
                src={url}
                className="w-full h-full rounded-lg object-cover"
              />
              <button
                type="button"
                onClick={(e) => {
                  e.stopPropagation();
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

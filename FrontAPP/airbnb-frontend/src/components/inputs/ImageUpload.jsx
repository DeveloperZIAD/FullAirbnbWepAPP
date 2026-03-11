import { useCallback } from "react";
import { TbPhotoPlus, TbX } from "react-icons/tb";

const ImageUpload = ({ onChange, value = [] }) => {
  // دالة التعامل مع الرفع
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
          // الحل هنا: استخدم الـ functional update بدلاً من الاعتماد على value الخارجية
          onChange((currentImages) => [
            ...currentImages,
            result.info.secure_url,
          ]);
        }
      },
    );

    widget.open();
    // قم بإزالة [onChange, value] من مصفوفة الاعتمادات وأبقِ onChange فقط
  }, [onChange]);

  // دالة لحذف صورة من المصفوفة
  const handleRemove = (urlToRemove) => {
    onChange(value.filter((url) => url !== urlToRemove));
  };

  return (
    <div className="space-y-4">
      {/* منطقة الرفع */}
      <div
        onClick={handleUpload}
        className="relative cursor-pointer hover:opacity-70 transition border-dashed border-2 p-10 border-neutral-300 flex flex-col justify-center items-center gap-4 text-neutral-600 rounded-xl"
      >
        <TbPhotoPlus size={50} />
        <div className="font-semibold text-lg">Click to upload images</div>
      </div>

      {/* عرض الصور المرفوعة كمعرض */}
      {value.length > 0 && (
        <div className="grid grid-cols-3 gap-4 mt-4">
          {value.map((url) => (
            <div key={url} className="relative aspect-square w-full">
              <img
                alt="Uploaded"
                src={url}
                className="w-full h-full rounded-lg object-cover"
              />
              <button
                type="button"
                onClick={() => handleRemove(url)}
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

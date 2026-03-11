import { TbPhotoPlus } from "react-icons/tb";
import { useCallback } from "react";

// ملاحظة: لاحقاً سنربط هذا بـ Cloudinary Widget
const ImageUpload = ({ onChange, value }) => {
  const handleUpload = useCallback(() => {
    // محاكاة لرفع صورة (لحد ما نربط الـ Cloudinary بجد)
    const mockImageUrl =
      "https://images.unsplash.com/photo-1564013799919-ab600027ffc6?auto=format&fit=crop&w=800";
    onChange(mockImageUrl);
  }, [onChange]);

  return (
    <div
      onClick={handleUpload}
      className="relative cursor-pointer hover:opacity-70 transition border-dashed border-2 p-20 border-neutral-300 flex flex-col justify-center items-center gap-4 text-neutral-600"
    >
      <TbPhotoPlus size={50} />
      <div className="font-semibold text-lg">Click to upload</div>
      {value && (
        <div className="absolute inset-0 w-full h-full">
          <img
            alt="Upload"
            style={{ objectFit: "cover" }}
            src={value}
            className="w-full h-full rounded-lg"
          />
        </div>
      )}
    </div>
  );
};

export default ImageUpload;

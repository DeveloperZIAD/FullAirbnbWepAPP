import { X } from "lucide-react";
import { useCallback, useEffect, useState } from "react";

const Modal = ({
  isOpen,
  onClose,
  onSubmit,
  title,
  body,
  footer,
  actionLabel,
  disabled,
  secondaryAction, // الزر الثاني (Back)
  secondaryActionLabel, // نص الزر الثاني
}) => {
  const [showModal, setShowModal] = useState(isOpen);

  useEffect(() => {
    setShowModal(isOpen);
  }, [isOpen]);

  const handleClose = useCallback(() => {
    if (disabled) return;
    setShowModal(false);
    setTimeout(() => {
      onClose();
    }, 300);
  }, [disabled, onClose]);

  const handleSubmit = useCallback(() => {
    if (disabled) return;
    onSubmit();
  }, [disabled, onSubmit]);

  const handleSecondaryAction = useCallback(() => {
    if (disabled || !secondaryAction) return;
    secondaryAction();
  }, [disabled, secondaryAction]);

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center overflow-x-hidden overflow-y-auto bg-neutral-800/70 outline-none focus:outline-none">
      <div className="relative w-full md:w-4/6 lg:w-3/6 xl:w-2/5 my-6 mx-auto h-full lg:h-auto md:h-auto">
        {/* Content */}
        <div
          className={`translate duration-300 h-full ${
            showModal
              ? "translate-y-0 opacity-100"
              : "translate-y-full opacity-0"
          }`}
        >
          <div className="bg-white h-full lg:h-auto border-0 rounded-lg shadow-lg relative flex flex-col w-full outline-none focus:outline-none">
            {/* Header */}
            <div className="flex items-center p-6 rounded-t justify-center relative border-b-[1px]">
              <button
                onClick={handleClose}
                className="p-1 border-0 hover:opacity-70 transition absolute left-9"
              >
                <X size={18} />
              </button>
              <div className="text-lg font-semibold">{title}</div>
            </div>

            {/* Body */}
            <div className="relative p-6 flex-auto">{body}</div>

            {/* Footer */}
            <div className="flex flex-col gap-2 p-6">
              <div className="flex flex-row items-center gap-4 w-full">
                {/* زر العودة يظهر فقط إذا تم تمريره */}
                {secondaryAction && secondaryActionLabel && (
                  <button
                    disabled={disabled}
                    onClick={handleSecondaryAction}
                    className="relative disabled:opacity-70 disabled:cursor-not-allowed rounded-lg hover:opacity-80 transition w-full bg-white border-black border-[1px] text-black py-3 font-semibold text-md"
                  >
                    {secondaryActionLabel}
                  </button>
                )}

                {/* زر الاستمرار الأساسي */}
                <button
                  disabled={disabled}
                  onClick={handleSubmit}
                  className="relative disabled:opacity-70 disabled:cursor-not-allowed rounded-lg hover:opacity-80 transition w-full bg-rose-500 border-rose-500 text-white py-3 font-semibold text-md"
                >
                  {actionLabel}
                </button>
              </div>
              {footer}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Modal;

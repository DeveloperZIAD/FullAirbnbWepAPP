import React, { useEffect, useState, useCallback } from "react";
import { IoMdClose } from "react-icons/io";

const Modal = ({
  isOpen,
  onClose,
  onSubmit,
  title,
  body,
  footer,
  actionLabel,
  disabled,
  secondaryAction,
  secondaryActionLabel,
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
    <div className="fixed inset-0 z-50 flex items-center justify-center overflow-x-hidden overflow-y-auto outline-none focus:outline-none bg-neutral-800/70 backdrop-blur-sm">
      <div className="relative w-full md:w-4/6 lg:w-3/6 xl:w-2/5 my-6 mx-auto h-full lg:h-auto md:h-auto">
        {/* Content */}
        <div
          className={`translate duration-300 h-full ${showModal ? "translate-y-0 opacity-100" : "translate-y-full opacity-0"}`}
        >
          <div className="translate h-full lg:h-auto md:h-auto border-0 rounded-2xl shadow-lg relative flex flex-col w-full bg-white outline-none focus:outline-none">
            {/* Header */}
            <div className="flex items-center p-6 rounded-t justify-center relative border-b-[1px]">
              <button
                onClick={handleClose}
                className="p-1 border-0 hover:opacity-70 transition absolute left-9"
              >
                <IoMdClose size={18} />
              </button>
              <div className="text-lg font-bold">{title}</div>
            </div>

            {/* Body */}
            <div className="relative p-6 flex-auto">{body}</div>

            {/* Footer (Buttons) */}
            <div className="flex flex-col gap-2 p-6">
              <div className="flex flex-row items-center gap-4 w-full">
                {secondaryAction && secondaryActionLabel && (
                  <button
                    disabled={disabled}
                    onClick={handleSecondaryAction}
                    className="w-full py-3 border-[1px] border-black rounded-lg font-semibold hover:bg-neutral-100 transition disabled:opacity-70"
                  >
                    {secondaryActionLabel}
                  </button>
                )}
                <button
                  disabled={disabled}
                  onClick={handleSubmit}
                  className="w-full py-3 bg-rose-500 text-white rounded-lg font-bold hover:bg-rose-600 transition disabled:opacity-70"
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

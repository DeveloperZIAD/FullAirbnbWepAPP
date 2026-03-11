import React, { useState, useMemo } from "react";
import Modal from "./Modal";
import CategoryInput from "../inputs/CategoryInput";
import { categories } from "../layout/Categories";
import Select from "react-select";
import countries from "world-countries";
import ImageUpload from "../inputs/ImageUpload";
import hostService from "../../services/hostService";
import listingService from "../../services/listingService";
import { toast } from "react-hot-toast";
// مكون فرعي للعداد (Counter) - يفضل وضعه في ملف مستقل لاحقاً
const Counter = ({ title, subtitle, value, onChange }) => {
  const onAdd = () => onChange(value + 1);
  const onReduce = () => {
    if (value === 1) return;
    onChange(value - 1);
  };

  return (
    <div className="flex flex-row items-center justify-between">
      <div className="flex flex-col">
        <div className="font-medium">{title}</div>
        <div className="font-light text-gray-500">{subtitle}</div>
      </div>
      <div className="flex flex-row items-center gap-4">
        <div
          onClick={onReduce}
          className="w-10 h-10 rounded-full border-[1px] border-neutral-400 flex items-center justify-center text-neutral-600 cursor-pointer hover:opacity-80 transition"
        >
          -
        </div>
        <div className="font-light text-xl text-neutral-600">{value}</div>
        <div
          onClick={onAdd}
          className="w-10 h-10 rounded-full border-[1px] border-neutral-400 flex items-center justify-center text-neutral-600 cursor-pointer hover:opacity-80 transition"
        >
          +
        </div>
      </div>
    </div>
  );
};

const STEPS = {
  CATEGORY: 0,
  LOCATION: 1,
  INFO: 2,
  IMAGES: 3,
  DESCRIPTION: 4,
  PRICE: 5,
};

const RentModal = ({ isOpen, onClose }) => {
  const [step, setStep] = useState(STEPS.CATEGORY);
  const [formData, setFormData] = useState({
    category: "",
    location: null,
    guestCount: 1,
    roomCount: 1,
    bathroomCount: 1,
    imageSrc: "",
    price: 1,
    title: "",
    description: "",
  });
  const [isLoading, setIsLoading] = useState(false);

  const onSubmit = async () => {
    // إذا لم نصل للخطوة الأخيرة، انتقل للخطوة التالية
    if (step !== STEPS.PRICE) {
      onNext();
      return;
    }

    // إذا وصلنا لخطوة السعر، ابدأ عملية الإنشاء والترقية
    setIsLoading(true);
    try {
      await makeHostAndListNewListing();

      toast.success("Listing created successfully!");
      // إذا نجحت العملية، أغلق المودال
      onClose();
      // يمكنك إضافة toast.success هنا
    } catch (error) {
      // التعامل مع الخطأ
      toast.error(
        "Something went wrong. Please try again." +
          (error.response?.data?.message || ""),
      );
      console.error(error);
    } finally {
      setIsLoading(false);
    }
  };
  const makeHostAndListNewListing = async () => {
    try {
      const currentUser = JSON.parse(localStorage.getItem("user"));
      // 1. ترقية المستخدم
      const upgradeResult = await hostService.upgradeToHost(currentUser.id);

      // 2. تجهيز البيانات حسب الـ DTO الخاص بك (بدون تغليف في listingDto)
      const listingPayload = {
        title: formData.title,
        description: formData.description,
        basePrice: parseFloat(formData.price),
        hostId: currentUser.id, // الـ UserId
        categoryName: formData.category, // هذا هو الاسم (مثل "Beach")
        guestCount: parseInt(formData.guestCount),
        roomCount: parseInt(formData.roomCount),
        bathroomCount: parseInt(formData.bathroomCount),
        imageUrls: [formData.imageSrc],
        city: formData.location?.label,
        countryCode: formData.location?.value,
        streetAddress: "N/A",
      };
      // 3. الإرسال
      console.log("Sending to API:", listingPayload);
      const listingResponse =
        await listingService.createListing(listingPayload);

      return listingResponse;
    } catch (error) {
      console.error("Error details:", error.response?.data);
      throw error;
    }
  };

  const formattedCountries = countries.map((item) => ({
    value: item.cca2,
    label: item.name.common,
    flag: item.flag,
    latlng: item.latlng,
    region: item.region,
  }));

  const onBack = () => setStep((value) => value - 1);
  const onNext = () => setStep((value) => value + 1);

  const setCustomValue = (id, value) => {
    setFormData((prev) => ({ ...prev, [id]: value }));
  };

  const actionLabel = useMemo(() => {
    if (step === STEPS.PRICE) return "Create";
    return "Next";
  }, [step]);

  const secondaryActionLabel = useMemo(() => {
    if (step === STEPS.CATEGORY) return undefined;
    return "Back";
  }, [step]);

  // --- محتوى الخطوات (Body Content) ---
  let bodyContent = (
    <div className="flex flex-col gap-8">
      <div className="text-start">
        <div className="text-2xl font-bold">
          Which of these best describes your place?
        </div>
        <div className="font-light text-neutral-500 mt-2">Pick a category</div>
      </div>
      <div className="grid grid-cols-1 md:grid-cols-2 gap-3 max-h-[50vh] overflow-y-auto pr-2">
        {categories.map((item) => (
          <div key={item.label} className="col-span-1">
            <CategoryInput
              onClick={(category) => setCustomValue("category", category)}
              selected={formData.category === item.label}
              label={item.label}
              icon={item.icon}
            />
          </div>
        ))}
      </div>
    </div>
  );

  if (step === STEPS.LOCATION) {
    bodyContent = (
      <div className="flex flex-col gap-8">
        <div className="text-start">
          <div className="text-2xl font-bold">Where is your place located?</div>
          <div className="font-light text-neutral-500 mt-2">
            Help guests find you!
          </div>
        </div>
        <Select
          placeholder="Anywhere"
          isClearable
          options={formattedCountries}
          value={formData.location}
          onChange={(value) => setCustomValue("location", value)}
          formatOptionLabel={(option) => (
            <div className="flex flex-row items-center gap-3">
              <div>{option.flag}</div>
              <div>
                {option.label},{" "}
                <span className="text-neutral-500 ml-1">{option.region}</span>
              </div>
            </div>
          )}
          classNames={{ control: () => "p-1 border-2" }}
        />
      </div>
    );
  }
  if (step === STEPS.IMAGES) {
    bodyContent = (
      <div className="flex flex-col gap-8">
        <div className="text-start">
          <div className="text-2xl font-bold">Add a photo of your place</div>
          <div className="font-light text-neutral-500 mt-2">
            Show guests what your place looks like!
          </div>
        </div>
        <ImageUpload
          value={formData.imageSrc}
          onChange={(value) => setCustomValue("imageSrc", value)}
        />
      </div>
    );
  }
  if (step === STEPS.INFO) {
    bodyContent = (
      <div className="flex flex-col gap-8">
        <div className="text-start">
          <div className="text-2xl font-bold">
            Share some basics about your place
          </div>
          <div className="font-light text-neutral-500 mt-2">
            What amenities do you have?
          </div>
        </div>
        <Counter
          title="Guests"
          subtitle="How many guests do you allow?"
          value={formData.guestCount}
          onChange={(value) => setCustomValue("guestCount", value)}
        />
        <hr />
        <Counter
          title="Rooms"
          subtitle="How many rooms do you have?"
          value={formData.roomCount}
          onChange={(value) => setCustomValue("roomCount", value)}
        />
        <hr />
        <Counter
          title="Bathrooms"
          subtitle="How many bathrooms do you have?"
          value={formData.bathroomCount}
          onChange={(value) => setCustomValue("bathroomCount", value)}
        />
      </div>
    );
  }
  // --- STEP 4: DESCRIPTION ---
  if (step === STEPS.DESCRIPTION) {
    bodyContent = (
      <div className="flex flex-col gap-8">
        <div className="text-start">
          <div className="text-2xl font-bold">
            How would you describe your place?
          </div>
          <div className="font-light text-neutral-500 mt-2">
            Short and sweet works best!
          </div>
        </div>
        <div className="flex flex-col gap-4">
          {/* Title Input */}
          <div className="flex flex-col gap-1">
            <label className="text-sm font-semibold text-neutral-700">
              Title
            </label>
            <input
              className="p-4 border-2 border-neutral-200 rounded-xl outline-none focus:border-black transition"
              placeholder="e.g. Modern Villa with Pool"
              value={formData.title}
              onChange={(e) => setCustomValue("title", e.target.value)}
            />
          </div>
          <hr />
          {/* Description TextArea */}
          <div className="flex flex-col gap-1">
            <label className="text-sm font-semibold text-neutral-700">
              Description
            </label>
            <textarea
              className="p-4 border-2 border-neutral-200 rounded-xl outline-none focus:border-black transition h-40 resize-none"
              placeholder="Tell guests what makes your place special..."
              value={formData.description}
              onChange={(e) => setCustomValue("description", e.target.value)}
            />
          </div>
        </div>
      </div>
    );
  }

  // --- STEP 5: PRICE ---
  if (step === STEPS.PRICE) {
    bodyContent = (
      <div className="flex flex-col gap-8">
        <div className="text-start">
          <div className="text-2xl font-bold">Now, set your price</div>
          <div className="font-light text-neutral-500 mt-2">
            How much do you charge per night?
          </div>
        </div>
        <div className="flex flex-col items-center justify-center p-10 border-2 border-neutral-200 rounded-2xl bg-neutral-50/50">
          <div className="flex flex-row items-center gap-2">
            <span className="text-4xl font-bold text-neutral-700">$</span>
            <input
              type="number"
              className="text-5xl font-bold w-full max-w-[200px] outline-none bg-transparent border-b-2 border-transparent focus:border-neutral-300 transition appearance-none"
              value={formData.price}
              onChange={(e) => setCustomValue("price", e.target.value)}
            />
          </div>
          <div className="text-neutral-500 mt-4 text-lg">per night</div>
        </div>
      </div>
    );
  }

  return (
    <Modal
      disabled={isLoading} // تعطيل الأزرار أثناء التحميل
      title="Airbnb your home!"
      isOpen={isOpen}
      onClose={onClose}
      // هنا المنطق الصحيح:
      onSubmit={onSubmit}
      actionLabel={actionLabel}
      secondaryActionLabel={secondaryActionLabel}
      secondaryAction={step === STEPS.CATEGORY ? undefined : onBack}
      body={bodyContent}
    />
  );
};

export default RentModal;

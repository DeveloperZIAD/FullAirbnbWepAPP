import { useState, useMemo, useCallback } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import qs from "query-string";

import useSearchModal from "@/hooks/useSearchModal";
import Modal from "@/components/shared/Modal";
import Calendar from "@/components/shared/Calendar";
import Counter from "@/components/shared/Counter";
import CountrySelect from "@/components/shared/CountrySelect";

const STEPS = {
  LOCATION: 0,
  DATE: 1,
  INFO: 2,
};

const SearchModal = () => {
  const navigate = useNavigate();
  const [params] = useSearchParams();
  const searchModal = useSearchModal();

  const [step, setStep] = useState(STEPS.LOCATION);
  const [location, setLocation] = useState(null);
  const [guestCount, setGuestCount] = useState(1);

  // حالات جديدة للأسعار
  const [minPrice, setMinPrice] = useState("");
  const [maxPrice, setMaxPrice] = useState("");

  const [dateRange, setDateRange] = useState({
    startDate: new Date(),
    endDate: new Date(),
    key: "selection",
  });

  const onBack = () => setStep((value) => value - 1);
  const onNext = () => setStep((value) => value + 1);

  const onSubmit = useCallback(async () => {
    if (step !== STEPS.INFO) {
      return onNext();
    }

    // let currentQuery = {};
    // if (params) {
    //   currentQuery = qs.parse(params.toString());
    // }

    // بناء كائن البحث النهائي حسب مسميات الـ Swagger
    const updatedQuery = {
      // ...currentQuery,
      Location: location?.label,
      Guests: guestCount,
      MinPrice: minPrice || undefined, // إرسال القيمة فقط إذا وجدت
      MaxPrice: maxPrice || undefined,
    };

    if (dateRange.startDate) {
      updatedQuery.CheckIn = dateRange.startDate.toISOString();
    }
    if (dateRange.endDate) {
      updatedQuery.CheckOut = dateRange.endDate.toISOString();
    }

    const url = qs.stringifyUrl(
      {
        url: "/",
        query: updatedQuery,
      },
      { skipNull: true },
    );

    setStep(STEPS.LOCATION);
    searchModal.onClose();
    navigate(url);
  }, [
    step,
    searchModal,
    location,
    guestCount,
    dateRange,
    navigate,
    params,
    minPrice,
    maxPrice,
  ]);

  const actionLabel = useMemo(
    () => (step === STEPS.INFO ? "Search" : "Next"),
    [step],
  );
  const secondaryActionLabel = useMemo(
    () => (step === STEPS.LOCATION ? undefined : "Back"),
    [step],
  );

  // --- محتوى الخطوات ---

  // الخطوة 1: الموقع
  let bodyContent = (
    <div className="flex flex-col gap-8">
      <div className="text-start">
        <div className="text-2xl font-bold">Where do you want to go?</div>
        <div className="text-neutral-500 mt-2">Find the perfect location!</div>
      </div>
      <CountrySelect
        value={location}
        onChange={(value) => setLocation(value)}
      />
    </div>
  );

  // الخطوة 2: التاريخ
  if (step === STEPS.DATE) {
    bodyContent = (
      <div className="flex flex-col gap-8">
        <div className="text-start">
          <div className="text-2xl font-bold">When do you plan to go?</div>
          <div className="text-neutral-500 mt-2">
            Make sure everyone is free!
          </div>
        </div>
        <Calendar
          value={dateRange}
          onChange={(value) => setDateRange(value.selection)}
        />
      </div>
    );
  }

  // الخطوة 3: الضيوف والأسعار
  if (step === STEPS.INFO) {
    bodyContent = (
      <div className="flex flex-col gap-8">
        <div className="text-start">
          <div className="text-2xl font-bold">More information</div>
          <div className="text-neutral-500 mt-2">
            Refine your search results!
          </div>
        </div>

        {/* التحكم في عدد الضيوف */}
        <Counter
          title="Guests"
          subtitle="How many guests are coming?"
          value={guestCount}
          onChange={setGuestCount}
        />

        <hr />

        {/* حقول ميزانية الأسعار */}
        <div className="flex flex-col gap-4">
          <div className="font-semibold text-lg">Price Range (Optional)</div>
          <div className="flex flex-row items-center gap-4">
            <div className="flex flex-col w-full">
              <label className="text-xs text-neutral-500 mb-1">Min Price</label>
              <input
                type="number"
                placeholder="$"
                value={minPrice}
                onChange={(e) => setMinPrice(e.target.value)}
                className="p-3 border-[1px] border-neutral-300 rounded-md outline-none focus:border-black transition"
              />
            </div>
            <div className="flex flex-col w-full">
              <label className="text-xs text-neutral-500 mb-1">Max Price</label>
              <input
                type="number"
                placeholder="$"
                value={maxPrice}
                onChange={(e) => setMaxPrice(e.target.value)}
                className="p-3 border-[1px] border-neutral-300 rounded-md outline-none focus:border-black transition"
              />
            </div>
          </div>
        </div>
      </div>
    );
  }

  return (
    <Modal
      isOpen={searchModal.isOpen}
      onClose={searchModal.onClose}
      onSubmit={onSubmit}
      title="Filters"
      actionLabel={actionLabel}
      secondaryActionLabel={secondaryActionLabel}
      secondaryAction={step === STEPS.LOCATION ? undefined : onBack}
      body={bodyContent}
    />
  );
};

export default SearchModal;

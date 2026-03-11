import { DateRange } from "react-date-range";
import "react-date-range/dist/styles.css";
import "react-date-range/dist/theme/default.css";

const Calendar = ({ value, onChange, disabledDates, minDate }) => {
  return (
    <DateRange
      rangeColors={["#262626"]}
      ranges={[value]}
      date={new Date()}
      onChange={onChange}
      direction="vertical"
      showDateDisplay={false}
      // إذا لم يتم تمرير minDate، نستخدم التاريخ الحالي كحالة احتياطية
      minDate={minDate || new Date()}
      disabledDates={disabledDates}
    />
  );
};

export default Calendar;

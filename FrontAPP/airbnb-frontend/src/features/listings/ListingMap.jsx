import React, { useMemo } from "react";
import { MapContainer, TileLayer, Marker, Circle } from "react-leaflet";
import L from "leaflet";
import "leaflet/dist/leaflet.css";

// إصلاح أيقونات Leaflet
import markerIcon2x from "leaflet/dist/images/marker-icon-2x.png";
import markerIcon from "leaflet/dist/images/marker-icon.png";
import markerShadow from "leaflet/dist/images/marker-shadow.png";

delete L.Icon.Default.prototype._getIconUrl;
L.Icon.Default.mergeOptions({
  iconUrl: markerIcon,
  iconRetinaUrl: markerIcon2x,
  shadowUrl: markerShadow,
});

const ListingMap = ({ locationName, address }) => {
  // 1. تحليل البيانات الوهمية (Faker) لاستخراج البلد والمدينة
  const { cityName, countryName } = useMemo(() => {
    const parts = locationName ? locationName.split(",") : [];
    const country =
      parts.length > 1
        ? parts[parts.length - 1].trim()
        : locationName || "Germany";
    const city = parts[0]?.trim() || "City";
    return { cityName: city, countryName: country };
  }, [locationName]);

  // 2. قاعدة بيانات المدن الحقيقية (لضمان البقاء داخل حدود الدولة)
  const geoLogic = {
    Germany: {
      default: [52.52, 13.405], // Berlin
      A: [53.5511, 9.9937], // Hamburg
      M: [48.1351, 11.582], // Munich
      C: [50.9375, 6.9603], // Cologne
    },
    Egypt: {
      default: [30.0444, 31.2357], // Cairo
      A: [31.2001, 29.9187], // Alexandria
      G: [30.0131, 31.2089], // Giza
      L: [25.6872, 32.6432], // Luxor
    },
    Jordan: {
      default: [31.9454, 35.9284], // Amman
      A: [29.5267, 35.0078], // Aqaba
      I: [32.5568, 35.8469], // Irbid
    },
    Seychelles: {
      default: [-4.6796, 55.4481], // Victoria
    },
    Dominica: {
      default: [15.3019, -61.3883], // Roseau
    },
    Tuvalu: {
      default: [-8.5211, 179.1962], // Funafuti
    },
    Argentina: {
      default: [-34.6037, -58.3816], // Buenos Aires
      C: [-31.4201, -64.1888], // Córdoba
    },
    Albania: {
      default: [41.3275, 19.8187], // Tirana
    },
    Andorra: {
      default: [42.5063, 1.5218], // Andorra la Vella
    },
    "United Arab Emirates": {
      default: [25.2048, 55.2708], // Dubai
      A: [24.4539, 54.3773], // Abu Dhabi
      S: [25.3463, 55.4209], // Sharjah
    },
    Poland: {
      default: [52.2297, 21.0122], // Warsaw
      K: [50.0647, 19.945], // Kraków
    },
    "Republic of Korea": {
      default: [37.5665, 126.978], // Seoul
      B: [35.1796, 129.0756], // Busan
    },
    "United States of America": {
      default: [40.7128, -74.006], // New York City
      L: [34.0522, -118.2437], // Los Angeles
      C: [41.8781, -87.6298], // Chicago
    },
    France: {
      default: [48.8566, 2.3522], // Paris
      M: [43.2965, 5.3698], // Marseille
    },
    Italy: {
      default: [41.9028, 12.4964], // Rome
      M: [45.4642, 9.19], // Milan
    },
    Cayman: {
      // Cayman Islands
      default: [19.3133, -81.2546], // George Town
    },
    Spain: {
      default: [40.4168, -3.7038], // Madrid
      B: [41.3851, 2.1734], // Barcelona
    },
    "Russian Federation": {
      default: [55.7558, 37.6173], // Moscow
      S: [59.9343, 30.3351], // Saint Petersburg
    },
    "Libyan Arab Jamahiriya": {
      default: [32.8872, 13.1913], // Tripoli
    },
    Gabon: {
      default: [0.4162, 9.4673], // Libreville
    },
    Bermuda: {
      default: [32.2949, -64.7814], // Hamilton
    },
    "Cocos (Keeling) Islands": {
      default: [-12.1642, 96.87], // West Island
    },
    Aruba: {
      default: [12.5211, -69.9683], // Oranjestad
    },
    Belgium: {
      default: [50.8503, 4.3517], // Brussels
    },
    Honduras: {
      default: [14.0723, -87.1921], // Tegucigalpa
    },
    Mongolia: {
      default: [47.8864, 106.9057], // Ulaanbaatar
    },
    Malaysia: {
      default: [3.139, 101.6869], // Kuala Lumpur
    },
    "Equatorial Guinea": {
      default: [3.7528, 8.7737], // Malabo
    },
    Kenya: {
      default: [-1.2921, 36.8219], // Nairobi
    },
    Montenegro: {
      default: [42.4411, 19.2636], // Podgorica
    },
    Jamaica: {
      default: [17.9712, -76.7928], // Kingston
    },
    "Falkland Islands (Malvinas)": {
      default: [-51.7963, -59.5236], // Stanley
    },
    Antarctica: {
      default: [-75.2509, 0.0], // South Pole (تقريبي)
    },
    Vietnam: {
      default: [10.8231, 106.6297], // Ho Chi Minh City
    },
    Eritrea: {
      default: [15.3229, 38.9381], // Asmara
    },
    "Holy See (Vatican City State)": {
      default: [41.9029, 12.4534], // Vatican City
    },
    "Bosnia and Herzegovina": {
      default: [43.8563, 18.4131], // Sarajevo
    },
    Tokelau: {
      default: [-9.1667, -171.8333], // Atafu (تقريبي)
    },
    Fiji: {
      default: [-17.7134, 178.065], // Suva
    },
    Namibia: {
      default: [-22.5609, 17.0658], // Windhoek
    },
    Bhutan: {
      default: [27.5142, 90.4336], // Thimphu
    },
    Bahamas: {
      default: [25.0582, -77.3431], // Nassau
    },
    Algeria: {
      default: [36.7372, 3.0869], // Algiers
    },
    Lithuania: {
      default: [54.6872, 25.2797], // Vilnius
    },
    "Hong Kong": {
      default: [22.3964, 114.1095], // Hong Kong
    },
    Samoa: {
      default: [-13.759, -172.1046], // Apia
    },
    Liechtenstein: {
      default: [47.141, 9.5209], // Vaduz
    },
    Bangladesh: {
      default: [23.8103, 90.4125], // Dhaka
    },
    "Saint Helena": {
      default: [-15.9254, -5.717], // Jamestown
    },
    "Saudi Arabia": {
      default: [24.7136, 46.6753], // Riyadh
    },
    Suriname: {
      default: [5.852, -55.2038], // Paramaribo
    },
    // إضافة دول أخرى لو ظهرت في الـ Faker
    default: {
      default: [30.0444, 31.2357], // Cairo, Egypt كـ fallback
    },
  };

  // 3. اختيار الموقع النهائي بناءً على الحرف الأول من المدينة الوهمية
  const position = useMemo(() => {
    const countryData = geoLogic[countryName] || geoLogic["Germany"]; // Germany كاحتياط
    const firstLetter = cityName.charAt(0).toUpperCase();

    // نختار المدينة الحقيقية المقابلة للحرف، أو العاصمة (default)
    const baseCoords = countryData[firstLetter] || countryData["default"];

    // إضافة "هزة" عشوائية بسيطة جداً لكي لا تتطابق العقارات في نفس المدينة
    const jitter = (Math.random() - 0.5) * 0.005;
    return [baseCoords[0] + jitter, baseCoords[1] + jitter];
  }, [cityName, countryName]);

  const fullAddress = `${address || ""} ${locationName || ""}`.trim();

  return (
    <div className="py-8 border-b border-gray-200">
      <div className="mb-6">
        <h3 className="text-xl font-semibold mb-1">Where you'll be</h3>
        <p className="text-neutral-500 font-light">{fullAddress}</p>
      </div>

      <div className="h-[480px] w-full rounded-2xl overflow-hidden z-0 border border-neutral-200 shadow-sm relative">
        <MapContainer
          center={position}
          zoom={13}
          scrollWheelZoom={false}
          className="h-full w-full"
          key={`${position[0]}-${position[1]}`}
        >
          <TileLayer
            attribution="&copy; OpenStreetMap"
            url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
          />

          {/* دائرة الخصوصية لـ Airbnb */}
          <Circle
            center={position}
            radius={800}
            pathOptions={{
              fillColor: "#FF385C",
              color: "#FF385C",
              fillOpacity: 0.1,
              weight: 2,
              dashArray: "5, 10",
            }}
          />

          <Marker position={position} />
        </MapContainer>
      </div>

      <div className="mt-4 flex items-center gap-2">
        <div className="w-2 h-2 rounded-full bg-rose-500 animate-pulse"></div>
        <p className="text-[11px] text-neutral-400 uppercase tracking-widest font-bold">
          Exact location provided after booking in {countryName}
        </p>
      </div>
    </div>
  );
};

export default ListingMap;

import {
  FaWifi,
  FaCar,
  FaSwimmingPool,
  FaTv,
  FaSnowflake,
} from "react-icons/fa";
import {
  MdOutlineKitchen,
  MdOutlineWash,
  MdPets,
  MdWorkOutline,
} from "react-icons/md";
import { GiBarbecue } from "react-icons/gi";

export const allAmenities = [
  { label: "Wifi", icon: FaWifi },
  { label: "Free parking", icon: FaCar },
  { label: "Pool", icon: FaSwimmingPool },
  { label: "Kitchen", icon: MdOutlineKitchen },
  { label: "TV", icon: FaTv },
  { label: "Air conditioning", icon: FaSnowflake },
  { label: "Washer", icon: MdOutlineWash },
  { label: "Pets allowed", icon: MdPets },
  { label: "Dedicated workspace", icon: MdWorkOutline },
  { label: "BBQ grill", icon: GiBarbecue },
];

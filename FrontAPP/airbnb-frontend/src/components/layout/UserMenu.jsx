import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import accountService from "@/services/accountService";
import useRentModal from "@/hooks/useRentModal";
import {
  DropdownMenuItem,
  DropdownMenuSeparator,
} from "@/components/ui/dropdown-menu";
import InboxPage from "@/pages/messagingSystem/InboxPage";

const UserMenu = ({ onLogout }) => {
  const [userRole, setUserRole] = useState(null);
  const currentUser = JSON.parse(localStorage.getItem("user"));
  const navigate = useNavigate();
  const rentModal = useRentModal();

  useEffect(() => {
    if (currentUser?.id) {
      accountService
        .getUserRole(currentUser.id)
        .then((role) => setUserRole(role))
        .catch((err) => console.error("Error fetching role", err));
    }
  }, [currentUser?.id]);

  if (!currentUser) return null;

  return (
    <>
      <DropdownMenuItem
        onClick={() => navigate("/messages")}
        className="py-3 cursor-pointer font-semibold"
      >
        Inbox
      </DropdownMenuItem>
      <DropdownMenuItem
        onClick={() => navigate("/trips")}
        className="py-3 cursor-pointer font-semibold"
      >
        My trips
      </DropdownMenuItem>

      <DropdownMenuItem
        onClick={() => navigate("/favorites")}
        className="py-3 cursor-pointer font-semibold"
      >
        My favorites
      </DropdownMenuItem>
      <DropdownMenuItem
        onClick={() => navigate("/properties")}
        className="py-3 cursor-pointer font-semibold"
      >
        My properties
      </DropdownMenuItem>

      {userRole === "Host" ? (
        <DropdownMenuItem
          onClick={() => navigate("/host/dashboard")}
          className="py-3 cursor-pointer font-semibold"
        >
          Host Dashboard
        </DropdownMenuItem>
      ) : (
        <DropdownMenuItem
          onClick={rentModal.onOpen}
          className="py-3 cursor-pointer font-semibold"
        >
          Become a host
        </DropdownMenuItem>
      )}

      <DropdownMenuSeparator />
      <DropdownMenuItem
        onClick={onLogout}
        className="py-3 cursor-pointer text-rose-500"
      >
        Logout
      </DropdownMenuItem>
    </>
  );
};

export default UserMenu;

import { useNavigate } from "react-router-dom";
import { Button } from "@/components/ui/button";

const EmptyState = ({
  title = "No exact matches",
  subtitle = "Try changing or removing some of your filters.",
  showReset,
}) => {
  const navigate = useNavigate();
  return (
    <div className="h-[60vh] flex flex-col gap-2 justify-center items-center">
      <div className="text-2xl font-bold">{title}</div>
      <div className="font-light text-neutral-500 mt-2">{subtitle}</div>
      <div className="w-48 mt-4">
        {showReset && (
          <Button variant="outline" onClick={() => navigate("/")}>
            Remove all filters
          </Button>
        )}
      </div>
    </div>
  );
};

export default EmptyState;

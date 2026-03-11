import { useNavigate } from "react-router-dom";

const NotFound = () => {
  const navigate = useNavigate();

  return (
    <div className="h-screen flex flex-col items-center justify-center gap-4">
      <h1 className="text-9xl font-bold text-rose-500">404</h1>
      <p className="text-2xl font-semibold">Oops! Page not found.</p>
      <p className="text-neutral-500">
        The page you're looking for doesn't exist.
      </p>
      <button
        onClick={() => navigate("/")}
        className="mt-4 px-6 py-3 bg-rose-500 text-white rounded-xl font-bold hover:shadow-md transition"
      >
        Go Back Home
      </button>
    </div>
  );
};

export default NotFound;

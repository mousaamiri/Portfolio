import { useEffect } from "react";
import { Navigate, Outlet, useLocation, useNavigate } from "react-router-dom";
import { isAuthenticated } from "../lib/api";

export default function ProtectedRoute() {
  const location = useLocation();
  const navigate = useNavigate();

  useEffect(() => {
    const onUnauthorized = () => navigate("/Admin/Login", { replace: true });
    window.addEventListener("admin-unauthorized", onUnauthorized);
    return () => window.removeEventListener("admin-unauthorized", onUnauthorized);
  }, [navigate]);

  if (!isAuthenticated()) {
    return <Navigate to="/Admin/Login" replace state={{ from: location.pathname }} />;
  }

  return <Outlet />;
}

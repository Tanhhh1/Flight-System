import { Navigate, Outlet } from "react-router-dom";
import { useSelector } from "react-redux";
import { isAdminRole } from "@/constants/auth";

function ClientProtectedRoute() {
    const { user } = useSelector((state) => state.auth);

    if (!user) return <Navigate to="/" replace />;
    if (isAdminRole(user.roles)) return <Navigate to="/admin" replace />;

    return <Outlet />;
}

export default ClientProtectedRoute;
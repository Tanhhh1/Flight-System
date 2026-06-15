import { Navigate, Outlet } from "react-router-dom";
import { useSelector } from "react-redux";
import { isAdminRole } from "@/constants/auth";

function AdminProtectedRoute() {
    const { user } = useSelector((state) => state.auth);

    if (!user) return <Navigate to="/admin/login" replace />;
    if (!isAdminRole(user.roles)) return <Navigate to="/admin/login" replace />;

    return <Outlet />;
}

export default AdminProtectedRoute;
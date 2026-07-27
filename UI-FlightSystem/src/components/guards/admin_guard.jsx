import { Navigate, Outlet } from "react-router-dom";
import { useSelector } from "react-redux";
import { isAdminRole } from "@/features/auth/auth_constants";

function AdminProtectedRoute() {
    const { user } = useSelector((state) => state.auth);

    if (!user) return <Navigate to="/admin/login" replace />;
    if (!isAdminRole(user.roles)) return <Navigate to="/admin/login" replace />;

    return <Outlet />;
}

export default AdminProtectedRoute;
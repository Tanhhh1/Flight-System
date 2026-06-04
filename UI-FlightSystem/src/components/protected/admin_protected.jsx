import React from "react";
import { Navigate, Outlet } from "react-router-dom";
import { useSelector } from "react-redux";

function AdminProtectedRoute() {
    const { user } = useSelector((state) => state.auth);

    if (!user) return <Navigate to="/admin/login" replace />;

    const roles = user.roles ?? [];
    if (!roles.includes("admin") && !roles.includes("staff"))
        return <Navigate to="/admin/login" replace />;

    return <Outlet />;
}

export default AdminProtectedRoute;
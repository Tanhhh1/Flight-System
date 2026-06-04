import React from "react";
import { Navigate, Outlet } from "react-router-dom";
import { useSelector } from "react-redux";

function ClientProtectedRoute() {
    const { user } = useSelector((state) => state.auth);

    if (!user) return <Navigate to="/login" replace />;

    const roles = user.roles ?? [];
    if (roles.includes("admin") || roles.includes("staff"))
        return <Navigate to="/admin" replace />;

    return <Outlet />;
}

export default ClientProtectedRoute;
import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import { Suspense } from "react";

import AdminLayout from "@/layouts/admin/index";
import ClientLayout from "@/layouts/user/index";

import AdminProtectedRoute from "@/components/protected/admin_protected";
import ClientProtectedRoute from "@/components/protected/client_protected";
import AuthGuard from "@/components/protected/auth_guard";

import AdminLogin from "@/features/admin/authentication/login";

import { adminPrivateRoutes, adminPaths } from "./configs/admin_routes";
import { clientPrivateRoutes, clientPublicRoutes } from "./configs/client_routes";

import "@/components/shared/page_load.css";

const PageLoading = () => (
    <div className="page-loading-container">
        <div className="page-loading-content">
            <i className="bx bx-loader-alt page-loading-icon"></i>
            <span>Đang tải dữ liệu...</span>
        </div>
    </div>
);

function App() {
    return (
        <BrowserRouter>
            <Suspense fallback={<PageLoading />}>
                <Routes>
                    <Route path={adminPaths.login} element={<AdminLogin />} />
                    <Route element={<AdminProtectedRoute />}>
                        <Route path={adminPaths.admin.root} element={<AdminLayout />}>
                            <Route index element={<Navigate to={adminPaths.admin.dashboard} replace />} />
                            {adminPrivateRoutes.map((route, index) => ( <Route key={index} path={route.path} element={route.element}/> ))}
                        </Route>
                    </Route>

                    <Route path="/" element={<AuthGuard><ClientLayout /></AuthGuard>}>
                        {clientPublicRoutes.map((route, index) => (
                            <Route key={index} path={route.path} element={route.element}>
                                {route.children?.map((subRoute, subIndex) => ( <Route key={subIndex} path={subRoute.path} element={subRoute.element}/> ))}
                            </Route>
                        ))}
                        <Route element={<ClientProtectedRoute />}>
                            {clientPrivateRoutes.map((route, index) => (
                                <Route key={index} path={route.path} element={route.element}>
                                    {route.children?.map((subRoute, subIndex) => ( <Route key={subIndex} path={subRoute.path} element={subRoute.element}/> ))}
                                </Route>
                            ))}
                        </Route>
                    </Route>
                </Routes>
            </Suspense>
        </BrowserRouter>
    );
}

export default App;
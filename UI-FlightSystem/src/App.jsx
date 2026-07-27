import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import { Suspense } from "react";

import AdminLayout from "@/layouts/admin/index";
import ClientLayout from "@/layouts/user/index";

import AdminProtectedRoute from "@/components/guards/admin_guard";
import ClientProtectedRoute from "@/components/guards/client_guard";
import AuthGuard from "@/components/guards/auth_guard";

import AdminLogin from "@/features/admin/auth/login_page";

import { 
    PATHS, 
    adminPrivateRoutes, 
    clientPrivateRoutes, 
    clientPublicRoutes 
} from "@/app/routes/index";

import "@/components/styles/page_load.css";

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
                    {/* Admin Routes */}
                    <Route path={PATHS.ADMIN.LOGIN} element={<AdminLogin />} />
                    
                    <Route element={<AdminProtectedRoute />}>
                        <Route path={PATHS.ADMIN.ROOT} element={<AdminLayout />}>
                            <Route index element={<Navigate to={PATHS.ADMIN.DASHBOARD} replace />} />
                            {adminPrivateRoutes.map((route, index) => (
                                <Route key={index} path={route.path} element={route.element} />
                            ))}
                        </Route>
                    </Route>

                    {/* Client Routes */}
                    <Route path="/" element={<AuthGuard><ClientLayout /></AuthGuard>}>
                        {/* Public Client Routes */}
                        {clientPublicRoutes.map((route, index) => (
                            <Route key={index} path={route.path} element={route.element}>
                                {route.children?.map((subRoute, subIndex) => (
                                    <Route key={subIndex} path={subRoute.path} element={subRoute.element} />
                                ))}
                            </Route>
                        ))}

                        {/* Private Client Routes */}
                        <Route element={<ClientProtectedRoute />}>
                            {clientPrivateRoutes.map((route, index) => (
                                <Route key={index} path={route.path} element={route.element}>
                                    {route.children?.map((subRoute, subIndex) => (
                                        <Route key={subIndex} path={subRoute.path} element={subRoute.element} />
                                    ))}
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
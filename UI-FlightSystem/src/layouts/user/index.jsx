import { useState } from "react";
import { useDispatch } from "react-redux";
import { Outlet } from "react-router-dom";
import Header from "@/layouts/user/header";
import Footer from "@/layouts/user/footer";
import LoginModal from "@/features/client/auth/login_modal";
import RegisterModal from "@/features/client/auth/register_modal";
import { openLoginModal } from "@/features/auth/auth_slice";
import "./layout.css";

function ClientLayout() {
    const dispatch = useDispatch();
    const [authMode, setAuthMode] = useState(null);

    const closeAuth = () => setAuthMode(null);
    const openLogin = () => {
        setAuthMode(null);
        dispatch(openLoginModal());
    };
    const openRegister = () => setAuthMode("register");

    return (
        <div className="client_layout">
            <Header onOpenLogin={openLogin} />
            <main className="client_main">
                <Outlet />
            </main>
            <Footer />
            <LoginModal onSwitchToRegister={openRegister} />
            <RegisterModal isOpen={authMode === "register"} onClose={closeAuth} onSwitchToLogin={openLogin} />
        </div>
    );
}

export default ClientLayout;
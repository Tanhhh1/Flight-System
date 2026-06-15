import { useState } from "react";
import { Outlet } from "react-router-dom";
import Header from "@/layouts/user/header";
import Footer from "@/layouts/user/footer";
import LoginModal from "@/features/user/authentication/login";
import RegisterModal from "@/features/user/authentication/register";
import "./layout.css";

function ClientLayout() {
    const [authMode, setAuthMode] = useState(null);

    const closeAuth = () => setAuthMode(null);
    const openLogin = () => setAuthMode("login");
    const openRegister = () => setAuthMode("register");

    return (
        <div className="client_layout">
            <Header onOpenLogin={openLogin} />
            <main className="client_main">
                <Outlet /> 
            </main>
            <Footer />
            <LoginModal isOpen={authMode === "login"} onClose={closeAuth} onSwitchToRegister={openRegister}/>
            <RegisterModal isOpen={authMode === "register"} onClose={closeAuth} onSwitchToLogin={openLogin}/>
        </div>
    );
};

export default ClientLayout;
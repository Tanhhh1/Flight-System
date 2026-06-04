import { Outlet } from "react-router-dom";
import Sidebar from "./sidebar";
import "./layout.css";

function AdminLayout() {
    return (
        <div className="admin_layout_wrapper">
            <Sidebar />
            <section className="section_container">
                <Outlet />
            </section>
        </div>
    );
}

export default AdminLayout;
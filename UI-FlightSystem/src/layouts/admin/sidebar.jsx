import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useDispatch, useSelector } from "react-redux";
import { logout } from "@/features/shared/auth/auth_slice";

import "./layout.css";
import "@/components/shared/list_shared.css";
import "@/components/shared/form_shared.css";
import "@/components/table/table.css";
import userImage from "@/assets/images/user.png";

const ROLE_LABEL = {
    admin: "Quản trị viên",
    staff: "Nhân viên",
};

function Sidebar() {
    const [active, setActive] = useState(true);
    const [open, setOpen] = useState(false);

    const dispatch = useDispatch();
    const navigate = useNavigate();
    const { user } = useSelector((state) => state.auth);

    const handleSidebarToggle = () => {
        if (active) setOpen(false);
        setActive(!active);
    };

    const handleLogout = async () => {
        await dispatch(logout());
        navigate("/admin/login", { replace: true });
    };

    const roleLabel = ROLE_LABEL[user?.roles?.[0]] ?? "Người dùng";

    return (
        <div className={active ? "sidebar active" : "sidebar"}>
            <div className="logo_content">
                <div className="logo">
                    <i className="bx bx-paper-plane" />
                    <div className="logo_name">SkyJourney</div>
                </div>
                <i className="bx bx-menu" id="btn" onClick={handleSidebarToggle} />
            </div>

            <ul className="nav_list">
                <li>
                    <Link to="/admin/dashboard">
                        <i className="bx bxs-dashboard" />
                        <span className="links_name">Bảng điều khiển</span>
                    </Link>
                </li>
                <li>
                    <Link to="/admin/accounts">
                        <i className="bx bxs-user-account" />
                        <span className="links_name">Quản lý Tài khoản</span>
                    </Link>
                </li>

                <li className={open ? "has_dropdown open" : "has_dropdown"}>
                    <div className="dropdown_btn" onClick={() => active && setOpen(!open)}>
                        <div className="dropdown_title">
                            <i className="bx bxs-data" />
                            <span className="links_name">Quản lý Danh mục</span>
                        </div>
                        <i className={open ? "bx bx-chevron-up arrow_icon" : "bx bx-chevron-down arrow_icon"} />
                    </div>
                    {open && active && (
                        <ul className="sidebar_dropdown_menu">
                            <li>
                                <Link to="/admin/airlines">
                                    <i className="bx bxs-plane-take-off" />
                                    <span className="links_name">Quản lý Hãng bay</span>
                                </Link>
                            </li>
                            <li>
                                <Link to="/admin/airports">
                                    <i className="bx bx-buildings" />
                                    <span className="links_name">Quản lý Sân bay</span>
                                </Link>
                            </li>
                            <li>
                                <Link to="/admin/planes">
                                    <i className="bx bxs-plane" />
                                    <span className="links_name">Quản lý Máy bay</span>
                                </Link>
                            </li>
                            <li>
                                <Link to="/admin/routes">
                                    <i className="bx bx-map-alt" />
                                    <span className="links_name">Quản lý Tuyến bay</span>
                                </Link>
                            </li>
                        </ul>
                    )}
                </li>

                <li>
                    <Link to="/admin/flights">
                        <i className="bx bxs-plane-alt" />
                        <span className="links_name">Quản lý Chuyến bay</span>
                    </Link>
                </li>
                <li>
                    <Link to="/admin/bookings">
                        <i className="bx bxs-book-bookmark" />
                        <span className="links_name">Quản lý Đơn đặt vé</span>
                    </Link>
                </li>
                <li>
                    <Link to="/admin/services">
                        <i className="bx bxs-cog" />
                        <span className="links_name">Quản lý Dịch vụ</span>
                    </Link>
                </li>
                <li>
                    <Link to="/admin/reviews">
                        <i className="bx bxs-message-dots" />
                        <span className="links_name">Quản lý Đánh giá</span>
                    </Link>
                </li>
            </ul>

            <div className="profile_content">
                <div className="profile">
                    <div className="profile_details">
                        <Link to="/admin/profile">
                            <img src={userImage} alt="user" className="user-avatar" />
                            <div className="name_job">
                                <div className="name">{user?.fullName ?? "Admin"}</div>
                                <div className="job">{roleLabel}</div>
                            </div>
                        </Link>
                    </div>
                    <i className="bx bx-log-out" id="log_out" onClick={handleLogout} title="Đăng xuất" />
                </div>
            </div>
        </div>
    );
}

export default Sidebar;
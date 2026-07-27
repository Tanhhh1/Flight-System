import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useDispatch, useSelector } from "react-redux";
import { logout } from "@/features/auth/auth_slice";
import { PATHS } from "@/app/routes/paths";

import "./layout.css";
import "@/components/styles/list_shared.css";
import "@/components/styles/form_shared.css";
import "@/components/styles/table.css";
import userImage from "@/assets/images/user.png";

const ROLE_LABEL = {
    admin: "Quản trị viên",
    staff: "Nhân viên",
};

function Sidebar() {
    const [active, setActive] = useState(true);
    const [openMenu, setOpenMenu] = useState(null);

    const dispatch = useDispatch();
    const navigate = useNavigate();
    const { user } = useSelector((state) => state.auth);

    const handleSidebarToggle = () => {
        if (active) setOpenMenu(null);
        setActive(!active);
    };

    const toggleMenu = (menu) => {
        if (!active) return;
        setOpenMenu((prev) => (prev === menu ? null : menu));
    };

    const handleLogout = async () => {
        await dispatch(logout());
        navigate(PATHS.ADMIN.LOGIN, { replace: true });
    };

    const roleLabel = ROLE_LABEL[user?.roles?.[0]] ?? "Người dùng";
    const adminRoot = PATHS.ADMIN.ROOT;

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
                    <Link to={PATHS.ADMIN.DASHBOARD}>
                        <i className="bx bxs-dashboard" />
                        <span className="links_name">Bảng điều khiển</span>
                    </Link>
                </li>
                <li>
                    <Link to={PATHS.ADMIN.ACCOUNTS}>
                        <i className="bx bxs-user-account" />
                        <span className="links_name">Quản lý Tài khoản</span>
                    </Link>
                </li>

                {/* Dropdown Danh mục */}
                <li className={openMenu === "category" ? "has_dropdown open" : "has_dropdown"}>
                    <div className="dropdown_btn" onClick={() => toggleMenu("category")}>
                        <div className="dropdown_title">
                            <i className="bx bxs-data" />
                            <span className="links_name">Quản lý Danh mục</span>
                        </div>
                        <i className={openMenu === "category" ? "bx bx-chevron-up arrow_icon" : "bx bx-chevron-down arrow_icon"} />
                    </div>
                    {openMenu === "category" && active && (
                        <ul className="sidebar_dropdown_menu">
                            <li>
                                <Link to={PATHS.ADMIN.AIRLINES}>
                                    <i className="bx bxs-plane-take-off" />
                                    <span className="links_name">Quản lý Hãng bay</span>
                                </Link>
                            </li>
                            <li>
                                <Link to={PATHS.ADMIN.AIRPORTS}>
                                    <i className="bx bx-buildings" />
                                    <span className="links_name">Quản lý Sân bay</span>
                                </Link>
                            </li>
                            <li>
                                <Link to={PATHS.ADMIN.PLANES}>
                                    <i className="bx bxs-plane" />
                                    <span className="links_name">Quản lý Máy bay</span>
                                </Link>
                            </li>
                            <li>
                                <Link to={PATHS.ADMIN.ROUTES}>
                                    <i className="bx bx-map-alt" />
                                    <span className="links_name">Quản lý Tuyến bay</span>
                                </Link>
                            </li>
                        </ul>
                    )}
                </li>

                <li>
                    <Link to={PATHS.ADMIN.FLIGHTS}>
                        <i className="bx bxs-plane-alt" />
                        <span className="links_name">Quản lý Chuyến bay</span>
                    </Link>
                </li>

                {/* Dropdown Đơn đặt vé */}
                <li className={openMenu === "booking" ? "has_dropdown open" : "has_dropdown"}>
                    <div className="dropdown_btn" onClick={() => toggleMenu("booking")}>
                        <div className="dropdown_title">
                            <i className="bx bxs-book-bookmark" />
                            <span className="links_name">Quản lý Đơn đặt vé</span>
                        </div>
                        <i className={openMenu === "booking" ? "bx bx-chevron-up arrow_icon" : "bx bx-chevron-down arrow_icon"} />
                    </div>
                    {openMenu === "booking" && active && (
                        <ul className="sidebar_dropdown_menu">
                            <li>
                                <Link to={PATHS.ADMIN.BOOKINGS}>
                                    <i className="bx bxs-book-content" />
                                    <span className="links_name">Danh sách đơn</span>
                                </Link>
                            </li>
                            <li>
                                <Link to={PATHS.ADMIN.SUPPORT}>
                                    <i className="bx bx-support" />
                                    <span className="links_name">Yêu cầu hỗ trợ</span>
                                </Link>
                            </li>
                        </ul>
                    )}
                </li>

                <li>
                    <Link to={PATHS.ADMIN.SERVICES}>
                        <i className="bx bxs-cog" />
                        <span className="links_name">Quản lý Dịch vụ</span>
                    </Link>
                </li>
                <li>
                    <Link to={PATHS.ADMIN.REVIEWS}>
                        <i className="bx bxs-message-dots" />
                        <span className="links_name">Quản lý Đánh giá</span>
                    </Link>
                </li>
            </ul>

            <div className="profile_content">
                <div className="profile">
                    <div className="profile_details">
                        <Link to={PATHS.ADMIN.PROFILE}>
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
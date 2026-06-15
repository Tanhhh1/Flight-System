import { useState, useEffect } from "react";
import { Link, Outlet, useLocation } from "react-router-dom";
import { clientPaths } from "@/configs/client_routes";
import { profileService } from "@/features/shared/profile/profile_service";
import { formatDate } from "@/utils/date_utils";
import "./profile_layout.css";

function ProfileLayout() {
    const location = useLocation();
    const [profile, setProfile] = useState(null);

    useEffect(() => { profileService.getProfile().then(({ data }) => { if (data.succeeded && data.result) { setProfile(data.result) } }) }, []);
    const isActive = (path) => { return location.pathname === `${clientPaths.profile.root}/${path}` };

    return (
        <div className="profile_container">
            <div className="profile_inner_layout">
                <aside className="profile_sidebar">
                    <div className="profile_user_card">
                        <div className="profile_avatar_circle">
                            {profile?.fullname?.charAt(0).toUpperCase() ?? "?"}
                        </div>
                        <div className="profile_user_info">
                            <h3 className="profile_fullname">{profile?.fullname ?? "..."}</h3>
                        </div>
                    </div>
                    <div className="profile_tier_badge">
                        <div className="tier_content">
                            <i className="bx bx-calendar tier_icon"></i>
                            <span>
                                Thành viên từ {profile ? formatDate(profile.createdAt) : "..."}
                            </span>
                        </div>
                    </div>
                    <nav className="profile_menu">
                        <div className="menu_section">
                            <Link to={`${clientPaths.profile.root}/${clientPaths.profile.verify}`} className={`menu_item ${isActive(clientPaths.profile.verify) ? "active" : ""}`}>
                                <i className="bx bx-trip"></i>
                                <span className="menu_label">Đặt chỗ ngồi</span>
                            </Link>
                            <Link to={`${clientPaths.profile.root}/${clientPaths.profile.transactions}`} className={`menu_item ${isActive(clientPaths.profile.transactions) ? "active" : ""}`}>
                                <i className="bx bx-receipt"></i>
                                <span className="menu_label">Danh sách giao dịch</span>
                            </Link>
                            <div className="menu_item">
                                <i className="bx bx-undo"></i>
                                <span className="menu_label">Hoàn vé / Đổi lịch</span>
                            </div>
                        </div>
                        <div className="menu_section">
                            <Link to={`${clientPaths.profile.root}/${clientPaths.profile.edit}`} className={`menu_item ${isActive(clientPaths.profile.edit) ? "active" : ""}`}>
                                <i className="bx bx-user-pin"></i>
                                <span className="menu_label">Tài khoản</span>
                            </Link>
                            <Link to={`${clientPaths.profile.root}/${clientPaths.profile.reviews}`} className={`menu_item ${isActive(clientPaths.profile.reviews) ? "active" : ""}`}>
                                <i className="bx bx-star"></i>
                                <span className="menu_label">Đánh giá hệ thống</span>
                            </Link>
                            <div className="menu_divider"></div>
                            <button className="menu_item logout_btn">
                                <i className="bx bx-power-off"></i>
                                <span className="menu_label">Đăng xuất</span>
                            </button>
                        </div>
                    </nav>
                </aside>
                <main className="profile_content">
                    <Outlet />
                </main>
            </div>
        </div>
    );
};

export default ProfileLayout;
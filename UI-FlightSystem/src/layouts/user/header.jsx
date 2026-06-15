import { useState } from "react";
import { Link } from "react-router-dom";
import { useSelector, useDispatch } from "react-redux";
import { clientPaths } from "@/configs/client_routes";
import { logout, openLoginModal } from "@/features/shared/auth/auth_slice";

function Header() {
  const dispatch = useDispatch();
  const user = useSelector((state) => state.auth?.user);
  const [menuOpen, setMenuOpen] = useState(false);
  const [userDropdownOpen, setUserDropdownOpen] = useState(false);

  const handleLogout = () => {
    dispatch(logout());
    setUserDropdownOpen(false);
  };

  const navLinks = [
    { icon: "bx bxs-plane-alt", label: "Vé máy bay", path: clientPaths.home },
    { icon: "bx bx-phone-call", label: "Liên hệ", path: clientPaths.contact },
    { icon: "bx bx-help-circle", label: "Trợ giúp", path: clientPaths.help },
    { icon: "bx bx-calendar", label: "Đặt chỗ của tôi", path: `${clientPaths.profile.root}/${clientPaths.profile.verify}` },
  ];

  return (
    <header className="header">
      <div className="header_top">
        <div className="header_top_inner">
          <Link to="/" className="header_brand">
            <i className="bx bx-paper-plane brand_icon"></i>
            <span className="brand_name">SkyJourney</span>
          </Link>

          <nav className={`header_nav ${menuOpen ? "nav_open" : ""}`}>
            {navLinks.map((link, i) => (
              <Link to={link.path} className="nav_link" key={i} onClick={() => setMenuOpen(false)}>
                <i className={link.icon}></i>
                <span>{link.label}</span>
              </Link>
            ))}
          </nav>

          <div className="header_actions">
            {user ? (
              <div className="user_menu" onClick={() => setUserDropdownOpen((v) => !v)}>
                <div className="user_avatar">
                  <i className="bx bx-user"></i>
                </div>
                <span className="user_name">{user.fullName || user.userName}</span>
                <i className={`bx bx-chevron-down chevron ${userDropdownOpen ? "rotated" : ""}`}></i>

                {userDropdownOpen && (
                  <div className="dropdown_menu user_dropdown">
                    <div className="dropdown_header user_header_bg">
                      <div className="dropdown_username">{user.fullName}</div>
                      <div className="dropdown_email">{user.email}</div>
                    </div>
                    <div className="dropdown_divider"></div>
                    <Link
                      to={`${clientPaths.profile.root}/${clientPaths.profile.edit}`}
                      className="dropdown_item"
                      onClick={() => setUserDropdownOpen(false)}
                    >
                      <i className="bx bx-edit"></i> Chỉnh sửa hồ sơ
                    </Link>
                    <Link
                      to={`${clientPaths.profile.root}/${clientPaths.profile.transactions}`}
                      className="dropdown_item"
                      onClick={() => setUserDropdownOpen(false)}
                    >
                      <i className="bx bx-list-ul"></i> Danh sách giao dịch
                    </Link>
                    <Link
                      to={`${clientPaths.profile.root}/${clientPaths.profile.reviews}`}
                      className="dropdown_item"
                      onClick={() => setUserDropdownOpen(false)}
                    >
                      <i className="bx bx-star"></i> Đánh giá của tôi
                    </Link>
                    <a href="#" className="dropdown_item">
                      <i className="bx bx-bell"></i> Thông báo giá vé
                    </a>
                    <div className="dropdown_divider"></div>
                    <button className="dropdown_item dropdown_logout" onClick={handleLogout}>
                      <i className="bx bx-log-out"></i> Đăng xuất
                    </button>
                  </div>
                )}
              </div>
            ) : (
              <button className="btn_signin" onClick={() => dispatch(openLoginModal())}>
                <i className="bx bx-log-in-circle"></i>
                <span>Đăng nhập</span>
              </button>
            )}

            <button className="btn_profile" onClick={() => setMenuOpen((v) => !v)}>
              <i className={`bx ${menuOpen ? "bx-x" : "bx-menu"}`}></i>
            </button>
          </div>
        </div>
      </div>
    </header>
  );
}

export default Header;
import { useState } from "react";
import { Link } from "react-router-dom";
import { clientPaths } from "@/configs/client_routes";

const Header = ({ onOpenLogin }) => {
  const [menuOpen, setMenuOpen] = useState(false);
  const [userDropdownOpen, setUserDropdownOpen] = useState(false);

  const navLinks = [
    { icon: "bx bxs-plane-alt", label: "Vé máy bay", path: "/" },
    { icon: "bx bx-phone-call", label: "Liên hệ", path: "/contact" },
    { icon: "bx bx-help-circle", label: "Trợ giúp", path: "/help" },
    { icon: "bx bx-calendar", label: "Đặt chỗ của tôi", path: "/my-bookings" },
  ];

  return (
    <header className="header">
      <div className="header-top">
        <div className="header-top-inner">
          <Link to="/" className="header-brand" style={{ textDecoration: 'none' }}>
            <i className="bx bx-paper-plane brand-icon"></i>
            <span className="brand-name">SkyJourney</span>
          </Link>

          <nav className={`header-nav ${menuOpen ? "nav-open" : ""}`}>
            {navLinks.map((link, i) => (
              <Link to={link.path} className="nav-link" key={i} onClick={() => setMenuOpen(false)}>
                <i className={link.icon}></i>
                <span>{link.label}</span>
              </Link>
            ))}
          </nav>

          <div className="header-actions">
            <div className="user-menu" onClick={() => setUserDropdownOpen(!userDropdownOpen)}>
              <div className="user-avatar">
                <i className="bx bx-user"></i>
              </div>
              <span className="user-name">Tuan</span>
              <i className={`bx bx-chevron-down chevron ${userDropdownOpen ? "rotated" : ""}`}></i>

              {userDropdownOpen && (
                <div className="dropdown-menu user-dropdown">
                  <div className="dropdown-header user-header-bg">
                    <div className="dropdown-username">Tuan Anh</div>
                    <div className="dropdown-email">tuananh@example.com</div>
                  </div>
                  <div className="dropdown-divider"></div>
                  <Link to={`${clientPaths.profile.root}/${clientPaths.profile.edit}`} className="dropdown-item" onClick={() => setUserDropdownOpen(false)}>
                    <i className="bx bx-edit"></i> Chỉnh sửa hồ sơ
                  </Link>
                  <Link to={`${clientPaths.profile.root}/${clientPaths.profile.transactions}`} className="dropdown-item" onClick={() => setUserDropdownOpen(false)}>
                    <i className="bx bx-list-ul"></i> Danh sách giao dịch
                  </Link>
                  <Link to={clientPaths.myTickets} className="dropdown-item" onClick={() => setUserDropdownOpen(false)}>
                    <i className="bx bx-calendar"></i> Đặt chỗ của tôi
                  </Link>
                  <a href="#" className="dropdown-item"><i className="bx bx-bell"></i> Thông báo giá vé</a>
                  <div className="dropdown-divider"></div>
                  <a href="#" className="dropdown-item dropdown-logout"><i className="bx bx-log-out"></i> Đăng xuất</a>
                </div>
              )}
            </div>

            <button className="btn-signin" onClick={onOpenLogin}>
              <i className="bx bx-log-in-circle"></i>
              <span>Đăng nhập</span>
            </button>

            <button className="btn-hamburger" onClick={() => setMenuOpen(!menuOpen)}>
              <i className={`bx ${menuOpen ? "bx-x" : "bx-menu"}`}></i>
            </button>
          </div>
        </div>
      </div>
    </header>
  );
};

export default Header;
function Footer() {
  const airlines = [
    "Vietnam Airlines", "Bamboo Airways", "VietJet Air",
    "Pacific Airlines", "Vietravel Airlines", "Scoot",
  ];

  const popularRoutes = [
    "Hà Nội → TP.HCM", "TP.HCM → Đà Nẵng", "Hà Nội → Đà Nẵng",
    "TP.HCM → Phú Quốc", "Hà Nội → Nha Trang", "TP.HCM → Hà Nội",
  ];

  return (
    <footer className="footer">
      <div className="footer_newsletter">
        <div className="newsletter_inner">
          <div className="newsletter_text">
            <i className="bx bx-bell-ring newsletter_icon"></i>
            <div>
              <h3>Nhận thông báo giá vé tốt nhất</h3>
              <p>Đăng ký để không bỏ lỡ ưu đãi hàng ngày</p>
            </div>
          </div>
          <div className="newsletter_form">
            <input type="email" placeholder="Nhập email của bạn..." className="newsletter_input" />
            <button className="newsletter_btn">
              <i className="bx bx-send"></i> Đăng ký
            </button>
          </div>
        </div>
      </div>

      <div className="footer_main">
        <div className="footer_inner">
          <div className="footer_brand_col">
            <div className="footer_brand">
              <i className="bx bx-paper-plane footer_brand_icon"></i>
              <span>SkyJourney</span>
            </div>
            <p className="footer_tagline">Đặt vé máy bay nhanh nhất, giá tốt nhất</p>
            <div className="footer_certs">
              <div className="cert_badge"><i className="bx bx-shield-quarter"></i> IATA</div>
              <div className="cert_badge"><i className="bx bx-check-shield"></i> Đã đăng ký BCT</div>
            </div>
            <div className="footer_socials">
              <a href="#" className="social_link facebook"><i className="bx bxl-facebook"></i></a>
              <a href="#" className="social_link instagram"><i className="bx bxl-instagram"></i></a>
              <a href="#" className="social_link tiktok"><i className="bx bxl-tiktok"></i></a>
              <a href="#" className="social_link youtube"><i className="bx bxl-youtube"></i></a>
            </div>
          </div>

          <div className="footer_links_col">
            <h4 className="footer_col_title">Về SkyJourney</h4>
            <ul className="footer_link_list">
              <li><a href="#"><i className="bx bx-chevron-right"></i> Cách đặt vé</a></li>
              <li><a href="#"><i className="bx bx-chevron-right"></i> Liên hệ chúng tôi</a></li>
              <li><a href="#"><i className="bx bx-chevron-right"></i> Trợ giúp & FAQ</a></li>
              <li><a href="#"><i className="bx bx-chevron-right"></i> Về chúng tôi</a></li>
              <li><a href="#"><i className="bx bx-chevron-right"></i> Điều khoản & Điều kiện</a></li>
            </ul>
          </div>

          <div className="footer_links_col">
            <h4 className="footer_col_title">Hãng đối tác</h4>
            <ul className="footer_link_list">
              {airlines.map((airline, i) => (
                <li key={i}><a href="#"><i className="bx bx-chevron-right"></i> {airline}</a></li>
              ))}
            </ul>
          </div>

          <div className="footer_links_col">
            <h4 className="footer_col_title">Đường bay phổ biến</h4>
            <ul className="footer_link_list">
              {popularRoutes.map((route, i) => (
                <li key={i}>
                  <a href="#">
                    <i className="bx bx-right-arrow-alt"></i> {route}
                  </a>
                </li>
              ))}
            </ul>
          </div>
        </div>
      </div>

      <div className="footer_bottom">
        <div className="footer_bottom_inner">
          <p>© 2026 SkyJourney. All rights reserved.</p>
          <div className="footer_bottom_links">
            <a href="#">Chính sách bảo mật</a>
            <span>·</span>
            <a href="#">Điều khoản sử dụng</a>
            <span>·</span>
            <a href="#">Sitemap</a>
          </div>
        </div>
      </div>
    </footer>
  );
};

export default Footer;
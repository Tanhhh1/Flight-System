import { useState } from "react";
import "./contact.css";

function Contact() {
  const [formData, setFormData] = useState({
    name: "",
    email: "",
    subject: "",
    message: "",
  });
  const [isSubmitted, setIsSubmitted] = useState(false);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    console.log("Dữ liệu liên hệ gửi đi:", formData);
    setIsSubmitted(true);
    setFormData({ name: "", email: "", subject: "", message: "" });
    setTimeout(() => setIsSubmitted(false), 5000);
  };

  return (
    <div className="contact_container">
      <section className="contact_hero">
        <div className="contact_hero_overlay"></div>
        <div className="contact_hero_content">
          <h1>Liên Hệ Với Chúng Tôi</h1>
          <p>SkyJourney luôn sẵn sàng lắng nghe và hỗ trợ bạn mọi lúc, mọi nơi</p>
        </div>
      </section>
      <section className="contact_main_content">
        <div className="contact_grid">
          <div className="contact_info_column">
            <h2>Thông Tin Liên Hệ</h2>
            <p className="contact_description">
              Bạn có thắc mắc về vé máy bay, thủ tục hoàn đổi hoặc cần hỗ trợ đặc biệt? 
              Hãy liên hệ với chúng tôi qua các kênh dưới đây hoặc để lại lời nhắn.
            </p>
            <div className="info_cards_list">
              <div className="info_card">
                <div className="info_icon_box">
                  <i className="bx bx-phone-call"></i>
                </div>
                <div className="info_text">
                  <h3>Tổng đài 24/7</h3>
                  <p className="highlight_text">1900 6000</p>
                  <p>Hỗ trợ khẩn cấp: +84 28 3930 0000</p>
                </div>
              </div>
              <div className="info_card">
                <div className="info_icon_box">
                  <i className="bx bx-envelope"></i>
                </div>
                <div className="info_text">
                  <h3>Email Hỗ Trợ</h3>
                  <p>support@skyjourney.com</p>
                  <p>corporate@skyjourney.com (Doanh nghiệp)</p>
                </div>
              </div>
              <div className="info_card">
                <div className="info_icon_box">
                  <i className="bx bx-map"></i>
                </div>
                <div className="info_text">
                  <h3>Trụ Sở Chính</h3>
                  <p>Tòa nhà Sky Tower, 123 Đường Trường Sơn, Phường 2, Quận Tân Bình, TP. Hồ Chí Minh, Việt Nam</p>
                </div>
              </div>
            </div>
            <div className="contact_socials">
              <h3>Kết nối với SkyJourney</h3>
              <div className="social_icons">
                <a href="#" className="social_link facebook"><i className="bx bxl-facebook"></i></a>
                <a href="#" className="social_link instagram"><i className="bx bxl-instagram"></i></a>
                <a href="#" className="social_link youtube"><i className="bx bxl-youtube"></i></a>
                <a href="#" className="social_link linkedin"><i className="bx bxl-linkedin"></i></a>
              </div>
            </div>
          </div>
          <div className="contact_form_column">
            <h2>Gửi Lời Nhắn Cho SkyJourney</h2>
            <p>Chúng tôi sẽ phản hồi email của bạn trong vòng 24 giờ làm việc.</p>
            {isSubmitted && (
              <div className="submit_success_alert">
                <i className="bx bx-check-circle"></i>
                <span>Cảm ơn bạn! Lời nhắn đã được gửi đi thành công.</span>
              </div>
            )}
            <form onSubmit={handleSubmit} className="contact_form">
              <div className="form_group_row">
                <div className="form_group">
                  <label htmlFor="name">Họ và tên</label>
                  <input value={formData.name} onChange={handleInputChange} placeholder="Nguyễn Văn A" />
                </div>
                <div className="form_group">
                  <label htmlFor="email">Địa chỉ Email</label>
                  <input value={formData.email} onChange={handleInputChange} placeholder="email@example.com" />
                </div>
              </div>

              <div className="form_group">
                <label htmlFor="subject">Chủ đề cần hỗ trợ</label>
                <input value={formData.subject} onChange={handleInputChange} placeholder="Ví dụ: Hoàn vé, Sai thông tin cá nhân..." />
              </div>

              <div className="form_group">
                <label htmlFor="message">Nội dung chi tiết</label>
                <textarea rows="6" value={formData.message} onChange={handleInputChange} placeholder="Nhập nội dung câu hỏi hoặc yêu cầu của bạn tại đây..."></textarea>
              </div>
              <button type="submit" className="btn_send_message">
                <i className="bx bx-paper-plane"></i> Gửi Yêu Cầu Hỗ Trợ
              </button>
            </form>
          </div>
        </div>
      </section>
    </div>
  );
};

export default Contact;
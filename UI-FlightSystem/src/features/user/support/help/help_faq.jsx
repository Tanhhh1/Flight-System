import { useState } from "react";
import "./help_faq.css";

function HelpFaq() {
  const [activeCategory, setActiveCategory] = useState("booking");
  const [expandedFaq, setExpandedFaq] = useState(null);

  const categories = [
    { id: "booking", name: "Đặt vé & Chỗ ngồi", icon: "bx bx-book-bookmark" },
    { id: "payment", name: "Thanh toán & Hóa đơn", icon: "bx bx-credit-card" },
    { id: "refund", name: "Hoàn vé & Thay đổi", icon: "bx bx-refresh" },
    { id: "luggage", name: "Hành lý & Quy định", icon: "bx bx-briefcase" },
  ];

  const faqData = {
    booking: [
      { id: "b1", q: "Làm thế nào để tôi đặt vé máy bay Nhiều thành phố?", a: "Tại khung tìm kiếm ở trang chủ, bạn bấm chọn tab 'Nhiều thành phố'. Hệ thống sẽ cho phép bạn thêm tối đa 5 chặng bay khác nhau bằng cách nhấn vào nút 'Thêm chuyến bay khác' dưới chân form." },
      { id: "b2", q: "Tôi có được chọn trước chỗ ngồi khi đặt vé trực tuyến không?", a: "Có. Sau bước điền thông tin hành khách tại trang đặt vé, hệ thống SkyJourney sẽ chuyển bạn tới sơ đồ khoang máy bay thực tế để bạn tự do lựa chọn ghế ngồi (Cửa sổ, lối đi hoặc ghế rộng chân)." },
      { id: "b3", q: "Làm sao để biết tôi đã đặt chỗ thành công hay chưa?", a: "Sau khi hoàn tất đặt vé, hệ thống sẽ gửi mã đặt chỗ (PNR) cùng thông tin chi tiết hành trình qua Email và SMS. Bạn cũng có thể kiểm tra trực tiếp tại mục 'Lịch sử giao dịch' trong hồ sơ cá nhân của mình." }
    ],
    payment: [
      { id: "p1", q: "SkyJourney hỗ trợ những phương thức thanh toán nào?", a: "Chúng tôi hỗ trợ đa dạng cổng thanh toán bao gồm: Thẻ ATM nội địa, quét mã VietQR (NAPAS), thẻ quốc tế (Visa/Mastercard/JCB) và Ví điện tử MoMo." },
      { id: "p2", q: "Thời hạn thanh toán vé máy bay sau khi giữ chỗ là bao lâu?", a: "Tùy thuộc vào hãng hàng không và thời gian cách giờ khởi hành, thời gian giữ chỗ thông thường kéo dài từ 30 phút đến 12 tiếng. Sau thời gian này nếu chưa thanh toán, hệ thống sẽ tự động hủy giao dịch." }
    ],
    refund: [
      { id: "r1", q: "Tôi muốn thay đổi ngày bay hoặc giờ bay thì phải làm thế nào?", a: "Bạn có thể vào mục Hồ sơ cá nhân > Quản lý đặt vé, chọn đơn hàng cần đổi và gửi yêu cầu Đổi lịch bay, hoặc liên hệ trực tiếp đến tổng đài 1900 6000 trước giờ khởi hành tối thiểu 3 tiếng." },
      { id: "r2", q: "Quy định hoàn tiền và phí hủy vé được tính như thế nào?", a: "Phí hoàn vé phụ thuộc vào điều kiện hạng vé bạn đã mua. Tiền hoàn lại sau khi trừ phí (nếu có) sẽ được chuyển về đúng phương thức thanh toán ban đầu của bạn trong vòng 3 - 7 ngày làm việc." }
    ],
    luggage: [
      { id: "l1", q: "Quy định về trọng lượng và kích thước hành lý xách tay?", a: "Hành khách tiêu chuẩn được phép mang theo 1 kiện hành lý xách tay và 1 phụ kiện cá nhân với tổng trọng lượng không quá 7kg (hoặc 10kg tùy hãng hàng không) và kích thước không vượt quá 56cm x 36cm x 23cm." },
      { id: "l2", q: "Tôi có thể mua thêm hành lý ký gửi sau khi đã xuất vé không?", a: "Có. Bạn có thể mua bổ sung hành lý ký gửi thông qua trang web SkyJourney muộn nhất là 3 tiếng trước giờ bay để nhận mức giá ưu đãi hơn so với mua trực tiếp tại quầy check-in sân bay." }
    ]
  };

  const toggleFaq = (id) => {
    setExpandedFaq(expandedFaq === id ? null : id);
  };

  return (
    <div className="faq_container">
      <section className="faq_search_banner">
        <div className="faq_banner_overlay"></div>
        <div className="faq_banner_content">
          <h1>Trung Tâm Trợ Giúp & FAQ</h1>
          <p>Tìm kiếm câu trả lời nhanh chóng cho các thắc mắc về hành trình của bạn</p>
        </div>
      </section>

      <section className="faq_main_content">
        <div className="faq_grid">
          <div className="category_sidebar">
            {categories.map((cat) => (
              <button key={cat.id} type="button" className={`category_btn ${activeCategory === cat.id ? "active" : ""}`} onClick={() => { setActiveCategory(cat.id); setExpandedFaq(null); }}>
                <i className={cat.icon}></i>
                <span>{cat.name}</span>
              </button>
            ))}

            <div className="faq_support_box">
              <h4>Không tìm thấy câu trả lời?</h4>
              <p>Đội ngũ hỗ trợ của chúng tôi luôn trực tuyến để giúp đỡ bạn.</p>
              <a href="/contact" className="faq_link_contact">Liên hệ ngay</a>
            </div>
          </div>

          <div className="faq_questions_list">
            <h2 className="category_title">
              {categories.find((c) => c.id === activeCategory)?.name}
            </h2>
            <div className="accordion_wrapper">
              {faqData[activeCategory].map((faq) => {
                const isExpanded = expandedFaq === faq.id;
                return (
                  <div key={faq.id} className={`accordion_item ${isExpanded ? "open" : ""}`}>
                    <button type="button" className="accordion_header" onClick={() => toggleFaq(faq.id)}>
                      <span className="question_text">{faq.q}</span>
                      <i className={`bx ${isExpanded ? "bx-minus" : "bx-plus"} status_icon`}></i>
                    </button>
                    <div className="accordion_body">
                      <div className="accordion_content_inner">
                        <p>{faq.a}</p>
                      </div>
                    </div>
                  </div>
                );
              })}
            </div>
          </div>
        </div>
      </section>
    </div>
  );
}

export default HelpFaq;
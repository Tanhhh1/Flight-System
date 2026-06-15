export const FEATURES = [
    { icon: "bx bx-check-shield", title: "Bảo mật thanh toán", desc: "Tích hợp các cổng thanh toán hàng đầu, an toàn tuyệt đối." },
    { icon: "bx bx-dollar-circle", title: "Giá tốt cạnh tranh", desc: "Hệ thống tự động quét và so sánh giá vé từ mọi hãng hàng không." },
    { icon: "bx bx-support", title: "Hỗ trợ 24/7", desc: "Đội ngũ tổng đài viên sẵn sàng xử lý sự cố hoàn/đổi vé bất cứ lúc nào." },
];

export const SEAT_CLASS_ID_MAP = {
    "Economy Class": 1,
    "Premium Economy": 2,
    "Business Class": 3,
    "First Class": 4,
};

export const DEFAULT_SINGLE_FLIGHT = { from: "", to: "", departureDate: "", returnDate: "" };

export const DEFAULT_MULTI_FLIGHTS = [
    { id: 1, from: "", to: "", departureDate: "" },
    { id: 2, from: "", to: "", departureDate: "" },
];

export const MAX_MULTI_FLIGHTS = 5;
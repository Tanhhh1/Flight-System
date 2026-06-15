export const VNPAY_METHOD = {
    DomesticCard: 1,
    EWallet: 2,
    InternationalCard: 3,
};

export const PAYMENT_METHOD_OPTIONS = [
    {
        id: VNPAY_METHOD.DomesticCard,
        title: "Thẻ nội địa",
        desc: "Thanh toán bằng thẻ ATM nội địa qua cổng VNPay.",
        icon: "bx bx-credit-card-alt",
    },
    {
        id: VNPAY_METHOD.EWallet,
        title: "Ví điện tử",
        desc: "Thanh toán bằng ví điện tử hoặc QR code qua ứng dụng ngân hàng.",
        icon: "bx bx-qr-scan",
    },
    {
        id: VNPAY_METHOD.InternationalCard,
        title: "Thẻ quốc tế",
        desc: "Áp dụng cho thẻ tín dụng, thẻ ghi nợ quốc tế Visa, Mastercard, JCB.",
        icon: "bx bx-globe",
    },
];
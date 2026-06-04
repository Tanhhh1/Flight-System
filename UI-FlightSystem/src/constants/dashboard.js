const CURRENT_YEAR = new Date().getFullYear();

export const DASHBOARD_YEAR_OPTIONS = [
    { value: CURRENT_YEAR, label: `Năm ${CURRENT_YEAR}` },
    { value: CURRENT_YEAR - 1, label: `Năm ${CURRENT_YEAR - 1}` },
    { value: CURRENT_YEAR - 2, label: `Năm ${CURRENT_YEAR - 2}` },
];

export const DASHBOARD_CHART_LABELS = [
    "Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4",
    "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8",
    "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12",
];

export const DASHBOARD_TABLE_HEADS = [
    "STT", "Mã đặt vé", "Khách hàng", "Hạng ghế",
    "Ngày đặt", "Loại chuyến", "Tổng tiền", "Trạng thái", "",
];

export const DASHBOARD_CARDS = [
    {
        key: "activeFlights",
        label: "Chuyến bay hoạt động",
        icon: "bx bxs-plane-take-off",
        color: "green",
        format: (v) => v?.toLocaleString("vi-VN") ?? "0",
    },
    {
        key: "ticketsSoldThisMonth",
        label: "Vé đã bán (Tháng)",
        icon: "bx bxs-coupon",
        color: "orange",
        format: (v) => v?.toLocaleString("vi-VN") ?? "0",
    },
    {
        key: "newMembersThisMonth",
        label: "Thành viên mới",
        icon: "bx bxs-user-voice",
        color: "red",
        format: (v) => v?.toLocaleString("vi-VN") ?? "0",
    },
    {
        key: "revenueThisMonth",
        label: "Doanh thu tháng này",
        icon: "bx bxs-bank",
        color: "blue",
        format: (v) => v ? v.toLocaleString("vi-VN") + " ₫" : "0 ₫",
    },
];
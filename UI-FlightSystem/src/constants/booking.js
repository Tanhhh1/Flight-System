export const TRIP_TYPE_OPTIONS = [
    { value: "", label: "Tất cả loại chuyến" },
    { value: "1", label: "Một chiều" },
    { value: "2", label: "Khứ hồi" },
    { value: "3", label: "Nhiều chặng" },
];

export const BOOKING_STATUS_LABEL = {
    Pending: "Chờ xác nhận",
    Confirmed: "Đã xác nhận",
    Cancelled: "Đã hủy",
    Expired: "Hết hạn",
    Failed: "Thất bại",
};

export const PASSENGER_TYPE_LABEL = {
    1: "Người lớn",
    2: "Trẻ em",
    3: "Trẻ sơ sinh",
};

export const BOOKING_TABLE_HEADS = [
    "#", "Mã booking", "Khách hàng", "Hạng ghế",
    "Loại chuyến", "Ngày đặt", "Tổng tiền", "Trạng thái", ""
];

export const PASSENGER_TABLE_HEADS = [
    "#", "Họ và tên", "Đối tượng", "Giới tính",
    "Ngày sinh", "Quốc tịch", "Giá vé"
];

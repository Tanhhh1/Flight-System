export const TRIP_TYPE_OPTIONS = [
    { value: "", label: "Tất cả loại chuyến" },
    { value: "1", label: "Một chiều" },
    { value: "2", label: "Khứ hồi" },
    { value: "3", label: "Nhiều chặng" },
];

export const BOOKING_STATUS_LABEL = {
    Pending: "Chờ thanh toán",
    Confirmed: "Đã thanh toán",
    Cancelled: "Đã hủy",
    Expired: "Hết hạn",
    Failed: "Thất bại",
};

export const TRIP_TYPE_LABEL = {
    "OneWay": "Một chiều",
    "RoundTrip": "Khứ hồi",
    "MultiCity": "Nhiều chặng"
};

export const SEAT_CLASSES_NAMES = {
    "Economy Class": "Phổ thông",
    "Premium Economy": "Phổ thông đặc biệt",
    "Business Class": "Thương gia" ,
    "First Class": "Hạng nhất",
};

export const PASSENGER_TYPE_LABEL = {
    1: "Người lớn",
    2: "Trẻ em",
    3: "Trẻ sơ sinh",
};

export const BOOKING_TABLE_HEADS = [
    "STT", "Mã đặt vé", "Khách hàng", "Hạng ghế",
    "Loại chuyến", "Ngày đặt", "Tổng tiền", "Trạng thái", "Hành động"
];

export const PASSENGER_TABLE_HEADS = [
    "STT", "Họ và tên", "Đối tượng", "Giới tính",
    "Ngày sinh", "Quốc tịch", "Giá vé"
];


export const TRIP_TYPE_MAP = {
    "one-way": 1,
    "round-trip": 2,
    "multi-city": 3,
};

export const DEFAULT_PASSENGER = {
    typeId: null,
    fullName: "",
    gender: "Nam",
    dob: "",
    nationality: "Việt Nam",
};

export const SEAT_CLASS_OPTIONS = {
    "Economy Class": "Phổ thông" ,
    "Premium Economy": "Phổ thông đặc biệt" ,
    "Business Class": "Thương gia",
    "First Class": "Hạng nhất",
};

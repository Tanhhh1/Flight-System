export const TABLE_HEADS = ["STT", "Điểm đi", "Điểm đến", "Thời gian", "Hãng / Máy bay", "Thời lượng", "Trạng thái", "Hành động"];

export const FLIGHT_STATUS_LABEL = {
    Active: "Hoạt động",
    Inactive: "Ngừng hoạt động",
    Delayed: "Bị trễ",
    Cancelled: "Đã hủy",
    Completed: "Đã hoàn thành",
};

export const FLIGHT_STATUS_FILTER = [
    { value: "Active", label: "Hoạt động" },
    { value: "Inactive", label: "Ngừng hoạt động" },
    { value: "Delayed", label: "Bị trễ" },
    { value: "Cancelled", label: "Đã hủy" },
    { value: "Completed", label: "Đã hoàn thành" },
];

export const EDIT_STATUS_OPTIONS = [
    { value: "Active", label: "Hoạt động" },
    { value: "Delayed", label: "Bị trễ" },
    { value: "Cancelled", label: "Đã hủy" },
];

export const SEAT_CLASSES = [
    { classId: 1, className: "Phổ thông" },
    { classId: 2, className: "Phổ thông đặc biệt" },
    { classId: 3, className: "Thương gia" },
    { classId: 4, className: "Hạng nhất" },
];

export const COMMON_RULES = {
    planeId: { required: "Máy bay không được để trống" },
    routeId: { required: "Tuyến bay không được để trống" },
    departureTime: { required: "Vui lòng nhập giờ khởi hành" },
};

export const DEFAULT_SEAT_PRICES = SEAT_CLASSES.map((c) => ({ classId: c.classId, price: "" }));

export const FLIGHT_CREATE_DEFAULT_VALUES = {
    planeId: "",
    routeId: "",
    departureTime: "",
    isRefund: false,
    isChange: false,
    status: "",
    segments: [],
    seatPrices: DEFAULT_SEAT_PRICES,
    serviceIds: [],
};

export const ROUTE_STATUS_LABEL = {
    Inactive: "Ngừng hoạt động",
    Active: "Hoạt động",
    Suspended: "Tạm ngưng",
};

export const ROUTE_STATUS_OPTIONS = [
    { value: "Active", label: "Hoạt động" },
    { value: "Suspended", label: "Tạm ngưng" },
];

export const TABLE_HEADS = [
    "STT", "Điểm đi", "Điểm đến", "Thời gian bay",
    "Trạng thái", "Ngày tạo", "Hành động"
];

export const CREATE_DEFAULT_VALUES = {
    originAirportId: "",
    destinationAirportId: "",
    flightDuration: "",
};

export const EDIT_DEFAULT_VALUES = {
    originAirportId: "",
    destinationAirportId: "",
    flightDuration: "",
    status: "Active",
};

export const BE_FIELD_MAP = {
    OriginAirportId: "originAirportId",
    DestinationAirportId: "destinationAirportId",
    FlightDuration: "flightDuration",
    Status: "status",
};

export const COMMON_RULES = {
    originAirportId: { required: "Vui lòng chọn sân bay khởi hành" },
    destinationAirportId: { required: "Vui lòng chọn sân bay đến" },
    flightDuration: { required: "Vui lòng nhập thời gian bay" },
};
export const AIRLINE_STATUS_LABEL = {
    Active: "Hoạt động",
    Suspended: "Tạm ngưng",
    Inactive: "Ngừng hoạt động"
};

export const TABLE_HEADS = [
    "STT", "Mã hãng", "Tên hãng hàng không", "Quốc gia",
    "Trạng thái", "Ngày tạo", "Hành động"
];

export const CREATE_DEFAULT_VALUES = {
    airlineName: "",
    airlineCode: "",
    country: "",
};

export const EDIT_DEFAULT_VALUES = {
    airlineName: "",
    airlineCode: "",
    country: "",
    status: "Active",
};

export const BE_FIELD_MAP = {
    AirlineName: "airlineName",
    AirlineCode: "airlineCode",
    Country: "country",
    Status: "status",
};
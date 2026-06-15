export const TABLE_HEADS = [
    "STT", "Mã sân bay", "Tên sân bay",
    "Thành phố", "Quốc gia", "Trạng thái", "Ngày tạo", "Hành động"
];

export const AIRPORT_STATUS_LABEL = {
    Active: "Hoạt động",
    Suspended: "Tạm ngưng",
    Inactive: "Ngừng hoạt động"
};

export const CREATE_DEFAULT_VALUES = {
    airportCode: "",
    airportName: "",
    city: "",
    country: "",
};

export const EDIT_DEFAULT_VALUES = {
    airportCode: "",
    airportName: "",
    city: "",
    country: "",
    status: "Active",
};

export const BE_FIELD_MAP = {
    AirportCode: "airportCode",
    AirportName: "airportName",
    City: "city",
    Country: "country",
};

export const COMMON_RULES = {
    airportCode: { required: "Mã sân bay không được để trống" },
    airportName: { required: "Tên sân bay không được để trống" },
    city: { required: "Thành phố không được để trống" },
    country: { required: "Quốc gia không được để trống" },
};

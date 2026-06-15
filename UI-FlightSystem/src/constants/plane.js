export const TABLE_HEADS = [
    "STT", "Tên máy bay", "Hãng hàng không",
    "Trạng thái", "Ngày tạo", "Hành động"
];

export const PLANE_STATUS_LABEL = {
    Active: "Hoạt động",
    Inactive: "Ngừng hoạt động",
    Delayed: "Bảo trì",
};

export const CREATE_DEFAULT_VALUES = {
    planeModel: "",
    airlineId: "",
};

export const EDIT_DEFAULT_VALUES = {
    planeModel: "",
    airlineId: "",
    status: "Active",
};

export const BE_FIELD_MAP = {
    PlaneModel: "planeModel",
    AirlineId: "airlineId",
};

export const COMMON_RULES = {
    planeModel: { required: "Máy bay không được để trống" },
    airlineId: { required: "Hãng hàng không không được để trống" },
};
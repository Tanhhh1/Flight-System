export const TABLE_HEADS = [
    "STT", "Tên dịch vụ", "Mô tả",
    "Trạng thái", "Ngày tạo", "Hành động"
];

export const CREATE_DEFAULT_VALUES = {
    serviceName: "", description: "", isActive: true
};

export const EDIT_DEFAULT_VALUES = {
    serviceName: "", description: "", isActive: true,
};

export const BE_FIELD_MAP = {
    ServiceName: "serviceName",
    Description: "description",
};

export const COMMON_RULES = {
    serviceName: { required: "Tên dịch vụ không được để trống" },
    description: { maxLength: { value: 1000, message: "Mô tả không được vượt quá 1000 ký tự" } }
};
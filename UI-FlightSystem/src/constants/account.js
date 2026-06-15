export const ROLE_LABEL = {
    admin: "Quản trị viên",
    staff: "Nhân viên",
    user: "Người dùng"
};

export const TABLE_HEADS = [
    "STT", "Tên đăng nhập", "Email", "Họ tên",
    "Vai trò", "Trạng thái", "Ngày tạo", "Hành động"
];

export const CREATE_DEFAULT_VALUES = {
    userName: "", fullname: "", email: "",
    phoneNumber: "", gender: "Nam", birthday: "",
    address: "", roleNames: "staff",
    password: "", confirmPassword: "",
};

export const EDIT_DEFAULT_VALUES = {
    userName: "", fullname: "", email: "",
    phoneNumber: "", gender: "Nam", birthday: "",
    address: "", roleNames: "staff", isActive: true,
};

export const BE_FIELD_MAP = {
    UserName: "userName", Email: "email",
    Fullname: "fullname", PhoneNumber: "phoneNumber",
    Gender: "gender", RoleNames: "roleNames", Password: "password",
};

export const COMMON_RULES = {
    email: {required: "Địa chỉ Email không được để trống" },
    fullname: { required: "Họ và tên không được để trống" },
};

export const CREATE_ONLY_RULES = {
    userName: { required: "Tên đăng nhập không được để trống" },
    password: { required: "Mật khẩu không được để trống" },
};
export const DEFAULT_VALUES_PW = {
    currentPassword: "",
    newPassword: "",
    confirmNewPassword: "",
};

export const DEFAULT_VALUES_INFO = {
    userName: "",
    email: "",
    fullname: "",
    phoneNumber: "",
    address: "",
    gender: "",
    birthday: "",
};

export const PROFILE_MESSAGES = {
    UPDATE_SUCCESS: "Cập nhật thông tin thành công!",
    CHANGE_PASSWORD_SUCCESS: "Đổi mật khẩu thành công, vui lòng đăng nhập lại!",
    PASSWORD_MISMATCH: "Xác nhận mật khẩu không khớp",
};

export const CHANGE_PASSWORD_RULES = {
    currentPassword: { required: "Mật khẩu hiện tại không được để trống" },
    newPassword: { required: "Mật khẩu mới không được để trống" },
    confirmNewPassword: { required: "Vui lòng xác nhận mật khẩu mới" },
};

export const REQUEST_TYPES = [
    { value: "Refund", label: "Hoàn vé" },
    { value: "Reschedule", label: "Đổi lịch bay" },
];

export const ROLES = {
    ADMIN: "admin",
    STAFF: "staff",
    USER: "user",
};

export const ADMIN_ROLES = [ROLES.ADMIN, ROLES.STAFF];
export const CLIENT_ROLES = [ROLES.USER];

export const isAdminRole = (roles = []) => roles.some((r) => ADMIN_ROLES.includes(r));
export const isClientRole = (roles = []) => roles.some((r) => CLIENT_ROLES.includes(r));

export const AUTH_MESSAGES = {
    LOGIN_FAILED: "Đăng nhập thất bại.",
    REGISTER_FAILED: "Đăng ký thất bại.",
    SERVER_ERROR: "Lỗi kết nối server.",
    SESSION_EXPIRED: "Phiên đăng nhập hết hạn, vui lòng đăng nhập lại.",
    ADMIN_ACCESS_DENIED: "Tài khoản không có quyền truy cập hệ thống quản trị.",
    PASSWORD_MISMATCH: "Mật khẩu xác nhận không khớp!",
};

export const LOGIN_VALIDATION_RULES = {
    username: { required: "Vui lòng nhập tên đăng nhập." },
    password: { required: "Vui lòng nhập mật khẩu." },
};

export const REGISTER_VALIDATION_RULES = {
    fullName: { required: "Vui lòng nhập họ và tên" },
    userName: { required: "Vui lòng nhập tên đăng nhập" },
    email: { required: "Vui lòng nhập địa chỉ email" },
    password: { required: "Vui lòng nhập mật khẩu" },
    confirmPassword: { required: "Vui lòng xác nhận lại mật khẩu" },
};
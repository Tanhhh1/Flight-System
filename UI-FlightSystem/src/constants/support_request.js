export const REQUEST_TYPE_LABEL = {
    Refund: "Hoàn vé",
    Reschedule: "Đổi lịch",
};

export const SUPPORT_STATUS_LABEL = {
    Pending: "Chờ xử lý",
    Approved: "Đã duyệt",
    Rejected: "Từ chối",
};

export const SUPPORT_STATUS_CLASS = {
    Pending: "status_pending",
    Approved: "status_confirmed",
    Rejected: "status_cancelled",
};

export const SUPPORT_STATUS_ICON = {
    Pending: "bx-time-five",
    Approved: "bx-check-circle",
    Rejected: "bx-x-circle",
};

export const REQUEST_TYPE_OPTIONS = [
    { value: "Refund", label: "Hoàn vé" },
    { value: "Reschedule", label: "Đổi lịch" },
];

export const REQUEST_TYPE_ENUM = {
    Refund: 1,
    Reschedule: 2,
};

export const TABLE_HEADS = [
    "STT", "Mã đơn", "Khách hàng", "Loại yêu cầu", 
    "Ngày gửi", "Trạng thái", "Hành động"
];
export const SEAT_STATUS = {
    AVAILABLE: 0,
    LOCKED: 1,
    BOOKED: 2,
};

export const SEAT_STATUS_LABEL = {
    [SEAT_STATUS.AVAILABLE]: "Trống",
    [SEAT_STATUS.LOCKED]: "Đang giữ",
    [SEAT_STATUS.BOOKED]: "Đã đặt",
};

export const SEAT_LOCK_DURATION = 15 * 60;

export const SEAT_CLASS_COLOR = {
    1: "#1e40af", 
    2: "#7e22ce",
    3: "#b45309",
};
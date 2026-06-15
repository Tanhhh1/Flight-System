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

export const SEAT_REVERSE_HUB_URL = `${import.meta.env.VITE_HUB_URL}/hubs/seat-reverse`;

export const SEAT_REVERSE_STORAGE_KEY = "seat_reverse_booking";

export const SEAT_LOCK_MINUTES = 10;

export const HUB_EVENTS = {
    SEAT_STATUS_CHANGED: "SeatStatusChanged",
    JOIN_FLIGHT: "JoinFlight",
    LEAVE_FLIGHT: "LeaveFlight",
};
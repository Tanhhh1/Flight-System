import api from "@/configs/axios_config";

const BASE = "/SeatReverse";

export const seatReverseService = {
    verifyBookingCode: (bookingCode) =>
        api.get(`${BASE}/verify`, { params: { bookingCode } }),

    getSeatMap: (flightId, bookingId) =>
        api.get(`${BASE}/seat-map`, { params: { flightId, bookingId } }),

    holdSeat: (data) =>
        api.post(`${BASE}/hold`, data),

    releaseSeat: (data) =>
        api.post(`${BASE}/release`, data),

    confirmSeats: (data) =>
        api.post(`${BASE}/confirm`, data),
};
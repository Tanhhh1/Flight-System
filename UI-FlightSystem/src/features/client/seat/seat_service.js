import http from "@/api/http";

const BASE = "/SeatReverse";

export const seatApi = {
    verifyBookingCode: (bookingCode) =>
        http.get(`${BASE}/verify`, { params: { bookingCode } }),

    getSeatMap: (flightId, bookingId) =>
        http.get(`${BASE}/seat-map`, { params: { flightId, bookingId } }),

    holdSeat: (data) =>
        http.post(`${BASE}/hold`, data),

    releaseSeat: (data) =>
        http.post(`${BASE}/release`, data),

    confirmSeats: (data) =>
        http.post(`${BASE}/confirm`, data),
};
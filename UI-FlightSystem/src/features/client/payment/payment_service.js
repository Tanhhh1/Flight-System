import http from "@/api/http";

const BASE = "/Payment";

export const paymentService = {
    getBookingDetail: (bookingId) => http.get(`/Booking/${bookingId}`),

    initiatePayment: (bookingId, method) =>
        http.post(`${BASE}/initiate`, { bookingId, method }),

    retryPayment: (bookingId, method) =>
        http.post(`${BASE}/retry`, { bookingId, method }),
};
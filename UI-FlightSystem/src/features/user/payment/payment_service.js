import axiosInstance from "@/configs/axios_config";

const BASE = "/Payment";

export const paymentService = {
    getBookingDetail: (bookingId) =>
        axiosInstance.get(`/Booking/${bookingId}`),

    initiatePayment: (bookingId, method) =>
        axiosInstance.post(`${BASE}/initiate`, { bookingId, method }),

    retryPayment: (bookingId, method) =>
        axiosInstance.post(`${BASE}/retry`, { bookingId, method }),
};
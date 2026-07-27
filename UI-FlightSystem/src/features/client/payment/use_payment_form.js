import { useState, useEffect, useCallback } from "react";
import { useSearchParams } from "react-router-dom";
import { paymentService } from "./payment_service";
import { VNPAY_METHOD } from "./payment_constants";

export function usePayment() {
    const [searchParams] = useSearchParams();
    const bookingId = Number(searchParams.get("bookingId"));
    const status = searchParams.get("status");

    const [booking, setBooking] = useState(null);
    const [selectedMethod, setSelectedMethod] = useState(VNPAY_METHOD.DomesticCard);
    const [loading, setLoading] = useState(false);
    const [fetchingBooking, setFetchingBooking] = useState(true);
    const [error, setError] = useState(null);

    const clearError = useCallback(() => setError(null), []);

    const isFailed = status === "failed";
    const isPending = !status;

    useEffect(() => {
        if (!bookingId) {
            setFetchingBooking(false);
            return;
        }

        const fetchBooking = async () => {
            try {
                setFetchingBooking(true);
                const res = await paymentService.getBookingDetail(bookingId);
                if (res.data?.succeeded) {
                    setBooking(res.data.result);
                } else {
                    setError("Không thể tải thông tin đặt vé.");
                }
            } catch {
                setError("Không thể tải thông tin đặt vé.");
            } finally {
                setFetchingBooking(false);
            }
        };

        fetchBooking();
    }, [bookingId]);

    const handleConfirmPayment = useCallback(async () => {
        if (!bookingId || !selectedMethod) return;

        try {
            setLoading(true);
            setError(null);

            const apiCall = isFailed
                ? paymentService.retryPayment
                : paymentService.initiatePayment;

            const res = await apiCall(bookingId, selectedMethod);

            if (!res.data?.succeeded) {
                setError(
                    res.data?.errors?.[0]?.errorMessage ??
                    (isFailed ? "Không thể thực hiện thanh toán lại." : "Không thể khởi tạo thanh toán.")
                );
                return;
            }

            const paymentUrl = res.data.result?.paymentUrl;
            if (paymentUrl) {
                window.location.href = paymentUrl;
            }
        } catch (err) {
            setError(
                err.response?.data?.errors?.[0]?.errorMessage ??
                err.response?.data?.message ??
                (isFailed ? "Không thể thực hiện thanh toán lại." : "Không thể khởi tạo thanh toán.")
            );
        } finally {
            setLoading(false);
        }
    }, [bookingId, selectedMethod, isFailed]);

    return { booking, selectedMethod, setSelectedMethod, loading, 
        fetchingBooking, error, clearError, isFailed, isPending, handleConfirmPayment,
    };
}
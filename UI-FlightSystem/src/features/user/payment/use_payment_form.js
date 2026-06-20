import { useState, useEffect, useCallback } from "react";
import { useSearchParams } from "react-router-dom";
import { paymentService } from "./payment_service";
import { PAYMENT_METHOD_OPTIONS, VNPAY_METHOD } from "@/constants/payment";

export function usePayment() {
    const [searchParams] = useSearchParams();
    const bookingId = Number(searchParams.get("bookingId"));

    const [booking, setBooking] = useState(null);
    const [selectedMethod, setSelectedMethod] = useState(VNPAY_METHOD.DomesticCard);
    const [loading, setLoading] = useState(false);
    const [fetchingBooking, setFetchingBooking] = useState(true);
    const [error, setError] = useState(null);

    const clearError = useCallback(() => setError(null), []);

    const status = searchParams.get("status");
    const isFailed = status === "failed";
    const isPending = !status;

    useEffect(() => {
        if (!bookingId) return;

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

    const handleInitiate = useCallback(async () => {
        if (!bookingId || !selectedMethod) return;

        try {
            setLoading(true);
            setError(null);
            const res = await paymentService.initiatePayment(bookingId, selectedMethod);

            if (!res.data?.succeeded) {
                setError(res.data?.errors?.[0]?.errorMessage ?? "Không thể khởi tạo thanh toán.");
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
                "Không thể khởi tạo thanh toán."
            );
        } finally {
            setLoading(false);
        }
    }, [bookingId, selectedMethod]);

    const handleRetry = useCallback(async () => {
        if (!bookingId || !selectedMethod) return;

        try {
            setLoading(true);
            setError(null);
            const res = await paymentService.retryPayment(bookingId, selectedMethod);

            if (!res.data?.succeeded) {
                setError(res.data?.errors?.[0]?.errorMessage ?? "Không thể thực hiện thanh toán lại.");
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
                "Không thể thực hiện thanh toán lại."
            );
        } finally {
            setLoading(false);
        }
    }, [bookingId, selectedMethod]);

    return {
        booking,
        selectedMethod,
        setSelectedMethod,
        paymentMethodOptions: PAYMENT_METHOD_OPTIONS,
        loading,
        fetchingBooking,
        error,
        clearError,
        isFailed,
        isPending,
        handleInitiate,
        handleRetry,
    };
}
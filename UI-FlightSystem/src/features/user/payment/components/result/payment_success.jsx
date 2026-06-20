import { useEffect, useState } from "react";
import { useSearchParams, useNavigate } from "react-router-dom";
import { paymentService } from "../../payment_service";
import { formatDateShort } from "@/utils/date_utils";
import "./payment_result.css"


function PaymentSuccess() {
    const [searchParams] = useSearchParams();
    const bookingId = Number(searchParams.get("bookingId"));
    const navigate = useNavigate();
    const [booking, setBooking] = useState(null);

    useEffect(() => {
        if (!bookingId) return;
        paymentService.getBookingDetail(bookingId).then(res => {
            if (res.data?.succeeded) setBooking(res.data.result);
        });
    }, [bookingId]);

    const formatCurrency = (value) =>
        new Intl.NumberFormat("vi-VN", { style: "currency", currency: "VND" }).format(value);

    return (
        <div className="result_container">
            <div className="result_card success">
                <div className="result_icon">
                    <i className="bx bx-check-circle" />
                </div>
                <h2>Thanh toán thành công!</h2>
                <p>Đặt vé của bạn đã được xác nhận. Vui lòng kiểm tra email để nhận thông tin chi tiết.</p>

                {booking && (
                    <div className="result_booking_summary">
                        <div className="result_row">
                            <span>Mã đặt vé</span>
                            <strong>{booking.bookingCode}</strong>
                        </div>
                        <div className="result_row">
                            <span>Tổng tiền</span>
                            <strong>{formatCurrency(booking.totalPrice)}</strong>
                        </div>
                        <div className="result_row">
                            <span>Ngày đặt</span>
                            <strong>{formatDateShort(booking.bookingDate)}</strong>
                        </div>
                    </div>
                )}

                <div className="result_actions">
                    <button className="btn_primary" onClick={() => navigate("/profile/transactions")}>
                        <i className="bx bx-list-ul" /> Xem lịch sử đặt vé
                    </button>
                    <button className="btn_secondary" onClick={() => navigate("/")}>
                        <i className="bx bx-home" /> Về trang chủ
                    </button>
                </div>
            </div>
        </div>
    );
}

export default PaymentSuccess;
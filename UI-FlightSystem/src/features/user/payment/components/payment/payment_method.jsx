import { usePayment } from "../../use_payment_form";
import AlertModal from "@/components/error/alert_modal";
import { formatTime, formatDateShort } from "@/utils/date_utils";
import { TRIP_TYPE_LABEL } from "@/constants/booking"
import "./payment_method.css";

function PaymentMethod() {
    const { booking, selectedMethod, setSelectedMethod, paymentMethodOptions, loading, fetchingBooking, error,
        clearError, isFailed, isPending, handleInitiate, handleRetry, } = usePayment();

    const formatCurrency = (value) =>
        new Intl.NumberFormat("vi-VN", { style: "currency", currency: "VND" }).format(value);

    if (fetchingBooking) {
        return (
            <div className="payment_container">
                <div className="payment_loading_state">
                    <i className="bx bx-loader-alt bx-spin" /> Đang tải thông tin đặt vé...
                </div>
            </div>
        );
    }

    if (!booking) return null;

    return (
        <>
            <AlertModal type="error" message={error} onClose={clearError} />
            <div className="payment_container">
                <div className="payment_inner_layout">
                    <div className="payment_main_content">
                        <h2 className="payment_step_title">Phương thức thanh toán</h2>
                        <p className="payment_step_subtitle">
                            Vui lòng lựa chọn một trong các hình thức thanh toán bảo mật dưới đây để hoàn tất đặt chỗ.
                        </p>

                        {isFailed && (
                            <div className="error_alert">
                                <i className="bx bx-error-circle" />
                                <div>
                                    <strong>Thanh toán trước đó thất bại</strong>
                                    <p>Vui lòng chọn phương thức thanh toán và thử lại.</p>
                                </div>
                            </div>
                        )}

                        <div className="payment_methods_panel">
                            <div className="panel_header">
                                <h4><i className="bx bx-shield-quarter" /> Chọn phương thức thanh toán</h4>
                            </div>
                            <div className="panel_body method_list_body">
                                {paymentMethodOptions.map((method) => {
                                    const isChecked = selectedMethod === method.id;
                                    return (
                                        <label key={method.id} className={`payment_method_item${isChecked ? " active_method" : ""}`}>
                                            <div className="method_radio_wrapper">
                                                <input type="radio" name="paymentMethod" value={method.id} checked={isChecked} onChange={() => setSelectedMethod(method.id)} className="method_radio_input"/>
                                            </div>
                                            <div className="method_icon_box">
                                                <i className={method.icon} />
                                            </div>
                                            <div className="method_details">
                                                <span className="method_title">{method.title}</span>
                                                <p className="method_desc">{method.desc}</p>
                                            </div>
                                        </label>
                                    );
                                })}
                            </div>
                        </div>

                        <div className="payment_agreement_box">
                            <p>
                                Bằng việc nhấn nút "Xác nhận thanh toán", bạn đồng ý chấp thuận với các
                                <a href="#/terms" target="_blank" rel="noreferrer"> Điều khoản điều kiện</a> và
                                <a href="#/policy" target="_blank" rel="noreferrer"> Chính sách hoàn trả</a> của SkyJourney.
                            </p>
                        </div>

                        <div className="form_submit_row">
                            <button type="button" className="btn_confirm_payment" onClick={isFailed ? handleRetry : handleInitiate} disabled={loading || (!isPending && !isFailed)}>
                                {loading
                                    ? <><i className="bx bx-loader-alt bx-spin" /> Đang xử lý...</>
                                    : <><i className="bx bx-lock-alt" /> {isFailed ? "Thử lại thanh toán" : "Xác nhận thanh toán"}: {formatCurrency(booking.totalPrice)}</>
                                }
                            </button>
                        </div>
                    </div>

                    <aside className="payment_summary_sidebar">
                        <div className="summary_panel">
                            <div className="panel_header">
                                <h4><i className="bx bx-receipt" /> Tóm tắt đặt vé</h4>
                            </div>
                            <div className="panel_body summary_body">

                                <div className="summary_booking_info">
                                    <span className="flight_badge_code">{booking.bookingCode}</span>
                                    <div className="summary_meta_row">
                                        <span className="summary_flight_label">Hạng ghế:</span>
                                        <span className="summary_flight_value">{booking.className}</span>
                                    </div>
                                    <div className="summary_meta_row">
                                        <span className="summary_flight_label">Loại chuyến:</span>
                                        <span className="summary_flight_value">{TRIP_TYPE_LABEL[booking.tripType]}</span>
                                    </div>
                                </div>

                                <div className="payment_summary_divider" />

                                {booking.flights.map((flight, idx) => (
                                    <div key={flight.flightId} className="summary_flight_item">
                                        <span className="summary_flight_label">
                                            {booking.flights.length === 1 ? "Chuyến bay" : `Chặng ${idx + 1}`}
                                        </span>
                                        <p className="summary_flight_route">
                                            {flight.originAirport} → {flight.destinationAirport}
                                        </p>
                                        <span className="summary_flight_detail">
                                            {formatTime(flight.departureTime)} · {formatDateShort(flight.departureTime)}
                                        </span>
                                        <span className="summary_flight_detail">
                                            {flight.airlineName} · {flight.planeModel}
                                        </span>
                                    </div>
                                ))}

                                <div className="payment_summary_divider" />

                                <div className="summary_passengers_list">
                                    <span className="breakdown_title">Hành khách:</span>
                                    {booking.flights[0]?.passengers.map((p, idx) => (
                                        <div key={idx} className="summary_row_item">
                                            <span>{p.fullName}</span>
                                            <span>{formatCurrency(p.unitPrice)}</span>
                                        </div>
                                    ))}
                                </div>

                                <div className="payment_summary_divider" />

                                <div className="summary_total_row">
                                    <span className="total_label">Tổng tiền thanh toán:</span>
                                    <span className="total_value">{formatCurrency(booking.totalPrice)}</span>
                                </div>
                                <p className="secure_payment_note">
                                    <i className="bx bx-check-shield" /> Dữ liệu thanh toán được mã hóa SSL 256-bit an toàn tuyệt đối.
                                </p>
                            </div>
                        </div>
                    </aside>

                </div>
            </div>
        </>
    );
}

export default PaymentMethod;
import { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useLocation, useNavigate } from "react-router-dom";
import { createSupportRequest, resetCreate } from "@/features/user/support/use_support_form";
import { flightSearchService } from "@/features/user/flight_search/flight_search_service";
import { formatDate, formatTime } from "@/utils/date_utils";
import { REQUEST_TYPE_ENUM } from "@/constants/support_request";
import AlertModal from "@/components/error/alert_modal";
import "./support_request_form.css";

const REQUEST_TYPES = [
    { value: "Refund", label: "Hoàn vé" },
    { value: "Reschedule", label: "Đổi lịch bay" },
];

function SupportRequestForm() {
    const dispatch = useDispatch();
    const navigate = useNavigate();
    const { state } = useLocation();
    const booking = state?.booking ?? null;

    const { isLoading, error, success } = useSelector((s) => s.createSupportRequest);

    const [requestType, setRequestType] = useState("Refund");
    const [reason, setReason] = useState("");

    const [flightSearchResults, setFlightSearchResults] = useState([]);
    const [isSearching, setIsSearching] = useState(false);
    const [searchDate, setSearchDate] = useState("");
    const [selectedFlightId, setSelectedFlightId] = useState(null);
    const [searchError, setSearchError] = useState("");

    const [alertState, setAlertState] = useState({ isOpen: false, message: "", type: "success" });
    const showAlert = (message, type = "error") => setAlertState({ isOpen: true, message, type });
    const closeAlert = () => setAlertState({ isOpen: false, message: "", type: "success" });

    useEffect(() => {
        setReason("");
        setFlightSearchResults([]);
        setSelectedFlightId(null);
        setSearchDate("");
        setSearchError("");
    }, [requestType]);

    useEffect(() => {
        if (success) {
            showAlert("Gửi yêu cầu hỗ trợ thành công! Chúng tôi sẽ liên hệ với bạn sớm nhất", "success");
            dispatch(resetCreate());
            setTimeout(() => navigate(-1), 1500);
        }
    }, [success, dispatch, navigate]);

    useEffect(() => {
        if (error) {
            showAlert(error, "error");
            dispatch(resetCreate());
        }
    }, [error, dispatch]);

    const handleSearchFlights = async () => {
        if (!searchDate) {
            setSearchError("Vui lòng chọn ngày khởi hành mới.");
            return;
        }
        setSearchError("");
        setIsSearching(true);
        setFlightSearchResults([]);
        setSelectedFlightId(null);
        try {
            const { data } = await flightSearchService.searchLeg({
                leg: {
                    from: booking.originAirport,
                    to: booking.destinationAirport,
                    departureDate: searchDate,
                },
                classId: booking.classId,
                filters: {},
            });
            if (data?.succeeded && data.result?.items?.length > 0) {
                setFlightSearchResults(data.result.items);
            } else {
                setSearchError("Không tìm thấy chuyến bay phù hợp cho ngày này.");
            }
        } catch {
            setSearchError("Lỗi khi tìm kiếm chuyến bay. Vui lòng thử lại.");
        } finally {
            setIsSearching(false);
        }
    };

    const handleSubmit = () => {
        if (requestType === "Refund" && !reason.trim()) {
            showAlert("Vui lòng nhập lý do hoàn vé.");
            return;
        }
        if (requestType === "Reschedule" && !selectedFlightId) {
            showAlert("Vui lòng chọn chuyến bay thay thế.");
            return;
        }

        const payload = {
            bookingId: booking.bookingId,
            requestType: REQUEST_TYPE_ENUM[requestType],
            reason: reason.trim() || null,
            newFlightId: requestType === "Reschedule" ? selectedFlightId : null,
        };

        dispatch(createSupportRequest(payload));
    };

    if (!booking) {
        return (
            <div className="support_form_wrapper">
                <div className="support_no_booking">
                    <i className="bx bx-error-circle" />
                    <p>Không tìm thấy thông tin giao dịch. Vui lòng quay lại và thử lại.</p>
                    <button className="btn_back" onClick={() => navigate(-1)}>
                        <i className="bx bx-arrow-back" /> Quay lại
                    </button>
                </div>
            </div>
        );
    }

    return (
        <div className="support_form_wrapper">
            <div className="setting_card_panel">
                <div className="panel_header">
                    <button className="btn_back_inline" onClick={() => navigate(-1)}>
                        <i className="bx bx-arrow-back" />
                    </button>
                    <h4>Gửi yêu cầu hỗ trợ</h4>
                </div>

                <div className="panel_body support_form_body">
                    <div className="support_booking_info">
                        <div className="support_booking_info_title">
                            <i className="bx bx-receipt" /> Thông tin đặt vé
                        </div>
                        <div className="support_booking_info_grid">
                            <div className="support_info_item">
                                <span className="support_info_label">Mã đơn</span>
                                <strong>{booking.bookingCode}</strong>
                            </div>
                            <div className="support_info_item">
                                <span className="support_info_label">Hành khách</span>
                                <span>{booking.fullname}</span>
                            </div>
                            <div className="support_info_item">
                                <span className="support_info_label">Ngày đặt</span>
                                <span>{formatDate(booking.bookingDate)}</span>
                            </div>
                            <div className="support_info_item">
                                <span className="support_info_label">Tổng tiền</span>
                                <strong className="support_price">
                                    {booking.totalPrice?.toLocaleString("vi-VN")}₫
                                </strong>
                            </div>
                        </div>
                    </div>

                    <div className="support_form_section">
                        <label className="support_form_label">
                            <i className="bx bx-list-ul" /> Loại yêu cầu
                        </label>
                        <div className="support_type_tabs">
                            {REQUEST_TYPES.map((t) => (
                                <button key={t.value} className={`support_type_tab ${requestType === t.value ? "active" : ""}`} onClick={() => setRequestType(t.value)}>
                                    {t.value === "Refund" ? <i className="bx bx-transfer-alt" /> : <i className="bx bx-calendar-edit" />} {t.label}
                                </button>
                            ))}
                        </div>
                    </div>

                    {requestType === "Refund" && (
                        <div className="support_form_section">
                            <label className="support_form_label" htmlFor="refund_reason">
                                <i className="bx bx-comment-detail" /> Lý do hoàn vé
                                <span className="support_required">*</span>
                            </label>
                            <textarea id="refund_reason" className="support_textarea" rows={5} placeholder="Mô tả lý do bạn muốn hoàn vé..." value={reason} onChange={(e) => setReason(e.target.value)} />
                            <span className="support_char_count">{reason.length} ký tự</span>
                        </div>
                    )}

                    {requestType === "Reschedule" && (
                        <>
                            <div className="support_form_section">
                                <div className="support_form_header">
                                    <label className="support_form_label">
                                        <i className="bx bx-search-alt" /> Tìm chuyến bay thay thế
                                    </label>
                                    <p className="support_hint">
                                        Chọn ngày khởi hành mới để tìm các chuyến bay cùng tuyến{" "}
                                        <strong>{booking.originAirport} → {booking.destinationAirport}</strong>.
                                    </p>
                                </div>
                                <div className="support_search_row">
                                    <input type="date" className="support_date_input" value={searchDate} min={new Date().toISOString().split("T")[0]} onChange={(e) => setSearchDate(e.target.value)} />
                                    <button className="btn_search_flight" onClick={handleSearchFlights} disabled={isSearching}>
                                        {isSearching ? <><i className="bx bx-loader-alt bx-spin" /> Đang tìm...</> : <><i className="bx bx-search" /> Tìm chuyến bay</>}
                                    </button>
                                </div>
                                {searchError && (
                                    <p className="support_search_error">
                                        <i className="bx bx-error" /> {searchError}
                                    </p>
                                )}
                            </div>

                            {flightSearchResults.map((flight) => (
                                <div key={flight.flightId} className={`support_flight_card ${selectedFlightId === flight.flightId ? "selected" : ""}`} onClick={() => setSelectedFlightId(flight.flightId)}>
                                    <div className="flight_card_radio">
                                        <div className={`radio_dot ${selectedFlightId === flight.flightId ? "active" : ""}`} />
                                    </div>
                                    <div className="flight_card_info">
                                        <div className="flight_card_route">
                                            <span className="flight_time">{formatTime(flight.departureTime)}</span>
                                            <span className="flight_arrow"><i className="bx bx-right-arrow-alt" /></span>
                                            <span className="flight_time">{formatTime(flight.arrivalTime)}</span>
                                            <span className="flight_duration">({flight.flightDuration} phút)</span>
                                        </div>
                                        <div className="flight_card_meta">
                                            <span><i className="bx bx-plane" /> {flight.airlineName}</span>
                                            <span>
                                                <i className="bx bx-chair" />
                                                {flight.seatClasses?.[0]?.availableSeats ?? 0} ghế trống
                                            </span>
                                        </div>
                                    </div>
                                    <div className="flight_card_price">
                                        <strong>{flight.seatClasses?.[0]?.price?.toLocaleString("vi-VN")}₫</strong>
                                    </div>
                                </div>
                            ))}

                            <div className="support_form_section">
                                <label className="support_form_label" htmlFor="reschedule_reason">
                                    <i className="bx bx-comment" /> Ghi chú thêm
                                </label>
                                <textarea id="reschedule_reason" className="support_textarea" rows={3} placeholder="Thêm ghi chú nếu cần..." value={reason} onChange={(e) => setReason(e.target.value)} />
                            </div>
                        </>
                    )}
                    <div className="support_form_actions">
                        <button className="btn_cancel" onClick={() => navigate(-1)}>
                            Huỷ
                        </button>
                        <button className="btn_submit_support" onClick={handleSubmit} disabled={isLoading}>
                            {isLoading ? <><i className="bx bx-loader-alt bx-spin" /> Đang gửi...</> : <><i className="bx bx-send" /> Gửi yêu cầu</>}
                        </button>
                    </div>
                </div>
            </div>

            {alertState.isOpen && (
                <AlertModal type={alertState.type} message={alertState.message} onClose={closeAlert} />
            )}
        </div>
    );
}

export default SupportRequestForm;
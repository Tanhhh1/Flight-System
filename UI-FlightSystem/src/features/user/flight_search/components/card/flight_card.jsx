import { useState } from "react";
import { useSelector } from "react-redux";
import { formatTime, formatDateShort } from "@/utils/date_utils";
import { formatDuration } from "@/utils/duration_utils";
import "./flight_card.css";

function FlightCard({ flight, onSelect, isSelected }) {
    const { searchMeta } = useSelector((s) => s.searchResults);
    const [activeTab, setActiveTab] = useState(null);
    const seatClass = flight.seatClasses?.find((c) => c.classId === searchMeta.classId) ?? flight.seatClasses?.[0];
    const handleTab = (key) => setActiveTab((prev) => (prev === key ? null : key));

    const segmentCount = flight.segments?.length ?? 0;
    const calculatedStopCount = segmentCount > 1 ? segmentCount - 1 : 0;

    return (
        <div className="flight_card">
            <div className="flight_card_main_info">
                <div className="airline_info">
                    <div className="airline_text">
                        <span className="airline_name_txt">{flight.airlineName}</span>
                        <small className="plane_name_txt">{flight.planeName}</small>
                    </div>
                </div>
                <div className="flight_timeline_wrapper">
                    <div className="time_node text_right">
                        <strong>{formatTime(flight.departureTime)}</strong>
                        <span className="airport_code">{flight.originAirportCode}</span>
                        <span className="airport_city">{flight.originCity}</span>
                    </div>

                    <div className="timeline_line_box">
                        <span className="duration_txt">{formatDuration(flight.flightDuration)}</span>
                        <div className="line_bar">
                        </div>
                        <span className={`stop_label ${calculatedStopCount === 0 ? "stop_direct" : "stop_layover"}`}>
                            {calculatedStopCount === 0 ? "Bay thẳng" : `${calculatedStopCount} điểm dừng`}
                        </span>
                    </div>

                    <div className="time_node text_left">
                        <strong>{formatTime(flight.arrivalTime)}</strong>
                        <span className="airport_code">{flight.destinationAirportCode}</span>
                        <span className="airport_city">{flight.destinationCity}</span>
                    </div>
                </div>

                <div className="flight_price_action">
                    <div className="price_box">
                        {seatClass ? (
                            <span className="unit_price">
                                {seatClass.price.toLocaleString("vi-VN")}
                                <small> VND/khách</small>
                            </span>
                        ) : (
                            <span className="no_seat_txt">Hết chỗ</span>
                        )}
                    </div>
                </div>
            </div>

            <div className="flight_card_footer_bar">
                <div className="flight_card_footer_tabs">
                    <button className={`footer_tab_btn${activeTab === "details" ? " active" : ""}`} onClick={() => handleTab("details")}>
                        <i className="bx bx-info-circle" /> Chi tiết chuyến bay
                        <i className={`bx ${activeTab === "details" ? "bx-chevron-up" : "bx-chevron-down"}`} />
                    </button>
                    <button className={`footer_tab_btn${activeTab === "refund" ? " active" : ""}`} onClick={() => handleTab("refund")}>
                        <i className="bx bx-money-withdraw" /> Hoàn vé
                        <i className={`bx ${activeTab === "refund" ? "bx-chevron-up" : "bx-chevron-down"}`} />
                    </button>
                    <button className={`footer_tab_btn${activeTab === "change" ? " active" : ""}`} onClick={() => handleTab("change")}>
                        <i className="bx bx-calendar-edit" /> Đổi lịch bay
                        <i className={`bx ${activeTab === "change" ? "bx-chevron-up" : "bx-chevron-down"}`} />
                    </button>
                </div>

                <div className="flight_card_footer_action">
                    {onSelect ? (
                        <button className="btn_select_flight" disabled={!seatClass} onClick={() => onSelect(flight)}>
                            {isSelected ? <><i className="bx bx-check" /> Đã chọn</> : "Chọn"}
                        </button>
                    ) : (
                        <button className="btn_select_flight" disabled={!seatClass}>Chọn</button>
                    )}
                </div>
            </div>

            {activeTab && (
                <div className="flight_dropdown_content_panel">
                    {activeTab === "details" && <FlightDetailTimeline flight={flight} seatClass={seatClass} />}
                    {activeTab === "refund" && <PolicyPanel allowed={flight.isRefund} type="refund" />}
                    {activeTab === "change" && <PolicyPanel allowed={flight.isChange} type="change" />}
                </div>
            )}
        </div>
    );
}

function FlightDetailTimeline({ flight, seatClass }) {
    const segments = flight.segments && flight.segments.length > 0 
        ? flight.segments 
        : [{
            stopOrder: 1,
            originCity: flight.originCity,
            originAirportCode: flight.originAirportCode,
            destinationCity: flight.destinationCity,
            destinationAirportCode: flight.destinationAirportCode,
            departureTime: flight.departureTime,
            arrivalTime: flight.arrivalTime,
            flightDuration: flight.flightDuration
        }];

    return (
        <div className="flight_detail_wrapper">
            <div className="detail_class_header">
                <span className="detail_class_badge"><i className="bx bx-diamond" /> {seatClass?.className ?? "—"}</span>
                {seatClass && (
                    <span className={`detail_seats_remain${seatClass.availableSeats <= 5 ? " seats_low" : ""}`}>
                        <i className="bx bx-chair" /> Còn {seatClass.availableSeats} ghế trống
                    </span>
                )}
            </div>
            <div className="flow_vertical_timeline">
                {segments.map((seg, idx) => {
                    const isFirst = idx === 0;
                    const isLast = idx === segments.length - 1;
                    const isStopover = !isLast;
                    const nextSeg = segments[idx + 1];
                    const layoverMins = nextSeg ? Math.round((new Date(nextSeg.departureTime) - new Date(seg.arrivalTime)) / 60000) : 0;

                    return (
                        <div key={seg.stopOrder ?? idx}>
                            <div className="timeline_node_item">
                                <div className="time_side">
                                    <strong>{formatTime(seg.departureTime)}</strong>
                                    <span>{formatDateShort(seg.departureTime)}</span>
                                </div>
                                <div className={`timeline_dot ${isFirst ? "timeline_dot_origin" : "timeline_dot_stop"}`} />
                                <div className="detail_location_content">
                                    <h5>{seg.originCity}</h5>
                                    <span className="detail_airport_code">{seg.originAirportCode}</span>
                                </div>
                            </div>
                            <div className="timeline_segment_block">
                                <div className="timeline_vertical_line" />
                                <div className="flight_inside_specs">
                                    <p className="spec_airline_row"><i className="bx bx-plane-take-off" /> {flight.airlineName} · {flight.planeName}</p>
                                    <p className="spec_duration_row"><i className="bx bx-time-five" /> Bay {formatDuration(seg.flightDuration)}</p>
                                </div>
                            </div>
                            <div className="timeline_node_item">
                                <div className="time_side">
                                    <strong>{formatTime(seg.arrivalTime)}</strong>
                                    <span>{formatDateShort(seg.arrivalTime)}</span>
                                </div>
                                <div className={`timeline_dot ${isLast ? "timeline_dot_dest" : "timeline_dot_stop"}`} />
                                <div className="detail_location_content">
                                    <h5>{seg.destinationCity}</h5>
                                    <span className="detail_airport_code">{seg.destinationAirportCode}</span>
                                </div>
                            </div>
                            {isStopover && layoverMins > 0 && (
                                <div className="layover_divider"><i className="bx bx-time-five" /> Chờ tại {seg.destinationCity} · {formatDuration(layoverMins)}</div>
                            )}
                        </div>
                    );
                })}
            </div>
        </div>
    );
}

function PolicyPanel({ allowed, type }) {
    if (type === "refund") {
        return (
            <div className="policy_panel">
                <div className={`policy_status_banner ${allowed ? "policy_allowed" : "policy_denied"}`}>
                    <i className={`bx ${allowed ? "bx-check-shield" : "bx-shield-x"}`} />
                    <div>
                        <strong>{allowed ? "Cho phép hoàn vé" : "Không hỗ trợ hoàn vé"}</strong>
                        <p>{allowed ? "Vé này có thể được hoàn tiền theo quy định của hãng. Phí hoàn vé (nếu có) sẽ được khấu trừ tại thời điểm xử lý." : "Vé này thuộc loại không hoàn tiền. Sau khi đặt thành công, khoản thanh toán sẽ không được hoàn lại dưới mọi hình thức."}</p>
                    </div>
                </div>
                {allowed && (
                    <ul className="policy_notes_list">
                        <li><i className="bx bx-time" /> Yêu cầu hoàn vé cần thực hiện ít nhất 24 giờ trước giờ khởi hành.</li>
                        <li><i className="bx bx-receipt" /> Tiền hoàn sẽ được trả về phương thức thanh toán gốc trong 7–14 ngày làm việc.</li>
                    </ul>
                )}
            </div>
        );
    }

    if (type === "change") {
        return (
            <div className="policy_panel">
                <div className={`policy_status_banner ${allowed ? "policy_allowed" : "policy_denied"}`}>
                    <i className={`bx ${allowed ? "bx-check-shield" : "bx-shield-x"}`} />
                    <div>
                        <strong>{allowed ? "Cho phép đổi lịch bay" : "Không hỗ trợ đổi lịch bay"}</strong>
                        <p>{allowed ? "Vé này được phép thay đổi ngày/giờ bay. Phí đổi lịch sẽ tính theo chính sách hãng và cộng thêm chênh lệch giá vé." : "Vé này không được phép thay đổi ngày hoặc giờ bay sau khi hệ thống đã xác nhận đặt chỗ thành công."}</p>
                    </div>
                </div>
                {allowed && (
                    <ul className="policy_notes_list">
                        <li><i className="bx bx-time" /> Yêu cầu đổi lịch phải thực hiện trước ít nhất 24 giờ so với giờ khởi hành.</li>
                        <li><i className="bx bx-transfer" /> Chỉ được đổi sang chuyến bay cùng hành trình ban đầu và cùng hạng ghế.</li>
                    </ul>
                )}
            </div>
        );
    }

    return null;
}

export default FlightCard;
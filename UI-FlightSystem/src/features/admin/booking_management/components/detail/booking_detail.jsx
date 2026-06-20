import React, { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useParams, useNavigate } from "react-router-dom";
import { fetchBookingById, clearDetail } from "../../booking_detail_slice";
import { BOOKING_STATUS_LABEL, PASSENGER_TYPE_LABEL, PASSENGER_TABLE_HEADS, TRIP_TYPE_LABEL, SEAT_CLASS_OPTIONS } from "@/constants/booking";
import { formatDuration } from "@/utils/duration_utils";
import { formatTime, formatDateShort } from "@/utils/date_utils"
import Table from "@/components/table/table";
import "./booking_detail.css";

function BookingDetail() {
    const { id } = useParams();
    const dispatch = useDispatch();
    const navigate = useNavigate();
    const { detail, isLoadingDetail, error } = useSelector((s) => s.bookingDetail);

    useEffect(() => { dispatch(fetchBookingById(id)); return () => dispatch(clearDetail()) }, [dispatch, id]);

    if (isLoadingDetail) return null;
    if (error) return <div className="state_message error_alert"><i className="bx bx-error-circle" />{error}</div>;
    if (!detail) return <div className="state_message">Không tìm thấy dữ liệu đơn đặt vé.</div>;

    const passengers = detail.flights?.[0]?.passengers ?? [];

    return (
        <div className="detail_container">
            <div className="detail_header">
                <div className="detail_title">
                    <h2>Chi tiết Đặt vé</h2>
                    <span className="booking_code">#{detail.bookingCode}</span>
                </div>
                <button className="btn_cancel" onClick={() => navigate(-1)}>
                    <i className="bx bx-arrow-back" /> Quay lại
                </button>
            </div>

            <div className="overview_card">
                <div className="info_grid">
                    <div className="info_item">
                        <label>Khách hàng đại diện</label>
                        <span>{detail.fullname}</span>
                    </div>
                    <div className="info_item">
                        <label>Hạng ghế</label>
                        <span>{SEAT_CLASS_OPTIONS[detail.className] ?? detail.className}</span>
                    </div>
                    <div className="info_item">
                        <label>Loại chuyến bay</label>
                        <span>{TRIP_TYPE_LABEL[detail.tripType] ?? detail.tripType}</span>
                    </div>
                    <div className="info_item">
                        <label>Ngày đặt vé</label>
                        <span>{new Date(detail.bookingDate).toLocaleString("vi-VN")}</span>
                    </div>
                    <div className="info_item">
                        <label>Tổng thanh toán</label>
                        <span className="price_cell">{detail.totalPrice?.toLocaleString("vi-VN")}₫</span>
                    </div>
                    <div className="info_item">
                        <label>Trạng thái đơn</label>
                        <span className={`status_badge ${detail.status?.toLowerCase()}`}>
                            {BOOKING_STATUS_LABEL[detail.status] ?? detail.status}
                        </span>
                    </div>
                </div>
            </div>

            <h3 className="section_title">
                <i className="bx bx-trip" /> Lịch trình chuyến bay
            </h3>

            <div className="flight_card">
                {detail.flights?.map((flight, fIndex) => (
                    <div className="flight_leg" key={flight.flightId || fIndex}>
                        <div className="flight_leg_header">
                            <div className="flight_route_summary">
                                {flight.originAirport} <i className="bx bx-right-arrow-alt" /> {flight.destinationAirport}
                            </div>
                            <div className="airline_info">
                                <span>{flight.airlineName} — {flight.planeModel}</span>
                            </div>
                        </div>

                        <div className="segment_timeline">
                            {(flight.segments?.length > 0 ? flight.segments : [flight]).map((seg, sIndex) => (
                                <div className="segment_node" key={seg.segmentOrder || sIndex}>
                                    <div className="segment_detail">
                                        <div className="segment_time">
                                            {formatTime(seg.departureTime)} — {formatTime(seg.arrivalTime)}
                                            <span>{formatDateShort(seg.departureTime)}</span>
                                        </div>
                                        <div className="segment_airport">
                                            <strong>{seg.originAirport}</strong> ({seg.originAirportName})
                                            <i className="bx bx-right-arrow-alt" />
                                            <strong>{seg.destinationAirport}</strong> ({seg.destinationAirportName})
                                        </div>
                                        <div className="segment_duration">
                                            <i className="bx bx-time-five" /> {formatDuration(seg.flightDuration)}
                                        </div>
                                    </div>
                                </div>
                            ))}
                        </div>
                    </div>
                ))}
            </div>

            <h3 className="section_title">
                <i className="bx bx-group" /> Danh sách hành khách
            </h3>

            <div className="passenger_table_wrapper">
                <Table
                    title={`${passengers.length} hành khách`}
                    heads={PASSENGER_TABLE_HEADS}
                    data={passengers}
                    render={(passenger, pIndex) => (
                        <tr key={pIndex}>
                            <td>{pIndex + 1}</td>
                            <td>{passenger.fullName}</td>
                            <td>{PASSENGER_TYPE_LABEL[passenger.typeId] || "Chưa xác định"}</td>
                            <td>{passenger.gender}</td>
                            <td>{formatDateShort(passenger.birthday)}</td>
                            <td>{passenger.country}</td>
                            <td className="price_cell">{passenger.unitPrice?.toLocaleString("vi-VN")}₫</td>
                        </tr>
                    )}
                />
            </div>
        </div>
    );
}

export default BookingDetail;
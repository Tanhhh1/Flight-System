import React, { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import { fetchBookings, setPage, setTripType, setBookingDate } from "../booking_slice";
import { BOOKING_STATUS_LABEL, BOOKING_TABLE_HEADS, TRIP_TYPE_LABEL, SEAT_CLASS_OPTIONS } from "@/constants/booking";
import { adminPaths } from "@/configs/admin_routes";
import Table from "@/components/table/table";
import Pagination from "@/components/pagination/pagination";
import { currentDate } from "@/utils/date_utils";

function Booking() {
    const dispatch = useDispatch();
    const navigate = useNavigate();
    const { items, pageIndex, pageSize, totalPages, totalCount, tripType, bookingDate, error, search } = useSelector((s) => s.booking) || {};

    useEffect(() => {
        dispatch(fetchBookings({ pageIndex, pageSize, tripType, bookingDate, search }))
    }, [dispatch, pageIndex, pageSize, tripType, bookingDate, search]);

    return (
        <div className="list_container">
            <div className="list_header">
                <div className="header_title_wrapper">
                    <h2>Quản lý Đơn đặt vé</h2>
                    <span className="date_badge">
                        <i className="bx bx-calendar" />
                        {currentDate()}
                    </span>
                </div>
            </div>
            <div className="list_toolbar">
                <div className="filter_select">
                    <select value={tripType} onChange={(e) => dispatch(setTripType(e.target.value))}>
                        <option value="">Tất cả loại chuyến</option>
                        <option value="1">Một chiều</option>
                        <option value="2">Khứ hồi</option>
                        <option value="3">Nhiều chặng</option>
                    </select>
                    <i className="bx bx-chevron-down select_arrow" />
                </div>
                <div className="filter_select">
                    <input type="date" value={bookingDate} onChange={(e) => dispatch(setBookingDate(e.target.value))} />
                </div>
            </div>
            {error && (
                <div className="error_alert">
                    <i className="bx bx-error-circle" />
                    <span>{error}</span>
                </div>
            )}
            <div className="table_card_wrapper">
                <Table
                    title="Danh sách đơn đặt vé"
                    link={{ href: "#", text: `Tổng số đơn đặt vé: ${totalCount}` }}
                    heads={BOOKING_TABLE_HEADS}
                    data={items}
                    render={(item, index) => (
                        <tr key={item.bookingId}>
                            <td>{(pageIndex - 1) * pageSize + index + 1}</td>
                            <td>{item.bookingCode}</td>
                            <td>{item.fullname}</td>
                            <td>{SEAT_CLASS_OPTIONS[item.className] ?? item.className}</td>
                            <td>{TRIP_TYPE_LABEL[item.tripType] ?? item.tripType}</td>
                            <td>{new Date(item.bookingDate).toLocaleDateString("vi-VN")}</td>
                            <td className="price_cell">{item.totalPrice.toLocaleString("vi-VN")}₫</td>
                            <td>
                                <span className={`status_dot_wrapper status_${item.status?.toLowerCase()}`}>
                                    <span className="status_dot" />
                                    {BOOKING_STATUS_LABEL[item.status] ?? item.status}
                                </span>
                            </td>
                            <td className="action_buttons_group">
                                <button className="btn_action" title="Xem chi tiết" onClick={() => navigate(`${adminPaths.admin.root}/${adminPaths.admin.bookings}/${item.bookingId}`)}>
                                    <i className="bx bx-show" />
                                </button>
                            </td>
                        </tr>
                    )}
                />
            </div>
            {totalPages > 1 && (<div className="pagination_footer"><Pagination page={pageIndex} total={totalPages} onChange={(p) => dispatch(setPage(p))} /></div>)}
        </div>
    );
}

export default Booking;
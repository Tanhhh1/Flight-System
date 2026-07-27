import { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import Table from "@/components/common/table";
import Pagination from "@/components/common/pagination";
import { PATHS } from "@/app/routes/paths";
import { currentDate } from "@/utils/date_utils";
import { BOOKING_STATUS_LABEL, BOOKING_TABLE_HEADS, TRIP_TYPE_LABEL, SEAT_CLASS_OPTIONS, TRIP_TYPE_OPTIONS } from "../booking_constants";
import { fetchBookings, setPage, setTripType, setBookingDate } from "../booking_slice";

function BookingList() {
    const dispatch = useDispatch();
    const navigate = useNavigate();
    const { items, pageIndex, pageSize, totalPages, totalCount, tripType, bookingDate, search, error, isLoading } = useSelector((state) => state.booking);
    useEffect(() => {dispatch(fetchBookings({pageIndex, pageSize, tripType, bookingDate, search }));
    }, [dispatch, pageIndex, pageSize, tripType, bookingDate, search]);

    return (
        <div className="list_container">
            <header className="list_header">
                <div className="header_title_wrapper">
                    <h2>Quản lý Đơn đặt vé</h2>
                    <span className="date_badge">
                        <i className="bx bx-calendar" /> {currentDate()}
                    </span>
                </div>
            </header>
            <div className="list_toolbar">
                <div className="filter_select">
                    <select value={tripType} onChange={(e) => dispatch(setTripType(e.target.value))}>
                        {TRIP_TYPE_OPTIONS.map((item) => (
                            <option key={item.value} value={item.value}>{item.label}</option>
                        ))}
                    </select>
                    <i className="bx bx-chevron-down select_arrow" />
                </div>
                <div className="filter_select">
                    <input type="date" value={bookingDate} onChange={(e) => dispatch(setBookingDate(e.target.value))}/>
                </div>
            </div>

            {error && (
                <div className="error_alert">
                    <i className="bx bx-error-circle" />
                    <span>{error}</span>
                </div>
            )}

            <main className="table_card_wrapper">
                <Table
                    title="Danh sách đơn đặt vé"
                    link={{ href: "#", text: `Tổng số đơn đặt vé: ${totalCount}`}}
                    heads={BOOKING_TABLE_HEADS}
                    data={items}
                    isLoading={isLoading}
                    render={(booking, index) => (
                        <tr key={booking.bookingId}>
                            <td>{(pageIndex - 1) * pageSize + index + 1}</td>
                            <td>{booking.bookingCode}</td>
                            <td>{booking.fullname}</td>
                            <td>{SEAT_CLASS_OPTIONS[booking.className] ?? booking.className}</td>
                            <td>{TRIP_TYPE_LABEL[booking.tripType] ?? booking.tripType}</td>
                            <td>{new Date(booking.bookingDate).toLocaleDateString("vi-VN")}</td>
                            <td className="price_cell">{booking.totalPrice?.toLocaleString("vi-VN")}₫</td>
                            <td>
                                <span className={`status_dot_wrapper status_${booking.status?.toLowerCase()}`}>
                                    <span className="status_dot" />
                                    {BOOKING_STATUS_LABEL[booking.status] ?? booking.status}
                                </span>
                            </td>
                            <td>
                                <div className="action_buttons_group">
                                    <button className="btn_action" title="Xem chi tiết"
                                        onClick={() => navigate(`${PATHS.ADMIN.ROOT}/${PATHS.ADMIN.BOOKINGS}/${booking.bookingId}`)}>
                                        <i className="bx bx-show" />
                                    </button>
                                </div>
                            </td>
                        </tr>
                    )}
                />
            </main>

            {totalPages > 1 && (
                <footer className="pagination_footer">
                    <Pagination page={pageIndex} total={totalPages} onChange={(page) => dispatch(setPage(page))}/>
                </footer>
            )}
        </div>
    );
}

export default BookingList;
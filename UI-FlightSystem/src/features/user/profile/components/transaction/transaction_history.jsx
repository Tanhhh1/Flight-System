import { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import { fetchMyBookings, setPage } from "@/features/user/booking/booking_slice";
import { BOOKING_STATUS_LABEL, TRIP_TYPE_LABEL, SEAT_CLASS_OPTIONS } from "@/constants/booking";
import { formatDate } from "@/utils/date_utils";
import Pagination from "@/components/pagination/pagination";
import { clientPaths } from "@/configs/client_routes";
import "./transaction_history.css";

const STATUS_CLASS_MAP = {
  Pending: { className: "status-pending", icon: "bx-time-five" },
  Confirmed: { className: "status-confirmed", icon: "bx-check-circle" },
  Cancelled: { className: "status-cancelled", icon: "bx-x-circle" },
  Expired: { className: "status-expired", icon: "bx-error-circle" },
  Failed: { className: "status-failed", icon: "bx-x-circle" },
};

const BLOCKED_STATUSES = ["Pending", "Failed"];

function TransactionHistory() {
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const { items, isLoading, pageIndex, totalPages } = useSelector((s) => s.myBookings);

  useEffect(() => {
    dispatch(fetchMyBookings({ pageIndex, pageSize: 2 }));
  }, [dispatch, pageIndex]);

  const handleSupportRequest = (booking) => {
    navigate(`${clientPaths.profile.root}/${clientPaths.profile.supportCreate}`, { state: { booking } });
  };

  return (
    <div className="transaction_history_wrapper">
      <div className="setting_card_panel">
        <div className="panel_header">
          <h4>Lịch sử giao dịch thanh toán</h4>
        </div>
        <div className="panel_body">
          <p className="transaction_intro_text">
            Xem lại lịch sử các giao dịch, hóa đơn thanh toán vé máy bay và các dịch vụ
            bổ sung bạn đã thực hiện trên hệ thống SkyJourney.
          </p>

          {!isLoading && items.length === 0 && (
            <div className="transaction_empty_box">
              <i className="bx bx-folder-open empty_icon" />
              <p>Bạn chưa thực hiện giao dịch thanh toán nào trên hệ thống.</p>
            </div>
          )}

          {!isLoading && items.length > 0 && (
            <div className="transaction_list">
              {items.map((booking) => {
                const statusLabel = BOOKING_STATUS_LABEL[booking.status] ?? booking.status;
                const { className: statusClass, icon: statusIcon } =
                  STATUS_CLASS_MAP[booking.status] ?? { className: "status_unknown", icon: "bx-help-circle" };
                const isBlocked = BLOCKED_STATUSES.includes(booking.status);

                return (
                  <div key={booking.bookingId} className="transaction_card">
                    <div className="transaction_item_header">
                      <span className="transaction_code">
                        Mã đơn: <strong>{booking.bookingCode}</strong> – {formatDate(booking.bookingDate)}
                      </span>
                      <div className="transaction_header_right">
                        <div className={`transaction_status ${statusClass}`}>
                          <i className={`bx ${statusIcon}`} /> {statusLabel}
                        </div>
                      </div>
                    </div>

                    <div className="transaction_item_body">
                      <div className="transaction_field">
                        <div className="field_label"><i className="bx bx-user" /> Hành khách</div>
                        {booking.fullname}
                      </div>
                      <div className="transaction_field">
                        <div className="field_label"><i className="bx bx-chair" /> Hạng ghế</div>
                        <span className="badge_light">
                          {SEAT_CLASS_OPTIONS[booking.className] ?? booking.className}
                        </span>
                      </div>
                      <div className="transaction_field">
                        <div className="field_label"><i className="bx bx-paper-plane" /> Loại chuyến</div>
                        {TRIP_TYPE_LABEL[booking.tripType]}
                      </div>
                      <div className="transaction_field">
                        <div className="field_label"><i className="bx bx-credit-card" /> Tổng tiền</div>
                        <strong>{booking.totalPrice?.toLocaleString("vi-VN")}₫</strong>
                      </div>
                    </div>

                    <div className="transaction_item_footer">
                      <button className={`btn_support_request ${isBlocked ? "btn_support_disabled" : ""}`} onClick={() => handleSupportRequest(booking)} disabled={isBlocked}>
                        Yêu cầu hỗ trợ {isBlocked && <i className="bx bx-lock-alt" />}
                      </button>
                    </div>
                  </div>
                );
              })}
            </div>
          )}

          {totalPages > 1 && (
            <div>
              <Pagination page={pageIndex} total={totalPages} onChange={(p) => dispatch(setPage(p))} />
            </div>
          )}
        </div>
      </div>
    </div>
  );
}

export default TransactionHistory;
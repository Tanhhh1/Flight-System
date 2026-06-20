import { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { fetchMySupportRequests, setPage } from "@/features/user/support/support_slice";
import {
    REQUEST_TYPE_LABEL,
    SUPPORT_STATUS_LABEL,
    SUPPORT_STATUS_CLASS,
    SUPPORT_STATUS_ICON,
} from "@/constants/support_request";
import { formatDate } from "@/utils/date_utils";
import Pagination from "@/components/pagination/pagination";
import "./support_request.css";

function MySupportRequests() {
    const dispatch = useDispatch();
    const { items, isLoading, pageIndex, totalPages } = useSelector((s) => s.mySupportRequests);

    useEffect(() => {
        dispatch(fetchMySupportRequests({ pageIndex, pageSize: 5 }));
    }, [dispatch, pageIndex]);

    return (
        <div className="support_request_wrapper">
            <div className="setting_card_panel">
                <div className="panel_header">
                    <h4>Yêu cầu hỗ trợ</h4>
                </div>
                <div className="panel_body">
                    {!isLoading && items.length === 0 && (
                        <div className="support_request_box">
                            <p>Bạn chưa có yêu cầu hỗ trợ nào.</p>
                        </div>
                    )}

                    {!isLoading && items.length > 0 && (
                        <div className="support_request_list">
                            {items.map((item) => {
                                const statusClass = SUPPORT_STATUS_CLASS[item.status] ?? "status-pending";
                                const statusIcon = SUPPORT_STATUS_ICON[item.status] ?? "bx-time-five";
                                return (
                                    <div key={item.requestId} className="support_card">
                                        <div className="support_item_header">
                                            <span className="support_code">
                                                Mã đơn: <strong>{item.bookingCode}</strong>
                                                {" "} — {REQUEST_TYPE_LABEL[item.requestType] ?? item.requestType}
                                            </span>
                                            <div className="support_header_right">
                                                <div className={`support_status ${statusClass}`}>
                                                    <i className={`bx ${statusIcon}`} />
                                                    {SUPPORT_STATUS_LABEL[item.status] ?? item.status}
                                                </div>
                                            </div>
                                        </div>
                                        <div className="support_item_body">
                                            <div className="support_field">
                                                <div className="field_label">
                                                    <i className="bx bx-calendar" /> Ngày gửi
                                                </div>
                                                {formatDate(item.createdAt)}
                                            </div>
                                            <div className="support_field">
                                                <div className="field_label">
                                                    <i className="bx bx-message-detail" /> Lý do
                                                </div>
                                                {item.reason}
                                            </div>
                                        </div>
                                    </div>
                                );
                            })}
                        </div>
                    )}

                    {totalPages > 1 && (
                        <div>
                            <Pagination
                                page={pageIndex}
                                total={totalPages}
                                onChange={(p) => dispatch(setPage(p))}
                            />
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
}

export default MySupportRequests;
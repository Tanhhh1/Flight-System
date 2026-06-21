import React, { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { fetchSupportRequests, setPage, setRequestType, setStatus } from "../support_slice";
import { REQUEST_TYPE_LABEL, REQUEST_TYPE_OPTIONS, SUPPORT_STATUS_CLASS, SUPPORT_STATUS_LABEL, TABLE_HEADS } from "@/constants/support_request";import { formatDate, currentDate } from "@/utils/date_utils";
import Table from "@/components/table/table";
import Pagination from "@/components/pagination/pagination";
import SupportRequestModal from "./modal/support_request_modal";

function SupportRequestList() {
    const dispatch = useDispatch();
    const { items, pageIndex, pageSize, totalPages, totalCount, isLoading, error, requestType, status } = useSelector((s) => s.supportRequest);

    const [selectedId, setSelectedId] = useState(null);

    const refresh = () => dispatch(fetchSupportRequests({ pageIndex, pageSize, requestType, status }));

    useEffect(() => {
        refresh();
    }, [dispatch, pageIndex, requestType, status]);

    return (
        <div className="list_container">
            <header className="list_header">
                <div className="header_title_wrapper">
                    <h2>Quản lý Yêu cầu hỗ trợ</h2>
                    <span className="date_badge">
                        <i className="bx bx-calendar" /> {currentDate()}
                    </span>
                </div>
            </header>

            <div className="list_toolbar">
                <div className="filter_select">
                    <select value={requestType} onChange={(e) => dispatch(setRequestType(e.target.value))}>
                        <option value="">Tất cả loại yêu cầu</option>
                        {REQUEST_TYPE_OPTIONS.map((opt) => (
                            <option key={opt.value} value={opt.value}>{opt.label}</option>
                        ))}
                    </select>
                    <i className="bx bx-chevron-down select_arrow" />
                </div>
                <div className="filter_select">
                    <select value={status} onChange={(e) => dispatch(setStatus(e.target.value))}>
                        <option value="">Tất cả trạng thái</option>
                        <option value="Pending">Chờ xử lý</option>
                        <option value="Approved">Đã duyệt</option>
                        <option value="Rejected">Từ chối</option>
                    </select>
                    <i className="bx bx-chevron-down select_arrow" />
                </div>
            </div>

            {error && (
                <div className="error_alert animate_fade_in">
                    <i className="bx bx-error-circle" /> <span>{error}</span>
                </div>
            )}

            <main className="table_card_wrapper">
                <Table
                    title="Danh sách yêu cầu hỗ trợ"
                    link={{ href: "#", text: `Tổng số yêu cầu: ${totalCount}` }}
                    heads={TABLE_HEADS}
                    data={items}
                    isLoading={isLoading}
                    render={(item, index) => {
                        const statusClass = SUPPORT_STATUS_CLASS[item.status] ?? "status-pending";
                        return (
                            <tr key={item.requestId} className="table_row_hover">
                                <td className="text_center text_muted">{(pageIndex - 1) * pageSize + index + 1}</td>
                                <td className="font_medium">{item.bookingCode}</td>
                                <td>{item.fullname}</td>
                                <td>{REQUEST_TYPE_LABEL[item.requestType] ?? item.requestType}</td>
                                <td className="text_muted font_small">{formatDate(item.createdAt)}</td>
                                <td>
                                    <span className={`status_dot_wrapper ${statusClass}`}>
                                        <span className="status_dot" />
                                        {SUPPORT_STATUS_LABEL[item.status] ?? item.status}
                                    </span>
                                </td>
                                <td>
                                    <div className="action_buttons_group">
                                        <button className="btn_action" title="Xem chi tiết" onClick={() => setSelectedId(item.requestId)} >
                                            <i className="bx bx-show" />
                                        </button>
                                    </div>
                                </td>
                            </tr>
                        );
                    }}
                />
            </main>

            {totalPages > 1 && (
                <footer className="pagination_footer">
                    <Pagination page={pageIndex} total={totalPages} onChange={(p) => dispatch(setPage(p))} />
                </footer>
            )}

            {selectedId && (
                <SupportRequestModal requestId={selectedId} onClose={() => setSelectedId(null)} onSuccess={refresh} />
            )}
        </div>
    );
}

export default SupportRequestList;
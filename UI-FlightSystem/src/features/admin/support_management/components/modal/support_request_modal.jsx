import { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { fetchSupportRequestById, approveSupportRequest, rejectSupportRequest } from "../../support_slice";
import { REQUEST_TYPE_LABEL } from "@/constants/support_request";
import { formatDate } from "@/utils/date_utils";
import "./support_request_modal.css";

function SupportRequestModal({ requestId, onClose, onSuccess }) {
    const dispatch = useDispatch();
    const { selectedItem: detail, isDetailLoading, actionLoading, actionError } = useSelector((s) => s.supportRequest);
    useEffect(() => { dispatch(fetchSupportRequestById(requestId)) }, [dispatch, requestId]);

    const handleApprove = async () => {
        const result = await dispatch(approveSupportRequest( requestId ));
        if (!result.error) {
            onSuccess?.();
            onClose();
        }
    };

    const handleReject = async () => {
        const result = await dispatch(rejectSupportRequest( requestId ));
        if (!result.error) {
            onSuccess?.();
            onClose();
        }
    };

    const isPending = detail?.status === "Pending";

    return (
        <div className="modal_overlay">
            <div className="form_modal">
                <div className="form_modal_header">
                    <h3><i className="bx bxs-notepad" /> Chi tiết Yêu cầu hỗ trợ</h3>
                    <button className="btn_close_modal" onClick={onClose}>
                        <i className="bx bx-x" />
                    </button>
                </div>

                <div className="form_modal_content">
                    {isDetailLoading && <div className="detail_loading">⏳ Đang tải dữ liệu...</div>}

                    {!isDetailLoading && detail && (
                        <>
                            {actionError && (
                                <div className="error_alert">
                                    <i className="bx bx-error-circle" /> <span>{actionError}</span>
                                </div>
                            )}
                            <div className="form_grid_two">
                                <div className="form_group">
                                    <label>Mã đơn đặt vé</label>
                                    <input type="text" value={detail.bookingCode || ""} disabled />
                                </div>
                                <div className="form_group">
                                    <label>Khách hàng</label>
                                    <input type="text" value={detail.customerName || ""} disabled />
                                </div>
                            </div>

                            <div className="form_grid_two">
                                <div className="form_group">
                                    <label>Loại yêu cầu</label>
                                    <input type="text" value={REQUEST_TYPE_LABEL[detail.requestType] ?? detail.requestType ?? ""} disabled />
                                </div>
                                <div className="form_group">
                                    <label>Ngày gửi</label>
                                    <input type="text" value={formatDate(detail.createdAt) || ""} disabled />
                                </div>
                            </div>

                            <div className="form_group" style={{ marginBottom: "16px" }}>
                                <label>Lý do từ khách hàng</label>
                                <textarea value={detail.reason || ""} disabled style={{ minHeight: "80px" }} />
                            </div>

                            {detail.adminNote && (
                                <div className="form_group">
                                    <label>Ghi chú của Admin</label>
                                    <textarea value={detail.adminNote} disabled style={{ minHeight: "80px" }} />
                                </div>
                            )}

                            {detail.newFlight && (
                                <div className="modal_section_block">
                                    <h4 className="section_subtitle">
                                        <i className="bx bxs-plane" /> Thông tin chuyến bay mới đề xuất
                                    </h4>
                                    
                                    <div className="form_group" style={{ marginBottom: "10px" }}>
                                        <label>Tuyến bay</label>
                                        <input type="text" value={`${detail.newFlight.originAirport} → ${detail.newFlight.destinationAirport}`} disabled />
                                    </div>

                                    <div className="form_grid_two">
                                        <div className="form_group">
                                            <label>Khởi hành</label>
                                            <input type="text" value={new Date(detail.newFlight.departureTime).toLocaleString("vi-VN")} disabled />
                                        </div>
                                        <div className="form_group">
                                            <label>Đến nơi</label>
                                            <input type="text" value={new Date(detail.newFlight.arrivalTime).toLocaleString("vi-VN")} disabled />
                                        </div>
                                    </div>
                                </div>
                            )}
                        </>
                    )}
                </div>

                {isPending && !isDetailLoading && (
                    <div className="modal_footer">
                        <button className="btn_cancel" onClick={handleReject} disabled={actionLoading}>
                            {actionLoading ? "Đang xử lý..." : "Từ chối"}
                        </button>
                        <button className="btn_submit" onClick={handleApprove} disabled={actionLoading}>
                            {actionLoading ? "Đang xử lý..." : "Phê duyệt đơn"}
                        </button>
                    </div>
                )}
            </div>
        </div>
    );
}

export default SupportRequestModal;
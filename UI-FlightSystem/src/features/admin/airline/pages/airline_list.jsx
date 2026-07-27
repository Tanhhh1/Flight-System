import React from "react";
import { useDispatch } from "react-redux";
import Table from "@/components/common/table";
import Pagination from "@/components/common/pagination";
import AirlineForm from "./airline_form";
import { airlineService } from "../airline_service";
import { fetchAirlines, fetchAirlineById, setPage, setSearch, setStatus, clearSelectedItem } from "../airline_slice";
import { formatDate, currentDate } from "@/utils/date_utils";
import { AIRLINE_STATUS_LABEL, TABLE_HEADS } from "../airline_constants";

import { useDataList } from "@/hooks/use_data_list";
import { useSearchFilter } from "@/hooks/use_search_filter";
import { useModalForm } from "@/hooks/use_modal_form";
import { useDetailLoader } from "@/hooks/use_detail_loader";
import { useDeleteConfirm } from "@/hooks/use_delete_confirm";

function AirlineList() {
    const dispatch = useDispatch();

    const { items, pageIndex, pageSize, totalPages, totalCount, error, isLoading, isDetailLoading, refresh } = useDataList({
        sliceName: "airline",
        fetchListThunk: fetchAirlines,
        selectExtraFilters: (state) => ({ status: state.status }),
    });

    const { searchInput, setSearchInput, handleFilterChange } = useSearchFilter({
        sliceName: "airline",
        setSearchAction: setSearch,
        setFilterAction: setStatus,
    });

    const { formState, openAdd, openEdit, closeModal } = useModalForm({ clearSelectedItemAction: clearSelectedItem });
    const { loadDetail } = useDetailLoader({ fetchByIdThunk: fetchAirlineById, onFulfilled: (data) => openEdit(data) });
    const toggleDelete = useDeleteConfirm({ onSuccess: refresh, deleteServiceFn: airlineService.delete });

    return (
        <div className="list_container">
            <div className="list_header">
                <div className="header_title_wrapper">
                    <h2>Quản lý Hãng bay</h2>
                    <span className="date_badge">
                        <i className="bx bx-calendar" />{currentDate()}
                    </span>
                </div>
                <button onClick={openAdd} className="btn_add">
                    <i className="bx bx-plus" />Thêm hãng bay
                </button>
            </div>

            <div className="list_toolbar">
                <div className="search_box">
                    <i className="bx bx-search search_icon" />
                    <input type="text" placeholder="Tìm tên hãng hàng không..." value={searchInput} onChange={(e) => setSearchInput(e.target.value)}/>
                </div>
                <div className="filter_select">
                    <select onChange={(e) => handleFilterChange(e.target.value)}>
                        <option value="">Tất cả trạng thái</option>
                        <option value="Active">Hoạt động</option>
                        <option value="Suspended">Tạm ngưng</option>
                        <option value="Inactive">Ngừng hoạt động</option>
                    </select>
                    <i className="bx bx-chevron-down select_arrow" />
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
                    title="Danh mục hãng hàng không"
                    link={{ href: "#", text: `Tổng số hãng bay: ${totalCount}` }}
                    heads={TABLE_HEADS}
                    data={items}
                    isLoading={isLoading}
                    render={(airline, index) => (
                        <tr key={airline.airlineId}>
                            <td>{(pageIndex - 1) * pageSize + index + 1}</td>
                            <td>{airline.airlineCode}</td>
                            <td>{airline.airlineName}</td>
                            <td>{airline.country}</td>
                            <td>
                                <span className={`status_dot_wrapper status_${airline.status?.toLowerCase()}`}>
                                    <span className="status_dot" />
                                    {AIRLINE_STATUS_LABEL[airline.status] ?? airline.status}
                                </span>
                            </td>
                            <td>{formatDate(airline.createdAt)}</td>
                            <td className="action_buttons_group">
                                <button onClick={() => loadDetail(airline.airlineId)} title="Sửa" className="btn_action" disabled={isDetailLoading}>
                                    <i className="bx bxs-edit" />
                                </button>
                                <button onClick={() => toggleDelete.open(airline)} title="Xóa" className="btn_action">
                                    <i className="bx bxs-trash" />
                                </button>
                            </td>
                        </tr>
                    )}
                />
            </div>

            {totalPages > 1 && (
                <div className="pagination_footer">
                    <Pagination page={pageIndex} total={totalPages} onChange={(p) => dispatch(setPage(p))} />
                </div>
            )}

            <AirlineForm
                key={formState.data?.airlineId ?? "new"}
                isOpen={formState.isOpen}
                onClose={closeModal}
                onSave={refresh}
                airlineData={formState.data}
                mode={formState.mode}
            />

            {toggleDelete.target && (
                <div className="modal_overlay">
                    <div className="modal_box animate_slide_up">
                        <div className="modal_header">
                            <div className="modal_title_icon">
                                <i className="bx bx-error" />
                            </div>
                            <h3>Xác nhận ngừng hoạt động</h3>
                        </div>
                        <div className="modal_body">
                            {toggleDelete.error && (
                                <div className="error_alert">
                                    <i className="bx bx-error-circle" />
                                    <span>{toggleDelete.error}</span>
                                </div>
                            )}
                            <p>Bạn có chắc chắn muốn chuyển trạng thái hãng bay thành <strong>Ngừng hoạt động</strong> không?</p>
                            <div className="user_confirm_card">
                                <i className="bx bxs-plane" />
                                <div>
                                    <div className="confirm_username">{toggleDelete.target.airlineName}</div>
                                    <div className="confirm_email">{toggleDelete.target.airlineCode}</div>
                                </div>
                            </div>
                        </div>
                        <div className="modal_footer">
                            <button className="btn_cancel" onClick={toggleDelete.close} disabled={toggleDelete.isLoading}>
                                Hủy
                            </button>
                            <button className="btn_submit btn_danger_action" disabled={toggleDelete.isLoading} onClick={() => toggleDelete.confirm((item) => item.airlineId)}>
                                {toggleDelete.isLoading ? "Đang xử lý..." : "Xác nhận"}
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
}

export default AirlineList;
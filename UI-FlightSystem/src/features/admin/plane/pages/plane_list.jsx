import { useDispatch } from "react-redux";
import Table from "@/components/common/table";
import Pagination from "@/components/common/pagination";
import { formatDate, currentDate } from "@/utils/date_utils";
import { TABLE_HEADS, PLANE_STATUS_LABEL } from "../plane_constants";
import { fetchPlanes, fetchPlaneById, setPage, setSearch, setStatus, clearSelectedItem } from "../plane_slice";
import { planeService } from "../plane_service";
import PlaneForm from "./plane_form";

import { useDataList } from "@/hooks/use_data_list";
import { useSearchFilter } from "@/hooks/use_search_filter";
import { useModalForm } from "@/hooks/use_modal_form";
import { useDetailLoader } from "@/hooks/use_detail_loader";
import { useDeleteConfirm } from "@/hooks/use_delete_confirm";

function PlaneList() {
    const dispatch = useDispatch();

    const { items, pageIndex, pageSize, totalPages, totalCount, error, isLoading, isDetailLoading, refresh } = useDataList({
        sliceName: "plane",
        fetchListThunk: fetchPlanes,
        selectExtraFilters: (state) => ({ status: state.status }),
    });

    const { searchInput, setSearchInput, handleFilterChange } = useSearchFilter({
        sliceName: "plane",
        setSearchAction: setSearch,
        setFilterAction: setStatus,
    });

    const { formState, openAdd, openEdit, closeModal } = useModalForm({ clearSelectedItemAction: clearSelectedItem });

    const { loadDetail } = useDetailLoader({
        fetchByIdThunk: fetchPlaneById,
        onFulfilled: (data) => openEdit(data),
    });

    const toggleActive = useDeleteConfirm({
        onSuccess: refresh,
        deleteServiceFn: planeService.delete,
    });

    return (
        <div className="list_container">
            <header className="list_header">
                <div className="header_title_wrapper">
                    <h2>Quản lý Máy bay</h2>
                    <span className="date_badge"><i className="bx bx-calendar-alt" /> {currentDate()}</span>
                </div>
                <button className="btn_add" onClick={openAdd}><i className="bx bx-plus-circle" /> Thêm máy bay</button>
            </header>

            <div className="list_toolbar">
                <div className="search_box">
                    <i className="bx bx-search search_icon" />
                    <input type="text" placeholder="Tìm kiếm theo tên máy bay..." value={searchInput} onChange={(e) => setSearchInput(e.target.value)} />
                </div>

                <div className="filter_select">
                    <select onChange={(e) => handleFilterChange(e.target.value)}>
                        <option value="">Tất cả trạng thái</option>
                        <option value="Active">Hoạt động</option>
                        <option value="Delayed">Bảo trì</option>
                        <option value="Inactive">Ngừng hoạt động</option>
                    </select>
                    <i className="bx bx-chevron-down select_arrow" />
                </div>
            </div>

            {error && (
                <div className="error_alert animate_fade_in">
                    <i className="bx bx-error-circle" />
                    <span>{error}</span>
                </div>
            )}

            <main className="table_card_wrapper">
                <Table
                    title="Danh sách máy bay"
                    link={{ href: "#", text: `Tổng số máy bay: ${totalCount}` }}
                    heads={TABLE_HEADS}
                    data={items}
                    isLoading={isLoading}
                    render={(plane, index) => (
                        <tr key={plane.planeId} className="table_row_hover">
                            <td className="text_center text_muted">{(pageIndex - 1) * pageSize + index + 1}</td>
                            <td className="font_medium">{plane.planeModel}</td>
                            <td>{plane.airlineName}</td>
                            <td>
                                <span className={`status_dot_wrapper status_${plane.status?.toLowerCase()}`}>
                                    <span className="status_dot" />
                                    {PLANE_STATUS_LABEL[plane.status] ?? plane.status}
                                </span>
                            </td>
                            <td className="text_muted font_small">{formatDate(plane.createdAt)}</td>
                            <td>
                                <div className="action_buttons_group">
                                    <button className="btn_action btn_edit" title="Chỉnh sửa" disabled={isDetailLoading} onClick={() => loadDetail(plane.planeId)}>
                                        <i className="bx bxs-edit-alt" />
                                    </button>
                                    <button className="btn_action" title="Ngừng hoạt động" onClick={() => toggleActive.open(plane)}>
                                        <i className="bx bxs-trash" />
                                    </button>
                                </div>
                            </td>
                        </tr>
                    )}
                />
            </main>

            {totalPages > 1 && (
                <footer className="pagination_footer">
                    <Pagination page={pageIndex} total={totalPages} onChange={(page) => dispatch(setPage(page))} />
                </footer>
            )}

            <PlaneForm
                key={formState.data?.planeId ?? "new"}
                isOpen={formState.isOpen}
                mode={formState.mode}
                planeData={formState.data}
                onSave={refresh}
                onClose={closeModal}
            />

            {toggleActive.target && (
                <div className="modal_overlay">
                    <div className="modal_box">
                        <div className="modal_header">
                            <div className="modal_title_icon"><i className="bx bx-error" /></div>
                            <h3>Xác nhận ngừng hoạt động</h3>
                        </div>

                        <div className="modal_body">
                            {toggleActive.error && (
                                <div className="error_alert">
                                    <i className="bx bx-error-circle" />
                                    <span>{toggleActive.error}</span>
                                </div>
                            )}
                            <p>Bạn có chắc chắn muốn chuyển trạng thái máy bay thành <strong>Ngừng hoạt động</strong> không?</p>
                            <div className="user_confirm_card">
                                <i className="bx bxs-plane-alt" />
                                <div>
                                    <div className="confirm_username">{toggleActive.target.planeModel}</div>
                                    <div className="confirm_email">{toggleActive.target.airlineName}</div>
                                </div>
                            </div>
                        </div>

                        <div className="modal_footer">
                            <button className="btn_cancel" disabled={toggleActive.isLoading} onClick={toggleActive.close}>Hủy</button>
                            <button className="btn_submit btn_danger_action" disabled={toggleActive.isLoading} onClick={() => toggleActive.confirm(item => item.planeId)}>
                                {toggleActive.isLoading ? "Đang xử lý..." : "Xác nhận"}
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
}

export default PlaneList;
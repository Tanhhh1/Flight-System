import { useDispatch } from "react-redux";
import Table from "@/components/common/table";
import Pagination from "@/components/common/pagination";
import { formatDate, currentDate } from "@/utils/date_utils";
import { TABLE_HEADS, AIRPORT_STATUS_LABEL } from "../airport_constants";
import { fetchAirports, fetchAirportById, setPage, setSearch, setStatus, clearSelectedItem } from "../airport_slice";
import { airportService } from "../airport_service";
import AirportForm from "./airport_form";

import { useDataList } from "@/hooks/use_data_list";
import { useSearchFilter } from "@/hooks/use_search_filter";
import { useModalForm } from "@/hooks/use_modal_form";
import { useDetailLoader } from "@/hooks/use_detail_loader";
import { useDeleteConfirm } from "@/hooks/use_delete_confirm";

function AirportList() {
    const dispatch = useDispatch();

    const { items, pageIndex, pageSize, totalPages, totalCount, error, isLoading, isDetailLoading, refresh } = useDataList({
        sliceName: "airport",
        fetchListThunk: fetchAirports,
        selectExtraFilters: (state) => ({ status: state.status }),
    });

    const { searchInput, setSearchInput, handleFilterChange } = useSearchFilter({
        sliceName: "airport",
        setSearchAction: setSearch,
        setFilterAction: setStatus,
    });

    const { formState, openAdd, openEdit, closeModal } = useModalForm({ clearSelectedItemAction: clearSelectedItem });

    const { loadDetail } = useDetailLoader({
        fetchByIdThunk: fetchAirportById,
        onFulfilled: (data) => openEdit(data),
    });

    const toggleActive = useDeleteConfirm({
        onSuccess: refresh,
        deleteServiceFn: airportService.delete,
    });

    return (
        <div className="list_container">
            <header className="list_header">
                <div className="header_title_wrapper">
                    <h2>Quản lý Sân bay</h2>
                    <span className="date_badge"><i className="bx bx-calendar-alt" /> {currentDate()}</span>
                </div>
                <button className="btn_add" onClick={openAdd}><i className="bx bx-plus-circle" /> Thêm sân bay</button>
            </header>

            <div className="list_toolbar">
                <div className="search_box">
                    <i className="bx bx-search search_icon" />
                    <input type="text" placeholder="Tìm theo mã, tên, thành phố..." value={searchInput} onChange={(e) => setSearchInput(e.target.value)} />
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
                <div className="error_alert animate_fade_in">
                    <i className="bx bx-error-circle" />
                    <span>{error}</span>
                </div>
            )}

            <main className="table_card_wrapper">
                <Table
                    title="Danh sách mạng lưới sân bay"
                    link={{ href: "#", text: `Tổng số sân bay: ${totalCount}` }}
                    heads={TABLE_HEADS}
                    data={items}
                    isLoading={isLoading}
                    render={(airport, index) => (
                        <tr key={airport.airportId} className="table_row_hover">
                            <td className="text_center text_muted">{(pageIndex - 1) * pageSize + index + 1}</td>
                            <td className="font_medium">{airport.airportCode}</td>
                            <td>{airport.airportName}</td>
                            <td>{airport.city}</td>
                            <td>{airport.country}</td>
                            <td>
                                <span className={`status_dot_wrapper status_${airport.status?.toLowerCase()}`}>
                                    <span className="status_dot" />
                                    {AIRPORT_STATUS_LABEL[airport.status] ?? airport.status}
                                </span>
                            </td>
                            <td className="text_muted font_small">{formatDate(airport.createdAt)}</td>
                            <td>
                                <div className="action_buttons_group">
                                    <button className="btn_action btn_edit" title="Chỉnh sửa" disabled={isDetailLoading} onClick={() => loadDetail(airport.airportId)}>
                                        <i className="bx bxs-edit-alt" />
                                    </button>
                                    <button className="btn_action" title="Ngừng hoạt động" onClick={() => toggleActive.open(airport)}>
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

            <AirportForm
                key={formState.data?.airportId ?? "new"}
                isOpen={formState.isOpen}
                mode={formState.mode}
                airportData={formState.data}
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
                            <p>Bạn có chắc chắn muốn chuyển trạng thái sân bay thành <strong>Ngừng hoạt động</strong> không?</p>
                            <div className="user_confirm_card">
                                <i className="bx bx-building" />
                                <div>
                                    <div className="confirm_username">{toggleActive.target.airportName}</div>
                                    <div className="confirm_email">{toggleActive.target.airportCode} · {toggleActive.target.city}</div>
                                </div>
                            </div>
                        </div>

                        <div className="modal_footer">
                            <button className="btn_cancel" disabled={toggleActive.isLoading} onClick={toggleActive.close}>Hủy</button>
                            <button className="btn_submit btn_danger_action" disabled={toggleActive.isLoading} onClick={() => toggleActive.confirm(item => item.airportId)}>
                                {toggleActive.isLoading ? "Đang xử lý..." : "Xác nhận"}
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
}

export default AirportList;
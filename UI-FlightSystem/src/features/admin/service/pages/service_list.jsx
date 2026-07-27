import { useDispatch } from "react-redux";
import Table from "@/components/common/table";
import Pagination from "@/components/common/pagination";
import ServiceForm from "./service_form";
import { serviceService } from "../service_service";
import { fetchServices, fetchServiceById, setPage, setSearch, setIsActive, clearSelectedItem,} from "../service_slice";
import { formatDate, currentDate } from "@/utils/date_utils";
import { TABLE_HEADS } from "../service_constants";

import { useDataList } from "@/hooks/use_data_list";
import { useSearchFilter } from "@/hooks/use_search_filter";
import { useModalForm } from "@/hooks/use_modal_form";
import { useDetailLoader } from "@/hooks/use_detail_loader";
import { useDeleteConfirm } from "@/hooks/use_delete_confirm";

function ServiceList() {
    const dispatch = useDispatch();

    const { items, pageIndex, pageSize, totalPages, totalCount, error, isLoading, isDetailLoading, refresh } = useDataList({
        sliceName: "service",
        fetchListThunk: fetchServices,
        selectExtraFilters: (state) => ({ isActive: state?.isActive }),
    });

    const { searchInput, setSearchInput, handleFilterChange } = useSearchFilter({
        sliceName: "service",
        setSearchAction: setSearch,
        setFilterAction: setIsActive,
    });

    const { formState, openAdd, openEdit, closeModal } = useModalForm({ clearSelectedItemAction: clearSelectedItem });

    const { loadDetail } = useDetailLoader({
        fetchByIdThunk: fetchServiceById,
        onFulfilled: (data) => openEdit(data),
    });

    const toggleActive = useDeleteConfirm({
        onSuccess: refresh,
        deleteServiceFn: serviceService.delete,
    });

    return (
        <div className="list_container">
            <div className="list_header">
                <div className="header_title_wrapper">
                    <h2>Quản lý Dịch vụ</h2>
                    <span className="date_badge"><i className="bx bx-calendar" />{currentDate()}</span>
                </div>
                <button onClick={openAdd} className="btn_add"><i className="bx bx-plus" /> Thêm dịch vụ</button>
            </div>

            <div className="list_toolbar">
                <div className="search_box">
                    <i className="bx bx-search search_icon" />
                    <input type="text" placeholder="Tìm kiếm theo tên dịch vụ..." value={searchInput} onChange={(e) => setSearchInput(e.target.value)} />
                </div>
                <div className="filter_select">
                    <select onChange={(e) => handleFilterChange(e.target.value)}>
                        <option value="">Tất cả trạng thái</option>
                        <option value="true">Đang hoạt động</option>
                        <option value="false">Ngừng cung cấp</option>
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
                    title="Danh sách dịch vụ"
                    link={{ href: "#", text: `Tổng số dịch vụ: ${totalCount}` }}
                    heads={TABLE_HEADS}
                    data={items}
                    isLoading={isLoading}
                    render={(service, index) => (
                        <tr key={service.serviceId}>
                            <td>{(pageIndex - 1) * pageSize + index + 1}</td>
                            <td>{service.serviceName}</td>
                            <td className="truncate_cell">{service.description}</td>
                            <td>
                                <span className={`status_dot_wrapper status_${service.isActive ? "active" : "inactive"}`}>
                                    <span className="status_dot" />
                                    {service.isActive ? "Hoạt động" : "Ngừng cung cấp"}
                                </span>
                            </td>
                            <td>{formatDate(service.createdAt)}</td>
                            <td className="action_buttons_group">
                                <button onClick={() => loadDetail(service.serviceId)} title="Sửa" className="btn_action" disabled={isDetailLoading}>
                                    <i className="bx bxs-edit" />
                                </button>
                                <button onClick={() => toggleActive.open(service)} title="Ngừng cung cấp" className="btn_action">
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

            <ServiceForm
                key={formState.data?.serviceId ?? "new"}
                isOpen={formState.isOpen}
                onClose={closeModal}
                onSave={refresh}
                serviceData={formState.data}
                mode={formState.mode}
            />

            {toggleActive.target && (
                <div className="modal_overlay animate_fade_in">
                    <div className="modal_box animate_slide_up">
                        <div className="modal_header">
                            <div className="modal_title_icon"><i className="bx bx-error" /></div>
                            <h3>Xác nhận ngừng cung cấp</h3>
                        </div>
                        <div className="modal_body">
                            {toggleActive.error && (
                                <div className="error_alert">
                                    <i className="bx bx-error-circle" />
                                    <span>{toggleActive.error}</span>
                                </div>
                            )}
                            <p>Bạn có chắc chắn muốn <strong>Ngừng cung cấp</strong> dịch vụ này không?</p>
                            <div className="user_confirm_card">
                                <i className="bx bxs-cog" />
                                <div>
                                    <div className="confirm_username">{toggleActive.target.serviceName}</div>
                                    <div className="confirm_email">{toggleActive.target.description}</div>
                                </div>
                            </div>
                        </div>
                        <div className="modal_footer">
                            <button className="btn_cancel" onClick={toggleActive.close} disabled={toggleActive.isLoading}>Hủy</button>
                            <button className="btn_submit btn_danger_action" disabled={toggleActive.isLoading} onClick={() => toggleActive.confirm(item => item.serviceId)}>
                                {toggleActive.isLoading ? "Đang xử lý..." : "Xác nhận"}
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
}

export default ServiceList;
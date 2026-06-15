import React, { useState, useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import Table from "@/components/table/table";
import Pagination from "@/components/pagination/pagination";
import ServiceForm from "./service_form";
import { serviceService } from "../service_service";
import { fetchServices, fetchServiceById, setPage, setSearch, setIsActive, clearSelectedItem } from "../service_slice";
import { useConfirmAction } from "@/hooks/use_shared_form";
import { useDebounce } from "@/hooks/use_debounce";
import { formatDate, currentDate } from "@/utils/date_utils";
import { TABLE_HEADS } from "@/constants/service";

function ServiceList() {
    const dispatch = useDispatch();
    const { items, pageIndex, pageSize, totalPages, totalCount, error, search, isActive, isLoading, isDetailLoading } = useSelector((state) => state.service) || {};

    const [searchInput, setSearchInput] = useState(search);
    const [formState, setFormState] = useState({ isOpen: false, mode: "add", data: null });
    const debouncedSearch = useDebounce(searchInput);
    const refresh = () => dispatch(fetchServices({ pageIndex, pageSize, search, isActive }));
    const toggleActive = useConfirmAction({ onSuccess: refresh });

    useEffect(() => { dispatch(fetchServices({ pageIndex, pageSize, search, isActive })) }, [dispatch, pageIndex, pageSize, search, isActive]);
    useEffect(() => { dispatch(setSearch(debouncedSearch)) }, [debouncedSearch, dispatch]);

    const openAdd = () => setFormState({ isOpen: true, mode: "add", data: null });
    const openEdit = async (service) => {
        const result = await dispatch(fetchServiceById(service.serviceId));
        if (fetchServiceById.fulfilled.match(result)) {
            setFormState({ isOpen: true, mode: "edit", data: result.payload });
        }
    };

    return (
        <div className="list_container">
            <div className="list_header">
                <div className="header_title_wrapper">
                    <h2>Quản lý Dịch vụ</h2>
                    <span className="date_badge"><i className="bx bx-calendar" />{currentDate()}</span>
                </div>
                <button onClick={openAdd} className="btn_add">
                    <i className="bx bx-plus" />Thêm dịch vụ
                </button>
            </div>
            <div className="list_toolbar">
                <div className="search_box">
                    <i className="bx bx-search search_icon" />
                    <input type="text" placeholder="Tìm kiếm theo tên dịch vụ..." value={searchInput} onChange={(e) => setSearchInput(e.target.value)} />
                </div>
                <div className="filter_select">
                    <select value={isActive} onChange={(e) => dispatch(setIsActive(e.target.value))}>
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
                                <button onClick={() => openEdit(service)} title="Sửa" className="btn_action" disabled={isDetailLoading}>
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
            {totalPages > 1 && ( <div className="pagination_footer"><Pagination page={pageIndex} total={totalPages} onChange={(p) => dispatch(setPage(p))} /></div>)}
            <ServiceForm key={formState.data?.serviceId ?? "new"} isOpen={formState.isOpen} onClose={() => { setFormState((s) => ({ ...s, isOpen: false })); dispatch(clearSelectedItem())}} onSave={refresh} serviceData={formState.data} mode={formState.mode}/>
            {toggleActive.target && (
                <div className="modal_overlay animate_fade_in">
                    <div className="modal_box animate_slide_up">
                        <div className="modal_header">
                            <div className="modal_title_icon">
                                <i className="bx bx-error" />
                            </div>
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
                                    <div className="confirm_email">{toggleActive.target.price?.toLocaleString("vi-VN")} đ</div>
                                </div>
                            </div>
                        </div>
                        <div className="modal_footer">
                            <button className="btn_cancel" onClick={toggleActive.close} disabled={toggleActive.isLoading}>
                                Hủy
                            </button>
                            <button className="btn_submit btn_danger_action" disabled={toggleActive.isLoading} onClick={() => toggleActive.confirm((service) => serviceService.delete(service.serviceId))}>
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
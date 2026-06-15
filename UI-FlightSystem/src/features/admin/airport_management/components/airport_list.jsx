import React, { useState, useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import Table from "@/components/table/table";
import Pagination from "@/components/pagination/pagination";
import AirportForm from "./airport_form";
import { airportService } from "../airport_service";
import { fetchAirports, fetchAirportById, setPage, setSearch, setStatus, clearSelectedItem } from "../airport_slice";
import { useConfirmAction } from "@/hooks/use_shared_form";
import { useDebounce } from "@/hooks/use_debounce";
import { formatDate, currentDate } from "@/utils/date_utils";
import { TABLE_HEADS, AIRPORT_STATUS_LABEL } from "@/constants/airport";

function AirportList() {
    const dispatch = useDispatch();
    const { items, pageIndex, pageSize, totalPages, totalCount, error, search, status, isLoading, isDetailLoading } = useSelector((state) => state.airport);
    const [searchInput, setSearchInput] = useState(search);
    const [formState, setFormState] = useState({ isOpen: false, mode: "add", data: null });
    const debouncedSearch = useDebounce(searchInput);
    const refresh = () => dispatch(fetchAirports({ pageIndex, pageSize, search, status }));
    const toggleActive = useConfirmAction({ onSuccess: refresh });

    useEffect(() => { dispatch(fetchAirports({ pageIndex, pageSize, search, status })) }, [dispatch, pageIndex, pageSize, search, status]);
    useEffect(() => { dispatch(setSearch(debouncedSearch)) }, [debouncedSearch, dispatch]);

    const openAdd = () => setFormState({ isOpen: true, mode: "add", data: null });

    const openEdit = async (airport) => {
        const result = await dispatch(fetchAirportById(airport.airportId));
        if (fetchAirportById.fulfilled.match(result)) {
            setFormState({ isOpen: true, mode: "edit", data: result.payload });
        }
    };

    return (
        <div className="list_container">
            <div className="list_header">
                <div className="header_title_wrapper">
                    <h2>Quản lý Sân bay</h2>
                    <span className="date_badge">
                        <i className="bx bx-calendar" />
                        {currentDate()}
                    </span>
                </div>
                <button onClick={openAdd} className="btn_add">
                    <i className="bx bx-plus" />
                    Thêm sân bay
                </button>
            </div>

            <div className="list_toolbar">
                <div className="search_box">
                    <i className="bx bx-search search_icon" />
                    <input type="text" placeholder="Tìm theo mã, tên, thành phố..." value={searchInput} onChange={(e) => setSearchInput(e.target.value)}/>
                </div>
                <div className="filter_select">
                    <select value={status} onChange={(e) => dispatch(setStatus(e.target.value))}>
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
                    title="Danh sách mạng lưới sân bay"
                    link={{ href: "#", text: `Tổng số sân bay: ${totalCount}` }}
                    heads={TABLE_HEADS}
                    data={items}
                    isLoading={isLoading}
                    render={(airport, index) => (
                        <tr key={airport.airportId}>
                            <td>{(pageIndex - 1) * pageSize + index + 1}</td>
                            <td>{airport.airportCode}</td>
                            <td>{airport.airportName}</td>
                            <td>{airport.city}</td>
                            <td>{airport.country}</td>
                            <td>
                                <span className={`status_dot_wrapper status_${airport.status?.toLowerCase()}`}>
                                    <span className="status_dot" />
                                    {AIRPORT_STATUS_LABEL[airport.status] ?? airport.status}
                                </span>
                            </td>
                            <td>{formatDate(airport.createdAt)}</td>
                            <td className="action_buttons_group">
                                <button onClick={() => openEdit(airport)} title="Sửa" className="btn_action" disabled={isDetailLoading}>
                                    <i className="bx bxs-edit" />
                                </button>
                                <button onClick={() => toggleActive.open(airport)} className="btn_action" title="Ngừng hoạt động">
                                    <i className="bx bxs-trash" />
                                </button>
                            </td>
                        </tr>
                    )}
                />
            </div>
            {totalPages > 1 && ( <div className="pagination_footer"><Pagination page={pageIndex} total={totalPages} onChange={(p) => dispatch(setPage(p))} /></div>)}
            <AirportForm key={formState.data?.airportId ?? "new"} isOpen={formState.isOpen} onClose={() => { setFormState((s) => ({ ...s, isOpen: false })); dispatch(clearSelectedItem())}} onSave={refresh} airportData={formState.data} mode={formState.mode}/>
            {toggleActive.target && (
                <div className="modal_overlay animate_fade_in">
                    <div className="modal_box animate_slide_up">
                        <div className="modal_header">
                            <div className="modal_title_icon">
                                <i className="bx bx-error" />
                            </div>
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
                            <button className="btn_cancel" onClick={toggleActive.close} disabled={toggleActive.isLoading}>
                                Hủy
                            </button>
                            <button className="btn_submit btn_danger_action" disabled={toggleActive.isLoading} onClick={() => toggleActive.confirm((airport) => airportService.delete(airport.airportId))}>
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
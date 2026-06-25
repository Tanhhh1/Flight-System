import React, { useState, useEffect, useMemo } from "react";
import { useDispatch, useSelector } from "react-redux";
import SearchableSelect from "@/components/select/searchables_select";
import Table from "@/components/table/table";
import Pagination from "@/components/pagination/pagination";
import RouteForm from "./route_form";
import { routeService } from "../route_service";
import { fetchRoutes, fetchRouteById, setPage, setStatusFilter, clearSelectedItem, setOriginAirportCode, setDestinationAirportCode } from "../route_slice";
import { useConfirmAction } from "@/hooks/use_shared_form";
import { formatDate, currentDate } from "@/utils/date_utils";
import { TABLE_HEADS, ROUTE_STATUS_LABEL } from "@/constants/route";
import { formatDuration } from "@/utils/duration_utils";
import { dataSearchService, DataSearch } from "@/services/data_search_service";

function RouteList() {
    const dispatch = useDispatch();
    const { items, pageIndex, pageSize, totalPages, totalCount, error, status, isLoading, isDetailLoading, originAirportCode, destinationAirportCode } = useSelector((state) => state.route) || {};
    
    const [airports, setAirports] = useState([]);
    const [formState, setFormState] = useState({ isOpen: false, mode: "add", data: null });

    const refresh = () => dispatch(fetchRoutes({ pageIndex, pageSize, status, originAirportCode, destinationAirportCode }));
    const toggleDelete = useConfirmAction({ onSuccess: refresh });

    useEffect(() => {
        dataSearchService
            .get([DataSearch.Airports])
            .then(({ data }) => { if (data.succeeded) setAirports(data.result.airports ?? []); })
            .catch(console.error);
    }, []);

    useEffect(() => { dispatch(fetchRoutes({ pageIndex, pageSize, status, originAirportCode, destinationAirportCode }))}, [dispatch, pageIndex, pageSize, status, originAirportCode, destinationAirportCode]);
    const airportOptions = useMemo(() => airports.map((a) => ({ id: a.airportCode, name: `${a.airportCode} — ${a.city}` })), [airports]);

    const openAdd = () => setFormState({ isOpen: true, mode: "add", data: null });
    const openEdit = async (route) => {
        const result = await dispatch(fetchRouteById(route.routeId));
        if (fetchRouteById.fulfilled.match(result)) {
            setFormState({ isOpen: true, mode: "edit", data: result.payload });
        }
    };

    return (
        <div className="list_container">
            <div className="list_header">
                <div className="header_title_wrapper">
                    <h2>Quản lý Tuyến bay</h2>
                    <span className="date_badge">
                        <i className="bx bx-calendar" />
                        {currentDate()}
                    </span>
                </div>
                <button onClick={openAdd} className="btn_add">
                    <i className="bx bx-plus" />
                    Thêm tuyến bay
                </button>
            </div>

            <div className="list_toolbar">
                <div className="filter_select">
                    <SearchableSelect
                        data={airportOptions}
                        value={originAirportCode}
                        onChange={(val) => dispatch(setOriginAirportCode(val))}
                        placeholder="Tất cả điểm đi"
                        itemKey="id"
                        displayValue="name"
                        searchFields={["name"]}
                    />
                </div>
                <div className="filter_select">
                    <SearchableSelect
                        data={airportOptions}
                        value={destinationAirportCode}
                        onChange={(val) => dispatch(setDestinationAirportCode(val))}
                        placeholder="Tất cả điểm đến"
                        itemKey="id"
                        displayValue="name"
                        searchFields={["name"]}
                    />
                </div>
                <div className="filter_select custom_native_select">
                    <select value={status} onChange={(e) => dispatch(setStatusFilter(e.target.value))}>
                        <option value="">Tất cả trạng thái</option>
                        <option value="Active">Hoạt động</option>
                        <option value="Suspended">Tạm ngưng</option>
                        <option value="Inactive">Ngừng hoạt động</option>
                    </select>
                    <i className="bx bx-chevron-down select_arrow" />
                </div>
            </div>
            {error && (<div className="error_alert"><i className="bx bx-error-circle" /><span>{error}</span></div>)}
            <div className="table_card_wrapper">
                <Table
                    title="Danh sách tuyến bay"
                    link={{ href: "#", text: `Tổng số tuyến bay: ${totalCount}` }}
                    heads={TABLE_HEADS}
                    data={items}
                    isLoading={isLoading}
                    render={(route, index) => (
                        <tr key={route.routeId}>
                            <td>{(pageIndex - 1) * pageSize + index + 1}</td>
                            <td>
                                <div className="flight_cell_airport">
                                    <div className="airport_code">{route.originAirportCode}</div>
                                    <div className="airport_city" style={{ fontSize: "12px", color: "#666" }}>{route.originCity || "N/A"}</div>
                                </div>
                            </td>
                            <td>
                                <div className="flight_cell_airport">
                                    <div className="airport_code">{route.destinationAirportCode}</div>
                                    <div className="airport_city" style={{ fontSize: "12px", color: "#666" }}>{route.destinationCity || "N/A"}</div>
                                </div>
                            </td>
                            <td>{formatDuration(route.flightDuration)}</td>
                            <td>
                                <span className={`status_dot_wrapper status_${route.status?.toLowerCase()}`}>
                                    <span className="status_dot" />
                                    {ROUTE_STATUS_LABEL[route.status] ?? route.status}
                                </span>
                            </td>
                            <td>{formatDate(route.createdAt)}</td>
                            <td className="action_buttons_group">
                                <button onClick={() => openEdit(route)} title="Sửa" className="btn_action" disabled={isDetailLoading}>
                                    <i className="bx bxs-edit" />
                                </button>
                                <button onClick={() => toggleDelete.open(route)} title="Xóa" className="btn_action">
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
            <RouteForm key={formState.data?.routeId ?? "new"} isOpen={formState.isOpen} onClose={() => { setFormState((s) => ({ ...s, isOpen: false })); dispatch(clearSelectedItem()); }} onSave={refresh} routeData={formState.data} mode={formState.mode}/>
            {toggleDelete.target && (
                <div className="modal_overlay animate_fade_in">
                    <div className="modal_box animate_slide_up">
                        <div className="modal_header">
                            <div className="modal_title_icon"><i className="bx bx-error" /></div>
                            <h3>Xác nhận xóa tuyến bay</h3>
                        </div>
                        <div className="modal_body">
                            {toggleDelete.error && (
                                <div className="error_alert"><i className="bx bx-error-circle" /><span>{toggleDelete.error}</span></div>
                            )}
                            <p>Bạn có chắc chắn muốn chuyển trạng thái tuyến bay thành <strong>Ngừng hoạt động</strong> không?</p>
                            <div className="user_confirm_card">
                                <i className="bx bx-transfer" />
                                <div>
                                    <div className="confirm_username">
                                        {toggleDelete.target.originAirportCode} ({toggleDelete.target.originCity}) → {toggleDelete.target.destinationAirportCode} ({toggleDelete.target.destinationCity})
                                    </div>
                                    <div className="confirm_email">{formatDuration(toggleDelete.target.flightDuration)}</div>
                                </div>
                            </div>
                        </div>
                        <div className="modal_footer">
                            <button className="btn_cancel" onClick={toggleDelete.close} disabled={toggleDelete.isLoading}>Hủy</button>
                            <button className="btn_submit btn_danger_action" disabled={toggleDelete.isLoading}
                                onClick={() => toggleDelete.confirm((route) => routeService.delete(route.routeId))}>
                                {toggleDelete.isLoading ? "Đang xử lý..." : "Xác nhận"}
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
}

export default RouteList;
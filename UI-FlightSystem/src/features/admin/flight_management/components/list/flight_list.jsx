import React, { useState, useEffect, useMemo } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import SearchableSelect from "@/components/select/searchables_select";
import Table from "@/components/table/table";
import Pagination from "@/components/pagination/pagination";
import { flightService } from "../../flight_service";
import { fetchFlights, setPage, setStatusFilter, setAirlineFilter, setDestinationAirportCode, setOriginAirportCode, setDepartureDate } from "../../flight_slice";
import { useConfirmAction } from "@/hooks/use_shared_form";
import { currentDate } from "@/utils/date_utils";
import { formatDuration } from "@/utils/duration_utils";
import { dataSearchService, DataSearch } from "@/services/data_search_service";
import { TABLE_HEADS, FLIGHT_STATUS_LABEL, FLIGHT_STATUS_FILTER } from "@/constants/flight";
import { adminPaths } from "@/configs/admin_routes";
import { formatTime, formatDateShort } from "@/utils/date_utils"
import "./flight_list.css";

function FlightList() {
    const dispatch = useDispatch();
    const navigate = useNavigate();
    const { items, pageIndex, pageSize, totalPages, totalCount, error, statusFilter, airlineFilter, originAirportCode, destinationAirportCode, departureDate, isLoading } = useSelector((state) => state.flight) || {};
    const [airports, setAirports] = useState([]);
    const [airlines, setAirlines] = useState([]);

    const refresh = () => dispatch(fetchFlights({ pageIndex, pageSize, originAirportCode, destinationAirportCode, departureDate, status: statusFilter, airlineId: airlineFilter }));
    const toggleActive = useConfirmAction({ onSuccess: refresh });

    useEffect(() => {
        dataSearchService
            .get([DataSearch.Airports, DataSearch.Airlines])
            .then(({ data }) => {
                if (data.succeeded) {
                    setAirports(data.result.airports ?? []);
                    setAirlines(data.result.airlines ?? []);
                }
            })
            .catch(console.error);
    }, []);

    useEffect(() => {
        dispatch(fetchFlights({ pageIndex, pageSize, originAirportCode, destinationAirportCode, departureDate, status: statusFilter, airlineId: airlineFilter }));
    }, [dispatch, pageIndex, pageSize, originAirportCode, destinationAirportCode, departureDate, statusFilter, airlineFilter]);

    const airportOptions = useMemo(() => airports.map((a) => ({ id: a.airportCode, name: `${a.airportCode} — ${a.city}` })), [airports]);
    const airlineOptions = useMemo(() => airlines.map((a) => ({ id: a.airlineId, name: a.airlineName })), [airlines]);

    return (
        <div className="list_container">
            <div className="list_header">
                <div className="header_title_wrapper">
                    <h2>Quản lý Chuyến bay</h2>
                    <span className="date_badge">
                        <i className="bx bx-calendar" />
                        {currentDate()}
                    </span>
                </div>
                <button onClick={() => navigate(`${adminPaths.admin.root}/${adminPaths.admin.flightNew}`)} className="btn_add">
                    <i className="bx bx-plus" /> Thêm chuyến bay
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
                <div className="filter_select">
                    <input type="date" value={departureDate} onChange={(e) => dispatch(setDepartureDate(e.target.value))} className="date_input" />
                </div>
                <div className="filter_select">
                    <SearchableSelect
                        data={airlineOptions}
                        value={airlineFilter}
                        onChange={(val) => dispatch(setAirlineFilter(val))}
                        placeholder="Tất cả hãng bay"
                        itemKey="id"
                        displayValue="name"
                        searchFields={["name"]}
                    />
                </div>
                <div className="filter_select custom_native_select">
                    <select value={statusFilter} onChange={(e) => dispatch(setStatusFilter(e.target.value))}>
                        <option value="">Tất cả trạng thái</option>
                        <option value="Active">Hoạt động</option>
                        <option value="Inactive">Ngừng hoạt động</option>
                        <option value="Delayed">Trì hoãn</option>
                        <option value="Cancelled">Hủy chuyến</option>
                        <option value="Completed">Đã hoàn thành</option>
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
                    title="Danh sách chuyến bay"
                    link={{ href: "#", text: `Tổng số chuyến bay: ${totalCount}` }}
                    heads={TABLE_HEADS}
                    data={items}
                    isLoading={isLoading}
                    render={(flight, index) => (
                        <tr key={flight.flightId}>
                            <td>{(pageIndex - 1) * pageSize + index + 1}</td>
                            <td>
                                <div className="flight_cell_airport">
                                    <div className="airport_code">{flight.originAirportCode}</div>
                                    <div className="airport_city">{flight.originCity}</div>
                                </div>
                            </td>
                            <td>
                                <div className="flight_cell_airport">
                                    <div className="airport_code">{flight.destinationAirportCode}</div>
                                    <div className="airport_city">{flight.destinationCity}</div>
                                </div>
                            </td>
                            <td>
                                <div className="flight_cell_datetime">
                                    <div className="time_block">
                                        <div className="flight_time">{ formatTime(flight.departureTime) } </div>
                                        <div className="flight_date">{ formatDateShort(flight.departureTime) }</div>
                                    </div>
                                    <i className="bx bx-right-arrow-alt flight_arrow" />
                                    <div className="time_block">
                                        <div className="flight_time">{ formatTime(flight.arrivalTime) }</div>
                                        <div className="flight_date">{ formatDateShort(flight.arrivalTime) }</div>
                                    </div>
                                </div>
                            </td>
                            <td>
                                <div className="flight_cell_airline">{flight.airlineName}</div>
                                <div className="flight_cell_plane">{flight.planeModel}</div>
                            </td>
                            <td><strong>{formatDuration(flight.flightDuration)}</strong></td>
                            <td>
                                <span className={`status_dot_wrapper status_${flight.status?.toLowerCase()}`}>
                                    <span className="status_dot" />
                                    {FLIGHT_STATUS_LABEL[flight.status] ?? flight.status}
                                </span>
                            </td>
                            <td className="action_buttons_group">
                                <button onClick={() => navigate(`${adminPaths.admin.root}/flights/edit/${flight.flightId}`)} title="Sửa" className="btn_action">
                                    <i className="bx bxs-edit" />
                                </button>
                                <button onClick={() => toggleActive.open(flight)} title="Ngừng hoạt động" className="btn_action" disabled={["Inactive", "Completed", "Cancelled"].includes(flight.status)}>
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

            {toggleActive.target && (
                <div className="modal_overlay animate_fade_in">
                    <div className="modal_box animate_slide_up">
                        <div className="modal_header">
                            <div className="modal_title_icon"><i className="bx bx-error" /></div>
                            <h3>Xác nhận ngừng hoạt động</h3>
                        </div>
                        <div className="modal_body">
                            {toggleActive.error && (
                                <div className="error_alert" style={{ marginBottom: 12 }}>
                                    <i className="bx bx-error-circle" />
                                    <span>{toggleActive.error}</span>
                                </div>
                            )}
                            <p>Bạn có chắc chắn muốn chuyển trạng thái chuyến bay thành <strong>Ngừng hoạt động</strong> không?</p>
                            <div className="user_confirm_card">
                                <i className="bx bxs-plane-take-off" />
                                <div>
                                    <div className="confirm_username">Chuyến bay #{toggleActive.target.flightId}</div>
                                    <div className="confirm_email">{toggleActive.target.originAirportCode} → {toggleActive.target.destinationAirportCode}</div>
                                </div>
                            </div>
                        </div>
                        <div className="modal_footer">
                            <button className="btn_cancel" onClick={toggleActive.close} disabled={toggleActive.isLoading}>Hủy</button>
                            <button className="btn_submit btn_danger_action" disabled={toggleActive.isLoading}
                                onClick={() => toggleActive.confirm((flight) => flightService.delete(flight.flightId))}>
                                {toggleActive.isLoading ? "Đang xử lý..." : "Xác nhận"}
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
}

export default FlightList;
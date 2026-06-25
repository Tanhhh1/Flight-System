import { useState, useEffect } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import SearchableSelect from "@/components/select/searchables_select";
import { DEFAULT_SINGLE_FLIGHT, DEFAULT_MULTI_FLIGHTS, MAX_MULTI_FLIGHTS, SEAT_CLASS_ID_MAP } from "@/constants/homepage";
import "./search_form.css";

function SearchForm({ airports = [], isModalView = false, onSuccess }) {
    const navigate = useNavigate();
    const [searchParams] = useSearchParams();
    const [searchType, setSearchType] = useState("one-way-round");
    const [isRoundTrip, setIsRoundTrip] = useState(false);
    const [seatClass, setSeatClass] = useState("Economy Class");
    const [singleFlight, setSingleFlight] = useState(DEFAULT_SINGLE_FLIGHT);
    const [flights, setFlights] = useState(DEFAULT_MULTI_FLIGHTS);
    const today = new Date().toLocaleDateString("en-CA");
    useEffect(() => {
        if (!isModalView) return;
        if (airports.length === 0) return;
        const type = searchParams.get("type") ?? "one-way";
        const seatClassParam = searchParams.get("seatClass") ?? "Economy Class";
        setSeatClass(seatClassParam);
        if (type === "multi-city") {
            setSearchType("multi-city");
            try {
                const legsRaw = JSON.parse(searchParams.get("legs") ?? "[]");
                if (legsRaw.length > 0) {
                    setFlights(legsRaw.map((l, i) => ({
                        id: l.id ?? Date.now() + i,
                        from: l.from ?? "",
                        to: l.to ?? "",
                        departureDate: l.departureDate ?? ""
                    })));
                }
            } catch (e) {
                console.error("Lỗi parse legs trong modal:", e);
            }
        } else {
            setSearchType("one-way-round");
            setIsRoundTrip(type === "round-trip");
            setSingleFlight({
                from: searchParams.get("origin") ?? "",
                to: searchParams.get("destination") ?? "",
                departureDate: searchParams.get("departureDate") ?? "",
                returnDate: searchParams.get("returnDate") ?? "",
            });
        }
    }, [isModalView, searchParams, airports]);

    const handleFlightChange = (index, field, value) => {
        const updatedFlights = [...flights];
        updatedFlights[index][field] = value;
        setFlights(updatedFlights);
    };

    const handleAddFlight = () => {
        if (flights.length < MAX_MULTI_FLIGHTS) {
            setFlights([...flights, { id: Date.now(), from: "", to: "", departureDate: "" }]);
        }
    };

    const handleRemoveFlight = (indexToRemove) => {
        if (flights.length > 2) {
            setFlights(flights.filter((_, idx) => idx !== indexToRemove));
        }
    };

    const handleSwapSingleLocations = () => {
        setSingleFlight({ ...singleFlight, from: singleFlight.to, to: singleFlight.from });
    };

    const handleSwapMultiLocations = (index) => {
        const updatedFlights = [...flights];
        const temp = updatedFlights[index].from;
        updatedFlights[index].from = updatedFlights[index].to;
        updatedFlights[index].to = temp;
        setFlights(updatedFlights);
    };

    const handleSearchSubmit = (e) => {
        e.preventDefault();
        const classId = SEAT_CLASS_ID_MAP[seatClass] ?? 1;
        const qs = new URLSearchParams();
        qs.set("classId", String(classId));
        qs.set("seatClass", seatClass);

        if (searchType === "one-way-round") {
            qs.set("type", isRoundTrip ? "round-trip" : "one-way");
            qs.set("origin", singleFlight.from);
            qs.set("destination", singleFlight.to);
            qs.set("departureDate", singleFlight.departureDate);
            if (isRoundTrip) qs.set("returnDate", singleFlight.returnDate);
        } else {
            qs.set("type", "multi-city");
            qs.set("legs", JSON.stringify(
                flights.map((f) => ({ from: f.from, to: f.to, departureDate: f.departureDate }))
            ));
        }
        navigate(`/search?${qs.toString()}`);
        onSuccess?.();
    };

    const renderAirportItem = (airport) => (
        <>
            <i className="bx bx-map"></i>
            <div className="item_info">
                <span className="item_city">
                    <span>{airport.city} — {airport.airportCode}</span>
                </span>
                <span className="item_airport">{airport.airportName}</span>
            </div>
        </>
    );

    return (
        <div className="search_box_container">
            <div className="search_top_filters">
                <div className="main_tabs">
                    <button type="button" className={`tab_btn_round ${searchType === "one-way-round" ? "active" : ""}`} onClick={() => setSearchType("one-way-round")}>
                        Một chiều / Khứ hồi
                    </button>
                    <button type="button" className={`tab_btn_round ${searchType === "multi-city" ? "active" : ""}`} onClick={() => setSearchType("multi-city")}>
                        Nhiều thành phố
                    </button>
                </div>
                <div className="filter_options">
                    {searchType === "one-way-round" && (
                        <label className="checkbox_label_home">
                            <input type="checkbox" checked={isRoundTrip} onChange={(e) => setIsRoundTrip(e.target.checked)} />
                            <span>Khứ hồi</span>
                        </label>
                    )}
                    <select className="flat_select" value={seatClass} onChange={(e) => setSeatClass(e.target.value)} >
                        <option value="Economy Class">Phổ thông</option>
                        <option value="Premium Economy">Phổ thông đặc biệt</option>
                        <option value="Business Class">Thương gia</option>
                        <option value="First Class">Hạng nhất</option>
                    </select>
                </div>
            </div>

            <form onSubmit={handleSearchSubmit} className="modern_search_form">
                {searchType === "one-way-round" ? (
                    <div className="form_row_block_group single_layout">
                        <div className="input_field_container">
                            <label className="input_field_title">Từ</label>
                            <SearchableSelect
                                className="airport_select airport_select_from"
                                data={airports}
                                value={singleFlight.from}
                                onChange={(val) => setSingleFlight({ ...singleFlight, from: val })}
                                placeholder="Chọn nơi đi..."
                                itemKey="airportCode"
                                displayValue={(a) => `${a.city} (${a.airportCode})`}
                                searchFields={["city", "airportCode"]}
                                renderItem={renderAirportItem}
                            />
                        </div>
                        <div className="swap_btn_container">
                            <button type="button" className="swap_locations_action" onClick={handleSwapSingleLocations} title="Đổi chiều">
                                <i className="bx bx-transfer-alt"></i>
                            </button>
                        </div>
                        <div className="input_field_container">
                            <label className="input_field_title">Đến</label>
                            <SearchableSelect
                                className="airport_select airport_select_to"
                                data={airports}
                                value={singleFlight.to}
                                onChange={(val) => setSingleFlight({ ...singleFlight, to: val })}
                                placeholder="Chọn nơi đến..."
                                itemKey="airportCode"
                                displayValue={(a) => `${a.city} (${a.airportCode})`}
                                searchFields={["city", "airportCode"]}
                                renderItem={renderAirportItem}
                            />
                        </div>
                        <div className="input_field_container">
                            <label className="input_field_title">Ngày khởi hành</label>
                            <div className="input_field_inner">
                                <i className="bx bx-calendar"></i>
                                <input type="date" min={today} value={singleFlight.departureDate} onChange={(e) => setSingleFlight({ ...singleFlight, departureDate: e.target.value })} required />
                            </div>
                        </div>
                        {isRoundTrip && (
                            <div className="input_field_container">
                                <label className="input_field_title">Ngày về</label>
                                <div className="input_field_inner">
                                    <i className="bx bx-calendar-check"></i>
                                    <input type="date" min={today} value={singleFlight.returnDate} onChange={(e) => setSingleFlight({ ...singleFlight, returnDate: e.target.value })} required />
                                </div>
                            </div>
                        )}
                        <div className="search_submit_container">
                            <button type="submit" className="btn_search_orange">
                                <i className="bx bx-search-alt"></i>
                            </button>
                        </div>
                    </div>
                ) : (
                    <div className="multi_city_container">
                        <div className="multi_city_scroll_box">
                            {flights.map((flight, idx) => (
                                <div className="form_row_block_group multi_layout" key={flight.id}>
                                    <div className="input_field_container">
                                        <label className="input_field_title">Từ</label>
                                        <SearchableSelect
                                            className="airport_select airport_select_from"
                                            data={airports}
                                            value={flight.from}
                                            onChange={(val) => handleFlightChange(idx, "from", val)}
                                            placeholder="Chọn nơi đi..."
                                            itemKey="airportCode"
                                            displayValue={(a) => `${a.city} (${a.airportCode})`}
                                            searchFields={["city", "airportCode"]}
                                            renderItem={renderAirportItem}
                                        />
                                    </div>
                                    <div className="swap_btn_container">
                                        <button type="button" className="swap_locations_action" onClick={() => handleSwapMultiLocations(idx)} title="Đổi chiều">
                                            <i className="bx bx-transfer-alt"></i>
                                        </button>
                                    </div>
                                    <div className="input_field_container">
                                        <label className="input_field_title">Đến</label>
                                        <SearchableSelect
                                            className="airport_select airport_select_to"
                                            data={airports}
                                            value={flight.to}
                                            onChange={(val) => handleFlightChange(idx, "to", val)}
                                            placeholder="Chọn nơi đến..."
                                            itemKey="airportCode"
                                            displayValue={(a) => `${a.city} (${a.airportCode})`}
                                            searchFields={["city", "airportCode"]}
                                            renderItem={renderAirportItem}
                                        />
                                    </div>
                                    <div className="input_field_container">
                                        <label className="input_field_title">Ngày khởi hành</label>
                                        <div className="input_field_inner">
                                            <i className="bx bx-calendar"></i>
                                            <input type="date" min={today} value={flight.departureDate} onChange={(e) => handleFlightChange(idx, "departureDate", e.target.value)} required />
                                        </div>
                                    </div>

                                    <div className="input_field_container delete_action_container">
                                        {flights.length > 2 ? (
                                            <>
                                                <label className="input_field_title highlight_red_text">Xóa</label>
                                                <button type="button" className="btn_remove_flight" onClick={() => handleRemoveFlight(idx)} title="Xóa chặng này">
                                                    <i className="bx bx-trash"></i>
                                                </button>
                                            </>
                                        ) : (
                                            <div className="empty_block_placeholder"></div>
                                        )}
                                    </div>
                                </div>
                            ))}
                        </div>
                        <div className="multi_city_bottom_actions">
                            {flights.length < 5 ? (
                                <button type="button" className="btn_add_flight_dashed" onClick={handleAddFlight}>
                                    <i className="bx bx-plus-circle"></i> THÊM CHUYẾN BAY KHÁC ({flights.length}/5)
                                </button>
                            ) : (
                                <span className="max_flights_alert">
                                    <i className="bx bx-info-circle"></i> Giới hạn tối đa 5 chặng bay
                                </span>
                            )}
                            <button type="submit" className="btn_search_submit_large">
                                <i className="bx bx-search-alt"></i> Tìm chuyến bay
                            </button>
                        </div>
                    </div>
                )}
            </form>
        </div>
    );
}

export default SearchForm;
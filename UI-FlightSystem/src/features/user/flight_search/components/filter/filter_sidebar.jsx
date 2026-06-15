import { useState, useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { setFilters, resetFilters, fetchFilterData } from "../../search_results_slice";
import { TIME_SLOTS, STOP_OPTIONS } from "@/constants/search";
import "./filter_sidebar.css";

const FilterSection = ({ id, title, collapsed, onToggle, children }) => (
    <div className="filter_section">
        <div className="filter_section_title" onClick={() => onToggle(id)}>
            <h4>{title}</h4>
            <i className={`bx ${collapsed ? "bx-chevron-down" : "bx-chevron-up"}`} />
        </div>
        <div className={`collapsible${collapsed ? " collapsed" : ""}`}>
            {children}
        </div>
    </div>
);

function FilterSidebar() {
    const dispatch = useDispatch();
    const { filters, filterData } = useSelector((s) => s.searchResults);
    const [collapsed, setCollapsed] = useState({});
    const [departSlot, setDepartSlot] = useState(null);
    const [arriveSlot, setArriveSlot] = useState(null);

    useEffect(() => {
        if (!filterData.airlines.length && !filterData.services.length && !filterData.loading) {
            dispatch(fetchFilterData());
        }
    }, []);

    const toggleSection = (key) => setCollapsed((prev) => ({ ...prev, [key]: !prev[key] }));
    const handleStop = (value) => dispatch(setFilters({ stopCount: value }));
    const handleAirline = (id, checked) => dispatch(setFilters({ airlineId: checked ? id : null }));

    const handleDepartSlot = (i) => {
        const next = departSlot === i ? null : i;
        setDepartSlot(next);
        dispatch(setFilters({
            departureFromHour: next !== null ? TIME_SLOTS[next].from : null,
            departureToHour: next !== null ? TIME_SLOTS[next].to : null,
        }));
    };

    const handleArriveSlot = (i) => {
        const next = arriveSlot === i ? null : i;
        setArriveSlot(next);
        dispatch(setFilters({
            arrivalFromHour: next !== null ? TIME_SLOTS[next].from : null,
            arrivalToHour: next !== null ? TIME_SLOTS[next].to : null,
        }));
    };

    const handleReset = () => {
        setDepartSlot(null);
        setArriveSlot(null);
        dispatch(resetFilters());
    };

    const activeFilterCount = [
        filters.stopCount != null,
        filters.airlineId != null,
        (filters.serviceIds?.length ?? 0) > 0,
        filters.departureFromHour != null,
        filters.arrivalFromHour != null,
    ].filter(Boolean).length;

    return (
        <aside className="filter_sidebar">
            <div className="filter_header">
                <h3>Bộ lọc {activeFilterCount > 0 && (<span className="filter_active_badge">{activeFilterCount}</span>)}</h3>
                {activeFilterCount > 0 && (
                    <button className="btn_reset_filter" onClick={handleReset}>
                        <i className="bx bx-reset" /> Đặt lại
                    </button>
                )}
            </div>

            <FilterSection id="stops" title="Số điểm dừng" collapsed={collapsed.stops} onToggle={toggleSection}>
                <div className="filter_options_list">
                    {STOP_OPTIONS.map(({ label, value }) => (
                        <label key={label} className="radio_label">
                            <input type="radio" name="stopCount" checked={filters.stopCount === value} onChange={() => handleStop(value)} />
                            <span className="option_text">{label}</span>
                        </label>
                    ))}
                </div>
            </FilterSection>

            <FilterSection id="airlines" title="Hãng hàng không" collapsed={collapsed.airlines} onToggle={toggleSection}>
                <div className="filter_options_list">
                    {(filterData.airlines.map((airline) => (
                        <label key={airline.airlineId} className="checkbox_label">
                            <input type="checkbox" checked={filters.airlineId === airline.airlineId} onChange={(e) => handleAirline(airline.airlineId, e.target.checked)} />
                            <span className="option_text">{airline.airlineName}</span>
                        </label>
                    ))
                    )}
                </div>
            </FilterSection>

            <FilterSection id="time" title="Thời gian bay" collapsed={collapsed.time} onToggle={toggleSection}>
                <div className="filter_time_block">
                    <h5>Giờ cất cánh</h5>
                    <div className="time_grid">
                        {TIME_SLOTS.map((slot, i) => (
                            <button key={i} className={`time_btn${departSlot === i ? " active" : ""}`} onClick={() => handleDepartSlot(i)}>
                                <span>{slot.label}</span>
                                <span className="time_range">{slot.range}</span>
                            </button>
                        ))}
                    </div>

                    <h5>Giờ hạ cánh</h5>
                    <div className="time_grid">
                        {TIME_SLOTS.map((slot, i) => (
                            <button key={i} className={`time_btn${arriveSlot === i ? " active" : ""}`} onClick={() => handleArriveSlot(i)}>
                                <span>{slot.label}</span>
                                <span className="time_range">{slot.range}</span>
                            </button>
                        ))}
                    </div>
                </div>
            </FilterSection>
        </aside>
    );
}

export default FilterSidebar;
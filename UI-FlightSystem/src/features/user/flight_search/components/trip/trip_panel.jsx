import { useState, useEffect } from "react";
import { useSelector, useDispatch } from "react-redux";
import { useSearchParams } from "react-router-dom";
import { formatDateLong, formatTime } from "@/utils/date_utils";
import { fetchHomeAirports } from "@/features/user/homepage/homepage_slice";
import SearchForm from "@/features/user/homepage/components/search/search_form";
import "./trip_panel.css";

const LegIndex = ({ index, isActive, isDone }) => (
    <span className={["trip_leg_index", isActive ? "active" : "", isDone ? "done" : ""].filter(Boolean).join(" ")}>
        {isDone ? <i className="bx bx-check" /> : index + 1}
    </span>
);

const LegButton = ({ isActive, isLocked, onClick, children }) => (
    <button className={["trip_summary_leg_btn", isActive ? "active" : "", isLocked ? "locked" : ""].filter(Boolean).join(" ")} onClick={onClick} disabled={isLocked}>
        {children}
    </button>
);

const OneWaySummary = ({ searchParams }) => {
    const origin = searchParams.get("origin") ?? "";
    const dest = searchParams.get("destination") ?? "";
    const depart = searchParams.get("departureDate") ?? "";

    return (
        <LegButton isActive isLocked={false} onClick={() => { }}>
            <LegIndex index={0} isActive isDone={false} />
            <div className="trip_leg_info">
                {depart && (
                    <span className="trip_leg_date">{formatDateLong(depart)}</span>
                )}
                <span className="trip_leg_route">{origin} → {dest}</span>
            </div>
        </LegButton>
    );
};

const RoundTripSummary = ({ searchParams, activeLegView, selectedLegs, onSelectLeg }) => {
    const origin = searchParams.get("origin") ?? "";
    const dest = searchParams.get("destination") ?? "";
    const depart = searchParams.get("departureDate") ?? "";
    const retDate = searchParams.get("returnDate") ?? "";
    const legs = [
        { label: "Chuyến đi", route: `${origin} → ${dest}`, date: depart },
        { label: "Chuyến về", route: `${dest} → ${origin}`, date: retDate },
    ];

    return (
        <>
            {legs.map((leg, i) => {
                const isActive = activeLegView === i;
                const selected = selectedLegs.find((s) => s.legIndex === i);
                const isLocked = i > selectedLegs.length;

                return (
                    <LegButton key={i} isActive={isActive} isLocked={isLocked}
                        onClick={() => {
                            if (!isLocked && onSelectLeg) onSelectLeg(i);
                        }}>
                        <LegIndex index={i} isActive={isActive} isDone={!!selected} />
                        <div className="trip_leg_info">
                            <span className="trip_leg_date">{leg.label} · {leg.date ? formatDateLong(leg.date) : ""}</span>
                            <span className={`trip_leg_route${isLocked ? " locked_txt" : ""}`}>{leg.route}</span>
                            {selected && (
                                <span className="trip_leg_selected_info">
                                    <i className="bx bx-plane-take-off" />
                                    {formatTime(selected.flight.departureTime)} · {selected.flight.airlineName}
                                </span>
                            )}
                            {!selected && !isLocked && (<span className="trip_leg_pending">{isActive ? "Đang chọn chuyến bay..." : "Chưa chọn"}</span>)}
                        </div>
                    </LegButton>
                );
            })}
        </>
    );
};

const MultiCitySummary = ({ searchParams, activeLegView, selectedLegs, onSelectLeg }) => {
    const legsRaw = (() => {
        try { return JSON.parse(searchParams.get("legs") ?? "[]"); } catch { return []; }
    })();

    return (
        <>
            {legsRaw.map((leg, i) => {
                const isActive = activeLegView === i;
                const selected = selectedLegs.find((s) => s.legIndex === i);
                const isLocked = i > selectedLegs.length;

                return (
                    <LegButton key={i} isActive={isActive} isLocked={isLocked}
                        onClick={() => {
                            if (!isLocked && onSelectLeg) onSelectLeg(i);
                        }}>
                        <LegIndex index={i} isActive={isActive} isDone={!!selected} />
                        <div className="trip_leg_info">
                            <span className="trip_leg_date">Chặng {i + 1} · {leg.departureDate ? formatDateLong(leg.departureDate) : ""}</span>
                            <span className={`trip_leg_route${isLocked ? " locked_txt" : ""}`}>{leg.from} → {leg.to}</span>
                            {selected && (
                                <span className="trip_leg_selected_info">
                                    <i className="bx bx-plane-take-off" />
                                    {formatTime(selected.flight.departureTime)} · {selected.flight.airlineName}
                                </span>
                            )}
                            {!selected && !isLocked && (<span className="trip_leg_pending">{isActive ? "Đang chọn chuyến bay..." : "Chưa chọn"}</span>)}
                        </div>
                    </LegButton>
                );
            })}
        </>
    );
};

function TripSummaryPanel({ activeLegView, onSelectLeg }) {
    const dispatch = useDispatch();
    const [searchParams] = useSearchParams();
    const { selectedLegs } = useSelector((s) => s.searchResults);
    const { airports } = useSelector((state) => state.homepage) ?? { airports: [] };
    const type = searchParams.get("type") ?? "one-way";
    const [isModalOpen, setIsModalOpen] = useState(false);

    useEffect(() => {
        if (!airports || airports.length === 0) {
            dispatch(fetchHomeAirports());
        }
    }, [dispatch, airports]);

    return (
        <div className="trip_summary_panel">
            <div className="trip_summary_header">
                <div className="trip_summary_header_left">
                    <i className="bx bx-trip" />
                    <span>Chuyến bay của bạn</span>
                </div>
                <button className="modify_search_btn" onClick={() => setIsModalOpen(true)}>
                    <i className="bx bx-edit-alt" />
                </button>
            </div>

            {type === "one-way" && <OneWaySummary searchParams={searchParams} />}
            {type === "round-trip" && <RoundTripSummary searchParams={searchParams} activeLegView={activeLegView} selectedLegs={selectedLegs} onSelectLeg={onSelectLeg} />}
            {type === "multi-city" && <MultiCitySummary searchParams={searchParams} activeLegView={activeLegView} selectedLegs={selectedLegs} onSelectLeg={onSelectLeg} />}

            {isModalOpen && (
                <div className="search_popup_overlay" onClick={() => setIsModalOpen(false)}>
                    <div className="search_popup_content" onClick={(e) => e.stopPropagation()}>
                        <div className="search_popup_header">
                            <h3><i className="bx bx-search-alt"></i> Thay đổi lịch trình tìm kiếm</h3>
                            <button className="close_popup_btn" onClick={() => setIsModalOpen(false)}>
                                <i className="bx bx-x"></i>
                            </button>
                        </div>
                        <div className="search_popup_body">
                            <SearchForm airports={airports} isModalView={true} onSuccess={() => setIsModalOpen(false)} />
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
}

export default TripSummaryPanel;
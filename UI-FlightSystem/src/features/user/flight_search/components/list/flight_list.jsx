import { useSearchParams } from "react-router-dom";
import { useSelector } from "react-redux";
import FlightCard from "../card/flight_card";
import "./flight_list.css";

function FlightList({ activeLegView, onFlightSelected }) {
    const [searchParams] = useSearchParams();
    const { outbound, inbound, legs, selectedLegs } = useSelector((s) => s.searchResults);
    const type = searchParams.get("type") ?? "one-way";

    const currentList = (() => {
        if (type === "round-trip") return activeLegView === 0 ? outbound : inbound;
        if (type === "multi-city") return legs[activeLegView] ?? { items: [], loading: false, error: null };
        return outbound;
    })();

    const handleSelect = (flight) => { onFlightSelected(flight) };

    const renderList = (list, withSelect = true) => {
        if (list.loading)
            return <div className="list_state"><i className="bx bx-loader-alt bx-spin" /> Đang tìm kiếm...</div>;
        if (list.error)
            return <div className="list_state list_error"><i className="bx bx-error-circle" /> {list.error}</div>;
        if (!list.items?.length)
            return <div className="list_state list_empty"><i className="bx bx-plane-land" /> Không tìm thấy chuyến bay phù hợp.</div>;

        return list.items.map((flight) => {
            const isSelected = selectedLegs.some(
                (s) => s.legIndex === activeLegView && s.flight.flightId === flight.flightId
            );

            return (
                <FlightCard key={flight.flightId} flight={flight} onSelect={withSelect ? () => handleSelect(flight) : undefined} isSelected={isSelected}/>
            );
        });
    };

    return (
        <div className="flight_list_wrapper">
            {type === "multi-city" && (() => {
                const legsRaw = (() => {
                    try { return JSON.parse(searchParams.get("legs") ?? "[]"); } catch { return []; }
                })();
                const leg = legsRaw[activeLegView];
                if (!leg) return null;
                return (
                    <div className="active_leg_header">
                        <i className="bx bx-map-alt" />
                        Chặng {activeLegView + 1}: <strong>{leg.from} → {leg.to}</strong>
                        <span className="active_leg_date">{leg.departureDate}</span>
                    </div>
                );
            })()}
            <div className="flight_cards_list">
                {renderList(currentList, true)}
            </div>
        </div>
    );
}

export default FlightList;
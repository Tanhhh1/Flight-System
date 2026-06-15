import { useSearchParams } from "react-router-dom";
import { formatTime } from "@/utils/date_utils";
import "./select_flight.css";

function SelectedFlightsBar({ selectedLegs, totalLegs, onRemove, onContinue }) {
    const [searchParams] = useSearchParams();
    const type = searchParams.get("type") ?? "one-way";
    const allSelected = selectedLegs.length === totalLegs;

    if (selectedLegs.length === 0) return null;

    const getLegLabel = (legIndex) => {
        if (type === "one-way") return "Chuyến bay";
        if (type === "round-trip") return legIndex === 0 ? "Chuyến đi" : "Chuyến về";
        return `Chặng ${legIndex + 1}`;
    };

    return (
        <div className="selected_flights_bar">
            <div className="selected_flights_bar_inner">
                <div className="selected_legs_list">
                    {selectedLegs.map(({ legIndex, flight }) => (
                        <div className="selected_leg_chip" key={legIndex}>
                            <div className="selected_leg_chip_info">
                                <span className="selected_leg_label">{getLegLabel(legIndex)}</span>
                                <span className="selected_leg_route">
                                    {flight.originAirportCode} → {flight.destinationAirportCode}
                                </span>
                                <span className="selected_leg_detail">
                                    <i className="bx bx-plane-take-off" />
                                    {formatTime(flight.departureTime)} · {flight.airlineName}
                                </span>
                            </div>
                            <button className="selected_leg_remove_btn" onClick={() => onRemove(legIndex)} title="Bỏ chọn chặng này">
                                <i className="bx bx-x" />
                            </button>
                        </div>
                    ))}

                    {Array.from({ length: totalLegs - selectedLegs.length }).map((_, i) => (
                        <div className="selected_leg_chip selected_leg_chip_empty" key={`empty_${i}`}>
                            <i className="bx bx-question-mark" />
                            <span>{getLegLabel(selectedLegs.length + i)} — Chưa chọn</span>
                        </div>
                    ))}
                </div>

                <button className={`btn_continue_booking${allSelected ? " ready" : ""}`} disabled={!allSelected} onClick={onContinue}>
                    {allSelected
                        ? <><i className="bx bx-check-circle" /> Tiếp tục đặt vé</>
                        : <><i className="bx bx-loader-circle" /> Chọn đủ {totalLegs} chặng ({selectedLegs.length}/{totalLegs})</>
                    }
                </button>
            </div>
        </div>
    );
}

export default SelectedFlightsBar;
import "./passenger_panel.css";

function PassengerPanel({ passengers, selectedPassengerId, activePending, onSelectPassenger }) {
    return (
        <div className="passenger_panel">
            <div className="passenger_panel_header">
                <i className="bx bx-group"></i>
                <span>Hành khách</span>
            </div>
            <div className="passenger_panel_list">
                {passengers.map((p) => {
                    const pending = Array.isArray(activePending)
                        ? activePending.find(a => a.passengerId === p.passengerId)
                        : null;
                    const isSelected = selectedPassengerId === p.passengerId;

                    return (
                        <button key={p.passengerId} className={`passenger_card ${isSelected ? "passenger_card_active" : ""} ${pending ? "passenger_card_assigned" : ""}`} onClick={() => onSelectPassenger(isSelected ? null : p.passengerId)}>
                            <div className="passenger_card_info">
                                <span className="passenger_card_name">{p.fullName}</span>
                                <span className="passenger_card_seat">
                                    {pending
                                        ? <><i className="bx bx-check-circle" /> {pending.seatNumber}</>
                                        : <><i className="bx bx-minus-circle" /> Chưa chọn ghế</>
                                    }
                                </span>
                            </div>
                            {isSelected && (
                                <span className="passenger_card_selecting_badge">Đang chọn</span>
                            )}
                        </button>
                    );
                })}
            </div>
        </div>
    );
}

export default PassengerPanel;
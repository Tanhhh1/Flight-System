import SeatClassGroup from "../section/seat_class";
import "./seat_map.css";

function SeatMap({ seatMap, selectedPassengerId, getPendingPassengerForSeat, onSeatClick, isLoading, error }) {
    if (isLoading) {
        return (
            <div className="seat_map_state">
                <i className="bx bx-loader-alt bx-spin seat_map_state_icon" />
                <span>Đang tải sơ đồ ghế...</span>
            </div>
        );
    }

    if (error) {
        return (
            <div className="seat_map_state seat_map_state_error">
                <i className="bx bx-error-circle seat_map_state_icon" />
                <span>{error}</span>
            </div>
        );
    }

    if (!seatMap) return null;

    return (
        <div className="seat_map">
            <div className="seat_map_legend">
                <div className="legend_item">
                    <span className="legend_dot legend_dot_available" />
                    <span>Trống</span>
                </div>
                <div className="legend_item">
                    <span className="legend_dot legend_dot_mine" />
                    <span>Đang chọn</span>
                </div>
                <div className="legend_item">
                    <span className="legend_dot legend_dot_locked" />
                    <span>Đang giữ</span>
                </div>
                <div className="legend_item">
                    <span className="legend_dot legend_dot_booked" />
                    <span>Đã đặt</span>
                </div>
            </div>

            <div className="seat_map_classes">
                {seatMap.classGroups.map((group, idx) => (
                    <div key={group.classId} className="seat_map_class_wrapper">
                        {idx > 0 && <div className="seat_map_class_divider" />}
                        <SeatClassGroup
                            group={group}
                            selectedPassengerId={selectedPassengerId}
                            getPendingPassengerForSeat={getPendingPassengerForSeat}
                            onSeatClick={onSeatClick}
                            disabled={!selectedPassengerId}
                        />
                    </div>
                ))}
            </div>
        </div>
    );
}

export default SeatMap;
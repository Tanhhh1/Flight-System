import React from "react";
import SeatCell from "../cell/seat_cell";
import "./seat_class.css";

const AISLE_AFTER_COL = 3;

function SeatClassGroup({ group, selectedPassengerId, getPendingPassengerForSeat, onSeatClick, disabled }) {
    const maxCol = Math.max(...group.rows.flatMap(r => r.seats.map(s => s.colIndex)));
    const hasAisle = maxCol >= 5;

    return (
        <div className="seat_class_group">
            <div className="seat_class_group_header">
                <span className="seat_class_group_badge">{group.className}</span>
            </div>

            <div className="seat_class_group_col_labels">
                {Array.from({ length: maxCol }, (_, i) => i + 1).map(col => (
                    <React.Fragment key={`col-label-${group.classId}-${col}`}>
                        {hasAisle && col === AISLE_AFTER_COL + 1 && (
                            <div className="seat_class_group_aisle_gap" />
                        )}
                        <div className="seat_class_group_col_label">
                            {String.fromCharCode(64 + col)}
                        </div>
                    </React.Fragment>
                ))}
            </div>

            <div className="seat_class_group_rows">
                {group.rows.map(row => (
                    <div key={`row-${group.classId}-${row.rowIndex}`} className="seat_row">
                        <span className="seat_row_index">{row.rowIndex}</span>
                        <div className="seat_row_seats">
                            {row.seats.map(seat => {
                                const pending = getPendingPassengerForSeat(seat.seatId);
                                const isSelectedByMe = pending?.passengerId === selectedPassengerId;

                                return (
                                    <React.Fragment key={`seat-${group.classId}-${row.rowIndex}-${seat.seatId}`}>
                                        {hasAisle && seat.colIndex === AISLE_AFTER_COL + 1 && (
                                            <div className="seat_class_group_aisle_gap" />
                                        )}
                                        <SeatCell
                                            seat={seat}
                                            isSelectedByMe={isSelectedByMe}
                                            pendingPassenger={pending}
                                            onClick={onSeatClick}
                                            disabled={disabled}
                                        />
                                    </React.Fragment>
                                );
                            })}
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
}

export default SeatClassGroup;
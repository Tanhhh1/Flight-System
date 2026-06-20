import { SEAT_STATUS } from "@/constants/seat_selection";
import "./seat_cell.css";

function SeatCell({ seat, isSelectedByMe, onClick, disabled }) {
    const { status, seatNumber, lockedByPassengerId } = seat;

    const getModifier = () => {
        if (status === SEAT_STATUS.BOOKED) return "booked";
        if (isSelectedByMe) return "mine";
        if (status === SEAT_STATUS.LOCKED && lockedByPassengerId) return "locked_other";
        if (status === SEAT_STATUS.LOCKED) return "locked";
        return "available";
    };

    const modifier = getModifier();
    const isClickable = modifier === "available" || modifier === "mine";

    return (
        <button className={`seat_cell seat_cell_${modifier}`} onClick={() => isClickable && !disabled && onClick(seat)} disabled={!isClickable || disabled}>
            <span className="seat_cell_number">{seatNumber}</span>
            {modifier === "mine" && <i className="bx bx-check seat_cell_icon" />}
        </button>
    );
}

export default SeatCell;
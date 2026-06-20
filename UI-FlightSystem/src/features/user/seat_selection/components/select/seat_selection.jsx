import { useState, useCallback, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { useDispatch } from "react-redux";
import { useSeatReverse } from "../../use_seat_selection";
import { useSeatHub } from "../../use_seat_hub";
import PassengerPanel from "../list/passenger_panel";
import SeatMap from "../map/seat_map";
import AlertModal from "@/components/error/alert_modal";
import { clientPaths } from "@/configs/client_routes";
import { resetConfirmed } from "../../seat_selection_slice";
import "./seat_selection.css";

function ConfirmDialog({ message, onConfirm, onCancel }) {
    return (
        <div className="seat_dialog_overlay">
            <div className="seat_dialog">
                <p className="seat_dialog_message">{message}</p>
                <div className="seat_dialog_actions">
                    <button className="seat_dialog_btn_cancel" onClick={onCancel}>Hủy</button>
                    <button className="seat_dialog_btn_confirm" onClick={onConfirm}>Xác nhận</button>
                </div>
            </div>
        </div>
    );
}

function SeatSelection() {
    const navigate = useNavigate();
    const dispatch = useDispatch();

    const {
        bookingInfo, activeFlightId, activeSeatMap, selectedPassengerId,
        activePassengers, activePending, allPending, isLoadingMap,
        isConfirming, mapError, actionError, isConfirmed,
        getPendingPassengerForSeat, handleRestore, handleFetchSeatMap,
        handleSelectFlight, handleSelectPassenger, handleSeatClick,
        handleConfirm, handleReset, handleClearActionError,
    } = useSeatReverse();

    const [dialog, setDialog] = useState(null);
    const [alertState, setAlertState] = useState({ isOpen: false, message: "", type: "success" });

    useSeatHub(activeFlightId);
    useEffect(() => {
        if (!bookingInfo) {
            handleRestore().catch(() => navigate(`${clientPaths.profile.root}/${clientPaths.profile.verify}`));
        }
    }, []);

    useEffect(() => {
        if (activeFlightId && !activeSeatMap) {
            handleFetchSeatMap(activeFlightId);
        }
    }, [activeFlightId]);

    useEffect(() => {
        if (isConfirmed) {
            setAlertState({ isOpen: true, message: "Đặt ghế thành công!", type: "success" });
            dispatch(resetConfirmed());
        }
    }, [isConfirmed, dispatch]);

    useEffect(() => {
        if (actionError) {
            setAlertState({ isOpen: true, message: actionError, type: "error" });
            handleClearActionError();
        }
    }, [actionError, handleClearActionError]);

    const handleAlertClose = useCallback(() => {
        const wasSuccess = alertState.type === "success";
        setAlertState({ isOpen: false, message: "", type: "success" });
        if (wasSuccess) {handleRestore().then(() => {if (activeFlightId) handleFetchSeatMap(activeFlightId)})}
    }, [alertState.type, handleRestore, handleFetchSeatMap, activeFlightId]);

    const handleSeatClickWithConfirm = useCallback((seat) => {
        if (!selectedPassengerId) return;
        const passenger = activePassengers.find(p => p.passengerId === selectedPassengerId);
        const passengerName = passenger?.fullName ?? "hành khách";

        if (seat.lockedByPassengerId === selectedPassengerId) {
            const existingAssignment = Array.isArray(activePending) ? activePending.find(a => a.passengerId === selectedPassengerId) : null;
            if (existingAssignment?.isBooked) return;

            setDialog({
                message: `Bỏ chọn ghế ${seat.seatNumber} cho ${passengerName}?`,
                onConfirm: () => { setDialog(null); handleSeatClick(seat); },
            });
            return;
        }
        setDialog({message: `Giữ ghế ${seat.seatNumber} cho ${passengerName}?`, onConfirm: () => { setDialog(null); handleSeatClick(seat); }});
    }, [selectedPassengerId, activePassengers, activePending, handleSeatClick]);

    const handleConfirmWithDialog = useCallback(() => {
        const totalPending = Object.values(allPending).flat().filter(a => !a.isBooked).length;
        if (totalPending === 0) return;
        setDialog({message: `Xác nhận đặt ${totalPending} ghế cho booking này?`, onConfirm: () => { setDialog(null); handleConfirm(); }});
    }, [allPending, handleConfirm]);

    if (!bookingInfo) {
        return (
            <div className="seat_selection_loading">
                <i className="bx bx-loader-alt bx-spin" />
                <span>Đang tải thông tin đặt vé...</span>
            </div>
        );
    }

    const flights = bookingInfo.flights ?? [];
    const totalPendingToConfirm = Object.values(allPending).flat().filter(a => !a.isBooked).length;

    return (
        <div className="seat_selection">
            {alertState.isOpen && (
                <AlertModal
                    type={alertState.type}
                    message={alertState.message}
                    onClose={handleAlertClose}
                />
            )}

            {dialog && (
                <ConfirmDialog
                    message={dialog.message}
                    onConfirm={dialog.onConfirm}
                    onCancel={() => setDialog(null)}
                />
            )}

            <div className="seat_selection_header">
                <div className="seat_selection_header_info">
                    <h2 className="seat_selection_title">
                        <i className="bx bxs-plane-alt"></i>
                        Chọn ghế ngồi
                    </h2>
                    <span className="seat_selection_booking_code">
                        Mã đặt vé: <strong>{bookingInfo.bookingCode}</strong>
                    </span>
                </div>
                <span className="seat_selection_class_badge">{bookingInfo.className}</span>
            </div>
            {flights.length > 1 && (
                <div className="seat_selection_flight_tabs">
                    {flights.map((f) => (
                        <button key={f.flightId} className={`flight_tab ${activeFlightId === f.flightId ? "flight_tab_active" : ""}`} onClick={() => handleSelectFlight(f.flightId)}>
                            {f.originCode} → {f.destinationCode}
                            {allPending[f.flightId]?.filter(a => !a.isBooked).length > 0 && (
                                <span className="flight_tab_badge">
                                    {allPending[f.flightId].filter(a => !a.isBooked).length}
                                </span>
                            )}
                        </button>
                    ))}
                </div>
            )}
            <div className="seat_selection_body">
                <div className="seat_selection_map_wrapper">
                    <SeatMap
                        seatMap={activeSeatMap}
                        selectedPassengerId={selectedPassengerId}
                        getPendingPassengerForSeat={getPendingPassengerForSeat}
                        onSeatClick={handleSeatClickWithConfirm}
                        isLoading={isLoadingMap}
                        error={mapError}
                    />
                </div>
                <div className="seat_selection_sidebar">
                    <PassengerPanel
                        passengers={activePassengers}
                        selectedPassengerId={selectedPassengerId}
                        activePending={activePending}
                        onSelectPassenger={handleSelectPassenger}
                    />
                </div>
            </div>

            <div className="seat_selection_footer">
                <span className="seat_selection_summary">
                    Chờ xác nhận: <strong>{totalPendingToConfirm}</strong> ghế
                </span>
                <div className="seat_selection_actions">
                    <button className="seat_selection_btn_cancel" onClick={() => { handleReset(); navigate(`${clientPaths.profile.root}/${clientPaths.profile.verify}`); }} disabled={isConfirming}>
                        <i className="bx bx-arrow-back"></i> Quay lại
                    </button>
                    <button className="seat_selection_btn_confirm" onClick={handleConfirmWithDialog} disabled={isConfirming || totalPendingToConfirm === 0}>
                        <i className={`bx ${isConfirming ? "bx-loader-alt bx-spin" : "bx-check"}`}></i>
                        {isConfirming ? " Đang xác nhận..." : " Xác nhận đặt ghế"}
                    </button>
                </div>
            </div>
        </div>
    );
}

export default SeatSelection;
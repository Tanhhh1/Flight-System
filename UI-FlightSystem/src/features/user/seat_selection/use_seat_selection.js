import { useDispatch, useSelector } from "react-redux";
import { useCallback } from "react";
import {
    verifyBookingCode, restoreBookingFromSession, fetchSeatMap,
    holdSeat, releaseSeat, confirmSeats,
    setActiveFlightId, setSelectedPassenger, setActionError,
    clearActionError, clearVerifyError, resetSeatReverse,
    selectBookingInfo, selectActiveFlightId, selectActiveSeatMap,
    selectSelectedPassengerId, selectActivePendingAssignments,
    selectPendingAssignments, selectIsVerifying, selectIsLoadingMap,
    selectIsHolding, selectIsConfirming, selectVerifyError,
    selectMapError, selectActionError, selectIsConfirmed,
} from "./seat_selection_slice";

export function useSeatReverse() {
    const dispatch = useDispatch();

    const bookingInfo = useSelector(selectBookingInfo);
    const activeFlightId = useSelector(selectActiveFlightId);
    const activeSeatMap = useSelector(selectActiveSeatMap);
    const selectedPassengerId = useSelector(selectSelectedPassengerId);
    const activePending = useSelector(selectActivePendingAssignments);
    const allPending = useSelector(selectPendingAssignments);
    const isVerifying = useSelector(selectIsVerifying);
    const isLoadingMap = useSelector(selectIsLoadingMap);
    const isHolding = useSelector(selectIsHolding);
    const isConfirming = useSelector(selectIsConfirming);
    const verifyError = useSelector(selectVerifyError);
    const mapError = useSelector(selectMapError);
    const actionError = useSelector(selectActionError);
    const isConfirmed = useSelector(selectIsConfirmed);

    const activePassengers = bookingInfo?.flights?.find(f => f.flightId === activeFlightId)?.passengers ?? [];
    const selectedPassenger = activePassengers.find(p => p.passengerId === selectedPassengerId) ?? null;
    const selectedPassengerPendingSeat = Array.isArray(activePending) ? (activePending.find(a => a.passengerId === selectedPassengerId) ?? null) : null;
    const getPendingPassengerForSeat = useCallback(
        (seatId) => {
            if (!Array.isArray(activePending)) return null;
            return activePending.find(a => a.seatId === seatId) ?? null;
        },
        [activePending]
    );

    const handleVerify = useCallback((bookingCode) => dispatch(verifyBookingCode(bookingCode)), [dispatch]);
    const handleRestore = useCallback(() => dispatch(restoreBookingFromSession()), [dispatch]);

    const handleFetchSeatMap = useCallback(
        (flightId) => {
            if (!bookingInfo?.bookingId) return;
            dispatch(fetchSeatMap({ flightId, bookingId: bookingInfo.bookingId }));
        },
        [dispatch, bookingInfo]
    );

    const handleSelectFlight = useCallback((flightId) => dispatch(setActiveFlightId(flightId)), [dispatch]);
    const handleSelectPassenger = useCallback((passengerId) => dispatch(setSelectedPassenger(passengerId)), [dispatch]);

    const handleSeatClick = useCallback(
        (seat) => {
            if (!selectedPassengerId || !bookingInfo?.bookingId) return;
            if (seat.status === 2) return;
            if (seat.status === 1 && seat.lockedByPassengerId !== selectedPassengerId) return;
            const existingAssignment = Array.isArray(activePending) ? activePending.find(a => a.passengerId === selectedPassengerId) : null;

            if (existingAssignment?.isBooked) {
                dispatch(setActionError(
                    `Hành khách đã được xác nhận ghế ${existingAssignment.seatNumber}. Không thể thay đổi.`
                ));
                return;
            }

            if (seat.lockedByPassengerId === selectedPassengerId) {
                dispatch(releaseSeat({
                    bookingId: bookingInfo.bookingId,
                    flightId: activeFlightId,
                    flightSeatId: existingAssignment?.flightSeatId ?? seat.flightSeatId,
                    seatId: existingAssignment?.seatId ?? seat.seatId,
                    passengerId: selectedPassengerId,
                }));
                return;
            }

            if (existingAssignment) {
                dispatch(setActionError(`Hành khách đang giữ ghế ${existingAssignment.seatNumber}. ` + `Vui lòng bỏ chọn ghế đó trước khi chọn ghế mới`));
                return;
            }

            dispatch(holdSeat({
                bookingId: bookingInfo.bookingId,
                flightId: activeFlightId,
                seatId: seat.seatId,
                passengerId: selectedPassengerId,
            }));
        },
        [dispatch, selectedPassengerId, bookingInfo, activeFlightId, activePending]
    );

    const handleConfirm = useCallback(
        () => {
            if (!bookingInfo?.bookingId) return;
            dispatch(confirmSeats({
                bookingId: bookingInfo.bookingId,
                assignments: allPending,
            }));
        },
        [dispatch, bookingInfo, allPending]
    );
    const handleReset = useCallback(() => dispatch(resetSeatReverse()), [dispatch]);
    const handleClearActionError = useCallback(() => dispatch(clearActionError()), [dispatch]);
    const handleClearVerifyError = useCallback(() => dispatch(clearVerifyError()), [dispatch]);

    return {
        bookingInfo, activeFlightId, activeSeatMap, selectedPassengerId,
        selectedPassenger, activePassengers, activePending, allPending,
        selectedPassengerPendingSeat, isVerifying, isLoadingMap, isHolding,
        isConfirming, verifyError, mapError, actionError, isConfirmed,
        getPendingPassengerForSeat, handleVerify, handleRestore, handleFetchSeatMap,
        handleSelectFlight, handleSelectPassenger, handleSeatClick,
        handleConfirm, handleReset, handleClearActionError, handleClearVerifyError,
    };
}
import { createSlice, createAsyncThunk, createSelector } from "@reduxjs/toolkit";
import { seatReverseService } from "./seat_selection_service";
import { SEAT_REVERSE_STORAGE_KEY, SEAT_STATUS } from "@/constants/seat_selection";

const parseError = (err) =>
    err?.response?.data?.errors?.[0]?.errorMessage ??
    err?.response?.data?.message ??
    err?.message ??
    "Đã xảy ra lỗi hệ thống";

const saveToSession = (bookingId, bookingCode) => {
    sessionStorage.setItem(
        SEAT_REVERSE_STORAGE_KEY,
        JSON.stringify({ bookingId, bookingCode })
    );
};

const loadFromSession = () => {
    try {
        const raw = sessionStorage.getItem(SEAT_REVERSE_STORAGE_KEY);
        return raw ? JSON.parse(raw) : null;
    } catch { return null; }
};

const clearSession = () => sessionStorage.removeItem(SEAT_REVERSE_STORAGE_KEY);

const buildPendingFromBooking = (bookingInfo) => {
    const pending = {};
    if (!bookingInfo?.flights) return pending;

    bookingInfo.flights.forEach(flight => {
        const bookedPassengers = flight.passengers.filter(
            p => p.flightSeatId && p.flightSeatId > 0 && p.seatNumber
        );
        if (bookedPassengers.length > 0) {
            pending[flight.flightId] = bookedPassengers.map(p => ({
                passengerId: p.passengerId,
                flightSeatId: p.flightSeatId,
                seatId: 0,
                seatNumber: p.seatNumber,
                isBooked: true,
            }));
        }
    });
    return pending;
};

export const verifyBookingCode = createAsyncThunk(
    "seatReverse/verifyBookingCode",
    async (bookingCode, { rejectWithValue }) => {
        try {
            const { data } = await seatReverseService.verifyBookingCode(bookingCode);
            if (!data.succeeded)
                return rejectWithValue(data.errors?.[0]?.errorMessage ?? "Booking code không hợp lệ");
            saveToSession(data.result.bookingId, bookingCode);
            return data.result;
        } catch (err) {
            return rejectWithValue(parseError(err));
        }
    }
);

export const restoreBookingFromSession = createAsyncThunk(
    "seatReverse/restoreFromSession",
    async (_, { rejectWithValue }) => {
        const session = loadFromSession();
        if (!session?.bookingCode) return rejectWithValue("Không có session");
        try {
            const { data } = await seatReverseService.verifyBookingCode(session.bookingCode);
            if (!data.succeeded) {
                clearSession();
                return rejectWithValue("Session không còn hợp lệ");
            }
            return data.result;
        } catch (err) {
            clearSession();
            return rejectWithValue(parseError(err));
        }
    }
);

export const fetchSeatMap = createAsyncThunk(
    "seatReverse/fetchSeatMap",
    async ({ flightId, bookingId }, { rejectWithValue }) => {
        try {
            const { data } = await seatReverseService.getSeatMap(flightId, bookingId);
            if (!data.succeeded)
                return rejectWithValue(data.errors?.[0]?.errorMessage ?? "Không lấy được sơ đồ ghế");
            return { flightId, seatMap: data.result };
        } catch (err) {
            return rejectWithValue(parseError(err));
        }
    }
);

export const holdSeat = createAsyncThunk(
    "seatReverse/holdSeat",
    async ({ bookingId, flightId, seatId, passengerId }, { rejectWithValue }) => {
        try {
            const { data } = await seatReverseService.holdSeat({
                bookingId, flightId, seatId, passengerId
            });
            if (!data.succeeded)
                return rejectWithValue(data.errors?.[0]?.errorMessage ?? "Giữ ghế thất bại");
            return {
                flightSeatId: data.result.flightSeatId,
                flightId: data.result.flightId,
                seatId: data.result.seatId,
                seatNumber: data.result.seatNumber,
                passengerId: data.result.passengerId,
            };
        } catch (err) {
            return rejectWithValue(parseError(err));
        }
    }
);

export const releaseSeat = createAsyncThunk(
    "seatReverse/releaseSeat",
    async ({ bookingId, flightId, flightSeatId, seatId, passengerId }, { rejectWithValue }) => {
        try {
            const { data } = await seatReverseService.releaseSeat({
                bookingId, flightId, flightSeatId, passengerId
            });
            if (!data.succeeded)
                return rejectWithValue(data.errors?.[0]?.errorMessage ?? "Bỏ chọn ghế thất bại");
            return { flightId, flightSeatId, seatId, passengerId };
        } catch (err) {
            return rejectWithValue(parseError(err));
        }
    }
);

export const confirmSeats = createAsyncThunk(
    "seatReverse/confirmSeats",
    async ({ bookingId, assignments }, { rejectWithValue }) => {
        try {
            const formattedAssignments = {};
            Object.entries(assignments).forEach(([flightId, list]) => {
                const toConfirm = list.filter(a => !a.isBooked);
                if (toConfirm.length > 0) {
                    formattedAssignments[flightId] = toConfirm.map(a => ({
                        passengerId: a.passengerId,
                        flightSeatId: a.flightSeatId,
                    }));
                }
            });

            if (Object.keys(formattedAssignments).length === 0)
                return rejectWithValue("Không có ghế nào cần xác nhận.");

            const { data } = await seatReverseService.confirmSeats({
                bookingId,
                assignments: formattedAssignments,
            });
            if (!data.succeeded)
                return rejectWithValue(data.errors?.[0]?.errorMessage ?? "Xác nhận đặt ghế thất bại");
            return data.result;
        } catch (err) {
            return rejectWithValue(parseError(err));
        }
    }
);

const initialState = {
    bookingInfo: null,
    activeFlightId: null,
    seatMaps: {},
    selectedPassengerId: null,
    pendingAssignments: {},
    isVerifying: false,
    isLoadingMap: false,
    isHolding: false,
    isReleasing: false,
    isConfirming: false,
    verifyError: null,
    mapError: null,
    actionError: null,
    isConfirmed: false,
};

const seatReverseSlice = createSlice({
    name: "seatReverse",
    initialState,
    reducers: {
        setActiveFlightId(state, action) {
            state.activeFlightId = action.payload;
            state.selectedPassengerId = null;
        },
        setSelectedPassenger(state, action) {
            state.selectedPassengerId = action.payload;
        },
        setActionError(state, action) {
            state.actionError = action.payload;
        },
        clearActionError(state) {
            state.actionError = null;
        },
        clearVerifyError(state) {
            state.verifyError = null;
        },
        updateSeatStatusFromHub(state, action) {
            const { flightId, seatId, status, lockedByPassengerId, flightSeatId } = action.payload;
            const map = state.seatMaps[flightId];
            if (!map) return;
            for (const group of map.classGroups) {
                for (const row of group.rows) {
                    const seat = row.seats.find(s => s.seatId === seatId);
                    if (seat) {
                        seat.status = status;
                        seat.lockedByPassengerId = lockedByPassengerId ?? null;
                        if (flightSeatId) seat.flightSeatId = flightSeatId;
                        return;
                    }
                }
            }
        },
        resetConfirmed(state) {
            state.isConfirmed = false;
            state.actionError = null;
        },
        resetSeatReverse() {
            clearSession();
            return initialState;
        },
    },

    extraReducers: (builder) => {
        builder
            .addCase(verifyBookingCode.pending, (state) => {
                state.isVerifying = true;
                state.verifyError = null;
            })
            .addCase(verifyBookingCode.fulfilled, (state, action) => {
                state.isVerifying = false;
                state.bookingInfo = action.payload;
                state.activeFlightId = action.payload.flights?.[0]?.flightId ?? null;
                state.pendingAssignments = buildPendingFromBooking(action.payload);
                state.isConfirmed = false;
            })
            .addCase(verifyBookingCode.rejected, (state, action) => {
                state.isVerifying = false;
                state.verifyError = action.payload;
            });

        builder
            .addCase(restoreBookingFromSession.fulfilled, (state, action) => {
                state.bookingInfo = action.payload;
                state.activeFlightId = action.payload.flights?.[0]?.flightId ?? null;
                state.pendingAssignments = buildPendingFromBooking(action.payload);
                state.isConfirmed = false;
            })
            .addCase(restoreBookingFromSession.rejected, (state) => {
                state.bookingInfo = null;
            });

        builder
            .addCase(fetchSeatMap.pending, (state) => {
                state.isLoadingMap = true;
                state.mapError = null;
            })
            .addCase(fetchSeatMap.fulfilled, (state, action) => {
                state.isLoadingMap = false;
                const { flightId, seatMap } = action.payload;
                state.seatMaps[flightId] = seatMap;

                const flight = state.bookingInfo?.flights?.find(f => f.flightId === flightId);
                if (!flight) return;

                const passengerIds = new Set(flight.passengers.map(p => p.passengerId));

                if (!state.pendingAssignments[flightId])
                    state.pendingAssignments[flightId] = [];

                for (const group of seatMap.classGroups) {
                    for (const row of group.rows) {
                        for (const seat of row.seats) {
                            if (
                                seat.status === SEAT_STATUS.LOCKED &&
                                seat.lockedByPassengerId != null &&
                                passengerIds.has(seat.lockedByPassengerId)
                            ) {
                                const existingIdx = state.pendingAssignments[flightId]
                                    .findIndex(a => a.passengerId === seat.lockedByPassengerId);

                                const lockedAssignment = {
                                    passengerId: seat.lockedByPassengerId,
                                    flightSeatId: seat.flightSeatId,
                                    seatId: seat.seatId,
                                    seatNumber: seat.seatNumber,
                                    isBooked: false,
                                };

                                if (existingIdx >= 0) {
                                    if (!state.pendingAssignments[flightId][existingIdx].isBooked) {
                                        state.pendingAssignments[flightId][existingIdx] = lockedAssignment;
                                    }
                                } else {
                                    state.pendingAssignments[flightId].push(lockedAssignment);
                                }
                            }
                        }
                    }
                }

                state.pendingAssignments[flightId].forEach(assignment => {
                    if ((!assignment.seatId || assignment.seatId === 0) && assignment.flightSeatId > 0) {
                        for (const group of seatMap.classGroups) {
                            for (const row of group.rows) {
                                const seat = row.seats.find(
                                    s => s.flightSeatId === assignment.flightSeatId
                                );
                                if (seat) {
                                    assignment.seatId = seat.seatId;
                                    return;
                                }
                            }
                        }
                    }
                });
            })
            .addCase(fetchSeatMap.rejected, (state, action) => {
                state.isLoadingMap = false;
                state.mapError = action.payload;
            });

        builder
            .addCase(holdSeat.pending, (state) => {
                state.isHolding = true;
                state.actionError = null;
            })
            .addCase(holdSeat.fulfilled, (state, action) => {
                state.isHolding = false;
                const { flightId, passengerId, flightSeatId, seatId, seatNumber } = action.payload;

                if (!state.pendingAssignments[flightId])
                    state.pendingAssignments[flightId] = [];

                const existingIdx = state.pendingAssignments[flightId]
                    .findIndex(a => a.passengerId === passengerId);

                const assignment = { passengerId, flightSeatId, seatId, seatNumber, isBooked: false };

                if (existingIdx >= 0) {
                    state.pendingAssignments[flightId][existingIdx] = assignment;
                } else {
                    state.pendingAssignments[flightId].push(assignment);
                }

                const map = state.seatMaps[flightId];
                if (map) {
                    for (const group of map.classGroups) {
                        for (const row of group.rows) {
                            const seat = row.seats.find(s => s.seatId === seatId);
                            if (seat) {
                                seat.status = SEAT_STATUS.LOCKED;
                                seat.lockedByPassengerId = passengerId;
                                seat.flightSeatId = flightSeatId;
                                return;
                            }
                        }
                    }
                }
            })
            .addCase(holdSeat.rejected, (state, action) => {
                state.isHolding = false;
                state.actionError = action.payload;
            });

        builder
            .addCase(releaseSeat.pending, (state) => {
                state.isReleasing = true;
                state.actionError = null;
            })
            .addCase(releaseSeat.fulfilled, (state, action) => {
                state.isReleasing = false;
                const { flightId, seatId, passengerId } = action.payload;

                if (state.pendingAssignments[flightId]) {
                    state.pendingAssignments[flightId] = state.pendingAssignments[flightId]
                        .filter(a => a.passengerId !== passengerId);
                }

                const map = state.seatMaps[flightId];
                if (map) {
                    for (const group of map.classGroups) {
                        for (const row of group.rows) {
                            const seat = row.seats.find(s => s.seatId === seatId);
                            if (seat) {
                                seat.status = SEAT_STATUS.AVAILABLE;
                                seat.lockedByPassengerId = null;
                                seat.flightSeatId = 0;
                                return;
                            }
                        }
                    }
                }
            })
            .addCase(releaseSeat.rejected, (state, action) => {
                state.isReleasing = false;
                state.actionError = action.payload;
            });

        builder
            .addCase(confirmSeats.pending, (state) => {
                state.isConfirming = true;
                state.actionError = null;
            })
            .addCase(confirmSeats.fulfilled, (state) => {
                state.isConfirming = false;
                state.isConfirmed = true;
            })
            .addCase(confirmSeats.rejected, (state, action) => {
                state.isConfirming = false;
                state.actionError = action.payload;
            });
    },
});

export const {
    setActiveFlightId, setSelectedPassenger, setActionError,
    clearActionError, clearVerifyError,
    updateSeatStatusFromHub, resetSeatReverse, resetConfirmed
} = seatReverseSlice.actions;

export default seatReverseSlice.reducer;

export const selectBookingInfo = (state) => state.seatReverse.bookingInfo;
export const selectActiveFlightId = (state) => state.seatReverse.activeFlightId;
export const selectActiveSeatMap = createSelector(
    [selectActiveFlightId, (state) => state.seatReverse.seatMaps],
    (activeFlightId, seatMaps) => activeFlightId ? seatMaps[activeFlightId] : null
);
export const selectSelectedPassengerId = (state) => state.seatReverse.selectedPassengerId;
export const selectPendingAssignments = (state) => state.seatReverse.pendingAssignments;
export const selectActivePendingAssignments = createSelector(
    [selectActiveFlightId, selectPendingAssignments],
    (activeFlightId, pendingAssignments) => {
        if (!activeFlightId) return [];
        const pending = pendingAssignments[activeFlightId];
        return Array.isArray(pending) ? pending : [];
    }
);
export const selectIsVerifying = (state) => state.seatReverse.isVerifying;
export const selectIsLoadingMap = (state) => state.seatReverse.isLoadingMap;
export const selectIsHolding = (state) => state.seatReverse.isHolding;
export const selectIsConfirming = (state) => state.seatReverse.isConfirming;
export const selectVerifyError = (state) => state.seatReverse.verifyError;
export const selectMapError = (state) => state.seatReverse.mapError;
export const selectActionError = (state) => state.seatReverse.actionError;
export const selectIsConfirmed = (state) => state.seatReverse.isConfirmed;
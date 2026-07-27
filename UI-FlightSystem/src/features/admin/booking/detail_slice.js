import { createSlice } from "@reduxjs/toolkit";
import { createAsyncAction } from "@/lib/redux/create_async_action";
import { bookingService } from "./booking_service";

export const fetchBookingById = createAsyncAction(
    "bookingDetail/fetchById", (id) => bookingService.getById(id)
);

const bookingDetailSlice = createSlice({
    name: "bookingDetail",
    initialState: {detail: null, isLoadingDetail: false, error: null},
    reducers: { clearDetail(state) {state.detail = null; state.error = null}},
    extraReducers: (builder) => {
        builder
            .addCase(fetchBookingById.pending, (state) => {
                state.isLoadingDetail = true;
                state.detail = null;
                state.error = null;
            })
            .addCase(fetchBookingById.fulfilled, (state, action) => {
                state.detail = action.payload;
                state.isLoadingDetail = false;
            })
            .addCase(fetchBookingById.rejected, (state, action) => {
                state.error = action.payload;
                state.isLoadingDetail = false;
            });
    },
});

export const { clearDetail } = bookingDetailSlice.actions;
export default bookingDetailSlice.reducer;
import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import { bookingService } from "@/features/admin/booking_management/booking_service";

export const fetchBookingById = createAsyncThunk(
    "bookingDetail/fetchById",
    async (id, { rejectWithValue }) => {
        try {
            const { data } = await bookingService.getById(id);
            if (!data.succeeded)
                return rejectWithValue(data.errors?.[0]?.errorMessage || "Không tìm thấy booking.");
            return data.result;
        } catch {
            return rejectWithValue("Lỗi kết nối server.");
        }
    }
);

const bookingDetailSlice = createSlice({
    name: "bookingDetail",
    initialState: {
        detail: null,
        isLoadingDetail: false,
        error: null,
    },
    reducers: {
        clearDetail: (state) => {
            state.detail = null;
            state.error = null;
        },
    },
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
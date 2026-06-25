import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import { supportRequestService } from "./support_service";

export const createSupportRequest = createAsyncThunk(
    "createSupportRequest/create",
    async (data, { rejectWithValue }) => {
        try {
            const { data: res } = await supportRequestService.create(data);
            if (!res.succeeded)
                return rejectWithValue(res.errors?.[0] ?? res.message ?? "Gửi yêu cầu thất bại.");
            return res.result;
        } catch (error) {
            const msg =
                error?.response?.data?.errors?.[0]?.errorMessage ||
                error?.response?.data?.message ||
                error.message;
            return rejectWithValue(msg);
        }
    }
);

const createSupportRequestSlice = createSlice({
    name: "createSupportRequest",
    initialState: {
        isLoading: false,
        error: null,
        success: false,
    },
    reducers: {
        resetCreate(state) {
            state.isLoading = false;
            state.error = null;
            state.success = false;
        },
    },
    extraReducers: (builder) => {
        builder
            .addCase(createSupportRequest.pending, (state) => {
                state.isLoading = true;
                state.error = null;
                state.success = false;
            })
            .addCase(createSupportRequest.fulfilled, (state) => {
                state.isLoading = false;
                state.success = true;
            })
            .addCase(createSupportRequest.rejected, (state, action) => {
                state.isLoading = false;
                state.error = action.payload;
            });
    },
});

export const { resetCreate } = createSupportRequestSlice.actions;
export default createSupportRequestSlice.reducer;
import { createAsyncThunk } from "@reduxjs/toolkit";
import { sharedSlice } from "@/store/shared_slice";
import { adminSupportRequestService } from "./support_service";

export const approveSupportRequest = createAsyncThunk(
    "supportRequest/approve",
    async (id, { rejectWithValue }) => {
        try {
            const { data } = await adminSupportRequestService.approve(id);
            if (!data.succeeded)
                return rejectWithValue(data.errors?.[0] ?? data.message ?? "Duyệt yêu cầu thất bại.");
            return data.result;
        } catch (err) {
            return rejectWithValue(err.response?.data?.message ?? err.message ?? "Lỗi kết nối server.");
        }
    }
);

export const rejectSupportRequest = createAsyncThunk(
    "supportRequest/reject",
    async (id, { rejectWithValue }) => {
        try {
            const { data } = await adminSupportRequestService.reject(id);
            if (!data.succeeded)
                return rejectWithValue(data.errors?.[0] ?? data.message ?? "Từ chối yêu cầu thất bại.");
            return data.result;
        } catch (err) {
            return rejectWithValue(err.response?.data?.message ?? err.message ?? "Lỗi kết nối server.");
        }
    }
);

const { slice, fetchItems: fetchSupportRequests, fetchById: fetchSupportRequestById } = sharedSlice(
    "supportRequest",
    adminSupportRequestService.getAll,
    {
        setRequestType: (state, action) => {
            state.requestType = action.payload;
            state.pageIndex = 1;
        },
        setStatus: (state, action) => {
            state.status = action.payload;
            state.pageIndex = 1;
        },
    },
    {
        requestType: "",
        status: "",
        actionLoading: false,
        actionError: null,
    },
    adminSupportRequestService.getById,
    (builder) => { 
        builder
            .addCase(approveSupportRequest.pending, (state) => {
                state.actionLoading = true;
                state.actionError = null;
            })
            .addCase(approveSupportRequest.fulfilled, (state) => {
                state.actionLoading = false;
            })
            .addCase(approveSupportRequest.rejected, (state, action) => {
                state.actionLoading = false;
                state.actionError = action.payload;
            })
            .addCase(rejectSupportRequest.pending, (state) => {
                state.actionLoading = true;
                state.actionError = null;
            })
            .addCase(rejectSupportRequest.fulfilled, (state) => {
                state.actionLoading = false;
            })
            .addCase(rejectSupportRequest.rejected, (state, action) => {
                state.actionLoading = false;
                state.actionError = action.payload;
            });
    }
);

export { fetchSupportRequests, fetchSupportRequestById };
export const { setPage, setRequestType, setStatus, clearSelectedItem } = slice.actions;
export default slice.reducer;
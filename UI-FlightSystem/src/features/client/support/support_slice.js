import { createSlice } from "@reduxjs/toolkit";
import { createAsyncAction } from "@/lib/redux/create_async_action";
import { createCrudSlice } from "@/lib/redux/create_crud_slice";
import { supportService } from "./support_service";

export const fetchMySupportRequests = createAsyncAction(
    "mySupportRequests/fetchMyRequests",
    (params) => supportService.getMy(params)
);

export const createSupportRequest = createAsyncAction(
    "createSupportRequest/create",
    (data) => supportService.create(data)
);

const mySupportRequestsSlice = createCrudSlice({
    name: "mySupportRequests",
    fetchAll: fetchMySupportRequests,
});

export const mySupportRequestsReducer = mySupportRequestsSlice.reducer;
export const { setPage: setMyRequestsPage, clearError: clearMyRequestsError } = mySupportRequestsSlice.actions;

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
export const createSupportRequestReducer = createSupportRequestSlice.reducer;

export default mySupportRequestsReducer;
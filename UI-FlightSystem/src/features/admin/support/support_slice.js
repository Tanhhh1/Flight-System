import { createAsyncAction } from "@/lib/redux/create_async_action";
import { createCrudSlice } from "@/lib/redux/create_crud_slice";

import { adminSupportRequestService } from "./support_service";

export const fetchSupportRequests = createAsyncAction(
    "supportRequest/fetchAll",
    (params) => adminSupportRequestService.getAll(params)
);

export const fetchSupportRequestById = createAsyncAction(
    "supportRequest/fetchById",
    (id) => adminSupportRequestService.getById(id)
);

export const approveSupportRequest = createAsyncAction(
    "supportRequest/approve",
    (id) => adminSupportRequestService.approve(id)
);

export const rejectSupportRequest = createAsyncAction(
    "supportRequest/reject",
    (id) => adminSupportRequestService.reject(id)
);

const { reducer, actions } = createCrudSlice({
    name: "supportRequest",
    fetchAll: fetchSupportRequests,
    fetchById: fetchSupportRequestById,
    initialState: {
        requestType: "",
        status: "",
        actionLoading: false,
        actionError: null,
    },

    reducers: {
        setRequestType(state, action) {
            state.requestType = action.payload;
            state.pageIndex = 1;
        },

        setStatus(state, action) {
            state.status = action.payload;
            state.pageIndex = 1;
        },
    },
});

const supportReducer = (state, action) => {
    state = reducer(state, action);
    switch (action.type) {
        case approveSupportRequest.pending.type:
        case rejectSupportRequest.pending.type:
            return {
                ...state,
                actionLoading: true,
                actionError: null,
            };
        case approveSupportRequest.fulfilled.type:
        case rejectSupportRequest.fulfilled.type:
            return {
                ...state,
                actionLoading: false,
            };
        case approveSupportRequest.rejected.type:
        case rejectSupportRequest.rejected.type:
            return {
                ...state,
                actionLoading: false,
                actionError: action.payload,
            };

        default:
            return state;
    }
};

export const { setPage, setSearch, setRequestType, setStatus, clearSelectedItem, clearError } = actions;

export default supportReducer;
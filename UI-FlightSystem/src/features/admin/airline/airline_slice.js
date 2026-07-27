import { createAsyncAction } from "@/lib/redux/create_async_action";
import { createCrudSlice } from "@/lib/redux/create_crud_slice";

import { airlineService } from "./airline_service";

export const fetchAirlines = createAsyncAction(
    "airline/fetchAll",
    (params) => airlineService.getAll(params)
);

export const fetchAirlineById = createAsyncAction(
    "airline/fetchById",
    (id) => airlineService.getById(id)
);

const { reducer, actions } = createCrudSlice({
    name: "airline",
    fetchAll: fetchAirlines,
    fetchById: fetchAirlineById,
    initialState: { status: "" },
    reducers: {
        setStatus(state, action) {
            state.status = action.payload;
            state.pageIndex = 1;
        },
    },
});

export const { setPage, setSearch, setStatus, clearSelectedItem, clearError } = actions;

export default reducer;
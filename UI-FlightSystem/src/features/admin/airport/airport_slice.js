import { createAsyncAction } from "@/lib/redux/create_async_action";
import { createCrudSlice } from "@/lib/redux/create_crud_slice";
import { airportService } from "./airport_service";

export const fetchAirports = createAsyncAction(
    "airport/fetchAll",
    (params) => airportService.getAll(params)
);

export const fetchAirportById = createAsyncAction(
    "airport/fetchById",
    (id) => airportService.getById(id)
);

const { reducer, actions } = createCrudSlice({
    name: "airport",
    fetchAll: fetchAirports,
    fetchById: fetchAirportById,
    initialState: {status: ""},
    reducers: {
        setStatus(state, action) {
            state.status = action.payload;
            state.pageIndex = 1;
        },
    },
});

export const { setPage, setSearch, setStatus, clearSelectedItem, clearError, } = actions;

export default reducer;
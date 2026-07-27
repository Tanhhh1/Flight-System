import { createCrudSlice } from "@/lib/redux/create_crud_slice";
import { createAsyncAction } from "@/lib/redux/create_async_action";
import { routeService } from "./route_service";

export const fetchRoutes = createAsyncAction(
    "route/fetchAll",
    (params) => routeService.getAll(params)
);

export const fetchRouteById = createAsyncAction(
    "route/fetchById",
    (id) => routeService.getById(id)
);

const { reducer, actions } = createCrudSlice({
    name: "route",
    fetchAll: fetchRoutes,
    fetchById: fetchRouteById,
    initialState: {
        status: "",
        originAirportCode: "",
        destinationAirportCode: "",
    },
    reducers: {
        setStatusFilter(state, action) {
            state.status = action.payload;
            state.pageIndex = 1;
        },
        setOriginAirportCode(state, action) {
            state.originAirportCode = action.payload;
            state.pageIndex = 1;
        },
        setDestinationAirportCode(state, action) {
            state.destinationAirportCode = action.payload;
            state.pageIndex = 1;
        },
    },
});

export const {
    setPage,
    setSearch,
    clearSelectedItem,
    clearError,
    setStatusFilter,
    setOriginAirportCode,
    setDestinationAirportCode,
} = actions;

export default reducer;
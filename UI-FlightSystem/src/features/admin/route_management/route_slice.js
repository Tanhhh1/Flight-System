import { sharedSlice } from "@/store/shared_slice";
import { routeService } from "./route_service";

const { slice, fetchItems: fetchRoutes, fetchById: fetchRouteById } = sharedSlice(
    "route",
    routeService.getAll,
    {
        setStatusFilter: (state, action) => {
            state.status = action.payload;
            state.pageIndex = 1;
        },
        setOriginAirportCode: (state, action) => {
            state.originAirportCode = action.payload;
            state.pageIndex = 1;
        },
        setDestinationAirportCode: (state, action) => {
            state.destinationAirportCode = action.payload;
            state.pageIndex = 1;
        },
    },
    {
        status: "",
        originAirportCode: "",
        destinationAirportCode: "",
    },
    routeService.getById
);

export { fetchRoutes, fetchRouteById };
export const { 
    setPage, setStatusFilter, clearSelectedItem,
    setOriginAirportCode, setDestinationAirportCode 
} = slice.actions;
export default slice.reducer;
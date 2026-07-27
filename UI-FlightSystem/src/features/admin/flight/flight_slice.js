import { createAsyncAction } from "@/lib/redux/create_async_action";
import { createCrudSlice } from "@/lib/redux/create_crud_slice";
import { flightService } from "./flight_service";

export const fetchFlights = createAsyncAction(
    "flight/fetchFlights",
    (params) => flightService.getAll(params)
);

const { reducer, actions } = createCrudSlice({
    name: "flight",
    fetchAll: fetchFlights,
    initialState: {
        statusFilter: "",
        airlineFilter: "",
        originAirportCode: "",
        destinationAirportCode: "",
        departureDate: "",
    },
    reducers: {
        setStatusFilter: (state, action) => {
            state.statusFilter = action.payload;
            state.pageIndex = 1;
        },
        setAirlineFilter: (state, action) => {
            state.airlineFilter = action.payload;
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
        setDepartureDate: (state, action) => {
            state.departureDate = action.payload;
            state.pageIndex = 1;
        },
    },
});

export const {
    setPage,
    setSearch,
    clearError,
    setStatusFilter,
    setAirlineFilter,
    setOriginAirportCode,
    setDestinationAirportCode,
    setDepartureDate,
} = actions;

export default reducer;
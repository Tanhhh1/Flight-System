import { sharedSlice } from "@/store/shared_slice";
import { flightService } from "./flight_service";

const { slice, fetchItems: fetchFlights } = sharedSlice(
    "flight",
    flightService.getAll,
    {
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
    {
        statusFilter: "",
        airlineFilter: "",
        originAirportCode: "",
        destinationAirportCode: "",
        departureDate: "",
    }
);

export { fetchFlights };
export const {
    setPage, setSearch,
    setStatusFilter, setAirlineFilter,
    setOriginAirportCode, setDestinationAirportCode, setDepartureDate,
} = slice.actions;
export default slice.reducer;
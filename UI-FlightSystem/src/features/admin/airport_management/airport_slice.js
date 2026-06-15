import { sharedSlice } from "@/store/shared_slice";
import { airportService } from "./airport_service";

const { slice, fetchItems: fetchAirports, fetchById: fetchAirportById } = sharedSlice(
    "airport",
    airportService.getAll,
    {
        setStatus: (state, action) => {
            state.status = action.payload;
            state.pageIndex = 1;
        },
    },
    {
        status: "",
    },
    airportService.getById
);

export { fetchAirports, fetchAirportById };
export const { setPage, setSearch, setStatus, clearSelectedItem } = slice.actions;
export default slice.reducer;
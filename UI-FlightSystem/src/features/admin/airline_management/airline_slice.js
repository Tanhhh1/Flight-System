import { sharedSlice } from "@/store/shared_slice";
import { airlineService } from "./airline_service";

const { slice, fetchItems: fetchAirlines, fetchById: fetchAirlineById } = sharedSlice(
    "airline",
    airlineService.getAll,
    {
        setStatus: (state, action) => {
            state.status = action.payload;
            state.pageIndex = 1;
        },
    },
    {
        status: "",
    },
    airlineService.getById
);

export { fetchAirlines, fetchAirlineById };
export const { setPage, setSearch, setStatus, clearSelectedItem } = slice.actions;
export default slice.reducer;
import { createCrudSlice } from "@/lib/redux/create_crud_slice";
import { createAsyncAction } from "@/lib/redux/create_async_action";
import { bookingService } from "./booking_service";

export const fetchMyBookings = createAsyncAction(
    "booking/fetchMyBookings",
    (params) => bookingService.getAll(params)
);

const { reducer, actions } = createCrudSlice({
    name: "booking",
    fetchAll: fetchMyBookings,
});

export const { setPage, setSearch, clearError } = actions;
export default reducer;
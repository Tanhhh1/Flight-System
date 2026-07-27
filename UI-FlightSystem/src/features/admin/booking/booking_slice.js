import { createAsyncAction } from "@/lib/redux/create_async_action";
import { createCrudSlice } from "@/lib/redux/create_crud_slice";
import { bookingService } from "./booking_service";

export const fetchBookings = createAsyncAction(
    "booking/fetchAll",
    (params) => bookingService.getAll(params)
);

const { reducer, actions } = createCrudSlice({
    name: "booking",
    fetchAll: fetchBookings,
    initialState: { tripType: "", classId: "", bookingDate: "" },
    reducers: {
        setTripType(state, action) {
            state.tripType = action.payload;
            state.pageIndex = 1;
        },
        setClassId(state, action) {
            state.classId = action.payload;
            state.pageIndex = 1;
        },
        setBookingDate(state, action) {
            state.bookingDate = action.payload;
            state.pageIndex = 1;
        },
    },
});

export const { setPage, setSearch, setTripType, setClassId, setBookingDate, clearError } = actions;

export default reducer;
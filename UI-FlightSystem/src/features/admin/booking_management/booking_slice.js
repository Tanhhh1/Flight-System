import { sharedSlice } from "@/store/shared_slice";
import { bookingService } from "./booking_service";

const { slice, fetchItems: fetchBookings } = sharedSlice(
    "booking",
    bookingService.getAll,
    {
        setTripType: (state, action) => {
            state.tripType = action.payload;
            state.pageIndex = 1;
        },
        setClassId: (state, action) => {
            state.classId = action.payload;
            state.pageIndex = 1;
        },
        setBookingDate: (state, action) => {
            state.bookingDate = action.payload;
            state.pageIndex = 1;
        },
    },
    {
        tripType: "",
        classId: "",
        bookingDate: "",
    }
);

export { fetchBookings };
export const { setPage, setTripType, setClassId, setBookingDate } = slice.actions;
export default slice.reducer;
import { sharedSlice } from "@/store/shared_slice";
import { bookingService } from "./booking_service";

const { slice, fetchItems: fetchMyBookings } = sharedSlice(
    "myBookings",
    bookingService.getAll,
);

export { fetchMyBookings };
export const { setPage } = slice.actions;
export default slice.reducer;
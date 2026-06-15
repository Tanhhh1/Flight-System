import { sharedSlice } from "@/store/shared_slice";
import { reviewService } from "./review_service";

const { slice, fetchItems: fetchMyReviews } = sharedSlice(
    "myReviews",
    reviewService.getMyReviews
);

export { fetchMyReviews };
export const { setPage, clearError } = slice.actions;
export default slice.reducer;
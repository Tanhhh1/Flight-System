import { sharedSlice } from "@/store/shared_slice";
import { reviewService } from "./review_service";

const { slice, fetchItems: fetchReviews } = sharedSlice(
    "review",
    reviewService.getAll,
    {
        setIsHidden: (state, action) => {
            state.isHidden  = action.payload;
            state.pageIndex = 1;
        },
    },
    {
        isHidden: "",
    }
);

export { fetchReviews };
export const { setPage, setSearch, setIsHidden } = slice.actions;
export default slice.reducer;
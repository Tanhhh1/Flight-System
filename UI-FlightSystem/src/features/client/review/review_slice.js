import { createAsyncAction } from "@/lib/redux/create_async_action";
import { createCrudSlice } from "@/lib/redux/create_crud_slice";
import { reviewApi } from "./review_service";

export const fetchMyReviews = createAsyncAction(
    "myReviews/fetchMyReviews",
    (params) => reviewApi.getMyReviews(params)
);

const { reducer, actions } = createCrudSlice({
    name: "myReviews",
    fetchAll: fetchMyReviews,
});

export const { setPage, setSearch, clearError } = actions;
export default reducer;
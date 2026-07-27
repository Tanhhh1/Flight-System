import { createAsyncAction } from "@/lib/redux/create_async_action";
import { createCrudSlice } from "@/lib/redux/create_crud_slice";
import { reviewService } from "./review_service";

export const fetchReviews = createAsyncAction(
    "review/fetchAll",
    (params) => reviewService.getAll(params)
);

const { reducer, actions } = createCrudSlice({
    name: "review",
    fetchAll: fetchReviews,
    initialState: { isHidden: "" },
    reducers: {
        setIsHidden(state, action) {
            state.isHidden = action.payload;
            state.pageIndex = 1;
        },
    },
});

export const { setPage, setSearch, setIsHidden, clearError } = actions;

export default reducer;
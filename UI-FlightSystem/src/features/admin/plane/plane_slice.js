import { createAsyncAction } from "@/lib/redux/create_async_action";
import { createCrudSlice } from "@/lib/redux/create_crud_slice";
import { planeService } from "./plane_service";

export const fetchPlanes = createAsyncAction(
    "plane/fetchAll",
    (params) => planeService.getAll(params)
);

export const fetchPlaneById = createAsyncAction(
    "plane/fetchById",
    (id) => planeService.getById(id)
);

const { reducer, actions } = createCrudSlice({
    name: "plane",
    fetchAll: fetchPlanes,
    fetchById: fetchPlaneById,
    initialState: { status: "" },
    reducers: {
        setStatus(state, action) {
            state.status = action.payload;
            state.pageIndex = 1;
        },
    },
});

export const { setPage, setSearch, setStatus, clearSelectedItem, clearError } = actions;

export default reducer;
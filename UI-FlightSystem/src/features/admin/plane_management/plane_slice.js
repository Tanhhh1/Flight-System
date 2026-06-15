import { sharedSlice } from "@/store/shared_slice";
import { planeService } from "./plane_service";

const { slice, fetchItems: fetchPlanes, fetchById: fetchPlaneById } = sharedSlice(
    "plane",
    planeService.getAll,
    {
        setStatus: (state, action) => {
            state.status = action.payload;
            state.pageIndex    = 1;
        },
    },
    { status: "" },
    planeService.getById
);

export { fetchPlanes, fetchPlaneById };
export const { setPage, setSearch, setStatus, clearSelectedItem } = slice.actions;
export default slice.reducer;
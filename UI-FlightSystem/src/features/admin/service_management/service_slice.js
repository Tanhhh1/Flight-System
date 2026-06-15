import { sharedSlice } from "@/store/shared_slice";
import { serviceService } from "./service_service";

const { slice, fetchItems: fetchServices, fetchById: fetchServiceById } = sharedSlice(
    "service",
    serviceService.getAll,
    {
        setIsActive: (state, action) => {
            state.isActive  = action.payload;
            state.pageIndex = 1;
        },
    },
    { isActive: "" },
    serviceService.getById
);

export { fetchServices, fetchServiceById };
export const { setPage, setSearch, setIsActive, clearSelectedItem } = slice.actions;
export default slice.reducer;
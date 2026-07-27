import { createCrudSlice } from "@/lib/redux/create_crud_slice";
import { createAsyncAction } from "@/lib/redux/create_async_action";
import { serviceService } from "./service_service";

export const fetchServices = createAsyncAction(
    "service/fetchAll",
    (params) => serviceService.getAll(params)
);

export const fetchServiceById = createAsyncAction(
    "service/fetchById",
    (id) => serviceService.getById(id)
);

const { reducer, actions } = createCrudSlice({
    name: "service",
    fetchAll: fetchServices,
    fetchById: fetchServiceById,
    initialState: {
        isActive: "",
    },
    reducers: {
        setIsActive(state, action) {
            state.isActive = action.payload;
            state.pageIndex = 1;
        },
    },
});

export const {
    setPage,
    setSearch,
    clearSelectedItem,
    clearError,
    setIsActive,
} = actions;

export default reducer;
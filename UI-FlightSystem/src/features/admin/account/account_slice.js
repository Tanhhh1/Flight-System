import { createAsyncAction } from "@/lib/redux/create_async_action";
import { createCrudSlice } from "@/lib/redux/create_crud_slice";
import { accountService } from "./account_service";

export const fetchAccounts = createAsyncAction(
    "account/fetchAll",
    (params) => accountService.getAll(params)
);

export const fetchAccountById = createAsyncAction(
    "account/fetchById",
    (id) => accountService.getById(id)
);

const { reducer, actions } = createCrudSlice({
    name: "account",
    fetchAll: fetchAccounts,
    fetchById: fetchAccountById,
    initialState: { roleName: "" },
    reducers: {
        setRoleName(state, action) {
            state.roleName = action.payload;
            state.pageIndex = 1;
        },
    },
});

export const { setPage, setSearch, setRoleName, clearSelectedItem, clearError } = actions;
export default reducer;
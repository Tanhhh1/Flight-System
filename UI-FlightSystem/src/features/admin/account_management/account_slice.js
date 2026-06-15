import { sharedSlice } from "@/store/shared_slice";
import { accountService } from "./account_service";

const { slice, fetchItems: fetchAccounts, fetchById: fetchAccountById } = sharedSlice(
    "account",
    accountService.getAll,
    {
        setRoleName: (state, action) => {
            state.roleName  = action.payload;
            state.pageIndex = 1;
        },
    },
    {
        roleName: "",
    },
    accountService.getById
);

export { fetchAccounts, fetchAccountById };
export const { setPage, setSearch, setRoleName, clearSelectedItem } = slice.actions;
export default slice.reducer;
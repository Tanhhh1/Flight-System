import { useState, useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useDebounce } from "@/hooks/use_debounce";

export function useSearchFilter({ sliceName, setSearchAction, setFilterAction }) {
    const dispatch = useDispatch();
    const sliceState = useSelector((state) => state[sliceName]);
    const searchValue = sliceState?.search ?? "";

    const [searchInput, setSearchInput] = useState(searchValue);
    const debouncedSearch = useDebounce(searchInput);

    useEffect(() => {setSearchInput(searchValue)}, [searchValue]);

    useEffect(() => {
        if (setSearchAction) {
            dispatch(setSearchAction(debouncedSearch));
        }
    }, [debouncedSearch, dispatch, setSearchAction]);

    const handleFilterChange = (value, filterAction = setFilterAction) => {
        if (filterAction) {
            dispatch(filterAction(value));
        }
    };

    return { searchInput, setSearchInput, handleFilterChange };
}
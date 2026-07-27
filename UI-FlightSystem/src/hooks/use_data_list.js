import { useEffect, useCallback } from "react";
import { useDispatch, useSelector } from "react-redux";

export function useDataList({ sliceName, fetchListThunk, selectExtraFilters = () => ({}) }) {
    const dispatch = useDispatch();
    const sliceState = useSelector((state) => state[sliceName]) ?? {};

    const { items = [], pageIndex = 1, pageSize = 10, totalPages = 0,
        totalCount = 0, search = "", error = null, isLoading = false, isDetailLoading = false,
    } = sliceState;

    const extraFilters = selectExtraFilters(sliceState);

    const refresh = useCallback(() => {
        dispatch( fetchListThunk({ pageIndex, pageSize, search, ...extraFilters }));
    }, [dispatch, fetchListThunk, pageIndex, pageSize, search, JSON.stringify(extraFilters)]);

    useEffect(() => {refresh()}, [refresh]);

    return { items, pageIndex, pageSize, totalPages, totalCount, search, error, isLoading, isDetailLoading, refresh };
}
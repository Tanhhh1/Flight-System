import { useCallback } from "react";
import { useDispatch } from "react-redux";

export function useDetailLoader({ fetchByIdThunk, onFulfilled, onError }) {
    const dispatch = useDispatch();

    const loadDetail = useCallback(
        async (id, itemFallback = null) => {
            const result = await dispatch(fetchByIdThunk(id));
            if (fetchByIdThunk.fulfilled.match(result)) {
                if (onFulfilled) onFulfilled(result.payload);
                return result.payload;
            } else if (fetchByIdThunk.rejected.match(result)) {
                if (onError) onError(result.error);
            }
            return null;
        },
        [dispatch, fetchByIdThunk, onFulfilled, onError]
    );

    return { loadDetail };
}
import { createSlice } from "@reduxjs/toolkit";

export function createCrudSlice({
    name,
    fetchAll,
    fetchById = null,
    initialState = {},
    reducers = {},
}) {
    const slice = createSlice({
        name,
        initialState: {
            items: [],
            selectedItem: null,
            pageIndex: 1,
            pageSize: 10,
            totalPages: 0,
            totalCount: 0,
            search: "",
            isLoading: false,
            isDetailLoading: false,
            error: null,
            ...initialState,
        },

        reducers: {
            setPage(state, action) {
                state.pageIndex = action.payload;
            },
            setSearch(state, action) {
                state.search = action.payload;
                state.pageIndex = 1;
            },
            clearSelectedItem(state) {
                state.selectedItem = null;
            },
            clearError(state) {
                state.error = null;
            },
            ...reducers,
        },
        extraReducers(builder) {
            builder
                .addCase(fetchAll.pending, (state) => {
                    state.isLoading = true;
                    state.error = null;
                })
                .addCase(fetchAll.fulfilled, (state, action) => {
                    state.isLoading = false;
                    state.items = action.payload?.items ?? [];
                    state.totalPages = action.payload?.totalPages ?? 0;
                    state.totalCount = action.payload?.totalCount ?? 0;
                })
                .addCase(fetchAll.rejected, (state, action) => {
                    state.isLoading = false;
                    state.error = action.payload;
                    state.items = [];
                });
            if (fetchById) {
                builder
                    .addCase(fetchById.pending, (state) => {
                        state.isDetailLoading = true;
                        state.selectedItem = null;
                    })
                    .addCase(fetchById.fulfilled, (state, action) => {
                        state.isDetailLoading = false;
                        state.selectedItem = action.payload;
                    })
                    .addCase(fetchById.rejected, (state, action) => {
                        state.isDetailLoading = false;
                        state.error = action.payload;
                    });
            }
        },
    });
    return {
        reducer: slice.reducer,
        actions: slice.actions,
    };
}
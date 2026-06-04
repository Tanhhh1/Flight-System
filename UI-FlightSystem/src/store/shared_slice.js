import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";

const parseError = (err) =>
    err?.response?.data?.message ?? err?.message ?? "Đã xảy ra lỗi hệ thống";

export function sharedSlice( name, fetchFn, extraSyncReducers = {}, extraInitialState = {}, getByIdFn = null) 
{
    const fetchItems = createAsyncThunk(
        `${name}/fetchAll`,
        async (params, { rejectWithValue }) => {
            try {
                const { data } = await fetchFn(params);
                if (data.succeeded === false) {
                    return rejectWithValue(data.message ?? "Lấy dữ liệu thất bại");
                }

                return data.result ?? data;
            } catch (err) {
                return rejectWithValue(parseError(err));
            }
        }
    );

    const fetchById = getByIdFn
        ? createAsyncThunk(
            `${name}/fetchById`,
            async (id, { rejectWithValue }) => {
                try {
                    const { data } = await getByIdFn(id);

                    if (data.succeeded === false) {
                        return rejectWithValue(data.message ?? "Không tìm thấy dữ liệu");
                    }

                    return data.result ?? data;
                } catch (err) {
                    return rejectWithValue(parseError(err));
                }
            }
        )
        : null;


    const slice = createSlice({
        name,
        initialState: { items: [], totalPages: 0, totalCount: 0, pageIndex: 1, pageSize: 10, search: "", selectedItem: null, 
            isLoading: false, isDetailLoading: false, error: null, ...extraInitialState },

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
                state.error = null;
            },
            clearError(state) {
                state.error = null;
            },
            ...extraSyncReducers,
        },

        extraReducers: (builder) => {
            builder
                .addCase(fetchItems.pending, (state) => {
                    state.isLoading = true;
                    state.error = null;
                })
                .addCase(fetchItems.fulfilled, (state, action) => {
                    state.isLoading = false;
                    state.items = action.payload?.items ?? [];
                    state.totalPages = action.payload?.totalPages ?? 0;
                    state.totalCount = action.payload?.totalCount ?? 0;
                })
                .addCase(fetchItems.rejected, (state, action) => {
                    state.isLoading = false;
                    state.error = action.payload;
                    state.items = [];
                });

            if (fetchById) {
                builder
                    .addCase(fetchById.pending, (state) => {
                        state.isDetailLoading = true;
                        state.error = null;
                        state.selectedItem = null;
                    })
                    .addCase(fetchById.fulfilled, (state, action) => {
                        state.isDetailLoading = false;
                        state.selectedItem = action.payload;
                    })
                    .addCase(fetchById.rejected, (state, action) => {
                        state.isDetailLoading = false;
                        state.error = action.payload;
                        state.selectedItem = null;
                    });
            }
        },
    });

    return { slice, fetchItems, fetchById };
}
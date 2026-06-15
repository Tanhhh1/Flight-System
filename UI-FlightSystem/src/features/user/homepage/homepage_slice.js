import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import { homepageService } from "./homepage_service";
import { dataSearchService, DataSearch } from "@/services/data_search_service";

export const fetchHomeReviews = createAsyncThunk(
    "home/fetchReviews",
    async (_, { rejectWithValue }) => {
        try {
            const { data } = await homepageService.getAllReviews({ pageIndex: 1, pageSize: 100 });
            if (data.succeeded === false) {
                return rejectWithValue(data.message ?? "Lấy đánh giá thất bại");
            }
            return data.result?.items ?? data.result ?? [];
        } catch (err) {
            return rejectWithValue(err?.response?.data?.message ?? err?.message ?? "Lỗi hệ thống");
        }
    }
);

export const fetchHomeAirports = createAsyncThunk(
    "home/fetchAirports",
    async (_, { rejectWithValue }) => {
        try {
            const { data } = await dataSearchService.get([DataSearch.Airports]);
            if (data.succeeded === false) {
                return rejectWithValue(data.message ?? "Lấy danh sách sân bay thất bại");
            }
            return data.result?.airports ?? [];
        } catch (err) {
            return rejectWithValue(err?.response?.data?.message ?? err?.message ?? "Lỗi hệ thống");
        }
    }
);

const homeSlice = createSlice({
    name: "home",
    initialState: {
        reviews: [],
        isLoadingReviews: false,
        airports: [],
        isLoadingAirports: false,
        error: null,
        searchForm: {
            originAirportCode: "",
            destinationAirportCode: "",
            departureDate: "",
        }
    },
    reducers: {
        setSearchField: (state, action) => {
            const { field, value } = action.payload;
            state.searchForm[field] = value;
        },
        clearSearchForm: (state) => {
            state.searchForm = {
                originAirportCode: "",
                destinationAirportCode: "",
                departureDate: "",
            };
        }
    },
    extraReducers: (builder) => {
        builder
            .addCase(fetchHomeReviews.pending, (state) => {
                state.isLoadingReviews = true;
                state.error = null;
            })
            .addCase(fetchHomeReviews.fulfilled, (state, action) => {
                state.isLoadingReviews = false;
                const allReviews = action.payload;
                state.reviews = allReviews.slice(-3).reverse();
            })
            .addCase(fetchHomeReviews.rejected, (state, action) => {
                state.isLoadingReviews = false;
                state.error = action.payload;
            })
            .addCase(fetchHomeAirports.pending, (state) => {
                state.isLoadingAirports = true;
            })
            .addCase(fetchHomeAirports.fulfilled, (state, action) => {
                state.isLoadingAirports = false;
                state.airports = action.payload;
            })
            .addCase(fetchHomeAirports.rejected, (state, action) => {
                state.isLoadingAirports = false;
                state.error = action.payload;
            });
    }
});

export const { setSearchField, clearSearchForm } = homeSlice.actions;
export default homeSlice.reducer;
import { createSlice } from "@reduxjs/toolkit";
import { createAsyncAction } from "@/lib/redux/create_async_action";
import { homepageService } from "./homepage_service";
import { dataSearchService, DataSearch } from "@/features/admin/plane/data_search_service";

const extractResult = (res) => {
    const responseData = res?.data ?? res;
    return responseData?.result ?? responseData ?? {};
};

export const fetchHomeReviews = createAsyncAction(
    "home/fetchReviews",
    async () => {
        const res = await homepageService.getAllReviews({ pageIndex: 1, pageSize: 100 });
        const responseData = res?.data ?? res;
        return responseData?.result?.items ?? responseData?.result ?? [];
    }
);

export const fetchHomeAirports = createAsyncAction(
    "home/fetchAirports",
    async () => {
        const res = await dataSearchService.get([DataSearch.Airports]);
        const result = extractResult(res);
        return result?.airports ?? [];
    }
);

const homepageSlice = createSlice({
    name: "homepage",
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
        },
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
        },
    },
    extraReducers: (builder) => {
        builder
            .addCase(fetchHomeReviews.pending, (state) => {
                state.isLoadingReviews = true;
                state.error = null;
            })
            .addCase(fetchHomeReviews.fulfilled, (state, action) => {
                state.isLoadingReviews = false;
                const allReviews = action.payload || [];
                const visibleReviews = allReviews.filter((rev) => !rev.isHidden);
                state.reviews = visibleReviews.slice(-3).reverse();
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
                state.airports = action.payload || [];
            })
            .addCase(fetchHomeAirports.rejected, (state, action) => {
                state.isLoadingAirports = false;
                state.error = action.payload;
            });
    },
});

export const { setSearchField, clearSearchForm } = homepageSlice.actions;
export default homepageSlice.reducer;
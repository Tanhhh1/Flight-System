import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import { dashboardService } from "./dashboard_service";

export const fetchDashboardSummary = createAsyncThunk(
    "dashboard/fetchSummary",
    async (_, { rejectWithValue }) => {
        const { data } = await dashboardService.getSummary();
        if (!data.succeeded)
            return rejectWithValue(data.errors?.[0]?.errorMessage || "Lỗi tải thống kê.");
        return data.result;
    }
);

export const fetchRevenueByYear = createAsyncThunk(
    "dashboard/fetchRevenue",
    async (year, { rejectWithValue }) => {
        const { data } = await dashboardService.getRevenue(year);
        if (!data.succeeded)
            return rejectWithValue(data.errors?.[0]?.errorMessage || "Lỗi tải doanh thu.");
        return data.result;
    }
);

const dashboardSlice = createSlice({
    name: "dashboard",
    initialState: { summary: null, revenue: [], loadingSummary: false, loadingRevenue: false, error: null },
    reducers: {},
    extraReducers: (builder) => {
        builder
            .addCase(fetchDashboardSummary.pending, (state) => {
                state.loadingSummary = true;
                state.error = null;
            })
            .addCase(fetchDashboardSummary.fulfilled, (state, action) => {
                state.summary = action.payload;
                state.loadingSummary = false;
            })
            .addCase(fetchDashboardSummary.rejected, (state, action) => {
                state.error = action.payload;
                state.loadingSummary = false;
            })
            .addCase(fetchRevenueByYear.pending, (state) => {
                state.loadingRevenue = true;
                state.error = null;
            })
            .addCase(fetchRevenueByYear.fulfilled, (state, action) => {
                state.revenue = action.payload;
                state.loadingRevenue = false;
            })
            .addCase(fetchRevenueByYear.rejected, (state, action) => {
                state.error = action.payload;
                state.loadingRevenue = false;
            });
    },
});

export default dashboardSlice.reducer;
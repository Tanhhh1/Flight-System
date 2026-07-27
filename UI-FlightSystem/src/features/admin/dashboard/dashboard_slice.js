import { createSlice } from "@reduxjs/toolkit";
import { createAsyncAction } from "@/lib/redux/create_async_action";
import { dashboardService } from "./dashboard_service";

export const fetchDashboardSummary = createAsyncAction("dashboard/fetchSummary", () => dashboardService.getSummary());
export const fetchRevenueByYear = createAsyncAction("dashboard/fetchRevenue", (year) => dashboardService.getRevenue(year));

const dashboardSlice = createSlice({
    name: "dashboard",
    initialState: { summary: null, revenue: [], loadingSummary: false, loadingRevenue: false, error: null },
    reducers: {},

    extraReducers(builder) {
        builder
            .addCase(fetchDashboardSummary.pending, (state) => {
                state.loadingSummary = true;
                state.error = null;
            })
            .addCase(fetchDashboardSummary.fulfilled, (state, action) => {
                state.loadingSummary = false;
                state.summary = action.payload;
            })
            .addCase(fetchDashboardSummary.rejected, (state, action) => {
                state.loadingSummary = false;
                state.error = action.payload;
            })
            .addCase(fetchRevenueByYear.pending, (state) => {
                state.loadingRevenue = true;
                state.error = null;
            })
            .addCase(fetchRevenueByYear.fulfilled, (state, action) => {
                state.loadingRevenue = false;
                state.revenue = action.payload;
            })
            .addCase(fetchRevenueByYear.rejected, (state, action) => {
                state.loadingRevenue = false;
                state.error = action.payload;
            });
    },
});

export default dashboardSlice.reducer;
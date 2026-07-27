import { createSlice } from "@reduxjs/toolkit";
import { createAsyncAction } from "@/lib/redux/create_async_action";
import { flightSearchService } from "./search_service";
import { dataSearchService, DataSearch } from "@/features/admin/plane/data_search_service";

const legInitState = {
    items: [],
    totalCount: 0,
    pageIndex: 1,
    pageSize: 10,
    loading: false,
    error: null,
};

const loadFromSession = () => {
    try {
        return {
            selectedLegs: JSON.parse(sessionStorage.getItem("selectedLegs") ?? "[]"),
            searchMeta: JSON.parse(sessionStorage.getItem("searchMeta") ?? "null")
                ?? { type: "one-way", seatClass: "Economy", classId: 1 },
        };
    } catch {
        return {
            selectedLegs: [],
            searchMeta: { type: "one-way", seatClass: "Economy", classId: 1 },
        };
    }
};

const session = loadFromSession();

export const fetchOutbound = createAsyncAction(
    "searchResults/fetchOutbound",
    (payload) => flightSearchService.searchOutbound(payload)
);

export const fetchInbound = createAsyncAction(
    "searchResults/fetchInbound",
    (payload) => flightSearchService.searchInbound(payload)
);

export const fetchLeg = createAsyncAction(
    "searchResults/fetchLeg",
    async ({ activeLegView, payload }) => {
        const res = await flightSearchService.searchLeg(payload);
        return { ...res, data: { ...res.data, result: { legIndex: activeLegView, result: res.data.result } } };
    }
);

export const fetchFilterData = createAsyncAction(
    "searchResults/fetchFilterData",
    async () => {
        const res = await dataSearchService.get([DataSearch.Airlines, DataSearch.Services]);
        const data = res.data;
        return {
            ...res,
            data: {
                ...data,
                result: {
                    airlines: data.result?.airlines ?? [],
                    services: data.result?.services ?? [],
                }
            }
        };
    }
);

export const fetchRoundTrip = (outboundParams, inboundParams) => (dispatch) =>
    Promise.all([dispatch(fetchOutbound(outboundParams)), dispatch(fetchInbound(inboundParams))]);

export const fetchMultiCity = (legsParamsArray) => (dispatch) => {
    dispatch(initLegs(legsParamsArray.length));
    return dispatch(fetchLeg({ activeLegView: 0, payload: legsParamsArray[0] }));
};

const searchResultsSlice = createSlice({
    name: "searchResults",
    initialState: {
        searchMeta: session.searchMeta,
        outbound: { ...legInitState },
        inbound: { ...legInitState },
        legs: [],
        selectedLegs: session.selectedLegs,
        filterData: { airlines: [], services: [], loading: false },
        filters: {
            stopCount: null,
            airlineId: null,
            serviceIds: [],
            departureFromHour: null,
            departureToHour: null,
            arrivalFromHour: null,
            arrivalToHour: null,
        },
    },
    reducers: {
        setSearchMeta: (state, action) => {
            state.searchMeta = { ...state.searchMeta, ...action.payload };
            sessionStorage.setItem("searchMeta", JSON.stringify(state.searchMeta));
        },
        setFilters: (state, action) => {
            state.filters = { ...state.filters, ...action.payload };
        },
        resetFilters: (state) => {
            state.filters = {
                stopCount: null,
                airlineId: null,
                serviceIds: [],
                departureFromHour: null,
                departureToHour: null,
                arrivalFromHour: null,
                arrivalToHour: null,
            };
        },
        initLegs: (state, action) => {
            state.legs = Array(action.payload).fill(null).map(() => ({ ...legInitState }));
            state.selectedLegs = [];
            sessionStorage.removeItem("selectedLegs");
        },
        selectLegFlight: (state, action) => {
            const { legIndex, flight } = action.payload;
            state.selectedLegs = state.selectedLegs.filter((s) => s.legIndex < legIndex);
            state.selectedLegs.push({ legIndex, flight });
            sessionStorage.setItem("selectedLegs", JSON.stringify(state.selectedLegs));
        },
        resetSelectedLegs: (state) => {
            state.selectedLegs = [];
            sessionStorage.removeItem("selectedLegs");
        },
        removeLegFlight: (state, action) => {
            state.selectedLegs = state.selectedLegs.filter((s) => s.legIndex !== action.payload);
            sessionStorage.setItem("selectedLegs", JSON.stringify(state.selectedLegs));
        },
    },
    extraReducers: (builder) => {
        builder
            .addCase(fetchOutbound.pending, (state) => {
                state.outbound.loading = true;
                state.outbound.error = null;
            })
            .addCase(fetchOutbound.fulfilled, (state, { payload }) => {
                state.outbound.loading = false;
                state.outbound.items = payload?.items ?? [];
                state.outbound.totalCount = payload?.totalCount ?? 0;
            })
            .addCase(fetchOutbound.rejected, (state, { payload }) => {
                state.outbound.loading = false;
                state.outbound.error = payload;
            })

            .addCase(fetchInbound.pending, (state) => {
                state.inbound.loading = true;
                state.inbound.error = null;
            })
            .addCase(fetchInbound.fulfilled, (state, { payload }) => {
                state.inbound.loading = false;
                state.inbound.items = payload?.items ?? [];
                state.inbound.totalCount = payload?.totalCount ?? 0;
            })
            .addCase(fetchInbound.rejected, (state, { payload }) => {
                state.inbound.loading = false;
                state.inbound.error = payload;
            })

            .addCase(fetchLeg.pending, (state, { meta }) => {
                const i = meta.arg?.activeLegView ?? 0;
                if (state.legs[i]) state.legs[i].loading = true;
            })
            .addCase(fetchLeg.fulfilled, (state, { payload }) => {
                const { legIndex, result } = payload ?? {};
                if (legIndex !== undefined) {
                    state.legs[legIndex] = {
                        ...legInitState,
                        loading: false,
                        items: result?.items ?? [],
                        totalCount: result?.totalCount ?? 0,
                    };
                }
            })
            .addCase(fetchLeg.rejected, (state, action) => {
                const i = action.meta.arg?.activeLegView ?? 0;
                if (state.legs[i]) {
                    state.legs[i].loading = false;
                    state.legs[i].error = action.payload;
                }
            })

            .addCase(fetchFilterData.pending, (state) => {
                state.filterData.loading = true;
            })
            .addCase(fetchFilterData.fulfilled, (state, { payload }) => {
                state.filterData.loading = false;
                state.filterData.airlines = payload?.airlines ?? [];
                state.filterData.services = payload?.services ?? [];
            })
            .addCase(fetchFilterData.rejected, (state) => {
                state.filterData.loading = false;
            });
    },
});

export const {
    setSearchMeta,
    setFilters,
    resetFilters,
    initLegs,
    selectLegFlight,
    resetSelectedLegs,
    removeLegFlight,
} = searchResultsSlice.actions;

export default searchResultsSlice.reducer;
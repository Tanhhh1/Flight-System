import { useState, useEffect } from "react";
import { useSearchParams } from "react-router-dom";
import { useDispatch, useSelector } from "react-redux";
import { useAuthNavigate } from "@/features/auth/hooks/use_auth_navigate";
import { fetchOutbound, fetchInbound, fetchLeg, setSearchMeta, selectLegFlight, resetSelectedLegs, resetFilters, removeLegFlight } from "../search_slice";

import FilterSidebar from "./components/filter_sidebar";
import FlightList from "./components/flight_list";
import TripSummaryPanel from "./components/trip_panel";
import SelectedFlightsBar from "./components/select_flight";
import { SEAT_CLASS_ID_MAP } from "@/features/client/homepage/homepage_constants"
import "./styles/flight_result.css";

function SearchPage() {
    const dispatch = useDispatch();
    const authNavigate = useAuthNavigate();
    const [searchParams] = useSearchParams();
    const { filters, searchMeta, selectedLegs } = useSelector((s) => s.searchResults);

    const type = searchParams.get("type") ?? "one-way";
    const [activeLegView, setActiveLegView] = useState(0);
    const searchParamsString = searchParams.toString();

    const totalLegs = (() => {
        if (type === "one-way") return 1;
        if (type === "round-trip") return 2;
        try {
            return JSON.parse(searchParams.get("legs") ?? "[]").length;
        } catch {
            return 1;
        }
    })();

    useEffect(() => {
        if (selectedLegs.length < totalLegs) {
            setActiveLegView(selectedLegs.length);
        } else {
            setActiveLegView(totalLegs - 1);
        }
    }, [selectedLegs.length, totalLegs]);

    useEffect(() => {
        const seatClass = searchParams.get("seatClass") ?? "Economy";
        const classId = SEAT_CLASS_ID_MAP[seatClass] || Number(searchParams.get("classId") ?? 1);
        
        dispatch(setSearchMeta({ type, classId, seatClass }));
        dispatch(resetSelectedLegs());
        dispatch(resetFilters());
    }, [searchParamsString, dispatch, type]);

    useEffect(() => {
        const { type: t, classId } = searchMeta;
        if (!t || !classId) return;
        const contextPayload = { searchParams, classId, filters };

        if (t === "one-way") {
            dispatch(fetchOutbound(contextPayload));
        } else if (t === "round-trip") {
            if (activeLegView === 0) {
                dispatch(fetchOutbound(contextPayload));
            } else {
                dispatch(fetchInbound(contextPayload));
            }
        } else if (t === "multi-city") {
            const legsRaw = (() => {
                try {
                    return JSON.parse(searchParams.get("legs") ?? "[]");
                } catch {
                    return [];
                }
            })();
            const currentLegData = legsRaw[activeLegView];
            if (!currentLegData) return;
            dispatch(fetchLeg({ activeLegView, payload: { leg: currentLegData, classId, filters } }));
        }
    }, [filters, activeLegView, searchMeta.type, searchMeta.classId, searchParamsString, dispatch, searchParams]);

    const handleFlightSelected = (flight) => {
        dispatch(selectLegFlight({ legIndex: activeLegView, flight }));
        dispatch(resetFilters());
        if (activeLegView + 1 < totalLegs) {
            setActiveLegView(activeLegView + 1);
        }
    };

    const handleReselect = (legIndex) => {
        dispatch(resetFilters());
        setActiveLegView(legIndex);
        const kept = selectedLegs.filter((s) => s.legIndex < legIndex);
        dispatch(resetSelectedLegs());
        kept.forEach((s) => dispatch(selectLegFlight(s)));
    };

    const handleRemoveLeg = (legIndex) => {
        dispatch(removeLegFlight(legIndex));
        dispatch(resetFilters());
        setActiveLegView(legIndex);
    };

    const handleContinueBooking = () => {
        const bookingUrl = `/booking?${searchParams.toString()}`;
        authNavigate(bookingUrl);
    };

    return (
        <div className="search_page_container">
            <div className="flight_result_layout">
                <div className="flight_result_left_col">
                    <TripSummaryPanel
                        activeLegView={activeLegView}
                        onReselect={handleReselect}
                        onSelectLeg={(idx) => {
                            dispatch(resetFilters());
                            setActiveLegView(idx);
                        }}
                    />
                    <FilterSidebar />
                </div>
                <div className="flight_result_list_col">
                    <FlightList
                        activeLegView={activeLegView}
                        onFlightSelected={handleFlightSelected}
                    />
                </div>
            </div>

            <SelectedFlightsBar
                selectedLegs={selectedLegs}
                totalLegs={totalLegs}
                onRemove={handleRemoveLeg}
                onContinue={handleContinueBooking}
            />
        </div>
    );
}

export default SearchPage;
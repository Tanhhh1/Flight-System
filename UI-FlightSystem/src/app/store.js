import { configureStore } from "@reduxjs/toolkit";

import accountReducer from "@/features/admin/account/account_slice";
import authReducer from "@/features/auth/auth_slice";
import reviewReducer from "@/features/admin/review/review_slice";
import serviceReducer from "@/features/admin/service/service_slice";
import airlineReducer from "@/features/admin/airline/airline_slice";
import airportReducer from "@/features/admin/airport/airport_slice";
import planeReducer from "@/features/admin/plane/plane_slice";
import routeReducer from "@/features/admin/route/route_slice";
import flightReducer from "@/features/admin/flight/flight_slice";
import bookingReducer from "@/features/admin/booking/booking_slice";
import bookingDetailReducer from "@/features/admin/booking/detail_slice";
import dashboardReducer from "@/features/admin/dashboard/dashboard_slice";
import supportRequestReducer from "@/features/admin/support/support_slice";

import myReviewReducer from "@/features/client/review/review_slice";
import myBookingsReducer from "@/features/client/booking/booking_slice";
import homepageReducer from "@/features/client/homepage/homepage_slice";
import searchResultsReducer from "@/features/client/search/search_slice";
import seatSelectionReducer from "@/features/client/seat/seat_slice";
import mySupportRequestsReducer, { createSupportRequestReducer } from "@/features/client/support/support_slice";

const store = configureStore({
    reducer: {
        account: accountReducer,
        auth: authReducer,
        review: reviewReducer,
        service: serviceReducer,
        airline: airlineReducer,
        airport: airportReducer,
        plane: planeReducer,
        route: routeReducer,
        flight: flightReducer,
        booking: bookingReducer,
        bookingDetail: bookingDetailReducer,
        dashboard: dashboardReducer,
        supportRequest: supportRequestReducer,

        myReviews: myReviewReducer,
        myBookings: myBookingsReducer,
        homepage: homepageReducer,
        searchResults: searchResultsReducer,
        seatReverse: seatSelectionReducer,
        mySupportRequests: mySupportRequestsReducer,
        createSupportRequest: createSupportRequestReducer,
    },
});

export default store;
import { configureStore } from "@reduxjs/toolkit";
import accountReducer from "@/features/admin/account_management/account_slice";
import authReducer from "@/features/shared/auth/auth_slice";
import reviewReducer from "@/features/admin/review_management/review_slice";
import serviceReducer from "@/features/admin/service_management/service_slice";
import airlineReducer from "@/features/admin/airline_management/airline_slice";
import airportReducer from "@/features/admin/airport_management/airport_slice";
import planeReducer from "@/features/admin/plane_management/plane_slice";
import routeReducer from "@/features/admin/route_management/route_slice";
import flightReducer from "@/features/admin/flight_management/flight_slice";
import bookingReducer from "@/features/admin/booking_management/booking_slice";
import bookingDetailReducer from "@/features/admin/booking_management/booking_detail_slice";
import dashboardReducer from "@/features/admin/dashboard/dashboard_slice";
import supportRequestReducer from "@/features/admin/support_management/support_slice";


import myReviewReducer from "@/features/user/profile/review_slice";
import myBookingsReducer from "@/features/user/booking/booking_slice";
import homepageReducer from "@/features/user/homepage/homepage_slice"
import searchResultsReducer from "@/features/user/flight_search/search_results_slice";
import seatSelectionReducer from "@/features/user/seat_selection/seat_selection_slice";
import mySupportRequestsReducer from "@/features/user/support/support_slice";
import createSupportRequestReducer from "@/features/user/support/use_support_form";

export const store = configureStore({
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
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
  },
});

export default store;
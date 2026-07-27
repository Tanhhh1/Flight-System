import { lazy } from "react";
import { PATHS } from "./paths";

const Dashboard = lazy(() => import("@/features/admin/dashboard/pages/dashboard"));
const AccountList = lazy(() => import("@/features/admin/account/pages/account_list"));
const ServiceList = lazy(() => import("@/features/admin/service/pages/service_list"));
const ReviewList = lazy(() => import("@/features/admin/review/pages/review_list"));
const AirlineList = lazy(() => import("@/features/admin/airline/pages/airline_list"));
const AirportList = lazy(() => import("@/features/admin/airport/pages/airport_list"));
const PlaneList = lazy(() => import("@/features/admin/plane/pages/plane_list"));
const RouteList = lazy(() => import("@/features/admin/route/pages/route_list"));
const BookingList = lazy(() => import("@/features/admin/booking/pages/booking_list"));
const BookingDetail = lazy(() => import("@/features/admin/booking/pages/booking_detail"));
const FlightList = lazy(() => import("@/features/admin/flight/pages/flight_list"));
const FlightForm = lazy(() => import("@/features/admin/flight/pages/flight_form"));
const Profile = lazy(() => import("@/features/admin/profile/profile_page"));
const SupportList = lazy(() => import("@/features/admin/support/pages/support_list"));

export const adminPrivateRoutes = [
    { path: PATHS.ADMIN.DASHBOARD, element: <Dashboard /> },
    { path: PATHS.ADMIN.ACCOUNTS, element: <AccountList /> },
    { path: PATHS.ADMIN.SERVICES, element: <ServiceList /> },
    { path: PATHS.ADMIN.REVIEWS, element: <ReviewList /> },
    { path: PATHS.ADMIN.AIRLINES, element: <AirlineList /> },
    { path: PATHS.ADMIN.AIRPORTS, element: <AirportList /> },
    { path: PATHS.ADMIN.PLANES, element: <PlaneList /> },
    { path: PATHS.ADMIN.ROUTES, element: <RouteList /> },
    { path: PATHS.ADMIN.BOOKINGS, element: <BookingList /> },
    { path: PATHS.ADMIN.BOOKING_DETAIL, element: <BookingDetail /> },
    { path: PATHS.ADMIN.FLIGHTS, element: <FlightList /> },
    { path: PATHS.ADMIN.FLIGHT_NEW, element: <FlightForm /> },
    { path: PATHS.ADMIN.FLIGHT_EDIT, element: <FlightForm /> },
    { path: PATHS.ADMIN.PROFILE, element: <Profile /> },
    { path: PATHS.ADMIN.SUPPORT, element: <SupportList /> },
];
import { lazy } from "react";

export const adminPaths = {
    login: "/admin/login",
    admin: {
        root: "/admin",
        dashboard: "dashboard",
        accounts: "accounts",
        services: "services",
        reviews: "reviews",
        airlines: "airlines",
        airports: "airports",
        planes: "planes",
        routes: "routes",
        bookings: "bookings",
        bookingDetail: "bookings/:id",
        flights: "flights",
        flightNew: "flights/add",
        flightEdit: "flights/edit/:id",
        profile: "profile",
    }
};

const Dashboard = lazy(() => import("@/features/admin/dashboard/dashboard"));
const AccountList = lazy(() => import("@/features/admin/account_management/account_list"));
const ServiceList = lazy(() => import("@/features/admin/service_management/service_list"));
const ReviewList = lazy(() => import("@/features/admin/review_management/review_list"));
const AirlineList = lazy(() => import("@/features/admin/airline_management/airline_list"));
const AirportList = lazy(() => import("@/features/admin/airport_management/airport_list"));
const PlaneList = lazy(() => import("@/features/admin/plane_management/plane_list"));
const RouteList = lazy(() => import("@/features/admin/route_management/route_list"));
const BookingList = lazy(() => import("@/features/admin/booking_management/booking_list"));
const BookingDetail = lazy(() => import("@/features/admin/booking_management/booking_detail"));
const FlightList = lazy(() => import("@/features/admin/flight_management/flight_list"));
const FlightForm = lazy(() => import("@/features/admin/flight_management/flight_form"));
const Profile = lazy(() => import("@/features/admin/profile_management/profile"));

export const adminPrivateRoutes = [
    { path: adminPaths.admin.dashboard, element: <Dashboard /> },
    { path: adminPaths.admin.accounts, element: <AccountList /> },
    { path: adminPaths.admin.services, element: <ServiceList /> },
    { path: adminPaths.admin.reviews, element: <ReviewList /> },
    { path: adminPaths.admin.airlines, element: <AirlineList /> },
    { path: adminPaths.admin.airports, element: <AirportList /> },
    { path: adminPaths.admin.planes, element: <PlaneList /> },
    { path: adminPaths.admin.routes, element: <RouteList /> },
    { path: adminPaths.admin.bookings, element: <BookingList /> },
    { path: adminPaths.admin.bookingDetail, element: <BookingDetail /> },
    { path: adminPaths.admin.flights, element: <FlightList /> },
    { path: adminPaths.admin.flightNew, element: <FlightForm /> },
    { path: adminPaths.admin.flightEdit, element: <FlightForm /> },
    { path: adminPaths.admin.profile, element: <Profile /> },
];
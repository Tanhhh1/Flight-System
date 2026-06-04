import { lazy } from "react";

export const clientPaths = {
    home: "/",
    search: "/search",
    contact: "/contact",
    help: "/help",
    login: "/login",
    register: "/register",
    booking: "/booking",
    payment: "/payment",
    profile: {
        root: "/profile",
        edit: "edit",
        transactions: "transactions",
        reviews: "reviews",
    }
};

const Homepage = lazy(() => import("@/features/user/homepage/homepage")); 
const Contact = lazy(() => import("@/features/user/support/contact"));
const HelpFaq = lazy(() => import("@/features/user/support/help_faq"));
const SearchFlight = lazy(() => import("@/features/user/flight_search/search_results"));
const PassengerInfo = lazy(() => import("@/features/user/booking/passenger_info"));
const PaymentMethod = lazy(() => import("@/features/user/payment/payment_method"));
const ProfileLayout = lazy(() => import("@/features/user/profile/profile_layout"));
const EditInfo = lazy(() => import("@/features/user/profile/edit_info"));
const TransactionHistory = lazy(() => import("@/features/user/profile/transaction_history"));
const SystemReview = lazy(() => import("@/features/user/profile/system_review"));

export const clientPublicRoutes = [
    { path: clientPaths.home, element: <Homepage /> },
    { path: clientPaths.contact, element: <Contact /> },
    { path: clientPaths.help, element: <HelpFaq /> },
    { path: clientPaths.search, element: <SearchFlight /> },
    { path: clientPaths.booking, element: <PassengerInfo /> },
    { path: clientPaths.payment, element: <PaymentMethod/>},
    {
        path: clientPaths.profile.root,
        element: <ProfileLayout />,
        children: [
            { path: clientPaths.profile.edit, element: <EditInfo /> },
            { path: clientPaths.profile.transactions, element: <TransactionHistory /> },
            { path: clientPaths.profile.reviews, element: <SystemReview /> },
        ]
    }
];

export const clientPrivateRoutes = [
    
];
import { lazy } from "react";

export const clientPaths = {
    home: "/",
    search: "/search",
    contact: "/contact",
    help: "/help",
    booking: "/booking",
    payment: "/payment",
    success: "/payment/success",
    seatSelection: "/seat-selection",
    profile: {
        root: "/profile",
        edit: "edit",
        transactions: "transactions",
        reviews: "reviews",
        verify: "verify",
        support: "support-requests",
        supportCreate: "support-requests/create",
    },
};

const Homepage = lazy(() => import("@/features/user/homepage/components/home/homepage"));
const Contact = lazy(() => import("@/features/user/support/contact/contact"));
const HelpFaq = lazy(() => import("@/features/user/support/help/help_faq"));
const SearchFlight = lazy(() => import("@/features/user/flight_search/components/result/flight_result"));
const PassengerInfo = lazy(() => import("@/features/user/booking/components/passenger_info"));
const PaymentMethod = lazy(() => import("@/features/user/payment/components/payment/payment_method"));
const PaymentSuccess = lazy(() => import("@/features/user/payment/components/result/payment_success"));
const ProfileLayout = lazy(() => import("@/features/user/profile/components/layout/profile_layout"));
const EditInfo = lazy(() => import("@/features/user/profile/components/info/edit_info"));
const TransactionHistory = lazy(() => import("@/features/user/profile/components/transaction/transaction_history"));
const SystemReview = lazy(() => import("@/features/user/profile/components/review/system_review"));
const Verify = lazy(() => import("@/features/user/profile/components/verify/booking_code_form"));
const SeatSelection = lazy(() => import("@/features/user/seat_selection/components/select/seat_selection"));
const MySupportRequests = lazy(() => import("@/features/user/profile/components/support/support_request"));
const SupportRequestForm = lazy(() => import("@/features/user/profile/components/support/support_request_form"));

export const clientPublicRoutes = [
    { path: clientPaths.home, element: <Homepage /> },
    { path: clientPaths.contact, element: <Contact /> },
    { path: clientPaths.help, element: <HelpFaq /> },
    { path: clientPaths.search, element: <SearchFlight /> },
    { path: clientPaths.booking, element: <PassengerInfo /> },
    { path: clientPaths.payment, element: <PaymentMethod /> },
    { path: clientPaths.success, element: <PaymentSuccess /> },
    
];

export const clientPrivateRoutes = [
    {
        path: clientPaths.profile.root,
        element: <ProfileLayout />,
        children: [
            { path: clientPaths.profile.edit, element: <EditInfo /> },
            { path: clientPaths.profile.transactions, element: <TransactionHistory /> },
            { path: clientPaths.profile.reviews, element: <SystemReview /> },
            { path: clientPaths.profile.verify, element: <Verify /> },
            { path: clientPaths.profile.support, element: <MySupportRequests /> },
            { path: clientPaths.profile.supportCreate, element: <SupportRequestForm /> },
        ],
    },
    { path: clientPaths.seatSelection, element: <SeatSelection /> },
];
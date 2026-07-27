import { lazy } from "react";
import { PATHS } from "./paths";

const Homepage = lazy(() => import("@/features/client/homepage/pages/homepage"));
const Contact = lazy(() => import("@/features/client/contact/contact"));
const HelpFaq = lazy(() => import("@/features/client/help/help_faq"));
const SearchFlight = lazy(() => import("@/features/client/search/pages/search"));
const PassengerInfo = lazy(() => import("@/features/client/booking/pages/booking_page"));
const PaymentMethod = lazy(() => import("@/features/client/payment/pages/payment_method"));
const PaymentSuccess = lazy(() => import("@/features/client/payment/pages/payment_success"));
const ProfileLayout = lazy(() => import("@/features/client/profile/profile_page"));
const EditInfo = lazy(() => import("@/features/client/profile/components/update_info"));
const TransactionHistory = lazy(() => import("@/features/client/profile/components/transaction_history"));
const SystemReview = lazy(() => import("@/features/client/profile/components/review"));
const Verify = lazy(() => import("@/features/client/profile/components/verify_code"));
const SeatSelection = lazy(() => import("@/features/client/seat/pages/seat_selection"));
const MySupportRequests = lazy(() => import("@/features/client/profile/components/support/support_request"));
const SupportRequestForm = lazy(() => import("@/features/client/profile/components/support/request_form"));

export const clientPublicRoutes = [
    { path: PATHS.CLIENT.HOME, element: <Homepage /> },
    { path: PATHS.CLIENT.CONTACT, element: <Contact /> },
    { path: PATHS.CLIENT.HELP, element: <HelpFaq /> },
    { path: PATHS.CLIENT.SEARCH, element: <SearchFlight /> },
    { path: PATHS.CLIENT.BOOKING, element: <PassengerInfo /> },
    { path: PATHS.CLIENT.PAYMENT, element: <PaymentMethod /> },
    { path: PATHS.CLIENT.SUCCESS, element: <PaymentSuccess /> },
];

export const clientPrivateRoutes = [
    {
        path: PATHS.CLIENT.PROFILE.ROOT,
        element: <ProfileLayout />,
        children: [
            { path: PATHS.CLIENT.PROFILE.EDIT, element: <EditInfo /> },
            { path: PATHS.CLIENT.PROFILE.TRANSACTIONS, element: <TransactionHistory /> },
            { path: PATHS.CLIENT.PROFILE.REVIEWS, element: <SystemReview /> },
            { path: PATHS.CLIENT.PROFILE.VERIFY, element: <Verify /> },
            { path: PATHS.CLIENT.PROFILE.SUPPORT, element: <MySupportRequests /> },
            { path: PATHS.CLIENT.PROFILE.SUPPORT_CREATE, element: <SupportRequestForm /> },
        ],
    },
    { path: PATHS.CLIENT.SEAT_SELECTION, element: <SeatSelection /> },
];
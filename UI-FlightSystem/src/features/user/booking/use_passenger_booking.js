import { useState, useEffect, useMemo, useCallback } from "react";
import { useSelector } from "react-redux";
import { useSearchParams, useNavigate } from "react-router-dom";
import { dataSearchService, DataSearch } from "@/services/data_search_service";
import { bookingService } from "./booking_service";
import { TRIP_TYPE_MAP, DEFAULT_PASSENGER } from "@/constants/booking";

export function usePassengerBooking() {
    const navigate = useNavigate();
    const [searchParams] = useSearchParams();
    const type = searchParams.get("type") ?? "one-way";

    const { selectedLegs, searchMeta } = useSelector((s) => s.searchResults);
    const { classId } = searchMeta;

    const [passengers, setPassengers] = useState([{ ...DEFAULT_PASSENGER }]);
    const [passengerTypes, setPassengerTypes] = useState([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    const clearError = useCallback(() => setError(null), []);

    useEffect(() => {
        const fetchPassengerTypes = async () => {
            try {
                const res = await dataSearchService.get([DataSearch.PassengerType]);
                const types = res.data?.result?.passengerTypes ?? [];
                setPassengerTypes(types);
                if (types.length > 0) {
                    setPassengers([{ ...DEFAULT_PASSENGER, typeId: types[0].typeId }]);
                }
            } catch {
                setError("Không thể tải danh sách loại hành khách.");
            }
        };
        fetchPassengerTypes();
    }, []);

    const { pricePerPassenger, grandTotal } = useMemo(() => {
        const pricePerPassenger = passengers.map((passenger) => {
            const passengerType = passengerTypes.find((t) => t.typeId === Number(passenger.typeId));
            const discountRate = passengerType?.discountRate ?? 0;

            const total = selectedLegs.reduce((sum, leg) => {
                const seatClass = leg.flight.seatClasses?.find((c) => c.classId === classId);
                const basePrice = seatClass?.price ?? 0;
                return sum + basePrice * (1 - discountRate);
            }, 0);

            return total;
        });

        const grandTotal = pricePerPassenger.reduce((sum, p) => sum + p, 0);
        return { pricePerPassenger, grandTotal };
    }, [passengers, passengerTypes, selectedLegs, classId]);

    const handleChange = useCallback((index, field, value) => {
        setPassengers((prev) => {
            const updated = [...prev];
            updated[index] = { ...updated[index], [field]: value };
            return updated;
        });
    }, []);

    const addPassenger = useCallback(() => { setPassengers((prev) => [...prev, { ...DEFAULT_PASSENGER, typeId: passengerTypes[0]?.typeId ?? null }]) }, [passengerTypes]);
    const removePassenger = useCallback((index) => {
        setPassengers((prev) => {
            if (prev.length === 1) return prev;
            return prev.filter((_, i) => i !== index);
        });
    }, []);

    const handleSubmit = async () => {
        const isMissingInfo = passengers.some(
            (p) => !p.fullName.trim() || !p.dob || !p.typeId
        );
        if (isMissingInfo) {
            setError("Vui lòng điền đầy đủ thông tin tất cả hành khách.");
            return;
        }
        if (selectedLegs.length === 0) {
            setError("Không tìm thấy thông tin chuyến bay.");
            return;
        }

        const flightIds = selectedLegs.map((leg) => leg.flight.flightId);

        const passengerList = passengers.map((passenger) => ({
            typeId: Number(passenger.typeId),
            fullName: passenger.fullName.trim(),
            gender: passenger.gender,
            birthday: passenger.dob,
            country: passenger.nationality,
        }));

        const payload = { classId, tripType: TRIP_TYPE_MAP[type], flightIds, passengers: passengerList };

        try {
            setLoading(true);
            setError(null);
            const res = await bookingService.create(payload);
            if (!res.data?.succeeded) {
                const message = res.data?.errors?.[0]?.errorMessage ?? "Đặt vé thất bại, vui lòng thử lại.";
                setError(message);
                return;
            }
            const bookingId = res.data?.result?.bookingId;
            navigate(`/payment?bookingId=${bookingId}&${searchParams.toString()}`);
        } catch (err) {
            const message =
                err.response?.data?.errors?.[0]?.errorMessage ??
                err.response?.data?.message ??
                "Đặt vé thất bại, vui lòng thử lại.";
            setError(message);
        } finally {
            setLoading(false);
        }
    };

    return {
        passengers, passengerTypes, pricePerPassenger, grandTotal, loading, clearError,
        error, selectedLegs, handleChange, addPassenger, removePassenger, handleSubmit,
    };
}
import { useEffect } from "react";
import { useForm, useFieldArray } from "react-hook-form";
import { flightService } from "./flight_service";
import { applyServerErrors } from "@/hooks/use_shared_form";
import { SEAT_CLASSES, COMMON_RULES, DEFAULT_SEAT_PRICES, FLIGHT_CREATE_DEFAULT_VALUES } from "@/constants/flight";

const toLocalDatetimeInput = (value) => {
    if (!value) return "";
    const d = new Date(value);
    const pad = (n) => String(n).padStart(2, "0");
    return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`;
};

const buildResetValues = (isEdit, flightData) => {
    if (!isEdit || !flightData) return FLIGHT_CREATE_DEFAULT_VALUES;
    return {
        planeId: String(flightData.planeId ?? ""),
        routeId: String(flightData.routeId ?? ""),
        departureTime: toLocalDatetimeInput(flightData.departureTime),
        isRefund: flightData.isRefund ?? false,
        isChange: flightData.isChange ?? false,
        status: flightData.status ?? "Active",
        segments: flightData.segments?.map((s) => ({
            segmentId: String(s.segmentId ?? ""),
            routeId: String(s.routeId ?? ""),
            departureTime: toLocalDatetimeInput(s.departureTime),
        })) ?? [],
        seatPrices: flightData.seatPrices?.length > 0
            ? SEAT_CLASSES.map((c) => {
                const found = flightData.seatPrices.find((p) => p.classId === c.classId);
                return { classId: c.classId, price: found ? String(found.price) : "" };
            })
            : DEFAULT_SEAT_PRICES,
        serviceIds: flightData.services?.map((s) => String(s.serviceId)) ?? [],
    };
};

const buildPayload = (values, isEdit, flightId) => {
    const payload = {
        planeId: parseInt(values.planeId),
        routeId: parseInt(values.routeId),
        departureTime: values.departureTime,
        isRefund: values.isRefund,
        isChange: values.isChange,
        segments: values.segments.map((s) => ({
            ...(s.segmentId ? { segmentId: parseInt(s.segmentId) } : {}),
            routeId: parseInt(s.routeId),
            departureTime: s.departureTime,
        })),
        seatPrices: values.seatPrices.map((p) => ({
            classId: p.classId,
            price: parseFloat(p.price),
        })),
        services: values.serviceIds.map((id) => ({ serviceId: parseInt(id) })),
    };
    if (isEdit) {
        payload.flightId = flightId;
        payload.status = values.status;
    }
    return payload;
};

export function useFlightForm({ mode, flightData, onSuccess, onClose }) {
    const isEdit = mode === "edit";
    const { register, handleSubmit, reset, setError, formState, control, setValue, watch } = useForm({defaultValues: FLIGHT_CREATE_DEFAULT_VALUES });
    const { fields: segmentFields, append: appendSegment, remove: removeSegment } = useFieldArray({ control, name: "segments" });
    const { fields: seatPriceFields } = useFieldArray({ control, name: "seatPrices" });
    useEffect(() => { reset(buildResetValues(isEdit, flightData))}, [isEdit, flightData, reset]);
    const enhancedRegister = (name, overrides) => register(name, { ...COMMON_RULES[name], ...overrides });
    const serviceIds = watch("serviceIds") ?? [];
    const toggleService = (serviceId) => {
        const id = String(serviceId);
        setValue( "serviceIds", serviceIds.includes(id) ? serviceIds.filter((s) => s !== id) : [...serviceIds, id]);
    };
    const onSubmit = handleSubmit(async (values) => {
        try {
            const payload = buildPayload(values, isEdit, flightData?.flightId);
            const { data } = isEdit ? await flightService.update(flightData.flightId, payload) : await flightService.create(payload);
            if (!data.succeeded) {
                applyServerErrors(setError, data);
                return;
            }
            if (!isEdit) reset();
            onSuccess?.();
            onClose?.();
        } catch (err) {
            applyServerErrors(
                setError,
                err.response?.data ?? { message: err.message ?? "Lỗi kết nối server" }
            );
        }
    });

    return { register: enhancedRegister, formState, onSubmit, isEdit, control, setValue, watch,
        segmentFields, appendSegment, removeSegment, seatPriceFields, serviceIds, toggleService,
    };
}
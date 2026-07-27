import { useEffect } from "react";
import { useFieldArray } from "react-hook-form";
import { useCrudForm } from "@/hooks/use_crud_form";
import { flightService } from "./flight_service";
import {
    SEAT_CLASSES,
    COMMON_RULES,
    DEFAULT_SEAT_PRICES,
    FLIGHT_CREATE_DEFAULT_VALUES,
} from "./flight_constants";

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
        segments:
            flightData.segments?.map((s) => ({
                segmentId: String(s.segmentId ?? ""),
                routeId: String(s.routeId ?? ""),
                departureTime: toLocalDatetimeInput(s.departureTime),
            })) ?? [],
        seatPrices:
            flightData.seatPrices?.length > 0
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
        segments: (values.segments || []).map((s) => ({
            ...(s.segmentId ? { segmentId: parseInt(s.segmentId) } : {}),
            routeId: parseInt(s.routeId),
            departureTime: s.departureTime,
        })),
        seatPrices: (values.seatPrices || []).map((p) => ({
            classId: p.classId,
            price: parseFloat(p.price),
        })),
        services: (values.serviceIds || []).map((id) => ({ serviceId: parseInt(id) })),
    };
    if (isEdit) {
        payload.flightId = flightId;
        payload.status = values.status;
    }
    return payload;
};

export function useFlightForm({ mode, flightData, onSuccess, onClose }) {
    const isEdit = mode === "edit";

    const crudForm = useCrudForm({
        isEdit,
        defaultValues: FLIGHT_CREATE_DEFAULT_VALUES,
        buildResetValues: () => buildResetValues(isEdit, flightData),
        onSubmitApi: (values) => {
            const payload = buildPayload(values, isEdit, flightData?.flightId);
            return isEdit
                ? flightService.update(flightData.flightId, payload)
                : flightService.create(payload);
        },
        onSuccess,
        onClose,
    });

    const { control, register, setValue, watch, reset } = crudForm;

    useEffect(() => {
        if (isEdit && flightData) {
            reset(buildResetValues(isEdit, flightData));
        }
    }, [isEdit, flightData, reset]);

    const { fields: segmentFields, append: appendSegment, remove: removeSegment } = useFieldArray({
        control,
        name: "segments",
    });

    const { fields: seatPriceFields } = useFieldArray({
        control,
        name: "seatPrices",
    });

    const enhancedRegister = (name, overrides) =>
        register(name, { ...COMMON_RULES[name], ...overrides });

    const serviceIds = watch("serviceIds") ?? [];
    const toggleService = (serviceId) => {
        const id = String(serviceId);
        setValue(
            "serviceIds",
            serviceIds.includes(id) ? serviceIds.filter((s) => s !== id) : [...serviceIds, id]
        );
    };

    return {
        ...crudForm,
        register: enhancedRegister,
        isEdit,
        segmentFields,
        appendSegment,
        removeSegment,
        seatPriceFields,
        serviceIds,
        toggleService,
    };
}
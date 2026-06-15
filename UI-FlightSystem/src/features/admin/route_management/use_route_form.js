import { useEffect, useState } from "react";
import { useCrudForm } from "@/hooks/use_shared_form";
import { routeService } from "./route_service";
import { airportService } from "@/features/admin/airport_management/airport_service";
import { CREATE_DEFAULT_VALUES, EDIT_DEFAULT_VALUES, BE_FIELD_MAP, COMMON_RULES } from "@/constants/route";

function buildPayload(values, isEdit, routeId) {
    const payload = {
        originAirportId: parseInt(values.originAirportId),
        destinationAirportId: parseInt(values.destinationAirportId),
        flightDuration: parseInt(values.flightDuration),
    };
    if (isEdit) {
        payload.routeId = routeId;
        payload.status = values.status;
    }
    return payload;
}

function buildResetValues(isEdit, routeData) {
    if (!isEdit || !routeData) return CREATE_DEFAULT_VALUES;
    return {
        originAirportId: routeData.originAirportId ?? "",
        destinationAirportId: routeData.destinationAirportId ?? "",
        flightDuration: routeData.flightDuration ?? "",
        status: routeData.status ?? "Active",
    };
}

export function useRouteForm({ mode, routeData, onSuccess, onClose }) {
    const isEdit = mode === "edit";
    const [airports, setAirports] = useState([]);
    const [airportsLoading, setAirportsLoading] = useState(false);

    useEffect(() => {
        setAirportsLoading(true);
        airportService.getAll({ pageIndex: 1, pageSize: 999, status: "Active" })
            .then(({ data }) => {
                if (data.succeeded) setAirports(data.result?.items ?? []);
            })
            .catch(() => setAirports([]))
            .finally(() => setAirportsLoading(false));
    }, []);

    const { register: baseRegister, formState, onSubmit, setValue, control } = useCrudForm({
        isEdit,
        defaultValues: isEdit ? EDIT_DEFAULT_VALUES : CREATE_DEFAULT_VALUES,
        buildResetValues: () => buildResetValues(isEdit, routeData),
        beFieldMap: BE_FIELD_MAP,
        onSubmitApi: (values) => {
            const payload = buildPayload(values, isEdit, routeData?.routeId);
            return isEdit ? routeService.update(routeData.routeId, payload) : routeService.create(payload);
        },
        onSuccess,
        onClose,
    });

    const register = (name, overrides) => baseRegister(name, { ...COMMON_RULES[name], ...overrides });
    return { register, formState, onSubmit, isEdit, setValue, control, airports, airportsLoading };
}
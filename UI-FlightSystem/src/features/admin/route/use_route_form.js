import { useCrudForm } from "@/hooks/use_crud_form";
import { routeService } from "./route_service";
import { CREATE_DEFAULT_VALUES, EDIT_DEFAULT_VALUES, BE_FIELD_MAP, COMMON_RULES } from "./route_constants";

function buildPayload(values, isEdit, routeId) {
    const payload = {
        originAirportId: parseInt(values.originAirportId, 10),
        destinationAirportId: parseInt(values.destinationAirportId, 10),
        flightDuration: parseInt(values.flightDuration, 10),
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

    const { register: baseRegister, formState, onSubmit, setValue, control, reset, setError,
    } = useCrudForm({
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

    const register = (name, overrides) =>
        baseRegister(name, { ...COMMON_RULES[name], ...overrides });

    return { register, formState, onSubmit, isEdit, setValue, control, reset, setError};
}
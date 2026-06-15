import { useCrudForm } from "@/hooks/use_shared_form";
import { airportService } from "./airport_service";
import { CREATE_DEFAULT_VALUES, EDIT_DEFAULT_VALUES, BE_FIELD_MAP, COMMON_RULES } from "@/constants/airport";

function buildPayload(values, isEdit, airportId) {
    const payload = {
        airportCode: values.airportCode,
        airportName: values.airportName,
        city: values.city,
        country: values.country,
    };
    if (isEdit) {
        payload.airportId = airportId;
        payload.status = values.status;
    }
    return payload;
}

function buildResetValues(isEdit, airportData) {
    if (!isEdit || !airportData) return CREATE_DEFAULT_VALUES;
    return {
        airportCode: airportData.airportCode ?? "",
        airportName: airportData.airportName ?? "",
        city: airportData.city ?? "",
        country: airportData.country ?? "",
        status: airportData.status ?? "Active",
    };
}

export function useAirportForm({ mode, airportData, onSuccess, onClose }) {
    const isEdit = mode === "edit";

    const { register: baseRegister, control, setValue, formState, onSubmit } = useCrudForm({
        isEdit,
        defaultValues: isEdit ? EDIT_DEFAULT_VALUES : CREATE_DEFAULT_VALUES,
        buildResetValues: () => buildResetValues(isEdit, airportData),
        beFieldMap: BE_FIELD_MAP,
        onSubmitApi: (values) => {
            const payload = buildPayload(values, isEdit, airportData?.airportId);
            return isEdit
                ? airportService.update(airportData.airportId, payload)
                : airportService.create(payload);
        },
        onSuccess,
        onClose,
    });

    const register = (name, overrides) =>
        baseRegister(name, { ...COMMON_RULES[name], ...overrides });

    return { register, control, setValue, formState, onSubmit, isEdit };
}
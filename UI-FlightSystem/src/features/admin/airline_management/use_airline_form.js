import { useCrudForm } from "@/hooks/use_shared_form";
import { airlineService } from "./airline_service";
import { CREATE_DEFAULT_VALUES, EDIT_DEFAULT_VALUES, BE_FIELD_MAP, COMMON_RULES } from "@/constants/airlines";

function buildPayload(values, isEdit, airlineId) {
    const payload = {
        airlineName: values.airlineName,
        airlineCode: values.airlineCode,
        country: values.country,
    };
    if (isEdit) {
        payload.airlineId = airlineId;
        payload.status = values.status;
    }
    return payload;
}

function buildResetValues(isEdit, airlineData) {
    if (!isEdit || !airlineData) return CREATE_DEFAULT_VALUES;
    return {
        airlineName: airlineData.airlineName ?? "",
        airlineCode: airlineData.airlineCode ?? "",
        country: airlineData.country ?? "",
        status: airlineData.status ?? "Active",
    };
}

export function useAirlineForm({ mode, airlineData, onSuccess, onClose }) {
    const isEdit = mode === "edit";

    const { register: baseRegister, setValue, control, formState, onSubmit } = useCrudForm({
        isEdit,
        defaultValues: isEdit ? EDIT_DEFAULT_VALUES : CREATE_DEFAULT_VALUES,
        buildResetValues: () => buildResetValues(isEdit, airlineData),
        beFieldMap: BE_FIELD_MAP,
        onSubmitApi: (values) => {
            const payload = buildPayload(values, isEdit, airlineData?.airlineId);
            return isEdit
                ? airlineService.update(airlineData.airlineId, payload)
                : airlineService.create(payload);
        },
        onSuccess,
        onClose,
    });

    const register = (name, overrides) =>
        baseRegister(name, { ...COMMON_RULES[name], ...overrides });

    return { register, control, setValue, formState, onSubmit, isEdit };
}
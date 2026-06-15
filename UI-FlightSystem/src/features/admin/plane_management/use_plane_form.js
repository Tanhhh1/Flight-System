import { useCrudForm } from "@/hooks/use_shared_form";
import { planeService } from "./plane_service";
import { CREATE_DEFAULT_VALUES, EDIT_DEFAULT_VALUES, BE_FIELD_MAP, COMMON_RULES } from "@/constants/plane";

function buildPayload(values, isEdit, planeId) {
    const payload = {
        planeModel: values.planeModel,
        airlineId: parseInt(values.airlineId),
    };
    if (isEdit) {
        payload.planeId = planeId;
        payload.status = values.status;
    }
    return payload;
}

function buildResetValues(isEdit, planeData) {
    if (!isEdit || !planeData) return CREATE_DEFAULT_VALUES;
    return {
        planeModel: planeData.planeModel ?? "",
        airlineId: planeData.airlineId ?? "",
        status: planeData.status ?? "Active",
    };
}

export function usePlaneForm({ mode, planeData, onSuccess, onClose }) {
    const isEdit = mode === "edit";

    const { register: baseRegister, control, formState, onSubmit, setValue } = useCrudForm({
        isEdit,
        defaultValues: isEdit ? EDIT_DEFAULT_VALUES : CREATE_DEFAULT_VALUES,
        buildResetValues: () => buildResetValues(isEdit, planeData),
        beFieldMap: BE_FIELD_MAP,
        data: planeData,
        onSubmitApi: (values) => {
            const payload = buildPayload(values, isEdit, planeData?.planeId);
            return isEdit
                ? planeService.update(planeData.planeId, payload)
                : planeService.create(payload);
        },
        onSuccess,
        onClose,
    });

    const register = (name, overrides) =>
        baseRegister(name, { ...COMMON_RULES[name], ...overrides });

    return { register, control, formState, onSubmit, isEdit, setValue };
}
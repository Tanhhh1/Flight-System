import { useCrudForm } from "@/hooks/use_shared_form";
import { serviceService } from "./service_service";
import { CREATE_DEFAULT_VALUES, EDIT_DEFAULT_VALUES, BE_FIELD_MAP, COMMON_RULES } from "@/constants/service";

function buildPayload(values, isEdit, serviceId) {
    const payload = {
        serviceName: values.serviceName,
        description: values.description,
    };
    if (isEdit) {
        payload.serviceId = serviceId;
        payload.isActive = values.isActive;
    }
    return payload;
}

function buildResetValues(isEdit, serviceData) {
    if (!isEdit || !serviceData) return CREATE_DEFAULT_VALUES;
    return {
        serviceName: serviceData.serviceName ?? "",
        description: serviceData.description ?? "",
        isActive: serviceData.isActive ?? true,
    };
}

export function useServiceForm({ mode, serviceData, onSuccess, onClose }) {
    const isEdit = mode === "edit";

    const { register: baseRegister, formState, onSubmit } = useCrudForm({
        isEdit,
        defaultValues: isEdit ? EDIT_DEFAULT_VALUES : CREATE_DEFAULT_VALUES,
        buildResetValues: () => buildResetValues(isEdit, serviceData),
        beFieldMap: BE_FIELD_MAP,
        onSubmitApi: (values) => {
            const payload = buildPayload(values, isEdit, serviceData?.serviceId);
            return isEdit ? serviceService.update(serviceData.serviceId, payload) : serviceService.create(payload);
        },
        onSuccess,
        onClose,
    });

    const register = (name, overrides) =>
        baseRegister(name, { ...COMMON_RULES[name], ...overrides });

    return { register, formState, onSubmit, isEdit };
}
import { useWatch } from "react-hook-form";
import { useCrudForm } from "@/hooks/use_shared_form";
import { accountService } from "./account_service";
import { CREATE_DEFAULT_VALUES, EDIT_DEFAULT_VALUES, BE_FIELD_MAP, COMMON_RULES, CREATE_ONLY_RULES } from "@/constants/account";

function buildPayload(values, isEdit, userId) {
    const { confirmPassword, birthday, roleNames, userName, ...rest } = values;
    const payload = {
        ...rest,
        birthday: birthday?.trim() || null,
        roleNames: [roleNames],
    };
    if (isEdit) {
        delete payload.password;
        payload.userId = userId;
    } else {
        payload.userName = userName;
    }
    return payload;
}

function buildResetValues(isEdit, accountData) {
    if (!isEdit || !accountData) return CREATE_DEFAULT_VALUES;
    return {
        userName: accountData.userName ?? "",
        fullname: accountData.fullname ?? "",
        email: accountData.email ?? "",
        phoneNumber: accountData.phoneNumber ?? "",
        gender: accountData.gender ?? "Nam",
        birthday: accountData.birthday?.split("T")[0] ?? "",
        address: accountData.address ?? "",
        roleNames: accountData.roles?.[0] ?? "staff",
    };
}

export function useAccountForm({ mode, accountData, onSuccess, onClose }) {
    const isEdit = mode === "edit";

    const { register: baseRegister, formState, control, onSubmit } = useCrudForm({
        isEdit,
        defaultValues: isEdit ? EDIT_DEFAULT_VALUES : CREATE_DEFAULT_VALUES,
        buildResetValues: () => buildResetValues(isEdit, accountData),
        beFieldMap: BE_FIELD_MAP,
        onSubmitApi: (values) => {
            const payload = buildPayload(values, isEdit, accountData?.userId);
            return isEdit ? accountService.update(accountData.userId, payload) : accountService.create(payload);
        },
        onSuccess,
        onClose,
    });

    const passwordValue = useWatch({ control, name: "password" });
    const validationRules = {
        ...COMMON_RULES,
        ...(!isEdit && {
            ...CREATE_ONLY_RULES,
            confirmPassword: {
                required: "Vui lòng xác nhận lại mật khẩu",
                validate: (value) => value === passwordValue || "Mật khẩu xác nhận không khớp",
            },
        }),
    };
    const register = (name, overrides) => baseRegister(name, { ...validationRules[name], ...overrides });
    return { register, formState, onSubmit, isEdit };
}
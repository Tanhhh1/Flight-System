import { useForm } from "react-hook-form";
import profileService from "../profile_service";
import { CHANGE_PASSWORD_RULES, PROFILE_MESSAGES, DEFAULT_VALUES_PW } from "../profile_constants";
import { applyServerErrors } from "@/utils/form_error";

export function useChangePassword({ onSuccess } = {}) {
    const { register, handleSubmit, reset, setError, formState, watch } = useForm({
        defaultValues: DEFAULT_VALUES_PW,
    });

    const newPasswordValue = watch("newPassword");

    const reg = (name) =>
        register(name, {
            ...CHANGE_PASSWORD_RULES[name],
            ...(name === "confirmNewPassword" && {
                validate: (v) => v === newPasswordValue || PROFILE_MESSAGES.PASSWORD_MISMATCH,
            }),
        });

    const onSubmit = handleSubmit(async (values) => {
        try {
            const { data } = await profileService.changePassword(values);
            if (!data.succeeded) {
                applyServerErrors(setError, data);
                return;
            }
            reset();
            if (onSuccess) onSuccess(PROFILE_MESSAGES.CHANGE_PASSWORD_SUCCESS);
        } catch (err) {
            applyServerErrors(setError, err.response?.data ?? { message: err.message });
        }
    });

    return { reg, formState, onSubmit };
}
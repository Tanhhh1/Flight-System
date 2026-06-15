import { useForm } from "react-hook-form";
import { profileService } from "./profile_service";
import { applyServerErrors } from "@/hooks/use_shared_form";

const DEFAULT_VALUES = {
    currentPassword: "",
    newPassword: "",
    confirmNewPassword: "",
};

const RULES = {
    currentPassword: { required: "Mật khẩu hiện tại không được để trống" },
    newPassword: { required: "Mật khẩu mới không được để trống"},
    confirmNewPassword: { required: "Vui lòng xác nhận mật khẩu mới" },
};

export function useChangePassword({ onSuccess } = {}) {
    const { register, handleSubmit, reset, setError, formState, watch } = useForm({
        defaultValues: DEFAULT_VALUES,
    });

    const newPasswordValue = watch("newPassword");

    const reg = (name) => register(name, {
        ...RULES[name],
        ...(name === "confirmNewPassword" && {
            validate: (v) => v === newPasswordValue || "Xác nhận mật khẩu không khớp",
        }),
    });

    const onSubmit = handleSubmit(async (values) => {
        try {
            const { data } = await profileService.changePassword(values);
            if (!data.succeeded) { applyServerErrors(setError, data); return; }
            reset();
            onSuccess?.("Đổi mật khẩu thành công, vui lòng đăng nhập lại!");
        } catch (err) {
            applyServerErrors(setError, err.response?.data ?? { message: err.message });
        }
    });

    return { reg, formState, onSubmit };
}
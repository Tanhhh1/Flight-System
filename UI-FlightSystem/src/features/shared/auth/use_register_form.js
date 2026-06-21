import { useForm } from "react-hook-form";
import { useDispatch, useSelector } from "react-redux";
import { signUp } from "./auth_slice";
import { applyServerErrors } from "@/hooks/use_shared_form";

const VALIDATION_RULES = {
    fullName: { required: "Vui lòng nhập họ và tên" },
    username: { required: "Vui lòng nhập tên đăng nhập" },
    email: { required: "Vui lòng nhập địa chỉ email" },
    password: { required: "Vui lòng nhập mật khẩu" },
    confirmPassword: { required: "Vui lòng xác nhận lại mật khẩu" },
};

export function useRegisterForm({ onSuccess }) {
    const dispatch = useDispatch();
    const { isLoading } = useSelector((state) => state.auth);

    const { register, handleSubmit, setError, formState, reset } = useForm({
        defaultValues: { fullName: "", email: "", password: "", confirmPassword: ""},
    });

    const onSubmit = handleSubmit(async (values) => {
        if (values.password !== values.confirmPassword) {
            setError("confirmPassword", {
                type: "manual",
                message: "Mật khẩu xác nhận không khớp!",
            });
            return;
        }
        const { confirmPassword, ...registerPayload } = values;
        const result = await dispatch(signUp(registerPayload));
        if (signUp.fulfilled.match(result)) {
            reset();
            if (onSuccess) {
                onSuccess();
            }
            return;
        }
        if (signUp.rejected.match(result)) {
            applyServerErrors(setError, {
                errors: [{ propertyName: null, errorMessage: result.payload || "Đăng ký thất bại." }],
            });
        }
    });

    const enhancedRegister = (name) => register(name, VALIDATION_RULES[name]);

    return { register: enhancedRegister, onSubmit, formState, isLoading, reset };
}
import { useForm } from "react-hook-form";
import { useDispatch, useSelector } from "react-redux";
import { signUp } from "../auth_slice";
import { REGISTER_VALIDATION_RULES, AUTH_MESSAGES } from "../auth_constants";
import { applyServerErrors } from "@/utils/form_error";

export function useRegisterForm({ onSuccess }) {
    const dispatch = useDispatch();
    const { isLoading } = useSelector((state) => state.auth);

    const { register, handleSubmit, setError, formState, reset } = useForm({
        defaultValues: { fullName: "", userName: "", email: "", password: "", confirmPassword: "" },
    });

    const onSubmit = handleSubmit(async (values) => {
        if (values.password !== values.confirmPassword) {
            setError("confirmPassword", {
                type: "manual",
                message: AUTH_MESSAGES.PASSWORD_MISMATCH,
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
            const errorPayload = result.payload;
            if (Array.isArray(errorPayload)) {
                applyServerErrors(setError, { errors: errorPayload });
            } else {
                applyServerErrors(setError, {
                    errors: [{ propertyName: null, errorMessage: errorPayload || AUTH_MESSAGES.REGISTER_FAILED }],
                });
            }
        }
    });

    const enhancedRegister = (name) => register(name, REGISTER_VALIDATION_RULES[name]);

    return { register: enhancedRegister, onSubmit, formState, isLoading, reset };
}
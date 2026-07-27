import { useForm } from "react-hook-form";
import { useDispatch, useSelector } from "react-redux";
import { signIn, clearCredentials, closeLoginModal } from "../auth_slice";
import { LOGIN_VALIDATION_RULES, AUTH_MESSAGES } from "../auth_constants";
import { applyServerErrors } from "@/utils/form_error";

export function useLoginForm({ onSuccess, onRoleBlocked }) {
    const dispatch = useDispatch();
    const { isLoading } = useSelector((state) => state.auth);

    const { register, handleSubmit, setError, formState, reset } = useForm({
        defaultValues: { username: "", password: "" },
    });

    const onSubmit = handleSubmit(async (values) => {
        const result = await dispatch(signIn(values));

        if (signIn.fulfilled.match(result)) {
            const { user } = result.payload;

            if (onRoleBlocked) {
                const isBlocked = onRoleBlocked(user, setError);
                if (isBlocked) {
                    dispatch(clearCredentials());
                    return;
                }
            }

            dispatch(closeLoginModal());
            reset();
            if (onSuccess) onSuccess(user);
            return;
        }

        if (signIn.rejected.match(result)) {
            const errorPayload = result.payload;
            if (Array.isArray(errorPayload)) {
                applyServerErrors(setError, { errors: errorPayload });
            } else {
                applyServerErrors(setError, {
                    errors: [{ propertyName: null, errorMessage: errorPayload || AUTH_MESSAGES.LOGIN_FAILED }],
                });
            }
        }
    });

    const enhancedRegister = (name) => register(name, LOGIN_VALIDATION_RULES[name]);

    return { register: enhancedRegister, onSubmit, formState, isLoading, reset };
}
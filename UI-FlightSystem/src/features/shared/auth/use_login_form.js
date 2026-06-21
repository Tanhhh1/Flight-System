import { useForm } from "react-hook-form";
import { useDispatch, useSelector } from "react-redux";
import { signIn, clearCredentials, closeLoginModal  } from "./auth_slice";
import { applyServerErrors } from "@/hooks/use_shared_form";

const VALIDATION_RULES = {
    LoginId: { required: "Vui lòng nhập email hoặc tên đăng nhập." },
    password: { required: "Vui lòng nhập mật khẩu." },
};

export function useLoginForm({ onSuccess, onRoleBlocked }) {
    const dispatch = useDispatch();
    const { isLoading } = useSelector((state) => state.auth);

    const { register, handleSubmit, setError, formState, reset } = useForm({
        defaultValues: { LoginId: "", password: "" },
    });

    const onSubmit = handleSubmit(async (values) => {
        const result = await dispatch(signIn(values));

        if (signIn.fulfilled.match(result)) {
            const { user } = result.payload;
            const isBlocked = onRoleBlocked(user, setError);
            if (isBlocked) {
                dispatch(clearCredentials());
                return;
            }
            dispatch(closeLoginModal());
            reset();
            onSuccess(user);
            return;
        }
        if (signIn.rejected.match(result)) {
            applyServerErrors(setError, {
                errors: [{ propertyName: null, errorMessage: result.payload || "Đăng nhập thất bại." }],
            });
        }
    });

    const enhancedRegister = (name) => register(name, VALIDATION_RULES[name]);

    return { register: enhancedRegister, onSubmit, formState, isLoading, reset };
}
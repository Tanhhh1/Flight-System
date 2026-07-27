import { useEffect } from "react";
import { useForm } from "react-hook-form";
import { applyServerErrors } from "@/utils/form_error";

export function useCrudForm({isEdit, defaultValues, buildResetValues, onSubmitApi, beFieldMap, onSuccess, onClose}) {
    const { register, handleSubmit, reset, setError, formState, control, watch, setValue } = useForm({ defaultValues });
    useEffect(() => { reset(buildResetValues()) }, [isEdit, reset]);

    const onSubmit = handleSubmit(async (values) => {
        try {
            const { data } = await onSubmitApi(values);
            if (!data.succeeded) {
                applyServerErrors( setError, data, beFieldMap );
                return;
            }
            if (!isEdit) { reset() }
            onSuccess?.();
            onClose?.();
        }
        catch (err) {
            applyServerErrors(
                setError, err.response?.data ?? { message: err.message ?? "Lỗi kết nối server." }, beFieldMap
            );
        }
    });
    return { register, reset, setError, formState, watch, control, setValue, onSubmit };
}
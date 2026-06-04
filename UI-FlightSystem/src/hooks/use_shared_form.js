import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";

const lcFirst = (str) => str.charAt(0).toLowerCase() + str.slice(1);

export function applyServerErrors(setError, apiData, beFieldMap = {}) {
    if (Array.isArray(apiData?.errors) && apiData.errors.length > 0) {
        apiData.errors.forEach(({ propertyName, errorMessage }) => {
            const field = !propertyName
                ? "root"
                : (beFieldMap[propertyName] ?? lcFirst(propertyName));
            setError(field, { message: errorMessage });
        });
        return;
    }
    const message =
        apiData?.message ?? apiData?.error ?? apiData?.title ?? "Lỗi không xác định từ server.";
    setError("root", { message });
}

export function useCrudForm({ isEdit, defaultValues, buildResetValues, onSubmitApi, beFieldMap, onSuccess, onClose }) {
    const { register, handleSubmit, reset, setError, formState, control, setValue } = useForm({
        defaultValues,
    });

    useEffect(() => {
        reset(buildResetValues());
    }, [isEdit, reset]);

    const onSubmit = handleSubmit(async (values) => {
        try {
            const { data } = await onSubmitApi(values);
            if (!data.succeeded) {
                applyServerErrors(setError, data, beFieldMap);
                return;
            }
            if (!isEdit) reset();
            onSuccess?.();
            onClose?.();
        } catch (err) {
            applyServerErrors(
                setError,
                err.response?.data ?? { message: err.message ?? "Lỗi kết nối server." },
                beFieldMap
            );
        }
    });

    return { register, reset, setError, formState, control, onSubmit, setValue };
}

export function useConfirmAction({ onSuccess }) {
    const [target, setTarget] = useState(null);
    const [error, setError] = useState(null);
    const [isLoading, setIsLoading] = useState(false);

    const open = (item) => { setTarget(item); setError(null); };
    const close = () => { setTarget(null); setError(null); };

    const confirm = async (apiFn) => {
        setError(null);
        setIsLoading(true);
        try {
            const { data } = await apiFn(target);
            if (!data.succeeded) {
                setError(
                    data.errors?.[0]?.errorMessage ?? data.message ?? "Thao tác thất bại"
                );
                return;
            }
            close();
            onSuccess?.();
        } catch (err) {
            setError(
                err.response?.data?.errors?.[0]?.errorMessage ??
                err.response?.data?.message ??
                err.message ??
                "Lỗi kết nối server"
            );
        } finally {
            setIsLoading(false);
        }
    };
    return { target, error, isLoading, open, close, confirm };
}


import { useState } from "react";

export function useConfirmAction({ onSuccess }) {
    const [target, setTarget] = useState(null);
    const [error, setError] = useState(null);
    const [isLoading, setIsLoading] = useState(false);
    const open = (item) => { setTarget(item); setError(null) };
    const close = () => { setTarget(null); setError(null) };

    const confirm = async (apiFn) => {
        setError(null);
        setIsLoading(true);

        try {
            const { data } = await apiFn(target);
            if (!data.succeeded) {
                setError( data.errors?.[0] ?.errorMessage ?? data.message ?? "Thao tác thất bại" );
                return;
            }
            close();
            onSuccess?.();
        }
        catch (err) {
            setError( err.response?.data?.errors?.[0] ?.errorMessage ?? err.response?.data?.message ?? err.message ?? "Lỗi kết nối server" );
        }
        finally {
            setIsLoading(false);
        }
    };
    return { target, error, isLoading, open, close, confirm };
}
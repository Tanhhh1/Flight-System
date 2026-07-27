import { useConfirmAction } from "@/hooks/use_confirm_action";

export function useDeleteConfirm({ onSuccess, deleteServiceFn }) {
    const confirmAction = useConfirmAction({ onSuccess });

    const handleConfirm = (idExtractor = (item) => item.id) => {
        return confirmAction.confirm((target) => deleteServiceFn(idExtractor(target)));
    };

    return {
        target: confirmAction.target,
        isLoading: confirmAction.isLoading,
        error: confirmAction.error,
        open: confirmAction.open,
        close: confirmAction.close,
        confirm: handleConfirm,
    };
}
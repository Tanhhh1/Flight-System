import { useCrudForm } from "@/hooks/use_crud_form";
import { supportService } from "./support_service";
import { DEFAULT_VALUES } from "@/features/admin/support/support_constants"

export function useSupportForm({ onSuccess, onClose } = {}) {
    const { register, formState, onSubmit, reset, control, setValue, setError } = useCrudForm({
        isEdit: false,
        defaultValues: DEFAULT_VALUES,
        buildResetValues: () => DEFAULT_VALUES,
        onSubmitApi: (values) => supportService.create(values),
        onSuccess: () => onSuccess?.("Gửi yêu cầu hỗ trợ thành công!"),
        onClose,
    });

    return { register, formState, onSubmit, reset, control, setValue, setError };
}
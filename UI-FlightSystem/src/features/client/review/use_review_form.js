import { useCrudForm } from "@/hooks/use_crud_form";
import { reviewApi } from "./review_service";
import { DEFAULT_VALUES, RULES } from "@/features/admin/review/review_constants";

export function useReviewForm({ onSuccess, onClose } = {}) {
    const { register: baseRegister, formState, onSubmit, ...rest } = useCrudForm({
        isEdit: false,
        defaultValues: DEFAULT_VALUES,
        buildResetValues: () => DEFAULT_VALUES,
        onSubmitApi: (values) => reviewApi.create(values),
        onSuccess: () => onSuccess?.("Gửi đánh giá thành công!"),
        onClose,
    });

    const register = (name, overrides) =>
        baseRegister(name, { ...RULES[name], ...overrides });

    return { register, formState, onSubmit, ...rest };
}
import { useCrudForm } from "@/hooks/use_shared_form";
import { reviewService } from "./review_service";
import { DEFAULT_VALUES, RULES } from "@/constants/review"

export function useReviewForm({ onSuccess, onClose } = {}) {
    const { register: baseRegister, formState, onSubmit } = useCrudForm({
        isEdit: false,
        defaultValues: DEFAULT_VALUES,
        buildResetValues: () => DEFAULT_VALUES,
        onSubmitApi: (values) => reviewService.create(values),
        onSuccess: () => onSuccess?.("Gửi đánh giá thành công!"),
        onClose,
    });

    const register = (name, overrides) =>
        baseRegister(name, { ...RULES[name], ...overrides });

    return { register, formState, onSubmit };
}
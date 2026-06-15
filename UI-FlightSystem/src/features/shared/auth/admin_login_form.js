import { useNavigate } from "react-router-dom";
import { useLoginForm } from "./use_login_form";
import { isAdminRole } from "@/constants/auth";
import { applyServerErrors } from "@/hooks/use_shared_form";

export function useAdminLoginForm() {
    const navigate = useNavigate();

    return useLoginForm({
        onSuccess: () => navigate("/admin/dashboard", { replace: true }),
        onRoleBlocked: (user, setError) => {
            const blocked = !isAdminRole(user.roles);
            if (blocked) {
                applyServerErrors(setError, {
                    errors: [{ propertyName: null, errorMessage: "Tài khoản không có quyền truy cập hệ thống quản trị." }],
                });
            }
            return blocked;
        },
    });
}
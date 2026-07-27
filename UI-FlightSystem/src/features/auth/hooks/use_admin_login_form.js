import { useNavigate } from "react-router-dom";
import { useLoginForm } from "../hooks/use_login_form";
import { isAdminRole, AUTH_MESSAGES } from "../auth_constants";
import { applyServerErrors } from "@/utils/form_error";

export function useAdminLoginForm() {
    const navigate = useNavigate();

    return useLoginForm({
        onSuccess: () => navigate("/admin/dashboard", { replace: true }),
        onRoleBlocked: (user, setError) => {
            const blocked = !isAdminRole(user.roles);
            if (blocked) {
                applyServerErrors(setError, {
                    errors: [
                        {
                            propertyName: null,
                            errorMessage: AUTH_MESSAGES.ADMIN_ACCESS_DENIED,
                        },
                    ],
                });
            }
            return blocked;
        },
    });
}
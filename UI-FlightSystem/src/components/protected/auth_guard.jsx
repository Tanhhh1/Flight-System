import { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { clearCredentials } from "@/features/shared/auth/auth_slice";
import { isAdminRole } from "@/constants/auth";

function AuthGuard({ children }) {
    const dispatch = useDispatch();
    const user = useSelector((state) => state.auth?.user);

    useEffect(() => {
        if (user && isAdminRole(user.roles)) {
            dispatch(clearCredentials());
        }
    }, []);

    return children;
}

export default AuthGuard;
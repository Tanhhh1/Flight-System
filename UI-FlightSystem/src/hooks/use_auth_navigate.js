import { useDispatch, useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import { openLoginModal } from "@/features/shared/auth/auth_slice";

export function useAuthNavigate() {
    const { user } = useSelector((state) => state.auth);
    const dispatch = useDispatch();
    const navigate = useNavigate();

    const authNavigate = (to) => {
        if (!user) {
            dispatch(openLoginModal(to));
            return;
        }
        navigate(to);
    };

    return authNavigate;
}
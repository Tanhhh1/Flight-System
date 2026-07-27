import { useEffect } from "react";
import { useForm } from "react-hook-form";
import { useDispatch } from "react-redux";
import profileService from "../profile_service";
import { PROFILE_MESSAGES, DEFAULT_VALUES_INFO } from "../profile_constants";
import { applyServerErrors } from "@/utils/form_error";
import { updateUserInfo } from "@/features/auth/auth_slice";


export function useProfileForm({ onSuccess } = {}) {
    const dispatch = useDispatch();
    const { register, handleSubmit, reset, setError, formState } = useForm({
        defaultValues: DEFAULT_VALUES_INFO,
    });

    useEffect(() => {
        profileService.getProfile().then(({ data }) => {
            if (data.succeeded && data.result) {
                const u = data.result;
                reset({
                    userName: u.userName ?? "",
                    email: u.email ?? "",
                    fullname: u.fullname ?? "",
                    phoneNumber: u.phoneNumber ?? "",
                    address: u.address ?? "",
                    gender: u.gender ?? "",
                    birthday: u.birthday?.split("T")[0] ?? "",
                });
            }
        });
    }, [reset]);

    const onSubmit = handleSubmit(async (values) => {
        try {
            const { data } = await profileService.updateProfile({
                ...values,
                birthday: values.birthday?.trim() || null,
            });

            if (!data.succeeded) {
                applyServerErrors(setError, data);
                return;
            }

            dispatch(updateUserInfo({ fullName: values.fullname, email: values.email }));
            if (onSuccess) onSuccess(PROFILE_MESSAGES.UPDATE_SUCCESS);
        } catch (err) {
            applyServerErrors(setError, err.response?.data ?? { message: err.message });
        }
    });

    return { register, formState, onSubmit };
}
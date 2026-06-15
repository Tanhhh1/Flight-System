import { useEffect } from "react";
import { useForm } from "react-hook-form";
import { useDispatch } from "react-redux";
import { profileService } from "./profile_service";
import { applyServerErrors } from "@/hooks/use_shared_form";
import { updateUserInfo } from "@/features/shared/auth/auth_slice";

const DEFAULT_VALUES = { userName: "", email: "", fullname: "", phoneNumber: "", address: "", gender: "", birthday: "" };

export function useProfileForm({ onSuccess } = {}) {
    const dispatch = useDispatch();
    const { register, handleSubmit, reset, setError, formState } = useForm({
        defaultValues: DEFAULT_VALUES,
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
            if (!data.succeeded) { applyServerErrors(setError, data); return; }
            dispatch(updateUserInfo({ fullName: values.fullname, email: values.email }));
            onSuccess?.("Cập nhật thông tin thành công!");
        } catch (err) {
            applyServerErrors(setError, err.response?.data ?? { message: err.message });
        }
    });

    return { register, formState, onSubmit };
}
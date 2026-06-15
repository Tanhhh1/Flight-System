import api from "@/configs/axios_config";

export const profileService = {
    getProfile: () => api.get("/profile"),
    updateProfile: (data) => api.put("/profile/update-profile", data),
    changePassword: (data) => api.put("/profile/change-password", data),
};
import http from "@/api/http";

const profileService = {
    getProfile: () => http.get("/profile"),
    updateProfile: (data) => http.put("/profile/update-profile", data),
    changePassword: (data) => http.put("/profile/change-password", data),
};

export default profileService;
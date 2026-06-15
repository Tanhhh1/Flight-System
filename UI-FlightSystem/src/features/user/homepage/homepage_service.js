import api from "@/configs/axios_config";

export const homepageService = {
    getAllReviews: (params) => api.get("/Review", { params }),
};
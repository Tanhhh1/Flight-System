import api from "@/configs/axios_config";
import { sharedService } from "@/services/shared_service";

const base = sharedService("/Review");

export const reviewService = {
    ...base,
    getMyReviews: (params) => api.get("/Review/my-review", { params }),
};
import api from "@/configs/axios_config";
import { sharedService } from "@/services/shared_service";

const base = sharedService("/SupportRequest");

export const supportRequestService = {
    ...base,
    getMy: (params) => api.get(`/SupportRequest/my-request`, { params }),
};
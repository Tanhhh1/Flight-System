import api from "@/configs/axios_config";
import { sharedService } from "@/services/shared_service";

const BASE_URL = "/admin/SupportRequest";
const base = sharedService(BASE_URL);

export const adminSupportRequestService = {
    ...base,
    approve: (id) => api.put(`${BASE_URL}/${id}/approve`),
    reject:  (id) => api.put(`${BASE_URL}/${id}/reject`),
};
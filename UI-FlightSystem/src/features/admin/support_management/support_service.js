import api from "@/configs/axios_config";
import { sharedService } from "@/services/shared_service";

const base = sharedService("/admin/SupportRequest");

export const adminSupportRequestService = {
    ...base,
    approve: (id, data) => api.put(`${base}/${id}/approve`, data),
    reject: (id, data) => api.put(`${base}/${id}/reject`, data),
};
import http from "@/api/http";
import { createCrudApi } from "@/api/crud_factory";

const BASE_URL = "/admin/SupportRequest";

export const adminSupportRequestService = {
    ...createCrudApi(BASE_URL),

    approve(id) {
        return http.put(`${BASE_URL}/${id}/approve`);
    },

    reject(id) {
        return http.put(`${BASE_URL}/${id}/reject`);
    },
};
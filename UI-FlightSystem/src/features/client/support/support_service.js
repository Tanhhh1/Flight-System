import { createCrudApi } from "@/api/crud_factory";
import http from "@/api/http";

const crudApi = createCrudApi("/SupportRequest");

export const supportService = {
    ...crudApi,
    getMy: (params) => http.get("/SupportRequest/my-request", { params }),
};
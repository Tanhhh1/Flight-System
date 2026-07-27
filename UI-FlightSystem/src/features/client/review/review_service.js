import http from "@/api/http";
import { createCrudApi } from "@/api/crud_factory";

const baseCrud = createCrudApi("/Review");

export const reviewApi = {
    ...baseCrud,
    getMyReviews: (params) => http.get("/Review/my-review", { params }),
};
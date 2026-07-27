import { createCrudApi } from "@/api/crud_factory";

const reviewApi = createCrudApi("/Review");

export const homepageService = {
    getAllReviews: (params) => reviewApi.getAll(params),
};
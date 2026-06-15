import api from "@/configs/axios_config";

export const dashboardService = {
    getSummary: () => api.get("/admin/Dashboard/summary"),
    getRevenue: (year) => api.get("/admin/Dashboard/revenue", { params: { year } }),
};
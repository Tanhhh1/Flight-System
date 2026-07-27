import http from "@/api/http";

export const dashboardService = {
    getSummary() {return http.get("/admin/Dashboard/summary")},
    getRevenue(year) {return http.get("/admin/Dashboard/revenue", {params: { year }})},
};
import http from "@/api/http";

const authService = {
    signIn: (data) => http.post("/Auth/sign-in", data),
    signUp: (data) => http.post("/Auth/sign-up", data),
    revoke: (data) => http.post("/Auth/revoke", data),
    refresh: (data) => http.post("/Auth/refresh", data),
};

export default authService;
import api from "@/configs/axios_config";

const authService = {
  signIn: (data) => api.post("/Auth/sign-in", data),
  signUp: (data) => api.post("/Auth/sign-up", data),
  revoke: (data) => api.post("/Auth/revoke", data),
  refresh: (data) => api.post("/Auth/refresh", data),
};

export default authService;
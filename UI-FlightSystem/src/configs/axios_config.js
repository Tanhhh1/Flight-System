import axios from "axios";
import { store } from "@/store";
import { updateTokens, logout } from "@/features/shared/auth/auth_slice";

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
});

let refreshPromise = null;

const getNewToken = () => {
  if (refreshPromise) return refreshPromise;

  refreshPromise = (async () => {
    try {
      const { token, refreshToken } = store.getState().auth ?? {};
      const { data } = await axios.post(
        `${import.meta.env.VITE_API_URL}/Auth/refresh`,
        { refreshToken, accessToken: token }
      );

      if (!data.succeeded || !data.result?.accessToken) {
        throw new Error(data.errors?.[0]?.errorMessage || "Làm mới phiên đăng nhập thất bại.");
      }

      store.dispatch(updateTokens({
        token: data.result.accessToken,
        refreshToken: data.result.refreshToken,
      }));

      return data.result.accessToken;
    } finally {
      refreshPromise = null;
    }
  })();

  return refreshPromise;
};

const handleRefreshFailed = () => {
  store.dispatch(logout());
  return Promise.reject(new Error("Phiên đăng nhập hết hạn, vui lòng đăng nhập lại."));
};

const isAccessTokenExpired = () => {
  const expiredAt = localStorage.getItem("tokenExpiredAt");
  if (!expiredAt) return true;
  return Date.now() > new Date(expiredAt).getTime();
};

const getToken = () => store.getState().auth?.token;
const getRefreshToken = () => store.getState().auth?.refreshToken;

api.interceptors.request.use(async (config) => {
  if (!getToken()) return config;

  try {
    const token = isAccessTokenExpired() && getRefreshToken()
      ? await getNewToken()
      : getToken();

    config.headers.Authorization = `Bearer ${token}`;
  } catch (err) {
    return handleRefreshFailed(err);
  }

  return config;
});

api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const { config: originalRequest, response } = error;

    if (originalRequest._retry || response?.status !== 401 || originalRequest.url === "/Auth/refresh") {
      return Promise.reject(error);
    }

    originalRequest._retry = true;

    try {
      const newToken = await getNewToken();
      originalRequest.headers.Authorization = `Bearer ${newToken}`;
      return api(originalRequest);
    } catch (err) {
      return handleRefreshFailed(err);
    }
  }
);

export default api;
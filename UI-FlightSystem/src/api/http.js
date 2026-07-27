import axios from "axios";

const http = axios.create({
    baseURL: import.meta.env.VITE_API_URL,
});

let refreshPromise = null;

const getToken = () => localStorage.getItem("token");
const getRefreshToken = () => localStorage.getItem("refreshToken");

const updateLocalStorageTokens = (token, refreshToken, expiredAt) => {
    localStorage.setItem("token", token);
    localStorage.setItem("refreshToken", refreshToken);
    if (expiredAt) localStorage.setItem("tokenExpiredAt", expiredAt);
};

const getNewToken = () => {
    if (refreshPromise) return refreshPromise;

    refreshPromise = (async () => {
        try {
            const refreshToken = getRefreshToken();
            const { data } = await axios.post(
                `${import.meta.env.VITE_API_URL}/Auth/refresh`,
                { refreshToken }
            );

            if (!data.succeeded || !data.result?.accessToken) {
                throw new Error(data.errors?.[0]?.errorMessage || data.message || "Làm mới phiên đăng nhập thất bại.");
            }

            const { accessToken, refreshToken: newRefreshToken, accessTokenExpires } = data.result;

            updateLocalStorageTokens(accessToken, newRefreshToken, accessTokenExpires);
            window.dispatchEvent(new CustomEvent("auth:tokens-updated", {
                detail: { token: accessToken, refreshToken: newRefreshToken }
            }));

            return accessToken;
        } finally {
            refreshPromise = null;
        }
    })();

    return refreshPromise;
};

const handleRefreshFailed = (err) => {
    localStorage.removeItem("user");
    localStorage.removeItem("token");
    localStorage.removeItem("refreshToken");
    localStorage.removeItem("tokenExpiredAt");

    window.dispatchEvent(new Event("auth:logout"));
    return Promise.reject(err || new Error("Phiên đăng nhập hết hạn, vui lòng đăng nhập lại."));
};

const isAccessTokenExpired = () => {
    const expiredAt = localStorage.getItem("tokenExpiredAt");
    if (!expiredAt) return true;
    return Date.now() >= new Date(expiredAt).getTime();
};

http.interceptors.request.use(async (config) => {
    if (config.url?.includes("/Auth/sign-in") || config.url?.includes("/Auth/sign-up")) {
        return config;
    }

    const token = getToken();
    if (!token) return config;

    try {
        let currentToken = token;
        if (isAccessTokenExpired() && getRefreshToken()) {
            currentToken = await getNewToken();
        }
        config.headers.Authorization = `Bearer ${currentToken}`;
    } catch (err) {
        return handleRefreshFailed(err);
    }

    return config;
});

http.interceptors.response.use(
    (response) => response,
    async (error) => {
        const { config: originalRequest, response } = error;

        if (
            !originalRequest ||
            originalRequest._retry ||
            response?.status !== 401 ||
            originalRequest.url?.includes("/Auth/refresh")
        ) {
            return Promise.reject(error);
        }

        originalRequest._retry = true;

        try {
            const newToken = await getNewToken();
            originalRequest.headers.Authorization = `Bearer ${newToken}`;
            return http(originalRequest);
        } catch (err) {
            return handleRefreshFailed(err);
        }
    }
);

export default http;
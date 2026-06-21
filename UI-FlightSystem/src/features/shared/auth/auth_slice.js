import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import axios from "axios";
import { jwtDecode } from "jwt-decode";

function parseUser(result) {
    try {
        const decoded = jwtDecode(result.accessToken);
        return {
            fullName: decoded.name ?? "",
            email: decoded.email ?? "",
            userName: decoded.unique_name ?? "",
            roles: decoded.role ? Array.isArray(decoded.role) ? decoded.role : [decoded.role] : [],
        };
    } catch {
        return {};
    }
}

const initialState = {
    user: (() => {
        try {
            const raw = localStorage.getItem("user");
            return raw ? JSON.parse(raw) : null;
        } catch { return null; }
    })(),
    token: localStorage.getItem("token"),
    refreshToken: localStorage.getItem("refreshToken"),
    tokenExpiredAt: localStorage.getItem("tokenExpiredAt"),
    isLoading: false,
    error: null,
    showLoginModal: false,
    returnTo: null,
};

export const signIn = createAsyncThunk(
    "auth/signIn",
    async (values, { dispatch, rejectWithValue }) => {
        try {
            const { data } = await axios.post(
                `${import.meta.env.VITE_API_URL}/Auth/sign-in`,
                values
            );
            if (!data.succeeded) {
                const errorMsg = data.errors?.[0]?.errorMessage || "Đăng nhập thất bại.";
                return rejectWithValue(errorMsg);
            }
            const user = parseUser(data.result);
            dispatch(setCredentials({
                user,
                token: data.result.accessToken,
                refreshToken: data.result.refreshToken,
            }));

            return { user, token: data.result.accessToken, refreshToken: data.result.refreshToken };
        } catch (err) {
            const errorMsg = err.response?.data?.errors?.[0]?.errorMessage || err.response?.data?.message || "Lỗi kết nối server.";
            return rejectWithValue(errorMsg);
        }
    }
);

export const signUp = createAsyncThunk(
    "auth/signUp",
    async (values, { rejectWithValue }) => {
        try {
            const { data } = await axios.post(
                `${import.meta.env.VITE_API_URL}/Auth/sign-up`,
                values
            );
            if (!data.succeeded) {
                const errorMsg = data.errors?.[0]?.errorMessage || "Đăng ký thất bại.";
                return rejectWithValue(errorMsg);
            }
            return data;
        } catch (err) {
            const errorMsg = err.response?.data?.errors?.[0]?.errorMessage || err.response?.data?.message || "Lỗi kết nối server.";
            return rejectWithValue(errorMsg);
        }
    }
);

export const logout = createAsyncThunk(
    "auth/logout",
    async (_, { getState }) => {
        try {
            const state = getState();
            const token = state.auth?.token;
            const refreshToken = state.auth?.refreshToken;

            if (refreshToken && token) {
                try {
                    await axios.post(
                        `${import.meta.env.VITE_API_URL}/Auth/revoke`,
                        { refreshToken },
                        { headers: { Authorization: `Bearer ${token}` } }
                    );
                } catch (err) {
                    console.warn("Revoke token warning:", err.response?.data?.errors?.[0]?.errorMessage || err.message);
                }
            }
        } catch (err) {
            console.warn("Logout preparation error:", err.message);
        }
        return { success: true };
    }
);

const authSlice = createSlice({
    name: "auth",
    initialState,
    reducers: {
        setCredentials: (state, action) => {
            const { user, token, refreshToken } = action.payload;
            const decoded = jwtDecode(token);
            const tokenExpiredAt = new Date(decoded.exp * 1000).toISOString();

            state.user = user;
            state.token = token;
            state.refreshToken = refreshToken;
            state.tokenExpiredAt = tokenExpiredAt;
            state.error = null;

            localStorage.setItem("user", JSON.stringify(user));
            localStorage.setItem("token", token);
            localStorage.setItem("refreshToken", refreshToken);
            localStorage.setItem("tokenExpiredAt", tokenExpiredAt);
        },
        updateTokens: (state, action) => {
            const { token, refreshToken } = action.payload;
            const decoded = jwtDecode(token);
            const tokenExpiredAt = new Date(decoded.exp * 1000).toISOString();

            state.token = token;
            state.refreshToken = refreshToken;
            state.tokenExpiredAt = tokenExpiredAt;

            localStorage.setItem("token", token);
            localStorage.setItem("refreshToken", refreshToken);
            localStorage.setItem("tokenExpiredAt", tokenExpiredAt);
        },
        updateUserInfo: (state, action) => {
            const { fullName, email } = action.payload;
            if (!state.user) return;
            state.user.fullName = fullName;
            state.user.email = email;
            localStorage.setItem("user", JSON.stringify(state.user));
        },
        clearCredentials: (state) => {
            state.user = null;
            state.token = null;
            state.refreshToken = null;
            state.tokenExpiredAt = null;
            state.error = null;

            localStorage.removeItem("user");
            localStorage.removeItem("token");
            localStorage.removeItem("refreshToken");
            localStorage.removeItem("tokenExpiredAt");
        },
        openLoginModal: (state, action) => {
            state.showLoginModal = true;
            state.returnTo = action.payload ?? null;
        },
        closeLoginModal: (state) => {
            state.showLoginModal = false;
            state.returnTo = null;
        },
    },
    extraReducers: (builder) => {
        builder
            .addCase(signIn.pending, (state) => {
                state.isLoading = true;
                state.error = null;
            })
            .addCase(signIn.fulfilled, (state) => {
                state.isLoading = false;
                state.error = null;
            })
            .addCase(signIn.rejected, (state, action) => {
                state.isLoading = false;
                state.error = action.payload || "Đăng nhập thất bại";
            })

            .addCase(signUp.pending, (state) => {
                state.isLoading = true;
                state.error = null;
            })
            .addCase(signUp.fulfilled, (state) => {
                state.isLoading = false;
                state.error = null;
            })
            .addCase(signUp.rejected, (state, action) => {
                state.isLoading = false;
                state.error = action.payload || "Đăng ký thất bại";
            })

            .addCase(logout.fulfilled, (state) => {
                state.user = null;
                state.token = null;
                state.refreshToken = null;
                state.isLoading = false;
                state.error = null;

                localStorage.removeItem("user");
                localStorage.removeItem("token");
                localStorage.removeItem("refreshToken");
                localStorage.removeItem("tokenExpiredAt");
            })
            .addCase(logout.rejected, (state) => {
                state.user = null;
                state.token = null;
                state.refreshToken = null;
                state.isLoading = false;
                state.error = null;

                localStorage.removeItem("user");
                localStorage.removeItem("token");
                localStorage.removeItem("refreshToken");
                localStorage.removeItem("tokenExpiredAt");
            });
    },
});

export const { setCredentials, updateTokens, clearCredentials, updateUserInfo, openLoginModal, closeLoginModal } = authSlice.actions;

export default authSlice.reducer;
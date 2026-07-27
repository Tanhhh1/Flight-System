import { createAsyncThunk } from "@reduxjs/toolkit";
import { parseApiError } from "@/lib/redux/parse_api_error";

export function createAsyncAction(type, apiCall) {
    return createAsyncThunk(
        type,
        async (payload, { rejectWithValue }) => {
            try {
                const response = await apiCall(payload);
                const responseData = response?.data ?? response;

                if (responseData?.succeeded === false) {
                    return rejectWithValue(responseData.message ?? "Thao tác thất bại.");
                }

                return responseData?.result ?? responseData;
            }
            catch (error) {
                return rejectWithValue(parseApiError(error));
            }
        }
    );
}
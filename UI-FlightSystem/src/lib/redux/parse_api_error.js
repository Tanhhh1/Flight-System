export function parseApiError(error) {

    return (
        error?.response?.data?.errors?.[0]?.errorMessage ??
        error?.response?.data?.message ??
        error?.message ??
        "Đã xảy ra lỗi hệ thống."
    );

}
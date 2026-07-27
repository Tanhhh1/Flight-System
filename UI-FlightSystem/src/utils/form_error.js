const lcFirst = (str) =>
    str.charAt(0).toLowerCase() + str.slice(1);

export function applyServerErrors(setError, apiData, beFieldMap = {}) {
    if (
        Array.isArray(apiData?.errors) &&
        apiData.errors.length > 0
    ) {
        apiData.errors.forEach(
            ({ propertyName, errorMessage }) => {
                const field = !propertyName
                    ? "root" : ( beFieldMap[propertyName] ?? lcFirst(propertyName));
                setError(field, { message: errorMessage });
            }
        );
        return;
    }
    const message = apiData?.message ?? apiData?.error ?? apiData?.title ??  "Lỗi không xác định từ server.";
    setError("root", { message });
}
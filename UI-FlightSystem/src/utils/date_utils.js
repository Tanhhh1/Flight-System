export const formatDate = (dateStr) =>
    dateStr ? new Date(dateStr).toLocaleDateString("vi-VN") : "—";

export const formatDateInput = (dateStr) =>
    dateStr?.split("T")[0] ?? "";

export const currentDate = () =>
    new Date().toLocaleDateString("vi-VN", {
        weekday: "long", year: "numeric",
        month: "long", day: "numeric",
    });

export const formatDateLong = (dateStr) => {
    if (!dateStr) return "—";
    return new Date(dateStr).toLocaleDateString("vi-VN", {
        weekday: "short",
        day: "numeric",
        month: "long",
        year: "numeric",
    });
};

export const formatDateShort = (dateStr) => {
    if (!dateStr) return "—";
    return new Date(dateStr).toLocaleDateString("vi-VN", {
        day: "2-digit",
        month: "2-digit",
        year: "numeric",
    });
};

export const formatTime = (dateStr) => {
    if (!dateStr) return "—";
    return new Date(dateStr).toLocaleTimeString("vi-VN", {
        hour: "2-digit",
        minute: "2-digit",
    });
};
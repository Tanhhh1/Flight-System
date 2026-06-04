export const formatDate = (dateStr) =>
    dateStr ? new Date(dateStr).toLocaleDateString("vi-VN") : "—";

export const formatDateInput = (dateStr) =>
    dateStr?.split("T")[0] ?? "";

export const currentDate = () =>
    new Date().toLocaleDateString("vi-VN", {
        weekday: "long", year: "numeric",
        month: "long", day: "numeric",
    });
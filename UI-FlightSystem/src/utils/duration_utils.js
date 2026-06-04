export const formatDuration = (minutes) => {
    if (!minutes || minutes < 0) return "—";
    const h = Math.floor(minutes / 60);
    const m = minutes % 60;

    if (h === 0) return `${m} phút`;
    if (m === 0) return `${h} giờ`;
    return `${h} giờ ${m} phút`;
};
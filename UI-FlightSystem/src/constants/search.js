export const TIME_SLOTS = [
    { label: "Đêm→Sáng", range: "00:00–06:00", from: 0, to: 5 },
    { label: "Sáng→Trưa", range: "06:00–12:00", from: 6, to: 11 },
    { label: "Trưa→Tối", range: "12:00–18:00", from: 12, to: 17 },
    { label: "Tối→Đêm", range: "18:00–24:00", from: 18, to: 23 },
];

export const STOP_OPTIONS = [
    { label: "Tất cả", value: null },
    { label: "Bay thẳng", value: 0 },
    { label: "1 điểm dừng", value: 1 },
    { label: "2+ điểm dừng", value: 2 },
];
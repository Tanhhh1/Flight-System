import { useEffect, useMemo, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import { Chart as ChartJS, CategoryScale, LinearScale, BarElement, Title, Tooltip, Legend } from "chart.js";
import { Bar } from "react-chartjs-2";
import Table from "@/components/common/table";
import { fetchDashboardSummary, fetchRevenueByYear } from "../dashboard_slice";
import { fetchBookings } from "@/features/admin/booking/booking_slice";
import { DASHBOARD_YEAR_OPTIONS, DASHBOARD_CHART_LABELS, DASHBOARD_CARDS, DASHBOARD_TABLE_HEADS } from "../dashboard_constants";
import { BOOKING_STATUS_LABEL, TRIP_TYPE_LABEL, SEAT_CLASSES_NAMES } from "@/features/admin/booking/booking_constants";
import { PATHS } from "@/app/routes/paths";
import { currentDate } from "@/utils/date_utils";
import "./dashboard.css";

ChartJS.register( CategoryScale, LinearScale, BarElement, Title, Tooltip, Legend );

function Dashboard() {
    const dispatch = useDispatch();
    const navigate = useNavigate();
    const [selectedYear, setSelectedYear] = useState(DASHBOARD_YEAR_OPTIONS[0].value);
    const { summary, revenue, loadingSummary } = useSelector((state) => state.dashboard);
    const { items: recentBookings } = useSelector((state) => state.booking);

    useEffect(() => {
        dispatch(fetchDashboardSummary());
        dispatch( fetchBookings({ pageIndex: 1, pageSize: 5 }));
    }, [dispatch]);

    useEffect(() => {
        dispatch(fetchRevenueByYear(selectedYear));
    }, [dispatch, selectedYear]);

    const chartData = useMemo(
        () => ({
            labels: DASHBOARD_CHART_LABELS,
            datasets: [
                {
                    label: "Doanh thu (VNĐ)",
                    data: revenue.map((item) => Number(item.revenue)),
                    backgroundColor: "#5277b4",
                    borderRadius: 6,
                },
            ],
        }),
        [revenue]
    );

    const chartOptions = useMemo(
        () => ({
            responsive: true,
            maintainAspectRatio: false,
            plugins: {legend: { display: false }},
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        callback(value) {
                            return (
                                value.toLocaleString("vi-VN") + " đ"
                            );
                        },
                    },
                },
            },
        }),
        []
    );

    return (
        <div className="dashboard_container">
            <div className="dashboard_header">
                <div>
                    <h2>Bảng điều khiển</h2>
                    <span>{currentDate()}</span>
                </div>
            </div>

            <div className="dashboard_content">
                <div className="dashboard_top">
                    <div className="content_cards">
                        {DASHBOARD_CARDS.map((card) => (
                            <div key={card.key} className={`card ${card.color}`}>
                                <i className={card.icon} />
                                <div className="card_info">
                                    <p>{card.label}</p>
                                    <h3>{loadingSummary ? "..." : card.format(summary?.[card.key])}</h3>
                                </div>
                            </div>
                        ))}
                    </div>

                    <div className="dashboard_chart_section">
                        <div className="chart_header">
                            <h3>Thống kê doanh thu theo tháng</h3>
                            <select className="year_select" value={selectedYear}
                                onChange={(e) => setSelectedYear( Number(e.target.value))}>
                                {DASHBOARD_YEAR_OPTIONS.map((year) => (
                                    <option key={year.value} value={year.value}>
                                        {year.label}
                                    </option>
                                ))}
                            </select>
                        </div>

                        <div className="chart_wrapper">
                            <Bar data={chartData} options={chartOptions}/>
                        </div>
                    </div>
                </div>
                <div className="dashboard_bottom">
                    <Table
                        title="Đơn đặt vé gần đây"
                        link={{
                            href: `${PATHS.ADMIN.ROOT}/${PATHS.ADMIN.BOOKINGS}`,
                            text: "Xem tất cả",
                        }}
                        heads={DASHBOARD_TABLE_HEADS}
                        data={recentBookings}
                        render={(item, index) => (
                            <tr key={item.bookingId}>
                                <td>{index + 1}</td>
                                <td>{item.bookingCode}</td>
                                <td>{item.fullname}</td>
                                <td>{SEAT_CLASSES_NAMES[item.className] ?? item.className}</td>
                                <td>
                                    {new Date(
                                        item.bookingDate
                                    ).toLocaleDateString("vi-VN")}
                                </td>
                                <td>{TRIP_TYPE_LABEL[item.tripType] ?? item.tripType}</td>
                                <td className="price_cell">
                                    {item.totalPrice.toLocaleString("vi-VN")}₫
                                </td>
                                <td>
                                    <span className={`status_dot_wrapper status_${item.status?.toLowerCase()}`}>
                                        <span className="status_dot"></span>
                                        {BOOKING_STATUS_LABEL[item.status] ?? item.status}
                                    </span>
                                </td>

                                <td>
                                    <div className="action_buttons_group">
                                        <button title="Xem chi tiết" className="btn_action"
                                            onClick={() => navigate(`${PATHS.ADMIN.ROOT}/${PATHS.ADMIN.BOOKINGS}/${item.bookingId}`)}>
                                            <i className="bx bx-show" />
                                        </button>
                                    </div>
                                </td>
                            </tr>
                        )}
                    />
                </div>
            </div>
        </div>
    );
}

export default Dashboard;
import { usePassengerBooking } from "../use_passenger_booking";
import AlertModal from "@/components/error/alert_modal";
import countries from "world-countries";
import SearchableSelect from "@/components/select/searchables_select";
import "./passenger_info.css";

function PassengerInfo() {
    const { passengers, passengerTypes, pricePerPassenger, grandTotal, loading, clearError,
        error, selectedLegs, handleChange, addPassenger, removePassenger, handleSubmit,
    } = usePassengerBooking();

    const countryOptions = countries
        .map((c) => ({
            code: c.cca2,
            name: c.translations?.vie?.common ?? c.name.common,
        }))
        .sort((a, b) => a.name.localeCompare(b.name, "vi"));

    const formatCurrency = (value) => new Intl.NumberFormat("vi-VN", { style: "currency", currency: "VND" }).format(value);

    return (
        <>
            <AlertModal type="error" message={error} onClose={clearError} />
            <div className="booking_container">
                <div className="booking_inner_layout">
                    <div className="booking_main_content">
                        <h2 className="booking_step_title">Thông tin hành khách</h2>
                        <p className="booking_step_subtitle">
                            Vui lòng điền chính xác thông tin như trên CCCD/Hộ chiếu dùng để bay.
                        </p>

                        {passengers.map((passenger, index) => (
                            <div key={index} className="passenger_card">
                                <div className="panel_header passenger_header">
                                    <h4>
                                        <i className="bx bx-user" /> Hành khách #{index + 1}
                                    </h4>
                                    {passengers.length > 1 && (
                                        <button type="button" className="btn_delete_passenger" onClick={() => removePassenger(index)}>
                                            <i className="bx bx-trash" /> Xóa
                                        </button>
                                    )}
                                </div>

                                <div className="panel_body">
                                    <div className="passenger_form_grid_row">
                                        <div className="form_group">
                                            <label className="form_label">Họ và Tên</label>
                                            <input type="text" value={passenger.fullName} onChange={(e) => handleChange(index, "fullName", e.target.value)} className="form_input_text" placeholder="VD: NGUYEN VAN A" />
                                        </div>
                                        <div className="form_group">
                                            <label className="form_label">Giới tính</label>
                                            <select value={passenger.gender} onChange={(e) => handleChange(index, "gender", e.target.value)} className="form_select">
                                                <option value="Nam">Nam</option>
                                                <option value="Nữ">Nữ</option>
                                            </select>
                                        </div>
                                        <div className="form_group">
                                            <label className="form_label">Ngày sinh</label>
                                            <input type="date" value={passenger.dob} onChange={(e) => handleChange(index, "dob", e.target.value)} className="form_input_text" />
                                        </div>
                                    </div>

                                    <div className="passenger_form_grid_row">
                                        <div className="form_group">
                                            <label className="form_label">Quốc tịch</label>
                                            <SearchableSelect
                                                data={countryOptions}
                                                value={passenger.nationality}
                                                onChange={(val) => handleChange(index, "nationality", val)}
                                                placeholder="Chọn quốc tịch..."
                                                itemKey="name"
                                                displayValue="name"
                                                searchFields={["name"]}
                                            />
                                        </div>
                                        <div className="form_group">
                                            <label className="form_label">Loại hành khách</label>
                                            <select value={passenger.typeId ?? ""} onChange={(e) => handleChange(index, "typeId", e.target.value)} className="form_select">
                                                {passengerTypes.map((t) => (
                                                    <option key={t.typeId} value={t.typeId}>
                                                        {t.typeName}
                                                    </option>
                                                ))}
                                            </select>
                                        </div>
                                        <div className="form_group">
                                            <label className="form_label">Giá vé (đã gồm thuế phí)</label>
                                            <input type="text" value={formatCurrency(pricePerPassenger[index] ?? 0)} className="form_input_text input_readonly_price" readOnly disabled />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        ))}

                        <button type="button" className="btn_add_passenger" onClick={addPassenger}>
                            <i className="bx bx-plus-circle" /> Thêm hành khách khác
                        </button>

                        <div className="form_submit_row">
                            <button type="button" className="btn_submit_booking" onClick={handleSubmit} disabled={loading}>
                                {loading ? <><i className="bx bx-loader-alt bx-spin" /> Đang xử lý...</> : <><i className="bx bx-check-circle" /> Tiếp tục thanh toán</>}
                            </button>
                        </div>
                    </div>

                    <aside className="booking_summary_sidebar">
                        <div className="summary_panel">
                            <div className="panel_header">
                                <h4><i className="bx bx-receipt" /> Chi tiết giá vé</h4>
                            </div>
                            <div className="summary_body">
                                {selectedLegs.map(({ legIndex, flight }) => (
                                    <div key={legIndex} className="summary_flight_mini">
                                        <span className="summary_flight_label">
                                            {selectedLegs.length === 1 ? "Chuyến bay" : `Chặng ${legIndex + 1}`}
                                        </span>
                                        <p className="summary_flight_route">
                                            {flight.originAirportCode} → {flight.destinationAirportCode}
                                        </p>
                                        <span className="summary_flight_label">{flight.airlineName}</span>
                                    </div>
                                ))}
                                <div className="summary_divider" />
                                <div className="summary_price_breakdown">
                                    <span className="breakdown_title">Tóm tắt hành khách:</span>
                                    {passengerTypes.map((type) => {
                                        const count = passengers.filter(
                                            (p) => Number(p.typeId) === type.typeId
                                        ).length;
                                        if (count === 0) return null;

                                        const indices = passengers.reduce((acc, p, i) => {
                                            if (Number(p.typeId) === type.typeId) acc.push(i);
                                            return acc;
                                        }, []);
                                        const subtotal = indices.reduce(
                                            (sum, i) => sum + (pricePerPassenger[i] ?? 0), 0
                                        );

                                        return (
                                            <div key={type.typeId} className="price_row">
                                                <span>{type.typeName} (x{count})</span>
                                                <span>{formatCurrency(subtotal)}</span>
                                            </div>
                                        );
                                    })}
                                </div>

                                <div className="summary_divider" />

                                <div className="summary_total_row">
                                    <span className="total_label">Tổng số tiền phải trả:</span>
                                    <span className="total_value">{formatCurrency(grandTotal)}</span>
                                </div>
                            </div>
                        </div>
                    </aside>

                </div>
            </div>
        </>
    );
}

export default PassengerInfo;
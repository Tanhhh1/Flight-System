import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useDispatch } from "react-redux";
import { useForm } from "react-hook-form";
import FieldError from "@/components/error/field_error";
import { verifyBookingCode } from "@/features/user/seat_selection/seat_selection_slice";
import { clientPaths } from "@/configs/client_routes";
import "./booking_code_form.css";

function BookingCodeForm() {
    const dispatch = useDispatch();
    const navigate = useNavigate();
    const [serverError, setServerError] = useState("");
    const [isLoading, setIsLoading] = useState(false);

    const { register, handleSubmit, formState: { errors } } = useForm();

    const onSubmit = handleSubmit(async ({ bookingCode }) => {
        setServerError("");
        setIsLoading(true);
        try {
            const result = await dispatch(verifyBookingCode(bookingCode.trim())).unwrap();
            if (result) navigate(clientPaths.seatSelection);
        } catch (err) {
            setServerError(err ?? "Booking code không hợp lệ.");
        } finally {
            setIsLoading(false);
        }
    });

    return (
        <div className="verify_code_panel">
            <div className="setting_card_panel">
                <div className="panel_header">
                    <h4>
                        <i className="bx bx-calendar-check"></i>
                        Đặt chỗ ngồi
                    </h4>
                </div>
                <div className="panel_body">
                    <p className="booking_code_desc">
                        Nhập mã đặt vé để truy cập trang chọn ghế cho chuyến bay của bạn.
                    </p>
                    <form onSubmit={onSubmit} className="booking_code_form">
                        <div className="form_group_full">
                            <label className="form_label">Mã đặt vé</label>
                            <div className="booking_code_input_row">
                                <input {...register("bookingCode")} type="text" className="booking_code_input" placeholder="Nhập mã đơn đặt vé" required />
                                <button type="submit" className="btn_primary_save" disabled={isLoading} >
                                    <i className={`bx ${isLoading ? "bx-loader-alt bx-spin" : "bx-search"}`}></i>
                                    {isLoading ? " Đang kiểm tra..." : " Tra cứu"}
                                </button>
                            </div>
                            <FieldError error={errors.bookingCode} />
                            {serverError && (
                                <span className="field_error_msg">
                                    <i className="bx bx-error-circle" /> {serverError}
                                </span>
                            )}
                        </div>
                    </form>

                    <div className="booking_code_hint">
                        <i className="bx bx-info-circle"></i>
                        <span>Mã đặt vé được gửi qua email sau khi thanh toán thành công.</span>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default BookingCodeForm;
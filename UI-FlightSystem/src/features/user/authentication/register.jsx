import React, { useState } from "react";
import { useRegisterForm } from "@/features/shared/auth/use_register_form";
import FieldError from "@/components/error/field_error";
import AlertModal from "@/components/error/alert_modal";

function Register({ isOpen, onClose, onSwitchToLogin }) {
  const [alertState, setAlertState] = useState({ isOpen: false, message: "", type: "success" });

  const { register, onSubmit, formState, isLoading } = useRegisterForm({
    onSuccess: () => {
      setAlertState({
        isOpen: true,
        message: "Đăng ký tài khoản thành công!",
        type: "success"
      });
    }
  });

  const { errors } = formState;

  if (!isOpen) return null;

  const handleCloseAlert = () => {
    setAlertState({ isOpen: false, message: "", type: "success" });
    onSwitchToLogin();
  };

  return (
    <div className="auth_overlay" onClick={onClose}>
      <div className="auth_modal_card" onClick={(e) => e.stopPropagation()}>
        <button className="auth_close_btn" onClick={onClose}>
          <i className="bx bx-x"></i>
        </button>
        <div className="auth_header">
          <div className="auth_brand">
            <i className="bx bx-paper-plane brand_icon"></i>
            <span>SkyJourney</span>
          </div>
          <h2>Tạo tài khoản mới</h2>
          <p>Trở thành thành viên để tích điểm đổi vé máy bay miễn phí.</p>
        </div>

        <form onSubmit={onSubmit} className="auth_form">
          {errors.root && (
            <div className="error_alert">
              <i className="bx bx-error-circle"></i> {errors.root.message}
            </div>
          )}
          <div className="auth_form_scrollable">
            <div className="auth_form_group">
              <label htmlFor="reg_name">Họ và tên</label>
              <div className="auth_input_wrapper">
                <i className="bx bx-user input_icon"></i>
                <input id="reg_name" placeholder="Nhập họ và tên" {...register("fullName")} />
              </div>
              <FieldError error={errors.fullName} />
            </div>
            <div className="auth_form_group">
              <label htmlFor="reg_username">Tên đăng nhập</label>
              <div className="auth_input_wrapper">
                <i className="bx bx-user input_icon"></i>
                <input id="reg_username" placeholder="Nhập tên đăng nhập" {...register("username")} />
              </div>
              <FieldError error={errors.username} />
            </div>
            <div className="auth_form_group">
              <label htmlFor="reg_email">Email</label>
              <div className="auth_input_wrapper">
                <i className="bx bx-envelope input_icon"></i>
                <input id="reg_email" placeholder="Nhập email" {...register("email")} />
              </div>
              <FieldError error={errors.email} />
            </div>
            <div className="auth_form_group">
              <label htmlFor="reg_password">Mật khẩu</label>
              <div className="auth_input_wrapper">
                <i className="bx bx-lock-alt input_icon"></i>
                <input id="reg_password" type="password" placeholder="Nhập mật khẩu" {...register("password")} />
              </div>
              <FieldError error={errors.password} />
            </div>
            <div className="auth_form_group">
              <label htmlFor="reg_confirm">Xác nhận mật khẩu</label>
              <div className="auth_input_wrapper">
                <i className="bx bx-check-shield input_icon"></i>
                <input id="reg_confirm" type="password" placeholder="Nhập lại mật khẩu" {...register("confirmPassword")} />
              </div>
              <FieldError error={errors.confirmPassword} />
            </div>
          </div>
          <button type="submit" className="auth_submit_btn" disabled={isLoading}>
            <i className="bx bx-user-plus"></i> {isLoading ? "Đang xử lý..." : "Đăng Ký Tài Khoản"}
          </button>
        </form>

        <div className="auth_footer_text">
          Bạn đã có tài khoản rồi?{" "}
          <button type="button" className="auth_switch_btn" onClick={onSwitchToLogin}>
            Đăng nhập ngay
          </button>
        </div>

      </div>

      {alertState.isOpen && (
        <AlertModal type={alertState.type} message={alertState.message} onClose={handleCloseAlert}/>
      )}
    </div>
  );
}

export default Register
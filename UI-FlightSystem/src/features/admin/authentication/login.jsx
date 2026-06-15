import React from "react";
import "./login.css";
import { useAdminLoginForm } from "@/features/shared/auth/admin_login_form";
import FieldError from "@/components/error/field_error";

function AdminLogin() {
  const { register, onSubmit, formState: { errors }, isLoading } = useAdminLoginForm();

  return (
    <div className="login_page">
      <div className="login_overlay">
        <div className="login_box">
          <div className="login_header">
            <h1>SkyJourney Admin</h1>
            <p>Manage your flight world with elegance and precision.</p>
          </div>
          <form onSubmit={onSubmit} className="login_form">
            {errors.root && (
              <div className="login_alert">
                <i className="bx bx-error-circle" />
                <span>{errors.root.message}</span>
              </div>
            )}
            <div className={`login_input_group ${errors.LoginId ? "has_error" : ""}`}>
              <i className="bx bx-user" />
              <input {...register("LoginId")} type="text" placeholder="Email hoặc tên đăng nhập" />
            </div>
            <FieldError error={errors.LoginId} />

            <div className={`login_input_group ${errors.password ? "has_error" : ""}`}>
              <i className="bx bx-lock-alt" />
              <input {...register("password")} type="password" placeholder="Mật khẩu" />
            </div>
            <FieldError error={errors.password} />

            <button type="submit" className="login_btn" disabled={isLoading}>
              {isLoading ? <><i className="bx bx-loader-alt bx-spin" /> Đang đăng nhập...</> : "Đăng nhập"}
            </button>
          </form>
        </div>
      </div>
    </div>
  );
}

export default AdminLogin;
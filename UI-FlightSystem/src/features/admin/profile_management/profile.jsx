import React, { useState, useEffect } from "react";
import "./profile.css";
import { useProfileForm } from "@/features/shared/profile/use_profile";
import { useChangePassword } from "@/features/shared/profile/use_change_password";
import FieldError from "@/components/error/field_error"; 
import AlertModal from "@/components/error/alert_modal";

function Profile() {
  const [currentDate, setCurrentDate] = useState("");
  const [alert, setAlert] = useState({ type: "", message: "" });

  const showAlert = (message, type = "success") => setAlert({ type, message });

  const { register: regProfile, formState: { errors: pErr, isSubmitting: isProfileSubmitting }, onSubmit: submitProfile } = useProfileForm({ onSuccess: showAlert });
  const { reg: regPassword, formState: { errors: pwErr, isSubmitting: isPasswordSubmitting }, onSubmit: submitPassword } = useChangePassword({ onSuccess: showAlert });

  useEffect(() => {
    const today = new Date();
    setCurrentDate(today.toLocaleDateString("vi-VN", {
      weekday: "long", year: "numeric", month: "long", day: "numeric",
    }));
  }, []);

  return (
    <div className="profile_container">
      <div className="profile_header">
        <h2>Hồ sơ cá nhân</h2>
        <span>{currentDate}</span>
      </div>
      <div className="profile_layout">
        <div className="profile_card">
          <div className="profile_card_header">
            <i className="bx bxs-edit"></i>
            <h3>Chỉnh sửa thông tin cá nhân</h3>
          </div>
          <form onSubmit={submitProfile} className="profile_form_content">
            {pErr.root && <div className="error_alert">{pErr.root.message}</div>}
            <div className="profile_form_grid">
              <div className="form_group">
                <label>Tên đăng nhập</label>
                <input type="text" {...regProfile("userName")} disabled />
              </div>
              <div className="form_group">
                <label>Email *</label>
                <input type="email" placeholder="example@gmail.com" {...regProfile("email")} />
                <FieldError error={pErr.email} />
              </div>
              <div className="form_group">
                <label>Họ và tên *</label>
                <input type="text" placeholder="Nhập đầy đủ họ tên" {...regProfile("fullname")} />
                <FieldError error={pErr.fullname} />
              </div>
              <div className="form_group">
                <label>Số điện thoại</label>
                <input type="tel" placeholder="Nhập số điện thoại" {...regProfile("phoneNumber")} />
                <FieldError error={pErr.phoneNumber} />
              </div>
              <div className="form_group">
                <label>Giới tính</label>
                <select {...regProfile("gender")}>
                  <option value="">-- Chọn giới tính --</option>
                  <option value="Nam">Nam</option>
                  <option value="Nữ">Nữ</option>
                </select>
                <FieldError error={pErr.gender} />
              </div>
              <div className="form_group">
                <label>Ngày sinh</label>
                <input type="date" {...regProfile("birthday")} />
                <FieldError error={pErr.birthday} />
              </div>
              <div className="form_group">
                <label>Địa chỉ thường trú</label>
                <input type="text" placeholder="Số nhà, tên đường, tỉnh thành..." {...regProfile("address")} />
                <FieldError error={pErr.address} />
              </div>
            </div>
            <div className="profile_form_footer">
              <button type="submit" className="btn_submit" disabled={isProfileSubmitting}>
                {isProfileSubmitting ? "Đang lưu..." : "Lưu thay đổi"}
              </button>
            </div>
          </form>
        </div>
        <div className="profile_card">
          <div className="profile_card_header">
            <i className="bx bxs-lock-alt"></i>
            <h3>Đổi mật khẩu</h3>
          </div>
          <form onSubmit={submitPassword} className="profile_form_content">
            {pwErr.root && <div className="error_alert">{pwErr.root.message}</div>}
            <div className="profile_form_grid">
              <div className="form_group">
                <label>Mật khẩu hiện tại *</label>
                <input type="password" placeholder="Nhập mật khẩu hiện tại" {...regPassword("currentPassword")} />
                <FieldError error={pwErr.currentPassword} />
              </div>
              <div className="form_group"></div>
              <div className="form_group">
                <label>Mật khẩu mới *</label>
                <input type="password" placeholder="Nhập mật khẩu mới" {...regPassword("newPassword")} />
                <FieldError error={pwErr.newPassword} />
              </div>
              <div className="form_group">
                <label>Xác nhận mật khẩu mới *</label>
                <input type="password" placeholder="Nhập lại mật khẩu mới" {...regPassword("confirmNewPassword")} />
                <FieldError error={pwErr.confirmNewPassword} />
              </div>
            </div>
            <div className="profile_form_footer">
              <button type="submit" className="btn_submit" disabled={isPasswordSubmitting}>
                {isPasswordSubmitting ? "Đang xử lý..." : "Đổi mật khẩu"}
              </button>
            </div>
          </form>
        </div>

      </div>

      <AlertModal type={alert.type} message={alert.message} onClose={() => setAlert({ type: "", message: "" })} />
    </div>
  );
}

export default Profile;
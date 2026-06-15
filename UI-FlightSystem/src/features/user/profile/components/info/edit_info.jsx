import { useState } from "react";
import { useProfileForm } from "@/features/shared/profile/use_profile";
import { useChangePassword } from "@/features/shared/profile/use_change_password";
import AlertModal from "@/components/error/alert_modal";
import FieldError from "@/components/error/field_error";
import "./edit_info.css";

function EditInfo() {
  const [alert, setAlert] = useState({ type: "", message: "" });

  const handleSuccess = (msg) => setAlert({ type: "success", message: msg });
  const handleClose = () => setAlert({ type: "", message: "" });

  const { register, formState: infoFormState, onSubmit: onSubmitInfo } = useProfileForm({ onSuccess: handleSuccess });
  const { reg, formState: pwFormState, onSubmit: onSubmitPassword } = useChangePassword({ onSuccess: handleSuccess });

  const infoErrors = infoFormState.errors;
  const pwErrors = pwFormState.errors;

  return (
    <>
      {alert.message && (
        <AlertModal type={alert.type} message={alert.message} onClose={handleClose} />
      )}

      <div className="edit_info_wrapper">
        <form onSubmit={onSubmitInfo} className="setting_card_panel">
          <div className="panel_header">
            <h4>Dữ liệu cá nhân</h4>
          </div>

          <div className="panel_body">
            <div className="form_row_grid_2col">
              <div className="form_group">
                <label className="form_label">Tên đầy đủ</label>
                <input {...register("fullname")} type="text" className="form_input_text" placeholder="Nhập họ và tên..." />
                <FieldError error={infoErrors.fullname} />
              </div>

              <div className="form_group">
                <label className="form_label">Địa chỉ Email</label>
                <input {...register("email")} type="email" className="form_input_text" placeholder="nhanvien@example.com" />
                <FieldError error={infoErrors.email} />
              </div>
            </div>

            <div className="form_row_grid_2col">
              <div className="form_group">
                <label className="form_label">Giới tính</label>
                <select {...register("gender")} className="form_select">
                  <option value="">-- Chọn giới tính --</option>
                  <option value="Nam">Nam</option>
                  <option value="Nữ">Nữ</option>
                </select>
                <FieldError error={infoErrors.gender} />
              </div>
              <div className="form_group">
                <label className="form_label">Ngày sinh</label>
                <input {...register("birthday")} type="date" className="form_input_text" />
                <FieldError error={infoErrors.birthday} />
              </div>
            </div>
            <div className="form_row_grid_2col">
              <div className="form_group">
                <label className="form_label">Tên đăng nhập</label>
                <input {...register("userName")} type="text" className="form_input_text" placeholder="Tên đăng nhập..." disabled />
              </div>
              <div className="form_group">
                <label className="form_label">Số điện thoại</label>
                <input {...register("phoneNumber")} type="text" className="form_input_text" placeholder="Số điện thoại..." />
                <FieldError error={infoErrors.phoneNumber} />
              </div>
            </div>
            <div className="form_group_full">
              <label className="form_label">Địa chỉ</label>
              <input {...register("address")} type="text" className="form_input_text" placeholder="Địa chỉ cư trú..." />
              <FieldError error={infoErrors.address} />
            </div>
            <FieldError error={infoErrors.root} />
            <div className="form_actions_footer">
              <button type="submit" className="btn_primary_save" disabled={infoFormState.isSubmitting}>
                <i className="bx bx-save"></i>
                {infoFormState.isSubmitting ? " Đang lưu..." : " Lưu thông tin"}
              </button>
            </div>
          </div>
        </form>

        <form onSubmit={onSubmitPassword} className="setting_card_panel">
          <div className="panel_header">
            <h4>Mật khẩu & Bảo mật</h4>
          </div>

          <div className="panel_body">
            <div className="form_group_full">
              <label className="form_label">Mật khẩu hiện tại</label>
              <input {...reg("currentPassword")} type="password" className="form_input_text" placeholder="Nhập mật khẩu hiện tại..." />
              <FieldError error={pwErrors.currentPassword} />
            </div>
            <div className="form_row_grid_2col">
              <div className="form_group">
                <label className="form_label">Mật khẩu mới</label>
                <input {...reg("newPassword")} type="password" className="form_input_text" placeholder="Tối thiểu 6 ký tự..." />
                <FieldError error={pwErrors.newPassword} />
              </div>
              <div className="form_group">
                <label className="form_label">Xác nhận mật khẩu mới</label>
                <input {...reg("confirmNewPassword")} type="password" className="form_input_text" placeholder="Nhập lại mật khẩu mới..." />
                <FieldError error={pwErrors.confirmNewPassword} />
              </div>
            </div>
            <FieldError error={pwErrors.root} />
            <div className="form_actions_footer">
              <button type="submit" className="btn_accent_save" disabled={pwFormState.isSubmitting} >
                <i className="bx bx-key"></i>
                {pwFormState.isSubmitting ? " Đang xử lý..." : " Đổi mật khẩu"}
              </button>
            </div>
          </div>
        </form>
      </div>
    </>
  );
};

export default EditInfo;
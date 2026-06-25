import React from "react";
import { useAccountForm } from "../use_account_form";
import FieldError from "@/components/error/field_error";

function AccountForm({ isOpen, onClose, onSave, accountData, mode }) {
  const { register, onSubmit, formState: { errors, isSubmitting }, isEdit } = useAccountForm({ mode, accountData, onSuccess: onSave, onClose });
  if (!isOpen) return null;
  return (
    <div className="modal_overlay">
      <div className="form_modal">
        <div className="form_modal_header">
          <h3>{isEdit ? "Cập nhật thông tin tài khoản" : "Thêm tài khoản mới"}</h3>
          <button className="btn_close_modal" onClick={onClose} type="button">
            <i className="bx bx-x" />
          </button>
        </div>
        <form onSubmit={onSubmit} className="form_modal_content">
          {errors.root && (
            <div className="error_alert">
              <i className="bx bx-error-circle" />
              <span>{errors.root.message}</span>
            </div>
          )}
          <div className="form_grid_two">
            <div className={`form_group ${errors.userName ? "has_error" : ""}`}>
              <label>Tên đăng nhập *</label>
              <input {...register("userName")} placeholder="Ví dụ: NguyenVanA123" disabled={isEdit} />
              <FieldError error={errors.userName} />
            </div>
            <div className={`form_group ${errors.email ? "has_error" : ""}`}>
              <label>Địa chỉ Email *</label>
              <input {...register("email")} placeholder="example@gmail.com" />
              <FieldError error={errors.email} />
            </div>
            <div className={`form_group ${errors.fullname ? "has_error" : ""}`}>
              <label>Họ và tên *</label>
              <input {...register("fullname")} placeholder="Nhập đầy đủ họ tên" />
              <FieldError error={errors.fullname} />
            </div>
            <div className={`form_group ${errors.phoneNumber ? "has_error" : ""}`}>
              <label>Số điện thoại</label>
              <input {...register("phoneNumber")} placeholder="Nhập số điện thoại" type="tel" />
              <FieldError error={errors.phoneNumber} />
            </div>
            <div className="form_group">
              <label>Ngày sinh</label>
              <input {...register("birthday")} type="date" />
              <FieldError error={null} />
            </div>
            <div className="form_group">
              <label>Địa chỉ</label>
              <input {...register("address")} placeholder="Số nhà, đường, tỉnh thành..." />
              <FieldError error={null} />
            </div>
            <div className="form_group">
              <label>Giới tính</label>
              <select {...register("gender")}>
                <option value="Nam">Nam</option>
                <option value="Nữ">Nữ</option>
              </select>
              <FieldError error={null} />
            </div>
            <div className={`form_group ${errors.roleNames ? "has_error" : ""}`}>
              <label>Vai trò</label>
              <select {...register("roleNames")}>
                <option value="admin">Quản trị viên</option>
                <option value="staff">Nhân viên</option>
              </select>
              <FieldError error={errors.roleNames} />
            </div>
            {!isEdit && (
              <>
                <div className={`form_group ${errors.password ? "has_error" : ""}`}>
                  <label>Mật khẩu *</label>
                  <input {...register("password")} type="password" placeholder="Nhập mật khẩu" />
                  <FieldError error={errors.password} />
                </div>
                <div className={`form_group ${errors.confirmPassword ? "has_error" : ""}`}>
                  <label>Xác nhận mật khẩu *</label>
                  <input {...register("confirmPassword")} type="password" placeholder="Nhập lại mật khẩu" />
                  <FieldError error={errors.confirmPassword} />
                </div>
              </>
            )}
          </div>
          <div className="form_modal_footer">
            <button type="button" className="btn_cancel" onClick={onClose}>Hủy bỏ</button>
            <button type="submit" className="btn_submit" disabled={isSubmitting}>
              {isSubmitting ? "Đang xử lý..." : isEdit ? "Lưu thay đổi" : "Tạo tài khoản"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

export default AccountForm;
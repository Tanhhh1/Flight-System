import React from "react";
import { useServiceForm } from "../use_service_form";
import FieldError from "@/components/error/field_error";

function ServiceForm({ isOpen, onClose, onSave, serviceData, mode }) {
  const { register, onSubmit, formState: { errors, isSubmitting }, isEdit } = useServiceForm({ mode, serviceData, onSuccess: onSave, onClose });
  if (!isOpen) return null;

  return (
    <div className="modal_overlay">
      <div className="form_modal">
        <div className="form_modal_header">
          <h3>{isEdit ? "Cập nhật dịch vụ" : "Thêm dịch vụ mới"}</h3>
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
          <div className={`form_group ${errors.serviceName ? "has_error" : ""}`}>
              <label>Tên dịch vụ *</label>
              <input {...register("serviceName")} placeholder="Nhập tên dịch vụ" />
              <FieldError error={errors.serviceName} />
            </div>
          <div className="form_group">
            <label>Mô tả</label>
            <textarea {...register("description")} placeholder="Nhập mô tả dịch vụ" rows={3} />
            <FieldError error={errors.description} />
          </div>
          <div className="form_modal_footer">
            <button type="button" className="btn_cancel" onClick={onClose}>Hủy bỏ</button>
            <button type="submit" className="btn_submit" disabled={isSubmitting}>
              {isSubmitting ? "Đang xử lý..." : isEdit ? "Lưu thay đổi" : "Tạo dịch vụ"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

export default ServiceForm;
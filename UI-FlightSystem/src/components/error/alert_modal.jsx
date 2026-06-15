import React from "react";
import "./error.css";

export default function AlertModal({ type, message, onClose }) {
  if (!message) return null;

  const isSuccess = type === "success";

  return (
    <div className="auth-alert-overlay" onClick={onClose}>
      <div className="auth-alert-box" onClick={(e) => e.stopPropagation()}>
        
        <div className={`auth-alert-icon-wrap ${isSuccess ? "alert-success" : "alert-danger"}`}>
          <i className={`bx ${isSuccess ? "bx-check-circle animate-scale" : "bx-error-alt animate-shake"}`}></i>
        </div>

        <div className="auth-alert-content">
          <h3 className="auth-alert-title">
            {isSuccess ? "Thành công!" : "Có lỗi xảy ra!"}
          </h3>
          <p className="auth-alert-message">{message}</p>
        </div>

        <div className="auth-alert-actions">
          <button className={`auth-alert-confirm-btn ${isSuccess ? "btn-success-color" : "btn-danger-color"}`} onClick={onClose}>
            Đóng
          </button>
        </div>

      </div>
    </div>
  );
}
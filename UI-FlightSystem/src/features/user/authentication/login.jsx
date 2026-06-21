import { useNavigate } from "react-router-dom";
import { useDispatch, useSelector } from "react-redux";
import { closeLoginModal } from "@/features/shared/auth/auth_slice";
import { useLoginForm } from "@/features/shared/auth/use_login_form";
import { isAdminRole } from "@/constants/auth";
import { applyServerErrors } from "@/hooks/use_shared_form";
import FieldError from "@/components/error/alert_modal";
import "./login.css";

function Login({ onSwitchToRegister }) {
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const { showLoginModal: isOpen, returnTo } = useSelector((state) => state.auth);

  const handleClose = () => dispatch(closeLoginModal());

  const { register, onSubmit, formState, isLoading, reset } = useLoginForm({
    onSuccess: (user) => {
      reset();
      if (returnTo) navigate(returnTo);
    },
    onRoleBlocked: (user, setError) => {
      const blocked = isAdminRole(user.roles);
      if (blocked) {
        applyServerErrors(setError, {
          errors: [{ propertyName: null, errorMessage: "Tài khoản quản trị không thể đăng nhập tại đây." }],
        });
      }
      return blocked;
    },
  });

  const { errors } = formState;
  if (!isOpen) return null;

  return (
    <div className="auth_overlay" onClick={handleClose}>
      <div className="auth_modal_card" onClick={(e) => e.stopPropagation()}>
        <button className="auth_close_btn" onClick={handleClose}>
          <i className="bx bx-x"></i>
        </button>

        <div className="auth_header">
          <div className="auth_brand">
            <i className="bx bx-paper-plane brand_icon"></i>
            <span>SkyJourney</span>
          </div>
          <h2>Chào mừng bạn trở lại</h2>
          <p>Đăng nhập để quản lý chuyến bay và nhận nhiều ưu đãi hấp dẫn.</p>
        </div>

        {errors.root && (
          <div className="error_alert">
            <i className="bx bx-error-circle"></i> {errors.root.message}
          </div>
        )}

        <form onSubmit={onSubmit} className="auth_form">
          <div className="auth_form_group">
            <label>Địa chỉ Email / Tên đăng nhập</label>
            <div className="auth_input_wrapper">
              <i className="bx bx-envelope input_icon"></i>
              <input {...register("LoginId")} placeholder="example@gmail.com" />
            </div>
            <FieldError error={errors.LoginId} />
          </div>

          <div className="auth_form_group">
            <div className="label_flex_row">
              <label>Mật khẩu</label>
            </div>
            <div className="auth_input_wrapper">
              <i className="bx bx-lock-alt input_icon"></i>
              <input {...register("password")} type="password" placeholder="Nhập mật khẩu của bạn" />
            </div>
            <FieldError error={errors.password} />
          </div>

          <button type="submit" className="auth_submit_btn" disabled={isLoading}>
            {isLoading
              ? <><i className="bx bx-loader-alt bx_spin"></i> Đang đăng nhập...</>
              : <><i className="bx bx-log-in-circle"></i> Đăng Nhập</>
            }
          </button>
        </form>

        <div className="auth_footer_text">
          Bạn chưa có tài khoản?{" "}
          <button type="button" className="auth_switch_btn" onClick={() => { handleClose(); onSwitchToRegister(); }}>
            Đăng ký ngay
          </button>
        </div>
      </div>
    </div>
  );
}

export default Login;
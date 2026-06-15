import React, { useMemo } from "react";
import { useAirlineForm } from "../use_airline_form";
import { COMMON_RULES } from "@/constants/airlines"
import countries from "world-countries";
import FieldError from "@/components/error/field_error";
import SearchableSelect from "@/components/select/searchables_select";
import { Controller } from "react-hook-form";

function AirlineForm({ isOpen, onClose, onSave, airlineData, mode }) {
  const { register, control, onSubmit, formState: { errors, isSubmitting }, isEdit } = useAirlineForm({ mode, airlineData, onSuccess: onSave, onClose });

  const countryOptions = useMemo(() =>
    countries
      .map((c) => ({ id: c.cca2, name: c.translations.vie?.common ?? c.name.common }))
      .sort((a, b) => a.name.localeCompare(b.name, "vi"))
    , []);

  if (!isOpen) return null;
  return (
    <div className="modal_overlay">
      <div className="form_modal">
        <div className="form_modal_header">
          <h3>{isEdit ? "Cập nhật hãng hàng không" : "Thêm hãng hàng không mới"}</h3>
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
            <div className={`form_group ${errors.airlineName ? "has_error" : ""}`}>
              <label>Tên hãng *</label>
              <input {...register("airlineName")} placeholder="Ví dụ: Vietnam Airlines" />
              <FieldError error={errors.airlineName} />
            </div>
            <div className={`form_group ${errors.airlineCode ? "has_error" : ""}`}>
              <label>Mã hãng *</label>
              <input {...register("airlineCode")} placeholder="Ví dụ: VN, VJ, QH..." />
              <FieldError error={errors.airlineCode} />
            </div>
            <div className={`form_group ${errors.country ? "has_error" : ""}`}>
              <label>Quốc gia *</label>
              <Controller
                name="country"
                control={control}
                rules={COMMON_RULES.country}
                render={({ field }) => (
                  <SearchableSelect
                    data={countryOptions}
                    value={field.value ?? ""}
                    onChange={(val) => field.onChange(val)}
                    placeholder="-- Chọn quốc gia --"
                    itemKey="id"
                    displayValue="name"
                    searchFields={["name"]}
                  />
                )}
              />
              <FieldError error={errors.country} />
            </div>
            {isEdit && (
              <div className="form_group">
                <label>Trạng thái</label>
                <select {...register("status")}>
                  {airlineData?.status === "Inactive" && (<option value="Inactive" disabled hidden>Không hoạt động</option>)}
                  <option value="Active">Hoạt động</option>
                  <option value="Suspended">Tạm ngưng</option>
                </select>
              </div>
            )}
          </div>
          <div className="form_modal_footer">
            <button type="button" className="btn_cancel" onClick={onClose}>Hủy bỏ</button>
            <button type="submit" className="btn_submit" disabled={isSubmitting}>
              {isSubmitting ? "Đang xử lý..." : isEdit ? "Lưu thay đổi" : "Thêm mới"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

export default AirlineForm;
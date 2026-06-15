import React, { useEffect, useMemo, useState } from "react";
import { Controller } from "react-hook-form";
import { usePlaneForm } from "../use_plane_form";
import { COMMON_RULES } from "@/constants/plane"
import { dataSearchService, DataSearch } from "@/services/data_search_service";
import FieldError from "@/components/error/field_error";
import SearchableSelect from "@/components/select/searchables_select";

function PlaneForm({ isOpen, onClose, onSave, planeData, mode }) {
  const { register, control, onSubmit, formState: { errors, isSubmitting }, isEdit } = usePlaneForm({ mode, planeData, onSuccess: onSave, onClose });
  const [airlines, setAirlines] = useState([]);

  useEffect(() => {
    if (!isOpen) return;
    dataSearchService
      .get([DataSearch.Airlines])
      .then(({ data }) => {if (data.succeeded) setAirlines(data.result.airlines ?? [])})
      .catch(console.error);
  }, [isOpen]);

  const airlineOptions = useMemo(() => airlines.map((a) => ({ id: a.airlineId, name: a.airlineName })), [airlines]);

  if (!isOpen) return null;
  return (
    <div className="modal_overlay">
      <div className="form_modal">
        <div className="form_modal_header">
          <h3>{isEdit ? "Cập nhật máy bay" : "Thêm máy bay mới"}</h3>
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
            <div className={`form_group ${errors.planeModel ? "has_error" : ""}`}>
              <label>Tên máy bay *</label>
              <input {...register("planeModel")} placeholder="Ví dụ: Boeing 737, Airbus A320..." />
              <FieldError error={errors.planeModel} />
            </div>
            <div className={`form_group ${errors.airlineId ? "has_error" : ""}`}>
              <label>Hãng hàng không *</label>
              <Controller
                name="airlineId"
                control={control}
                rules={COMMON_RULES.airlineId}
                render={({ field }) => (
                  <SearchableSelect
                    data={airlineOptions}
                    value={field.value ?? ""}
                    onChange={(val) => field.onChange(val)}
                    placeholder="-- Chọn hãng hàng không --"
                    itemKey="id"
                    displayValue="name"
                    searchFields={["name"]}
                  />
                )}/>
              <FieldError error={errors.airlineId} />
            </div>
            {isEdit && (
              <div className="form_group">
                <label>Trạng thái</label>
                <select {...register("status")}>
                  {planeData?.status === "Inactive" && (<option value="Inactive" disabled hidden>Không hoạt động</option>)}
                  <option value="Active">Hoạt động</option>
                  <option value="Delayed">Bảo trì</option>
                </select>
              </div>
            )}
          </div>
          <div className="form_modal_footer">
            <button type="button" className="btn_cancel" onClick={onClose}>Hủy bỏ</button>
            <button type="submit" className="btn_submit" disabled={isSubmitting}>
              {isSubmitting ? "Đang xử lý..." : isEdit ? "Lưu thay đổi" : "Tạo máy bay"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

export default PlaneForm;
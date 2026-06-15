import React, { useEffect, useState, useMemo } from "react";
import { Controller } from "react-hook-form";
import { useRouteForm } from "../use_route_form";
import { COMMON_RULES } from "@/constants/route"
import { dataSearchService, DataSearch } from "@/services/data_search_service";
import FieldError from "@/components/error/field_error";
import SearchableSelect from "@/components/select/searchables_select";

function RouteForm({ isOpen, onClose, onSave, routeData, mode }) {
  const { register, control, formState: { errors, isSubmitting }, onSubmit, isEdit, setValue } =
    useRouteForm({ mode, routeData, onSuccess: onSave, onClose });

  const [airports, setAirports] = useState([]);

  useEffect(() => {
    if (!isOpen) return;
    dataSearchService
      .get([DataSearch.Airports])
      .then(({ data }) => {
        if (data.succeeded) {
          const listAirports = data.result.airports ?? [];
          setAirports(listAirports);
          if (isEdit && routeData) {
            if (routeData.originAirportId) setValue("originAirportId", String(routeData.originAirportId));
            if (routeData.destinationAirportId) setValue("destinationAirportId", String(routeData.destinationAirportId));
          }
        }
      })
      .catch(console.error);
  }, [isOpen, routeData, isEdit, setValue]);

  const airportOptions = useMemo(() =>
    airports.map((a) => ({
      id: String(a.airportId),
      name: `${a.airportCode} — ${a.airportName}`,
    }))
  , [airports]);

  if (!isOpen) return null;
  return (
    <div className="modal_overlay">
      <div className="form_modal">
        <div className="form_modal_header">
          <h3>{isEdit ? "Cập nhật tuyến bay" : "Thêm tuyến bay mới"}</h3>
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
            <div className="form_column">
              <div className={`form_group ${errors.originAirportId ? "has_error" : ""}`}>
                <label>Sân bay khởi hành *</label>
                <Controller
                  name="originAirportId"
                  control={control}
                  rules={COMMON_RULES.originAirportId}
                  render={({ field }) => (
                    <SearchableSelect
                      data={airportOptions}
                      value={field.value ?? ""}
                      onChange={(val) => field.onChange(val)}
                      placeholder="-- Chọn sân bay khởi hành --"
                      itemKey="id"
                      displayValue="name"
                      searchFields={["name"]}
                    />
                  )}
                />
                <FieldError error={errors.originAirportId} />
              </div>
              <div className={`form_group ${errors.flightDuration ? "has_error" : ""}`}>
                <label>Thời gian bay (phút) *</label>
                <input type="number" placeholder="Ví dụ: 120" min="1" {...register("flightDuration")} />
                <FieldError error={errors.flightDuration} />
              </div>
            </div>
            <div className="form_column">
              <div className={`form_group ${errors.destinationAirportId ? "has_error" : ""}`}>
                <label>Sân bay đến *</label>
                <Controller
                  name="destinationAirportId"
                  control={control}
                  rules={COMMON_RULES.destinationAirportId}
                  render={({ field }) => (
                    <SearchableSelect
                      data={airportOptions}
                      value={field.value ?? ""}
                      onChange={(val) => field.onChange(val)}
                      placeholder="-- Chọn sân bay đến --"
                      itemKey="id"
                      displayValue="name"
                      searchFields={["name"]}
                    />
                  )}
                />
                <FieldError error={errors.destinationAirportId} />
              </div>
              {isEdit && (
                <div className="form_group">
                  <label>Trạng thái</label>
                  <select {...register("status")}>
                    {routeData?.status === "Inactive" && (<option value="Inactive" disabled hidden>Không hoạt động</option>)}
                    <option value="Active">Hoạt động</option>
                    <option value="Suspended">Tạm ngưng</option>
                  </select>
                </div>
              )}
            </div>
          </div>
          <div className="form_modal_footer">
            <button type="button" className="btn_cancel" onClick={onClose}>Hủy bỏ</button>
            <button type="submit" className="btn_submit" disabled={isSubmitting}>
              {isSubmitting ? "Đang xử lý..." : isEdit ? "Lưu thay đổi" : "Tạo tuyến bay"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

export default RouteForm;
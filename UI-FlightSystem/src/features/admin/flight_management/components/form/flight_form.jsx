import React, { useEffect, useState, useMemo } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { Controller } from "react-hook-form";
import { useFlightForm } from "../../use_flight_form";
import { COMMON_RULES } from "@/constants/flight";
import SearchableSelect from "@/components/select/searchables_select";
import { flightService } from "../../flight_service";
import { dataSearchService, DataSearch } from "@/services/data_search_service";
import { SEAT_CLASSES } from "@/constants/flight";
import { currentDate } from "@/utils/date_utils";
import FieldError from "@/components/error/field_error";
import "./flight_form.css"

function FlightFormPage() {
  const navigate = useNavigate();
  const { id } = useParams();
  const isEdit = !!id;
  const mode = isEdit ? "edit" : "add";
  const [flightData, setFlightData] = useState(null);
  const [planes, setPlanes] = useState([]);
  const [routes, setRoutes] = useState([]);
  const [services, setServices] = useState([]);

  useEffect(() => {
    if (!isEdit) return;
    flightService.getById(id).then(({ data }) => { if (data.succeeded) setFlightData(data.result) }).catch(console.error)}, [id]);

  useEffect(() => {
    dataSearchService
      .get([DataSearch.Planes, DataSearch.Services, DataSearch.Routes])
      .then(({ data }) => {
        if (data.succeeded) {
          setPlanes(data.result.planes ?? []);
          setServices(data.result.services ?? []);
          setRoutes(data.result.routes ?? []);
        }
      })
      .catch(console.error)
  }, []);

  const { register, control, onSubmit, formState: { errors, isSubmitting }, watch, setValue, segmentFields,
    appendSegment, removeSegment, seatPriceFields, serviceIds, toggleService, } = useFlightForm({
      mode, flightData,
      onSuccess: () => navigate("/admin/flights"),
      onClose: () => navigate("/admin/flights"),
    });

  const isRefund = watch("isRefund");
  const isChange = watch("isChange");

  const planeOptions = useMemo(() => planes.map((p) => ({ id: String(p.planeId), name: `${p.planeModel} - ${p.airlineName}` })), [planes]);
  const routeOptions = useMemo(() => routes.map((r) => ({ id: String(r.routeId), name: `${r.originAirportCode} (${r.originAirportName}) → ${r.destinationAirportCode} (${r.destinationAirportName})` })), [routes]);
  const segmentRouteOptions = useMemo(() => routes.map((r) => ({ id: String(r.routeId), name: `${r.originAirportCode} → ${r.destinationAirportCode}` })), [routes]);

  return (
    <div className="ff_container">
      <div className="ff_page_header">
        <div>
          <h2>{isEdit ? "Cập nhật chuyến bay" : "Thêm chuyến bay mới"}</h2>
          <span>{currentDate()}</span>
        </div>
        <div className="ff_page_actions">
          <button type="button" className="btn_cancel" onClick={() => navigate("/admin/flights")}>
            <i className="bx bx-arrow-back"/> Quay lại
          </button>
          <button type="submit" form="flight_form" className="btn_submit" disabled={isSubmitting}>
            {isSubmitting ? "Đang xử lý..." : isEdit ? "Lưu thay đổi" : "Tạo chuyến bay"}
          </button>
        </div>
      </div>

      <form id="flight_form" onSubmit={onSubmit}>
        {errors.root && (
          <div className="error_alert">
            <i className="bx bx-error-circle" />
            <span>{errors.root.message}</span>
          </div>
        )}

        <div className="ff_layout">
          <div className="ff_main">
            <div className="ff_card">
              <div className="ff_section_header">
                <div className="ff_section_number">1</div>
                <div>
                  <div className="ff_section_title">
                    <i className="bx bx-info-circle" />Thông tin cơ bản
                  </div>
                  <div className="ff_section_subtitle">Chọn máy bay, tuyến bay và giờ bay</div>
                </div>
              </div>
              <div className="ff_card_body">
                <div className="ff_grid_two">
                  <div className={`form_group ${errors.planeId ? "has_error" : ""}`}>
                    <label>Máy bay *</label>
                    <Controller
                      name="planeId"
                      control={control}
                      rules={COMMON_RULES.planeId}
                      render={({ field }) => (
                        <SearchableSelect
                          data={planeOptions}
                          value={String(field.value ?? "")}
                          onChange={(val) => field.onChange(val ?? "")}
                          placeholder="-- Chọn máy bay --"
                          itemKey="id"
                          displayValue="name"
                          searchFields={["name"]}
                        />
                      )}/>
                    <FieldError error={errors.planeId} />
                  </div>

                  <div className={`form_group ${errors.routeId ? "has_error" : ""}`}>
                    <label>Tuyến bay *</label>
                    <Controller
                      name="routeId"
                      control={control}
                      rules={COMMON_RULES.routeId}
                      render={({ field }) => (
                        <SearchableSelect
                          data={routeOptions}
                          value={String(field.value ?? "")}
                          onChange={(val) => field.onChange(val ?? "")}
                          placeholder="-- Chọn tuyến bay --"
                          itemKey="id"
                          displayValue="name"
                          searchFields={["name"]}
                        />
                      )}/>
                    <FieldError error={errors.routeId} />
                  </div>

                  <div className={`form_group ${errors.departureTime ? "has_error" : ""}`}>
                    <label>Giờ khởi hành *</label>
                    <input {...register("departureTime")} type="datetime-local" />
                    <FieldError error={errors.departureTime} />
                  </div>

                  {isEdit && (
                    <div className="form_group">
                      <label>Trạng thái</label>
                      <select {...register("status")}>
                        {flightData?.status === "Inactive" && (<option value="Inactive" disabled hidden>Không hoạt động</option>)}
                        <option value="Active">Hoạt động</option>
                        <option value="Delayed">Trì hoãn</option>
                        <option value="Cancelled">Hủy chuyến</option>
                      </select>
                    </div>
                  )}

                  {isEdit && flightData && (
                    <div className="form_group" style={{ gridColumn: "span 2" }}>
                      <label>Thông tin hiện tại</label>
                      <input disabled value={`${flightData.planeName} — ${flightData.originAirportCode} → ${flightData.destinationAirportCode}`} />
                    </div>
                  )}
                </div>
              </div>
            </div>

            <div className="ff_card">
              <div className="ff_section_header">
                <div className="ff_section_number">2</div>
                <div>
                  <div className="ff_section_title">
                    <i className="bx bx-shield" />Chính sách
                  </div>
                  <div className="ff_section_subtitle">Các chính sách áp dụng cho chuyến bay</div>
                </div>
              </div>
              <div className="ff_card_body">
                <div className="ff_policy_grid">
                  <label className={`ff_policy_option ${isRefund ? "active" : ""}`}>
                    <input {...register("isRefund")} type="checkbox" onChange={(e) => setValue("isRefund", e.target.checked)} />
                    <div className="ff_policy_icon"><i className="bx bx-undo" /></div>
                    <div className="ff_policy_text">
                      <strong>Hoàn vé</strong>
                      <span>Cho phép hoàn tiền</span>
                    </div>
                  </label>
                  <label className={`ff_policy_option ${isChange ? "active" : ""}`}>
                    <input {...register("isChange")} type="checkbox" onChange={(e) => setValue("isChange", e.target.checked)} />
                    <div className="ff_policy_icon"><i className="bx bx-transfer" /></div>
                    <div className="ff_policy_text">
                      <strong>Đổi vé</strong>
                      <span>Cho phép đổi chuyến</span>
                    </div>
                  </label>
                </div>
              </div>
            </div>

            <div className="ff_card">
              <div className="ff_section_header">
                <div className="ff_section_number">3</div>
                <div>
                  <div className="ff_section_title">
                    <i className="bx bx-map-alt" />
                    Chặng bay
                  </div>
                  <div className="ff_section_subtitle">Thêm chặng dừng nếu có (không bắt buộc)</div>
                </div>
              </div>
              <div className="ff_card_body">
                <div className="ff_segments">
                  {segmentFields.length === 0 && (
                    <div className="ff_empty_state">
                      <i className="bx bx-transfer" />
                      <span>Chưa có chặng dừng — chuyến bay thẳng</span>
                    </div>
                  )}
                  {segmentFields.map((field, index) => (
                    <div key={field.id} className="ff_segment_item">
                      <div className="ff_segment_order">{index + 1}</div>
                      <div className="ff_segment_fields">
                        <div className={`form_group ${errors.segments?.[index]?.routeId ? "has_error" : ""}`}>
                          <label>Tuyến chặng *</label>
                          <Controller
                            name={`segments.${index}.routeId`}
                            control={control}
                            rules={{ required: "Vui lòng chọn tuyến chặng" }}
                            render={({ field: segmentField }) => (
                              <SearchableSelect
                                data={segmentRouteOptions}
                                value={String(segmentField.value ?? "")}
                                onChange={(val) => segmentField.onChange(val ?? "")}
                                placeholder="-- Chọn tuyến --"
                                itemKey="id"
                                displayValue="name"
                                searchFields={["name"]}
                              />
                            )}
                          />
                          <FieldError error={errors.segments?.[index]?.routeId} />
                        </div>

                        <div className={`form_group ${errors.segments?.[index]?.departureTime ? "has_error" : ""}`}>
                          <label>Giờ khởi hành chặng *</label>
                          <input {...register(`segments.${index}.departureTime`, { required: "Vui lòng nhập giờ" })} type="datetime-local" />
                          <FieldError error={errors.segments?.[index]?.departureTime} />
                        </div>
                      </div>
                      <div className="ff_segment_actions">
                        <button type="button" className="ff_btn_remove" onClick={() => removeSegment(index)}>
                          <i className="bx bx-trash" />
                        </button>
                      </div>
                    </div>
                  ))}
                </div>
                <button type="button" className="ff_btn_add_segment" onClick={() => appendSegment({ segmentId: "", routeId: "", departureTime: "" })}>
                  <i className="bx bx-plus" /> Thêm chặng dừng
                </button>
              </div>
            </div>
          </div>

          <div className="ff_sidebar">
            <div className="ff_card">
              <div className="ff_section_header">
                <div className="ff_section_number">4</div>
                <div>
                  <div className="ff_section_title">
                    <i className="bx bx-purchase-tag" />
                    Giá vé theo hạng ghế
                  </div>
                  <div className="ff_section_subtitle">Nhập giá cho từng hạng ghế</div>
                </div>
              </div>
              <div className="ff_card_body">
                <div className="ff_seat_prices">
                  {seatPriceFields.map((field, index) => {
                    const priceValue = watch(`seatPrices.${index}.price`);
                    const formatted = priceValue ? Number(priceValue).toLocaleString("vi-VN") + " đ" : "";
                    return (
                      <div key={field.id} className="ff_seat_price_item">
                        <div className="ff_seat_class_label">
                          <i className="bx bx-chair" />
                          {SEAT_CLASSES[index]?.className}
                        </div>
                        <div className="ff_seat_price_input">
                          <input {...register(`seatPrices.${index}.price`, { required: `Giá ${SEAT_CLASSES[index]?.className} không được để trống`, min: { value: 0, message: "Giá không được âm" }, })} type="number" placeholder="Nhập giá" />
                          <span className="ff_currency">VNĐ</span>
                        </div>
                        {formatted && <div className="ff_seat_price_preview">{formatted}</div>}
                        <FieldError error={errors.seatPrices?.[index]?.price} />
                      </div>
                    );
                  })}
                </div>
              </div>
            </div>

            <div className="ff_card">
              <div className="ff_section_header">
                <div className="ff_section_number">5</div>
                <div>
                  <div className="ff_section_title">
                    <i className="bx bx-package" />Dịch vụ đi kèm
                  </div>
                  <div className="ff_section_subtitle">Chọn dịch vụ cho chuyến bay</div>
                </div>
              </div>
              <div className="ff_card_body">
                <div className="ff_services">
                  {services.map((s) => {
                    const checked = serviceIds.includes(String(s.serviceId));
                    return (
                      <div key={s.serviceId} className={`ff_service_item ${checked ? "active" : ""}`} onClick={() => toggleService(s.serviceId)}>
                        <div className="ff_service_info">
                          <div className="ff_service_name">{s.serviceName}</div>
                        </div>
                      </div>
                    );
                  })}
                </div>
              </div>
            </div>
          </div>
        </div>
      </form>
    </div>
  );
}

export default FlightFormPage;
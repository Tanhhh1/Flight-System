import React, { useMemo } from "react";
import { Controller } from "react-hook-form";
import { useAirportForm } from "../use_airport_form";
import { COMMON_RULES } from "@/constants/airport"
import countries from "world-countries";
import FieldError from "@/components/error/field_error";
import SearchableSelect from "@/components/select/searchables_select";

function AirportForm({ isOpen, onClose, onSave, airportData, mode }) {
    const { register, control, onSubmit, formState: { errors, isSubmitting }, isEdit } = useAirportForm({ mode, airportData, onSuccess: onSave, onClose });
    const countryOptions = useMemo(() => countries.map((c) => ({ id: c.cca2, name: c.translations.vie?.common ?? c.name.common })).sort((a, b) => a.name.localeCompare(b.name, "vi")), []);

    if (!isOpen) return null;
    return (
        <div className="modal_overlay">
            <div className="form_modal">
                <div className="form_modal_header">
                    <h3>{isEdit ? "Cập nhật sân bay" : "Thêm sân bay mới"}</h3>
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
                        <div className={`form_group ${errors.airportCode ? "has_error" : ""}`}>
                            <label>Mã sân bay (IATA) *</label>
                            <input {...register("airportCode")} placeholder="Ví dụ: SGN, HAN, SIN..." />
                            <FieldError error={errors.airportCode} />
                        </div>
                        <div className={`form_group ${errors.airportName ? "has_error" : ""}`}>
                            <label>Tên sân bay *</label>
                            <input {...register("airportName")} placeholder="Ví dụ: Sân bay Quốc tế Changi" />
                            <FieldError error={errors.airportName} />
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
                                )} />
                            <FieldError error={errors.country} />
                        </div>
                        <div className={`form_group ${errors.city ? "has_error" : ""}`}>
                            <label>Thành phố *</label>
                            <Controller
                                name="country"
                                control={control}
                                render={({ field }) => (<input {...register("city")} placeholder="Nhập tên thành phố" disabled={!field.value}/>)}
                            />
                            <FieldError error={errors.city} />
                        </div>
                        {isEdit && (
                            <div className="form_group">
                                <label>Trạng thái</label>
                                <select {...register("status")}>
                                    {airportData?.status === "Inactive" && (<option value="Inactive" disabled hidden>Không hoạt động</option>)}
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

export default AirportForm;
import React, { useState, useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import Table from "@/components/table/table";
import Pagination from "@/components/pagination/pagination";
import PlaneForm from "./plane_form";
import { planeService } from "../plane_service";
import { fetchPlanes, fetchPlaneById, setPage, setSearch, setStatus, clearSelectedItem } from "../plane_slice";
import { useConfirmAction } from "@/hooks/use_shared_form";
import { useDebounce } from "@/hooks/use_debounce";
import { formatDate, currentDate } from "@/utils/date_utils";
import { TABLE_HEADS, PLANE_STATUS_LABEL } from "@/constants/plane";

function PlaneList() {
  const dispatch = useDispatch();
  const { items, pageIndex, pageSize, totalPages, totalCount, error, search, status, isLoading, isDetailLoading } = useSelector((state) => state.plane) || {};

  const [searchInput, setSearchInput] = useState(search);
  const [formState, setFormState] = useState({ isOpen: false, mode: "add", data: null });
  const debouncedSearch = useDebounce(searchInput);
  const refresh = () => dispatch(fetchPlanes({ pageIndex, pageSize, search, status }));
  const toggleActive = useConfirmAction({ onSuccess: refresh });

  useEffect(() => { dispatch(fetchPlanes({ pageIndex, pageSize, search, status })) }, [dispatch, pageIndex, pageSize, search, status]);
  useEffect(() => { dispatch(setSearch(debouncedSearch)) }, [debouncedSearch, dispatch]);

  const openAdd = () => setFormState({ isOpen: true, mode: "add", data: null });
  const openEdit = async (plane) => {
    const result = await dispatch(fetchPlaneById(plane.planeId));
    if (fetchPlaneById.fulfilled.match(result)) {
      setFormState({ isOpen: true, mode: "edit", data: result.payload });
    }
  };

  return (
    <div className="list_container">
      <div className="list_header">
        <div className="header_title_wrapper">
          <h2>Quản lý Máy bay</h2>
          <span className="date_badge">
            <i className="bx bx-calendar" />
            {currentDate()}
          </span>
        </div>
        <button onClick={openAdd} className="btn_add">
          <i className="bx bx-plus" />
          Thêm máy bay
        </button>
      </div>
      <div className="list_toolbar">
        <div className="search_box">
          <i className="bx bx-search search_icon" />
          <input type="text" placeholder="Tìm kiếm theo tên máy bay..." value={searchInput} onChange={(e) => setSearchInput(e.target.value)}/>
        </div>
        <div className="filter_select">
          <select value={status} onChange={(e) => dispatch(setStatus(e.target.value))}>
            <option value="">Tất cả trạng thái</option>
            <option value="Active">Hoạt động</option>
            <option value="Delayed">Bảo trì</option>
            <option value="Inactive">Ngừng hoạt động</option>
          </select>
          <i className="bx bx-chevron-down select_arrow" />
        </div>
      </div>
      {error && (
        <div className="error_alert">
          <i className="bx bx-error-circle" />
          <span>{error}</span>
        </div>
      )}
      <div className="table_card_wrapper">
        <Table
          title="Danh sách máy bay"
          link={{ href: "#", text: `Tổng số máy bay: ${totalCount}` }}
          heads={TABLE_HEADS}
          data={items}
          isLoading={isLoading}
          render={(plane, index) => (
            <tr key={plane.planeId}>
              <td>{(pageIndex - 1) * pageSize + index + 1}</td>
              <td>{plane.planeModel}</td>
              <td>{plane.airlineName}</td>
              <td>
                <span className={`status_dot_wrapper status_${plane.status?.toLowerCase()}`}>
                  <span className="status_dot" />
                  {PLANE_STATUS_LABEL[plane.status] ?? plane.status}
                </span>
              </td>
              <td>{formatDate(plane.createdAt)}</td>
              <td className="action_buttons_group">
                <button onClick={() => openEdit(plane)} title="Sửa" className="btn_action" disabled={isDetailLoading}>
                  <i className="bx bxs-edit" />
                </button>
                <button onClick={() => toggleActive.open(plane)} title="Ngừng hoạt động" className="btn_action">
                  <i className="bx bxs-trash" />
                </button>
              </td>
            </tr>
          )}
        />
      </div>
      {totalPages > 1 && ( <div className="pagination_footer"><Pagination page={pageIndex} total={totalPages} onChange={(p) => dispatch(setPage(p))} /></div>)}
      <PlaneForm key={formState.data?.planeId ?? "new"} isOpen={formState.isOpen} onClose={() => { setFormState((s) => ({ ...s, isOpen: false })); dispatch(clearSelectedItem());}} onSave={refresh} planeData={formState.data} mode={formState.mode}/>
      {toggleActive.target && (
        <div className="modal_overlay animate_fade_in">
          <div className="modal_box animate_slide_up">
            <div className="modal_header">
              <div className="modal_title_icon">
                <i className="bx bx-error" />
              </div>
              <h3>Xác nhận ngừng hoạt động</h3>
            </div>
            <div className="modal_body">
              {toggleActive.error && (
                <div className="error_alert">
                  <i className="bx bx-error-circle" />
                  <span>{toggleActive.error}</span>
                </div>
              )}
              <p>Bạn có chắc chắn muốn chuyển trạng thái máy bay thành <strong>Ngừng hoạt động</strong> không?</p>
              <div className="user_confirm_card">
                <i className="bx bxs-plane-alt" />
                <div>
                  <div className="confirm_username">{toggleActive.target.planeModel}</div>
                  <div className="confirm_email">{toggleActive.target.airlineName}</div>
                </div>
              </div>
            </div>
            <div className="modal_footer">
              <button className="btn_cancel" onClick={toggleActive.close} disabled={toggleActive.isLoading}>
                Hủy
              </button>
              <button className="btn_submit btn_danger_action" disabled={toggleActive.isLoading} onClick={() => toggleActive.confirm((plane) => planeService.delete(plane.planeId))}>
                {toggleActive.isLoading ? "Đang xử lý..." : "Xác nhận"}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

export default PlaneList;
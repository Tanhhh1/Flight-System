import React, { useState, useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import Table from "@/components/table/table";
import Pagination from "@/components/pagination/pagination";
import { reviewService } from "../review_service";
import { fetchReviews, setPage, setSearch, setIsHidden } from "../review_slice";
import { useConfirmAction } from "@/hooks/use_shared_form";
import { useDebounce } from "@/hooks/use_debounce";
import { formatDate, currentDate } from "@/utils/date_utils";
import { TABLE_HEADS, HIDDEN_LABEL } from "@/constants/review";

function ReviewList() {
  const dispatch = useDispatch();
  const { items, pageIndex, pageSize, totalPages, totalCount, error, search, isHidden, isLoading } = useSelector((state) => state.review) || {};

  const [searchInput, setSearchInput] = useState(search);
  const debouncedSearch = useDebounce(searchInput);

  const refresh = () => dispatch(fetchReviews({ pageIndex, pageSize, search, isHidden }));
  const confirmDelete = useConfirmAction({ onSuccess: refresh });

  useEffect(() => { dispatch(fetchReviews({ pageIndex, pageSize, search, isHidden }))}, [dispatch, pageIndex, pageSize, search, isHidden]);
  useEffect(() => { dispatch(setSearch(debouncedSearch))}, [debouncedSearch, dispatch]);

  return (
    <div className="list_container">
      <div className="list_header">
        <div className="header_title_wrapper">
          <h2>Quản lý Đánh giá</h2>
          <span className="date_badge">
            <i className="bx bx-calendar" />
            {currentDate()}
          </span>
        </div>
      </div>

      <div className="list_toolbar">
        <div className="search_box">
          <i className="bx bx-search search_icon" />
          <input type="text" placeholder="Tìm kiếm theo nội dung, tên người dùng..." value={searchInput} onChange={(e) => setSearchInput(e.target.value)}/>
        </div>
        <div className="filter_select">
          <select value={isHidden} onChange={(e) => dispatch(setIsHidden(e.target.value))}>
            <option value="">Tất cả trạng thái</option>
            <option value="false">Hiển thị</option>
            <option value="true">Đã ẩn</option>
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
          title="Danh sách đánh giá"
          link={{ href: "#", text: `Tổng số đánh giá: ${totalCount}` }}
          heads={TABLE_HEADS}
          data={items}
          isLoading={isLoading}
          render={(review, index) => (
            <tr key={review.reviewId}>
              <td>{(pageIndex - 1) * pageSize + index + 1}</td>
              <td>{review.userName}</td>
              <td className="truncate_cell">{review.content}</td>
              <td>{formatDate(review.createdAt)}</td>
              <td>
                <span className={`status_dot_wrapper status_${review.isHidden ? "inactive" : "active"}`}>
                  <span className="status_dot" />
                  {HIDDEN_LABEL[review.isHidden]}
                </span>
              </td>
              <td className="action_buttons_group">
                <button onClick={() => confirmDelete.open(review)} title="Ẩn đánh giá" className="btn_action">
                  <i className="bx bxs-hide" />
                </button>
              </td>
            </tr>
          )}
        />
      </div>
      {totalPages > 1 && ( <div className="pagination_footer"><Pagination page={pageIndex} total={totalPages} onChange={(p) => dispatch(setPage(p))} /></div>)}
      {confirmDelete.target && (
        <div className="modal_overlay animate_fade_in">
          <div className="modal_box animate_slide_up">
            <div className="modal_header">
              <div className="modal_title_icon">
                <i className="bx bx-error" />
              </div>
              <h3>Xác nhận ẩn đánh giá</h3>
            </div>
            <div className="modal_body">
              {confirmDelete.error && (
                <div className="error_alert">
                  <i className="bx bx-error-circle" />
                  <span>{confirmDelete.error}</span>
                </div>
              )}
              <p>Bạn có chắc chắn muốn <strong>Ẩn</strong> bài đánh giá này không?</p>
              <div className="user_confirm_card">
                <i className="bx bx-comment-detail" />
                <div>
                  <div className="confirm_username">{confirmDelete.target.userName}</div>
                  <div className="confirm_email">{formatDate(confirmDelete.target.createdAt)}</div>
                </div>
              </div>
            </div>

            <div className="modal_footer">
              <button className="btn_cancel" onClick={confirmDelete.close} disabled={confirmDelete.isLoading}>
                Hủy
              </button>
              <button className="btn_submit btn_danger_action" disabled={confirmDelete.isLoading} onClick={() => confirmDelete.confirm((review) => reviewService.delete(review.reviewId))}>
                {confirmDelete.isLoading ? "Đang xử lý..." : "Xác nhận"}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

export default ReviewList;
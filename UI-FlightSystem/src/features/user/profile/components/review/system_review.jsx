import { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { fetchMyReviews, setPage } from "../../review_slice";
import { useReviewForm } from "../../use_review_form";
import AlertModal from "@/components/error/alert_modal";
import FieldError from "@/components/error/field_error";
import Pagination from "@/components/pagination/pagination";
import { formatDate } from "@/utils/date_utils";
import "./system_review.css";

function SystemReview() {
    const dispatch = useDispatch();
    const { items, isLoading, pageIndex, totalPages } = useSelector((s) => s.myReviews);
    const [alert, setAlert] = useState({ type: "", message: "" });
    useEffect(() => { dispatch(fetchMyReviews({ pageIndex, pageSize: 5 })) }, [dispatch, pageIndex]);
    const refetch = () => dispatch(fetchMyReviews({ pageIndex, pageSize: 5 }));
    const { register, formState: { errors, isSubmitting }, onSubmit } = useReviewForm({
        onSuccess: (msg) => {
            setAlert({ type: "success", message: msg });
            refetch();
        }
    });
    const handleClose = () => setAlert({ type: "", message: "" });
    return (
        <>
            {alert.message && (
                <AlertModal type={alert.type} message={alert.message} onClose={handleClose} />
            )}
            <div className="system_review_wrapper">
                <form onSubmit={onSubmit} className="setting_card_panel">
                    <div className="panel_header">
                        <h4>Đánh giá & Góp ý hệ thống</h4>
                    </div>
                    <div className="panel_body">
                        <p className="review_intro_text">
                            Trải nghiệm của bạn là ưu tiên hàng đầu của SkyJourney. Hãy chia sẻ cảm nhận hoặc đóng góp ý kiến để chúng tôi không ngừng cải tiến hệ thống và dịch vụ!
                        </p>
                        <div className="form_group_full">
                            <label className="form_label">Nội dung góp ý chi tiết</label>
                            <textarea {...register("content")} className="form_textarea" placeholder="Hãy viết cảm nhận, lỗi hệ thống gặp phải hoặc đề xuất tính năng mới mà bạn mong muốn có..." rows="5" />
                            <FieldError error={errors.content} />
                            <FieldError error={errors.root} />
                        </div>
                        <div className="form_actions_footer">
                            <button type="submit" className="btn_primary_save" disabled={isSubmitting}>
                                <i className="bx bx-paper-plane"></i>
                                {isSubmitting ? " Đang gửi..." : " Gửi đánh giá"}
                            </button>
                        </div>
                    </div>
                </form>

                <div className="setting_card_panel">
                    <div className="panel_header">
                        <h4>Lịch sử đóng góp ý kiến</h4>
                    </div>
                    <div className="panel_body">
                        {!isLoading && items.length === 0 && (
                            <p className="review_empty">Bạn chưa có đánh giá nào.</p>
                        )}
                        {items.map((review) => (
                            <div key={review.reviewId} className="review_history_item">
                                <div className="item_header">
                                    <span className="item_date">
                                        <i className="bx bx-time-five"></i> {formatDate(review.createdAt)}
                                    </span>
                                    {review.isHidden ? (
                                        <div className="item_status warning">
                                            <i className="bx bx-hide"></i> Đã bị ẩn
                                        </div>
                                    ) : (
                                        <div className="item_status success">
                                            <i className="bx bx-check-circle"></i> Hệ thống đã ghi nhận
                                        </div>
                                    )}
                                </div>
                                <p className="item_content">{review.content}</p>
                            </div>
                        ))}
                        {totalPages > 1 && (
                            <Pagination page={pageIndex} total={totalPages} onChange={(p) => dispatch(setPage(p))} />
                        )}
                    </div>
                </div>
            </div>
        </>
    );
};

export default SystemReview;
import "./review_list.css";

function ReviewList({ reviews }) {
  return (
    <div className="reviews_grid">
      {reviews.map((rev) => (
        <div className="review_card" key={rev.reviewId}>
          <p className="review_comment">"{rev.content}"</p>
          <div className="review_user_info">
            <div>
              <h4 className="review_name">{rev.userName}</h4>
              <span className="review_role">{rev.userEmail}</span>
            </div>
          </div>
        </div>
      ))}
    </div>
  );
}

export default ReviewList;
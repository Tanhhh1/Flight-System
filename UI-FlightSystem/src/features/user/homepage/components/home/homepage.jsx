import { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { fetchHomeReviews, fetchHomeAirports } from "../../homepage_slice";
import { FEATURES } from "@/constants/homepage";
import SearchForm from "../search/search_form";
import ReviewList from "../review/review_list";
import "./homepage.css";

function Homepage() {
  const dispatch = useDispatch();
  const { reviews, airports } = useSelector((state) => state.homepage);

  useEffect(() => {
    dispatch(fetchHomeReviews());
    dispatch(fetchHomeAirports());
  }, [dispatch]);

  return (
    <div className="home_container">
      <section className="hero_section">
        <div className="hero_overlay"></div>
        <div className="hero_content">
          <h1 className="hero_title">Khám Phá Thế Giới Theo Cách Của Bạn</h1>
          <p className="hero_subtitle">Tìm kiếm vé máy bay giá rẻ, trải nghiệm chuyến đi an toàn cùng SkyJourney</p>
          <SearchForm airports={airports} />
        </div>
      </section>

      <section className="section features_section">
        <div className="features_inner_container">
          <h2 className="section_title text_center">Tại sao nên đặt vé tại SkyJourney?</h2>
          <div className="features_grid">
            {FEATURES.map((feat, index) => (
              <div className="feature_card" key={index}>
                <div className="feature_icon_box">
                  <i className={feat.icon}></i>
                </div>
                <h3 className="feature_card_title">{feat.title}</h3>
                <p className="feature_card_desc">{feat.desc}</p>
              </div>
            ))}
          </div>
        </div>
      </section>

      <section className="section reviews_section">
        <div className="section_header text_center_wrapper">
          <h2 className="section_title text_center">Đánh giá từ người dùng</h2>
          <p className="section_subtitle text_center">Những chia sẻ chân thực từ các hành khách đã trải nghiệm dịch vụ</p>
        </div>
        <ReviewList reviews={reviews} />
      </section>
    </div>
  );
}

export default Homepage;
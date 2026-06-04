import React from "react";
import "./pagination.css";

function Pagination({ page, total, onChange }) {
    const pages = [];

    let start = Math.max(1, page - 2);
    let end = Math.min(total, page + 2);

    if (end - start < 4) {
        if (start === 1) {
            end = Math.min(total, 5);
        } else {
            start = Math.max(1, end - 4);
        }
    }

    if (start > 1) {
        pages.push(1);
        if (start > 2) {
            pages.push("...");
        }
    }

    for (let i = start; i <= end; i++) {
        pages.push(i);
    }

    if (end < total) {
        if (end < total - 1) {
            pages.push("...");
        }
        pages.push(total);
    }

    return (
        <div className="pagination">
            <button className="pagination_btn prev" onClick={() => onChange(page - 1)} disabled={page === 1} type="button">
                <i className="bx bx-chevron-left"></i>
            </button>

            {pages.map((p, index) => (
                <button key={index} className={`pagination_btn ${p === page ? "active" : ""} ${p === "..." ? "ellipsis" : ""}`} onClick={() => typeof p === "number" && onChange(p)} disabled={p === "..."} type="button">
                    {p}
                </button>
            ))}

            <button className="pagination_btn next" onClick={() => onChange(page + 1)} disabled={page === total} type="button">
                <i className="bx bx-chevron-right"></i>
            </button>
        </div>
    );
}

export default Pagination;
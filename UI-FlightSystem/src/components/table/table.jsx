import React from "react";
import { Link } from "react-router-dom";


function Table({ heads, data, render, title, link }) {
    return (
        <div className="content_table">
            {(title || link) && (
                <div className="table_header">
                    <div className="table_title">
                        {title && <h3>{title}</h3>}
                    </div>
                    <div className="table_all">
                        {link && link.href === "#" ? (<span>{link.text}</span>) : (link && (<Link to={link.href}>{link.text}</Link>))}
                    </div>
                </div>
            )}
            <div className="table_responsive">
                <table>
                    <thead>
                        <tr>{heads.map((header, index) => (<th key={index}>{header}</th>))}</tr>
                    </thead>
                    <tbody>
                        {data && data.length > 0 ? (data.map((item, index) => render(item, index))) : (
                            <tr>
                                <td colSpan={heads.length} className="no_data">
                                    Không có dữ liệu hiển thị.
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>
        </div>
    );
}

export default Table;
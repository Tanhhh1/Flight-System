import React, { useState } from "react";
import { useDispatch } from "react-redux";
import Table from "@/components/common/table";
import Pagination from "@/components/common/pagination";
import AlertModal from "@/components/common/alert_modal";
import { formatDate, currentDate } from "@/utils/date_utils";
import { ROLE_LABEL, TABLE_HEADS } from "../account_constants";
import { fetchAccounts, fetchAccountById, setPage, setSearch, setRoleName, clearSelectedItem } from "../account_slice";
import { accountService } from "../account_service";
import AccountForm from "./account_form";

import { useDataList } from "@/hooks/use_data_list";
import { useSearchFilter } from "@/hooks/use_search_filter";
import { useModalForm } from "@/hooks/use_modal_form";
import { useDetailLoader } from "@/hooks/use_detail_loader";
import { useDeleteConfirm } from "@/hooks/use_delete_confirm";

function AccountList() {
    const dispatch = useDispatch();
    const [alertState, setAlertState] = useState({ isOpen: false, message: "", type: "error" });

    const { items, pageIndex, pageSize, totalPages, totalCount, error, isLoading, isDetailLoading, refresh 
    } = useDataList({
        sliceName: "account",
        fetchListThunk: fetchAccounts,
        selectExtraFilters: (state) => ({ roleName: state.roleName }),
    });

    const { searchInput, setSearchInput, handleFilterChange } = useSearchFilter({
        sliceName: "account",
        setSearchAction: setSearch,
        setFilterAction: setRoleName,
    });

    const { formState, openAdd, openEdit, closeModal } = useModalForm({ 
        clearSelectedItemAction: clearSelectedItem 
    });

    const { loadDetail } = useDetailLoader({
        fetchByIdThunk: fetchAccountById,
        onFulfilled: (data) => openEdit(data),
    });

    const toggleActive = useDeleteConfirm({
        onSuccess: refresh,
        deleteServiceFn: accountService.delete,
    });

    const handleEditClick = (account) => {
        if (account.roles?.[0] === "user") {
            setAlertState({ 
                isOpen: true, 
                type: "error", 
                message: "Không thể chỉnh sửa tài khoản người dùng thông thường" 
            });
            return;
        }
        loadDetail(account.userId);
    };

    return (
        <div className="list_container">
            <header className="list_header">
                <div className="header_title_wrapper">
                    <h2>Quản lý Tài khoản</h2>
                    <span className="date_badge">
                        <i className="bx bx-calendar-alt" /> {currentDate()}
                    </span>
                </div>
                <button className="btn_add" onClick={openAdd}>
                    <i className="bx bx-plus-circle" /> Thêm tài khoản mới
                </button>
            </header>

            <div className="list_toolbar">
                <div className="search_box">
                    <i className="bx bx-search search_icon" />
                    <input 
                        type="text" 
                        placeholder="Tìm kiếm theo họ tên, tên đăng nhập..." 
                        value={searchInput} 
                        onChange={(e) => setSearchInput(e.target.value)} 
                    />
                </div>

                <div className="filter_select">
                    <select onChange={(e) => handleFilterChange(e.target.value)}>
                        <option value="">Tất cả vai trò</option>
                        <option value="admin">Quản trị viên</option>
                        <option value="staff">Nhân viên</option>
                        <option value="user">Người dùng</option>
                    </select>
                    <i className="bx bx-chevron-down select_arrow" />
                </div>
            </div>

            {error && (
                <div className="error_alert animate_fade_in">
                    <i className="bx bx-error-circle" />
                    <span>{error}</span>
                </div>
            )}

            <main className="table_card_wrapper">
                <Table
                    title="Danh sách thành viên"
                    link={{ href: "#", text: `Tổng số tài khoản: ${totalCount}` }}
                    heads={TABLE_HEADS}
                    data={items}
                    isLoading={isLoading}
                    render={(account, index) => (
                        <tr key={account.userId} className="table_row_hover">
                            <td className="text_center text_muted">{(pageIndex - 1) * pageSize + index + 1}</td>
                            <td className="font_medium">{account.userName}</td>
                            <td className="text_muted">{account.email}</td>
                            <td>{account.fullname}</td>
                            <td><span className={`role_badge role_${account.roles?.[0]}`}>{ROLE_LABEL[account.roles?.[0]] ?? "Chưa rõ"}</span></td>
                            <td>
                                <span className={`status_dot_wrapper status_${account.isActive ? "active" : "inactive"}`}>
                                    <span className="status_dot" />
                                    {account.isActive ? "Hoạt động" : "Bị khóa"}
                                </span>
                            </td>
                            <td className="text_muted font_small">{formatDate(account.createdAt)}</td>
                            <td>
                                <div className="action_buttons_group">
                                    <button className="btn_action btn_edit" title="Chỉnh sửa" disabled={isDetailLoading} onClick={() => handleEditClick(account)}>
                                        <i className="bx bxs-edit-alt" />
                                    </button>
                                    <button className="btn_action" title={account.isActive ? "Khóa tài khoản" : "Mở khóa tài khoản"} onClick={() => toggleActive.open(account)}>
                                        <i className={`bx ${account.isActive ? "bxs-lock" : "bxs-lock-open"}`} />
                                    </button>
                                </div>
                            </td>
                        </tr>
                    )}
                />
            </main>

            {totalPages > 1 && (
                <footer className="pagination_footer">
                    <Pagination page={pageIndex} total={totalPages} onChange={(page) => dispatch(setPage(page))} />
                </footer>
            )}

            <AccountForm
                key={formState.data?.userId ?? "new"}
                isOpen={formState.isOpen}
                mode={formState.mode}
                accountData={formState.data}
                onSave={refresh}
                onClose={closeModal}
            />

            {toggleActive.target && (
                <div className="modal_overlay">
                    <div className="modal_box">
                        <div className="modal_header">
                            <div className="modal_title_icon"><i className="bx bx-shield-quarter" /></div>
                            <h3>Xác nhận thay đổi</h3>
                        </div>

                        <div className="modal_body">
                            {toggleActive.error && (
                                <div className="error_alert">
                                    <i className="bx bx-error-circle" />
                                    <span>{toggleActive.error}</span>
                                </div>
                            )}
                            <p>Bạn có chắc chắn muốn <strong>{toggleActive.target.isActive ? "Khóa" : "Mở khóa"}</strong> trạng thái hoạt động của tài khoản:</p>
                            <div className="user_confirm_card">
                                <i className="bx bx-user" />
                                <div>
                                    <div className="confirm_username">{toggleActive.target.userName}</div>
                                    <div className="confirm_email">{toggleActive.target.email}</div>
                                </div>
                            </div>
                        </div>

                        <div className="modal_footer">
                            <button className="btn_cancel" disabled={toggleActive.isLoading} onClick={toggleActive.close}>Hủy bỏ</button>
                            <button className="btn_submit btn_danger_action" disabled={toggleActive.isLoading} onClick={() => toggleActive.confirm((item) => item.userId)}>
                                {toggleActive.isLoading ? "Đang xử lý..." : "Xác nhận"}
                            </button>
                        </div>
                    </div>
                </div>
            )}

            {alertState.isOpen && (
                <AlertModal 
                    type={alertState.type} 
                    message={alertState.message} 
                    onClose={() => setAlertState({ isOpen: false, message: "", type: "error" })}
                />
            )}
        </div>
    );
}

export default AccountList;
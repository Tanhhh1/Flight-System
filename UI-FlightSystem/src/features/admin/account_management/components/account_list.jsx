import React, { useState, useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import Table from "@/components/table/table";
import Pagination from "@/components/pagination/pagination";
import AccountForm from "./account_form";
import { accountService } from "../account_service";
import { useConfirmAction } from "@/hooks/use_shared_form";
import { fetchAccounts, fetchAccountById, setPage, setSearch, setRoleName, clearSelectedItem } from "../account_slice";
import { useDebounce } from "@/hooks/use_debounce";
import { formatDate, currentDate } from "@/utils/date_utils";
import { ROLE_LABEL, TABLE_HEADS } from "@/constants/account";
import AlertModal from "@/components/error/alert_modal";

function AccountList() {
    const dispatch = useDispatch();
    const { items, pageIndex, pageSize, totalPages, totalCount, error, search, roleName, isDetailLoading, isLoading } = useSelector((state) => state.account);
    const [searchInput, setSearchInput] = useState(search);
    const [formState, setFormState] = useState({ isOpen: false, mode: "add", data: null });
    const [alertState, setAlertState] = useState({ isOpen: false, message: "", type: "success" });
    const debouncedSearch = useDebounce(searchInput);
    const refresh = () => dispatch(fetchAccounts({ pageIndex, pageSize, search, roleName }));
    const toggleActive = useConfirmAction({ onSuccess: refresh });

    useEffect(() => { dispatch(fetchAccounts({ pageIndex, pageSize, search, roleName })); }, [dispatch, pageIndex, pageSize, search, roleName]);
    useEffect(() => { dispatch(setSearch(debouncedSearch)); }, [debouncedSearch, dispatch]);

    const openAdd = () => setFormState({ isOpen: true, mode: "add", data: null });

    const openEdit = async (account) => {
        if (account.roles?.[0] === "user") {
            setAlertState({
                isOpen: true,
                message: "Không thể chỉnh sửa tài khoản người dùng thông thường",
                type: "error"
            });
            return;
        }
        const result = await dispatch(fetchAccountById(account.userId));
        if (fetchAccountById.fulfilled.match(result)) {
            setFormState({ isOpen: true, mode: "edit", data: result.payload });
        }
    };

    return (
        <div className="list_container">
            <header className="list_header">
                <div className="header_title_wrapper">
                    <h2>Quản lý Tài khoản</h2>
                    <span className="date_badge"><i className="bx bx-calendar-alt" /> {currentDate()}</span>
                </div>
                <button onClick={openAdd} className="btn_add">
                    <i className="bx bx-plus-circle" /> Thêm tài khoản mới
                </button>
            </header>

            <div className="list_toolbar">
                <div className="search_box">
                    <i className="bx bx-search search_icon" />
                    <input type="text" placeholder="Tìm kiếm theo họ tên, tên đăng nhập..." value={searchInput} onChange={(e) => setSearchInput(e.target.value)} />
                </div>
                <div className="filter_select">
                    <select value={roleName} onChange={(e) => dispatch(setRoleName(e.target.value))}>
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
                            <td>
                                <span className={`role_badge role_${account.roles?.[0]}`}>
                                    {ROLE_LABEL[account.roles?.[0]] || "Chưa rõ"}
                                </span>
                            </td>
                            <td>
                                <span className={`status_dot_wrapper status_${account.isActive ? "active" : "inactive"}`}>
                                    <span className="status_dot"></span>
                                    {account.isActive ? "Hoạt động" : "Bị khóa"}
                                </span>
                            </td>
                            <td className="text_muted font_small">{formatDate(account.createdAt)}</td>
                            <td>
                                <div className="action_buttons_group">
                                    <button onClick={() => openEdit(account)} className="btn_action btn_edit" title="Chỉnh sửa" disabled={isDetailLoading}>
                                        <i className="bx bxs-edit-alt" />
                                    </button>
                                    <button onClick={() => toggleActive.open(account)} className="btn_action">
                                        <i className={`bx ${account.isActive ? "bxs-lock" : "bxs-lock-open"}`} />
                                    </button>
                                </div>
                            </td>
                        </tr>
                    )}
                />
            </main>
            {totalPages > 1 && (<footer className="pagination_footer"><Pagination page={pageIndex} total={totalPages} onChange={(p) => dispatch(setPage(p))} /></footer>)}
            <AccountForm key={formState.data?.userId ?? "new"} isOpen={formState.isOpen} onClose={() => { setFormState((s) => ({ ...s, isOpen: false })); dispatch(clearSelectedItem()) }} onSave={refresh} accountData={formState.data} mode={formState.mode} />
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
                            <p>Bạn có chắc chắn muốn <strong>{toggleActive.target.isActive ? "Khóa" : "Mở Khóa"}</strong> hành trình hoạt động của tài khoản:</p>
                            <div className="user_confirm_card">
                                <i className="bx bx-user" />
                                <div>
                                    <div className="confirm_username">{toggleActive.target.userName}</div>
                                    <div className="confirm_email">{toggleActive.target.email}</div>
                                </div>
                            </div>
                        </div>
                        <div className="modal_footer">
                            <button className="btn_cancel" onClick={toggleActive.close} disabled={toggleActive.isLoading}>Hủy bỏ</button>
                            <button className="btn_submit btn_danger_action" disabled={toggleActive.isLoading}
                                onClick={() => toggleActive.confirm((account) => accountService.delete(account.userId))}>
                                {toggleActive.isLoading ? "Đang xử lý..." : "Xác nhận"}
                            </button>
                        </div>
                    </div>
                </div>
            )}
            {alertState.isOpen && (
                <AlertModal type={alertState.type} message={alertState.message} onClose={() => setAlertState({ isOpen: false, message: "", type: "success" })}/>
            )}
        </div>
    );
}

export default AccountList;
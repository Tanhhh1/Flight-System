import { useState, useCallback } from "react";
import { useDispatch } from "react-redux";

export function useModalForm({ clearSelectedItemAction } = {}) {
    const dispatch = useDispatch();
    const [formState, setFormState] = useState({ isOpen: false, mode: "add", data: null });

    const openAdd = useCallback(() => {setFormState({ isOpen: true, mode: "add", data: null })}, []);
    const openEdit = useCallback((data) => {setFormState({ isOpen: true, mode: "edit", data })}, []);

    const closeModal = useCallback(() => {
        setFormState((prev) => ({ ...prev, isOpen: false }));
        if (clearSelectedItemAction) { dispatch(clearSelectedItemAction()) }
    }, [dispatch, clearSelectedItemAction]);

    return { formState, setFormState, openAdd, openEdit, closeModal };
}
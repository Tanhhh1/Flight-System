import { useState, useEffect, useRef } from "react";
import { createPortal } from "react-dom";

function SearchableSelect({
    data = [], value = "", onChange, placeholder = "",
    className = "", itemKey = "id", displayValue = "name",
    searchFields = ["name"], renderItem
}) {
    const [isOpen, setIsOpen] = useState(false);
    const [searchText, setSearchText] = useState("");
    const containerRef = useRef(null);
    const [dropdownStyle, setDropdownStyle] = useState({});

    useEffect(() => {
        const selectedItem = data.find(item => item[itemKey] === value);
        if (selectedItem) {
            setSearchText(typeof displayValue === "function" ? displayValue(selectedItem) : selectedItem[displayValue]);
        } else {
            setSearchText("");
        }
    }, [value, data, itemKey, displayValue]);

    const updateDropdownPosition = () => {
        if (containerRef.current) {
            const rect = containerRef.current.getBoundingClientRect();
            setDropdownStyle({
                position: "fixed",
                top: `${rect.bottom + 4}px`,
                left: `${rect.left}px`,
                width: `${rect.width}px`,
                zIndex: 999999
            });
        }
    };

    useEffect(() => {
        function handleClickOutside(event) {
            if (containerRef.current && !containerRef.current.contains(event.target)) {
                const dropdownEl = document.querySelector(".searchable_select_dropdown");
                if (dropdownEl && dropdownEl.contains(event.target)) return;
                setIsOpen(false);
            }
        }

        if (isOpen) {
            updateDropdownPosition();
            window.addEventListener("scroll", updateDropdownPosition, true);
            window.addEventListener("resize", updateDropdownPosition);
        }

        document.addEventListener("mousedown", handleClickOutside);
        return () => {
            document.removeEventListener("mousedown", handleClickOutside);
            window.removeEventListener("scroll", updateDropdownPosition, true);
            window.removeEventListener("resize", updateDropdownPosition);
        };
    }, [isOpen]);

    const filteredData = data.filter(item =>
        searchFields.some(field => {
            const val = item[field];
            return val && val.toString().toLowerCase().includes(searchText.toLowerCase());
        })
    );

    const handleSelect = (item) => {
        onChange(item[itemKey]);
        setSearchText(typeof displayValue === "function" ? displayValue(item) : item[displayValue]);
        setIsOpen(false);
    };

    return (
        <div className={`searchable_select_container ${className}`} ref={containerRef}>
            <div className="searchable_select_inner">
                <input type="text" className="searchable_select_input" placeholder={placeholder} value={searchText}
                    onChange={(e) => { setSearchText(e.target.value); setIsOpen(true); updateDropdownPosition(); if (!e.target.value) onChange(""); }}
                    onFocus={() => { setIsOpen(true); updateDropdownPosition(); }}/>
            </div>

            {isOpen && createPortal(
                <div className="searchable_select_dropdown" style={dropdownStyle}>
                    {filteredData.length > 0 ? (
                        filteredData.map((item) => (
                            <div key={item[itemKey]} className={`searchable_select_item ${className}`} onClick={() => handleSelect(item)}>
                                {renderItem ? renderItem(item) : (
                                    <span className="searchable_select_text">{item[displayValue]}</span>
                                )}
                            </div>
                        ))
                    ) : (
                        <div className="searchable_select_no_options">Không tìm thấy kết quả</div>
                    )}
                </div>,
                document.body
            )}
        </div>
    );
}

export default SearchableSelect;
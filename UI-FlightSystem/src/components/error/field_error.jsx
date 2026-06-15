import React from "react";
import "./error.css";

export default function FieldError({ error }) {
  if (!error) return null;
  return (
    <span className="field_error_msg">
      <i className="bx bx-error-circle" /> {error.message}
    </span>
  );
}
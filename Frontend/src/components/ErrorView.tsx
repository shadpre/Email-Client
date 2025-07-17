import React from "react";

/**
 * Props for the ErrorView component
 */
interface ErrorViewProps {
  /** Error message to display */
  message: string;
  /** Callback function when retry button is clicked */
  onRetry?: () => void;
  /** Custom retry button text */
  retryButtonText?: string;
}

/**
 * Component that displays error messages with an optional retry button.
 * Used when operations fail and user intervention is needed.
 */
export const ErrorView: React.FC<ErrorViewProps> = ({
  message,
  onRetry,
  retryButtonText = "Retry",
}) => {
  return (
    <div
      style={{
        textAlign: "center",
        padding: "50px 20px",
        backgroundColor: "#fff5f5",
        borderRadius: "8px",
        border: "1px solid #fed7d7",
        margin: "20px 0",
      }}
    >
      {/* Error icon */}
      <div
        style={{
          fontSize: "48px",
          color: "#e53e3e",
          marginBottom: "20px",
        }}
      >
        ⚠️
      </div>

      {/* Error message */}
      <p
        style={{
          color: "#e53e3e",
          marginBottom: "30px",
          fontSize: "16px",
          lineHeight: "1.5",
          maxWidth: "500px",
          margin: "0 auto 30px auto",
        }}
      >
        {message}
      </p>

      {/* Retry button */}
      {onRetry && (
        <button
          onClick={onRetry}
          style={{
            padding: "12px 24px",
            backgroundColor: "#28a745",
            color: "white",
            border: "none",
            borderRadius: "6px",
            cursor: "pointer",
            fontSize: "14px",
            fontWeight: "500",
            transition: "background-color 0.2s ease",
          }}
          onMouseEnter={(e) => {
            e.currentTarget.style.backgroundColor = "#218838";
          }}
          onMouseLeave={(e) => {
            e.currentTarget.style.backgroundColor = "#28a745";
          }}
        >
          {retryButtonText}
        </button>
      )}
    </div>
  );
};

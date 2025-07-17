import React from "react";

/**
 * Props for the StatusMessage component
 */
interface StatusMessageProps {
  /** The status message to display */
  message: string;
  /** The type of message that determines styling */
  type: "info" | "success" | "error" | "warning";
  /** Optional callback when message is dismissed */
  onDismiss?: () => void;
}

/**
 * Component that displays status messages with appropriate styling based on message type.
 * Supports info, success, error, and warning message types with optional dismiss functionality.
 */
export const StatusMessage: React.FC<StatusMessageProps> = ({
  message,
  type,
  onDismiss,
}) => {
  // Define colors and styles based on message type
  const getMessageStyles = () => {
    const baseStyles = {
      padding: "12px 16px",
      borderRadius: "4px",
      margin: "10px 0",
      border: "1px solid",
      fontSize: "14px",
      lineHeight: "1.4",
      position: "relative" as const,
      display: "flex",
      alignItems: "center",
      justifyContent: "space-between",
    };

    switch (type) {
      case "success":
        return {
          ...baseStyles,
          backgroundColor: "#d4edda",
          borderColor: "#c3e6cb",
          color: "#155724",
        };
      case "error":
        return {
          ...baseStyles,
          backgroundColor: "#f8d7da",
          borderColor: "#f5c6cb",
          color: "#721c24",
        };
      case "warning":
        return {
          ...baseStyles,
          backgroundColor: "#fff3cd",
          borderColor: "#ffeaa7",
          color: "#856404",
        };
      case "info":
      default:
        return {
          ...baseStyles,
          backgroundColor: "#d1ecf1",
          borderColor: "#bee5eb",
          color: "#0c5460",
        };
    }
  };

  // Get appropriate icon based on message type
  const getIcon = () => {
    switch (type) {
      case "success":
        return "✓";
      case "error":
        return "✗";
      case "warning":
        return "⚠";
      case "info":
      default:
        return "ℹ";
    }
  };

  if (!message.trim()) {
    return null; // Don't render empty messages
  }

  return (
    <div style={getMessageStyles()}>
      <div style={{ display: "flex", alignItems: "center" }}>
        {/* Icon */}
        <span
          style={{
            marginRight: "8px",
            fontWeight: "bold",
            fontSize: "16px",
          }}
        >
          {getIcon()}
        </span>

        {/* Message text */}
        <span>{message}</span>
      </div>

      {/* Dismiss button */}
      {onDismiss && (
        <button
          onClick={onDismiss}
          style={{
            background: "none",
            border: "none",
            fontSize: "18px",
            cursor: "pointer",
            color: "inherit",
            padding: "0",
            marginLeft: "10px",
            opacity: 0.7,
            transition: "opacity 0.2s ease",
          }}
          onMouseEnter={(e) => {
            e.currentTarget.style.opacity = "1";
          }}
          onMouseLeave={(e) => {
            e.currentTarget.style.opacity = "0.7";
          }}
          title="Dismiss message"
        >
          ×
        </button>
      )}
    </div>
  );
};

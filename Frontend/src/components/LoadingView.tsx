import React from "react";

/**
 * Props for the LoadingView component
 */
interface LoadingViewProps {
  /** Optional loading message to display */
  message?: string;
  /** Optional additional content to show below the loading message */
  children?: React.ReactNode;
}

/**
 * Component that displays a loading state with optional custom content.
 * Used when data is being fetched or operations are in progress.
 */
export const LoadingView: React.FC<LoadingViewProps> = ({
  message = "Loading...",
  children,
}) => {
  return (
    <div
      style={{
        textAlign: "center",
        padding: "50px 20px",
        backgroundColor: "#f8f9fa",
        borderRadius: "8px",
        margin: "20px 0",
      }}
    >
      {/* Loading spinner */}
      <div
        style={{
          width: "40px",
          height: "40px",
          border: "4px solid #e3e3e3",
          borderTop: "4px solid #007bff",
          borderRadius: "50%",
          animation: "spin 1s linear infinite",
          margin: "0 auto 20px auto",
        }}
      />

      {/* Loading message */}
      <p
        style={{
          fontSize: "16px",
          color: "#495057",
          margin: "0 0 20px 0",
          fontWeight: "500",
        }}
      >
        {message}
      </p>

      {/* Additional content */}
      {children}

      {/* Add CSS animation for spinner */}
      <style>
        {`
          @keyframes spin {
            0% { transform: rotate(0deg); }
            100% { transform: rotate(360deg); }
          }
        `}
      </style>
    </div>
  );
};

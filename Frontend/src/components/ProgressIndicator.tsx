import React from "react";
import { ProcessingStatus } from "../types";

/**
 * Props for the ProgressIndicator component
 */
interface ProgressIndicatorProps {
  /** Current processing status with progress information */
  status: ProcessingStatus;
}

/**
 * Component that displays a progress bar and status information for long-running operations.
 * Shows batch progress, email counts, and completion percentage with visual indicators.
 */
export const ProgressIndicator: React.FC<ProgressIndicatorProps> = ({
  status,
}) => {
  if (!status.isProcessing) {
    return null; // Don't render anything if not processing
  }

  return (
    <div style={{ marginTop: "20px" }}>
      {/* Main progress information */}
      <p>
        <strong>
          Processing {status.totalEmails.toLocaleString()} emails...
        </strong>
      </p>

      {/* Batch progress */}
      <p>
        Batch {status.currentBatch} of {status.totalBatches}
      </p>

      {/* Visual progress bar */}
      <div
        style={{
          width: "300px",
          height: "20px",
          backgroundColor: "#e0e0e0",
          borderRadius: "10px",
          margin: "10px auto",
          overflow: "hidden",
        }}
      >
        <div
          style={{
            width: `${status.progressPercentage}%`,
            height: "100%",
            backgroundColor: "#28a745",
            borderRadius: "10px",
            transition: "width 0.3s ease",
          }}
        />
      </div>

      {/* Percentage and email count */}
      <p>{status.progressPercentage.toFixed(1)}% complete</p>
      <p style={{ fontSize: "12px", color: "#666" }}>
        Processed {status.processedEmails.toLocaleString()} emails
      </p>

      {/* Current operation status */}
      {status.currentOperation && (
        <p style={{ fontSize: "14px", fontStyle: "italic", color: "#555" }}>
          {status.currentOperation}
        </p>
      )}
    </div>
  );
};

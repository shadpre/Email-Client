import React from "react";
import { SenderGroup } from "../types";

/**
 * Props for the SenderGroupList component
 */
interface SenderGroupListProps {
  /** Array of sender groups to display */
  senderGroups: SenderGroup[];
  /** Callback function called when delete button is clicked for a sender */
  onDeleteSender: (senderEmail: string) => void;
  /** Whether a delete operation is currently in progress */
  isDeleting: boolean;
}

/**
 * Component that displays a list of email sender groups with delete functionality.
 * Shows sender email, email count, and provides delete button for each sender.
 */
export const SenderGroupList: React.FC<SenderGroupListProps> = ({
  senderGroups,
  onDeleteSender,
  isDeleting,
}) => {
  if (senderGroups.length === 0) {
    return (
      <div style={{ marginTop: "20px", textAlign: "center", color: "#666" }}>
        <p>No emails found. Connect to your email account to see senders.</p>
      </div>
    );
  }

  return (
    <div style={{ marginTop: "20px" }}>
      <h3>Email Senders ({senderGroups.length} unique senders)</h3>

      {/* Table header */}
      <div
        style={{
          display: "flex",
          fontWeight: "bold",
          borderBottom: "2px solid #ccc",
          padding: "10px 0",
          backgroundColor: "#f8f9fa",
        }}
      >
        <div style={{ flex: 2, paddingLeft: "10px" }}>Sender Email</div>
        <div style={{ flex: 1, textAlign: "center" }}>Email Count</div>
        <div style={{ flex: 1, textAlign: "center" }}>Actions</div>
      </div>

      {/* Sender list */}
      <div
        style={{
          maxHeight: "400px",
          overflowY: "auto",
          border: "1px solid #ddd",
        }}
      >
        {senderGroups.map((group, index) => (
          <div
            key={group.senderEmail}
            style={{
              display: "flex",
              alignItems: "center",
              padding: "12px 0",
              borderBottom:
                index < senderGroups.length - 1 ? "1px solid #eee" : "none",
              backgroundColor: index % 2 === 0 ? "#ffffff" : "#f8f9fa",
              transition: "background-color 0.2s ease",
            }}
            onMouseEnter={(e) => {
              e.currentTarget.style.backgroundColor = "#e9ecef";
            }}
            onMouseLeave={(e) => {
              e.currentTarget.style.backgroundColor =
                index % 2 === 0 ? "#ffffff" : "#f8f9fa";
            }}
          >
            {/* Sender email */}
            <div
              style={{
                flex: 2,
                paddingLeft: "10px",
                wordBreak: "break-word",
                fontSize: "14px",
              }}
            >
              {group.senderEmail}
            </div>

            {/* Email count */}
            <div
              style={{
                flex: 1,
                textAlign: "center",
                fontWeight: "500",
                color: "#495057",
              }}
            >
              {group.emailCount.toLocaleString()}
            </div>

            {/* Delete button */}
            <div style={{ flex: 1, textAlign: "center" }}>
              <button
                onClick={() => onDeleteSender(group.senderEmail)}
                disabled={isDeleting}
                style={{
                  backgroundColor: isDeleting ? "#6c757d" : "#dc3545",
                  color: "white",
                  border: "none",
                  padding: "6px 12px",
                  borderRadius: "4px",
                  cursor: isDeleting ? "not-allowed" : "pointer",
                  fontSize: "12px",
                  transition: "background-color 0.2s ease",
                }}
                onMouseEnter={(e) => {
                  if (!isDeleting) {
                    e.currentTarget.style.backgroundColor = "#c82333";
                  }
                }}
                onMouseLeave={(e) => {
                  if (!isDeleting) {
                    e.currentTarget.style.backgroundColor = "#dc3545";
                  }
                }}
              >
                {isDeleting ? "Deleting..." : "Delete All"}
              </button>
            </div>
          </div>
        ))}
      </div>

      {/* Summary information */}
      <div
        style={{
          marginTop: "10px",
          fontSize: "12px",
          color: "#666",
          textAlign: "center",
        }}
      >
        Total emails:{" "}
        {senderGroups
          .reduce((sum, group) => sum + group.emailCount, 0)
          .toLocaleString()}
      </div>
    </div>
  );
};

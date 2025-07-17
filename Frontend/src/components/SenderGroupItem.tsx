import React from "react";
import { SenderGroup } from "../types";

/**
 * Props for the SenderGroupItem component
 */
interface SenderGroupItemProps {
  /** The sender group data to display */
  group: SenderGroup;
  /** Whether this group is currently expanded */
  isExpanded: boolean;
  /** Callback when the group expand/collapse is toggled */
  onToggle: () => void;
  /** Callback when delete all button is clicked */
  onDeleteAll: () => void;
}

/**
 * Component that displays a single sender group with expandable email details.
 * Shows sender information, email count, total size, and individual email details when expanded.
 */
export const SenderGroupItem: React.FC<SenderGroupItemProps> = ({
  group,
  isExpanded,
  onToggle,
  onDeleteAll,
}) => {
  // Defensive programming: ensure group has required properties
  if (!group) {
    console.warn("SenderGroupItem: group is null or undefined");
    return null;
  }

  const safeGroup = {
    senderEmail: group.SenderEmail || "unknown@example.com",
    senderName:
      group.SenderName && group.SenderName.trim() !== ""
        ? group.SenderName
        : group.SenderEmail || "Unknown sender",
    emailCount: group.EmailCount || 0,
    totalSize: group.TotalSize || 0,
    emails: group.Emails || [],
  };

  /**
   * Formats byte size into human-readable format
   */
  const formatSize = (bytes: number): string => {
    if (bytes === 0) return "0 B";
    const k = 1024;
    const sizes = ["B", "KB", "MB", "GB"];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + " " + sizes[i];
  };

  return (
    <div
      style={{
        border: "1px solid #ddd",
        borderRadius: "8px",
        overflow: "hidden",
        backgroundColor: "#f9f9f9",
        marginBottom: "10px",
        transition: "box-shadow 0.2s ease",
      }}
      onMouseEnter={(e) => {
        e.currentTarget.style.boxShadow = "0 2px 8px rgba(0,0,0,0.1)";
      }}
      onMouseLeave={(e) => {
        e.currentTarget.style.boxShadow = "none";
      }}
    >
      {/* Header with sender info and controls */}
      <div
        style={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          padding: "15px",
          backgroundColor: "#fff",
          cursor: "pointer",
          borderBottom: isExpanded ? "1px solid #ddd" : "none",
          transition: "background-color 0.2s ease",
        }}
        onClick={onToggle}
        onMouseEnter={(e) => {
          e.currentTarget.style.backgroundColor = "#f8f9fa";
        }}
        onMouseLeave={(e) => {
          e.currentTarget.style.backgroundColor = "#fff";
        }}
      >
        {/* Sender information */}
        <div style={{ flex: 1 }}>
          <h3 style={{ margin: "0 0 5px 0", fontSize: "16px", color: "#333" }}>
            {safeGroup.senderName}
          </h3>
          <p style={{ margin: 0, color: "#666", fontSize: "14px" }}>
            {safeGroup.senderEmail}
          </p>
        </div>

        {/* Statistics */}
        <div style={{ textAlign: "right", marginRight: "15px" }}>
          <div style={{ fontSize: "14px", fontWeight: "bold", color: "#333" }}>
            {safeGroup.emailCount.toLocaleString()} email
            {safeGroup.emailCount !== 1 ? "s" : ""}
          </div>
          <div style={{ fontSize: "12px", color: "#666" }}>
            {formatSize(safeGroup.totalSize)}
          </div>
        </div>

        {/* Action buttons and expand indicator */}
        <div style={{ display: "flex", gap: "10px", alignItems: "center" }}>
          <button
            onClick={(e) => {
              e.stopPropagation();
              onDeleteAll();
            }}
            style={{
              padding: "6px 12px",
              backgroundColor: "#dc3545",
              color: "white",
              border: "none",
              borderRadius: "4px",
              cursor: "pointer",
              fontSize: "12px",
              transition: "background-color 0.2s ease",
            }}
            onMouseEnter={(e) => {
              e.currentTarget.style.backgroundColor = "#c82333";
            }}
            onMouseLeave={(e) => {
              e.currentTarget.style.backgroundColor = "#dc3545";
            }}
          >
            Delete All
          </button>

          <span
            style={{
              fontSize: "18px",
              color: "#666",
              transition: "transform 0.2s ease",
              transform: isExpanded ? "rotate(90deg)" : "rotate(0deg)",
            }}
          >
            ▶
          </span>
        </div>
      </div>

      {/* Expandable email details */}
      {isExpanded && (
        <div style={{ padding: "15px", backgroundColor: "#f8f9fa" }}>
          <h4
            style={{ margin: "0 0 15px 0", fontSize: "14px", color: "#495057" }}
          >
            Email Details ({safeGroup.emails.length} emails):
          </h4>

          {/* Scrollable email list */}
          <div style={{ maxHeight: "300px", overflowY: "auto" }}>
            {safeGroup.emails.map((email, index) => {
              // Defensive programming for individual email objects
              const safeEmail = {
                subject: email?.Subject || "(No Subject)",
                date: email?.Date || null,
                size: email?.Size || 0,
              };

              return (
                <div
                  key={index}
                  style={{
                    padding: "12px",
                    marginBottom: "8px",
                    backgroundColor: "#fff",
                    borderRadius: "4px",
                    border: "1px solid #eee",
                    transition: "border-color 0.2s ease",
                  }}
                  onMouseEnter={(e) => {
                    e.currentTarget.style.borderColor = "#007bff";
                  }}
                  onMouseLeave={(e) => {
                    e.currentTarget.style.borderColor = "#eee";
                  }}
                >
                  {/* Email subject */}
                  <div
                    style={{
                      fontWeight: "500",
                      marginBottom: "6px",
                      fontSize: "14px",
                      color: "#333",
                      lineHeight: "1.4",
                      wordBreak: "break-word",
                    }}
                  >
                    {safeEmail.subject}
                  </div>

                  {/* Email metadata */}
                  <div
                    style={{
                      fontSize: "12px",
                      color: "#666",
                      display: "flex",
                      justifyContent: "space-between",
                      alignItems: "center",
                      flexWrap: "wrap",
                      gap: "8px",
                    }}
                  >
                    <span>
                      📅{" "}
                      {safeEmail.date
                        ? new Date(safeEmail.date).toLocaleDateString("en-US", {
                            year: "numeric",
                            month: "short",
                            day: "numeric",
                            hour: "2-digit",
                            minute: "2-digit",
                          })
                        : "Unknown date"}
                    </span>
                    <span>📊 {formatSize(safeEmail.size)}</span>
                  </div>
                </div>
              );
            })}
          </div>
        </div>
      )}
    </div>
  );
};

import React, { useState, useEffect, useCallback } from "react";
import { SenderGroup, ProcessingStatus } from "../types";
import { emailService } from "../services/emailService";
import { SearchAndSort } from "./SearchAndSort";
import { SenderGroupItem } from "./SenderGroupItem";
import { LoadingView } from "./LoadingView";
import { ErrorView } from "./ErrorView";
import { ProgressIndicator } from "./ProgressIndicator";

export const EmailManager: React.FC = () => {
  const [senderGroups, setSenderGroups] = useState<SenderGroup[]>([]);
  const [filteredGroups, setFilteredGroups] = useState<SenderGroup[]>([]);
  const [loading, setLoading] = useState(true);
  const [expandedGroups, setExpandedGroups] = useState<Set<string>>(new Set());
  const [searchTerm, setSearchTerm] = useState("");
  const [sortBy, setSortBy] = useState<"count" | "size" | "name">("count");
  const [error, setError] = useState<string | null>(null);
  const [processingStatus, setProcessingStatus] =
    useState<ProcessingStatus | null>(null);

  const filterAndSortGroups = useCallback(() => {
    let filtered = senderGroups;

    // Filter by search term
    if (searchTerm) {
      filtered = filtered.filter(
        (group) =>
          group.senderName.toLowerCase().includes(searchTerm.toLowerCase()) ||
          group.senderEmail.toLowerCase().includes(searchTerm.toLowerCase())
      );
    }

    // Sort by selected criteria
    filtered = [...filtered].sort((a, b) => {
      switch (sortBy) {
        case "count":
          return b.emailCount - a.emailCount;
        case "size":
          return b.totalSize - a.totalSize;
        case "name":
          return a.senderName.localeCompare(b.senderName);
        default:
          return 0;
      }
    });

    setFilteredGroups(filtered);
  }, [senderGroups, searchTerm, sortBy]);

  useEffect(() => {
    loadEmails();
  }, []);

  useEffect(() => {
    filterAndSortGroups();
  }, [senderGroups, searchTerm, sortBy, filterAndSortGroups]);

  const loadEmails = async () => {
    try {
      setLoading(true);
      setError(null);
      setProcessingStatus(null);
      console.log("Loading emails from backend...");

      // Start polling for progress if it's a large mailbox
      const progressInterval = setInterval(async () => {
        try {
          const status = await emailService.getProcessingStatus();
          setProcessingStatus(status);
          if (!status.isProcessing) {
            clearInterval(progressInterval);
          }
        } catch (e) {
          // Ignore progress polling errors
        }
      }, 1000);

      const groups = await emailService.getEmailsBySender();
      console.log("Received groups:", groups);
      setSenderGroups(groups);
      clearInterval(progressInterval);
      setProcessingStatus(null);
    } catch (error: any) {
      console.error("Failed to load emails:", error);
      setError(
        error.message ||
          "Failed to load emails. Please check your connection and try again."
      );
      setProcessingStatus(null);
    } finally {
      setLoading(false);
    }
  };

  const toggleGroup = (senderEmail: string) => {
    const newExpanded = new Set(expandedGroups);
    if (newExpanded.has(senderEmail)) {
      newExpanded.delete(senderEmail);
    } else {
      newExpanded.add(senderEmail);
    }
    setExpandedGroups(newExpanded);
  };

  const deleteAllFromSender = async (
    senderEmail: string,
    emailCount: number
  ) => {
    try {
      const deletedCount = await emailService.deleteEmailsBySender(
        senderEmail,
        emailCount
      );
      if (deletedCount > 0) {
        alert(`${deletedCount} emails from ${senderEmail} have been deleted.`);
        await loadEmails(); // Refresh the list
      }
    } catch (error) {
      console.error("Failed to delete emails:", error);
      alert("Failed to delete emails. Please try again.");
    }
  };

  if (loading) {
    return (
      <LoadingView message="Loading emails...">
        {processingStatus && processingStatus.isProcessing && (
          <ProgressIndicator status={processingStatus} />
        )}
      </LoadingView>
    );
  }

  if (error) {
    return (
      <ErrorView
        message={error}
        onRetry={loadEmails}
        retryButtonText="Retry Loading Emails"
      />
    );
  }

  return (
    <div style={{ maxWidth: "1200px", margin: "0 auto", padding: "20px" }}>
      <div
        style={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          marginBottom: "20px",
        }}
      >
        <h2>Email Manager ({filteredGroups.length} senders)</h2>
        <button
          onClick={loadEmails}
          style={{
            padding: "10px 20px",
            backgroundColor: "#28a745",
            color: "white",
            border: "none",
            borderRadius: "5px",
            cursor: "pointer",
            fontSize: "14px",
          }}
        >
          Refresh
        </button>
      </div>

      <SearchAndSort
        searchTerm={searchTerm}
        onSearchChange={setSearchTerm}
        sortBy={sortBy}
        onSortChange={setSortBy}
        resultCount={filteredGroups.length}
      />

      {filteredGroups.length === 0 ? (
        <div
          style={{
            textAlign: "center",
            padding: "50px",
            backgroundColor: "#f8f9fa",
            borderRadius: "8px",
          }}
        >
          <p style={{ fontSize: "16px", color: "#666" }}>
            No emails found. Please check your IMAP connection settings.
          </p>
        </div>
      ) : (
        <div style={{ display: "flex", flexDirection: "column" }}>
          {filteredGroups.map((group) => (
            <SenderGroupItem
              key={group.senderEmail}
              group={group}
              isExpanded={expandedGroups.has(group.senderEmail)}
              onToggle={() => toggleGroup(group.senderEmail)}
              onDeleteAll={() =>
                deleteAllFromSender(group.senderEmail, group.emailCount)
              }
            />
          ))}
        </div>
      )}
    </div>
  );
};

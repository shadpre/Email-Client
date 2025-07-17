import React, { useState, useEffect, useCallback } from "react";
import { SenderGroup, ProcessingStatus, DateFilter } from "../types";
import { emailService } from "../services/emailService";
import { SearchAndSort } from "./SearchAndSort";
import { SenderGroupItem } from "./SenderGroupItem";
import { LoadingView } from "./LoadingView";
import { ErrorView } from "./ErrorView";
import { ProgressIndicator } from "./ProgressIndicator";
import { DateFilterComponent } from "./DateFilterComponent";

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
  const [dateFilter, setDateFilter] = useState<DateFilter | null>(null);

  const filterAndSortGroups = useCallback(() => {
    let filtered = senderGroups;

    // Filter by search term
    if (searchTerm) {
      filtered = filtered.filter(
        (group) =>
          group.SenderName.toLowerCase().includes(searchTerm.toLowerCase()) ||
          group.SenderEmail.toLowerCase().includes(searchTerm.toLowerCase())
      );
    }

    // Sort by selected criteria
    filtered = [...filtered].sort((a, b) => {
      switch (sortBy) {
        case "count":
          return b.EmailCount - a.EmailCount;
        case "size":
          return b.TotalSize - a.TotalSize;
        case "name":
          return a.SenderName.localeCompare(b.SenderName);
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

  // Handle date filter changes
  const handleDateFilterChange = (newDateFilter: DateFilter | null) => {
    setDateFilter(newDateFilter);
    loadEmails(newDateFilter);
  };

  const loadEmails = async (currentDateFilter: DateFilter | null = null) => {
    try {
      setLoading(true);
      setError(null);
      setProcessingStatus(null);

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

      // Use filtered service if date filter is provided, otherwise use the standard service
      const groups = currentDateFilter
        ? await emailService.getEmailsBySenderWithFilter(currentDateFilter)
        : await emailService.getEmailsBySender();

      setSenderGroups(groups || []);
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
      let deletedCount: number;
      
      if (dateFilter) {
        // Use filtered delete when date filter is active
        deletedCount = await emailService.deleteEmailsBySenderWithFilter(
          senderEmail,
          emailCount,
          dateFilter
        );
      } else {
        // Use regular delete when no date filter is active
        deletedCount = await emailService.deleteEmailsBySender(
          senderEmail,
          emailCount
        );
      }
      
      if (deletedCount > 0) {
        const filterMessage = dateFilter ? " (matching the current date filter)" : "";
        alert(`${deletedCount} emails from ${senderEmail}${filterMessage} have been deleted.`);
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
          onClick={() => loadEmails(dateFilter)}
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

      <DateFilterComponent
        dateFilter={dateFilter}
        onDateFilterChange={handleDateFilterChange}
        isLoading={loading}
      />

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
              key={group.SenderEmail}
              group={group}
              isExpanded={expandedGroups.has(group.SenderEmail)}
              onToggle={() => toggleGroup(group.SenderEmail)}
              onDeleteAll={() =>
                deleteAllFromSender(group.SenderEmail, group.EmailCount)
              }
            />
          ))}
        </div>
      )}
    </div>
  );
};

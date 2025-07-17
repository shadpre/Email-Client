import React, { useState } from "react";
import { DateFilter, DateFilterType, DateFilterHelpers } from "../types";

/**
 * Props for the DateFilterComponent
 */
interface DateFilterComponentProps {
  /** Current date filter */
  dateFilter: DateFilter | null;
  /** Callback when date filter changes */
  onDateFilterChange: (filter: DateFilter | null) => void;
  /** Whether filtering is currently in progress */
  isLoading?: boolean;
}

/**
 * Component that provides date filtering controls for email retrieval.
 * Supports preset periods (30/90/180/365 days) and custom date ranges.
 */
export const DateFilterComponent: React.FC<DateFilterComponentProps> = ({
  dateFilter,
  onDateFilterChange,
  isLoading = false,
}) => {
  const [showCustomRange, setShowCustomRange] = useState(
    dateFilter?.FilterType === DateFilterType.DateRange
  );
  const [customStartDate, setCustomStartDate] = useState(
    dateFilter?.StartDate?.toISOString().split("T")[0] || ""
  );
  const [customEndDate, setCustomEndDate] = useState(
    dateFilter?.EndDate?.toISOString().split("T")[0] || ""
  );

  /**
   * Handles preset filter selection
   */
  const handlePresetChange = (value: string) => {
    setShowCustomRange(false);

    switch (value) {
      case "all":
        onDateFilterChange(null);
        break;
      case "7":
        onDateFilterChange(DateFilterHelpers.olderThan(7));
        break;
      case "30":
        onDateFilterChange(DateFilterHelpers.olderThan(30));
        break;
      case "90":
        onDateFilterChange(DateFilterHelpers.olderThan(90));
        break;
      case "6months":
        onDateFilterChange(DateFilterHelpers.olderThanMonths(6));
        break;
      case "1year":
        onDateFilterChange(DateFilterHelpers.olderThanYears(1));
        break;
      case "2years":
        onDateFilterChange(DateFilterHelpers.olderThanYears(2));
        break;
      case "custom":
        setShowCustomRange(true);
        // Keep current custom range if it exists
        if (dateFilter?.FilterType === DateFilterType.DateRange) {
          onDateFilterChange(dateFilter);
        }
        break;
      default:
        onDateFilterChange(null);
    }
  };

  /**
   * Handles custom date range changes
   */
  const handleCustomDateChange = () => {
    if (customStartDate && customEndDate) {
      const startDate = new Date(customStartDate);
      const endDate = new Date(customEndDate);

      if (startDate <= endDate) {
        onDateFilterChange(DateFilterHelpers.between(startDate, endDate));
      } else {
        console.warn("Start date must be before end date");
      }
    }
  };

  /**
   * Gets the current preset value for the dropdown
   */
  const getCurrentPresetValue = (): string => {
    if (!dateFilter) return "all";

    if (dateFilter.FilterType === DateFilterType.OlderThanDays) {
      return dateFilter.Days?.toString() || "all";
    }

    if (dateFilter.FilterType === DateFilterType.OlderThanMonths) {
      if (dateFilter.Months === 6) return "6months";
      return "all";
    }

    if (dateFilter.FilterType === DateFilterType.OlderThanYears) {
      if (dateFilter.Years === 1) return "1year";
      if (dateFilter.Years === 2) return "2years";
      return "all";
    }

    if (dateFilter.FilterType === DateFilterType.DateRange) {
      return "custom";
    }

    return "all";
  };

  return (
    <div
      style={{
        marginBottom: "20px",
        padding: "15px",
        border: "1px solid #ddd",
        borderRadius: "8px",
        backgroundColor: "#f9f9f9",
      }}
    >
      <div
        style={{
          marginBottom: "15px",
          fontSize: "16px",
          fontWeight: "bold",
          color: "#333",
        }}
      >
        📅 Filter by Date
      </div>

      <div
        style={{
          display: "flex",
          gap: "15px",
          alignItems: "center",
          flexWrap: "wrap",
          marginBottom: showCustomRange ? "15px" : "0",
        }}
      >
        {/* Preset filter dropdown */}
        <select
          value={getCurrentPresetValue()}
          onChange={(e) => handlePresetChange(e.target.value)}
          disabled={isLoading}
          style={{
            padding: "8px 12px",
            border: "1px solid #ddd",
            borderRadius: "4px",
            fontSize: "14px",
            backgroundColor: "white",
            cursor: isLoading ? "not-allowed" : "pointer",
            opacity: isLoading ? 0.6 : 1,
            minWidth: "200px",
          }}
        >
          <option value="all">All emails</option>
          <option value="7">Older than 7 days</option>
          <option value="30">Older than 30 days</option>
          <option value="90">Older than 90 days</option>
          <option value="6months">Older than 6 months</option>
          <option value="1year">Older than 1 year</option>
          <option value="2years">Older than 2 years</option>
          <option value="custom">Custom date range</option>
        </select>

        {/* Clear filter button */}
        {dateFilter && (
          <button
            onClick={() => onDateFilterChange(null)}
            disabled={isLoading}
            style={{
              padding: "8px 12px",
              border: "1px solid #dc3545",
              borderRadius: "4px",
              backgroundColor: "#fff",
              color: "#dc3545",
              cursor: isLoading ? "not-allowed" : "pointer",
              fontSize: "14px",
              opacity: isLoading ? 0.6 : 1,
            }}
          >
            Clear Filter
          </button>
        )}
      </div>

      {/* Custom date range inputs */}
      {showCustomRange && (
        <div
          style={{
            display: "flex",
            gap: "15px",
            alignItems: "center",
            flexWrap: "wrap",
            padding: "10px",
            backgroundColor: "white",
            borderRadius: "4px",
            border: "1px solid #ddd",
          }}
        >
          <label style={{ fontSize: "14px", fontWeight: "500" }}>From:</label>
          <input
            type="date"
            value={customStartDate}
            onChange={(e) => {
              setCustomStartDate(e.target.value);
              if (e.target.value && customEndDate) {
                // Auto-apply when both dates are set
                setTimeout(handleCustomDateChange, 100);
              }
            }}
            disabled={isLoading}
            style={{
              padding: "6px 10px",
              border: "1px solid #ddd",
              borderRadius: "4px",
              fontSize: "14px",
              cursor: isLoading ? "not-allowed" : "pointer",
              opacity: isLoading ? 0.6 : 1,
            }}
          />

          <label style={{ fontSize: "14px", fontWeight: "500" }}>To:</label>
          <input
            type="date"
            value={customEndDate}
            onChange={(e) => {
              setCustomEndDate(e.target.value);
              if (customStartDate && e.target.value) {
                // Auto-apply when both dates are set
                setTimeout(handleCustomDateChange, 100);
              }
            }}
            disabled={isLoading}
            style={{
              padding: "6px 10px",
              border: "1px solid #ddd",
              borderRadius: "4px",
              fontSize: "14px",
              cursor: isLoading ? "not-allowed" : "pointer",
              opacity: isLoading ? 0.6 : 1,
            }}
          />

          <button
            onClick={handleCustomDateChange}
            disabled={isLoading || !customStartDate || !customEndDate}
            style={{
              padding: "6px 12px",
              border: "1px solid #007bff",
              borderRadius: "4px",
              backgroundColor: "#007bff",
              color: "white",
              cursor:
                isLoading || !customStartDate || !customEndDate
                  ? "not-allowed"
                  : "pointer",
              fontSize: "14px",
              opacity:
                isLoading || !customStartDate || !customEndDate ? 0.6 : 1,
            }}
          >
            Apply Range
          </button>
        </div>
      )}

      {/* Current filter display */}
      {dateFilter && (
        <div
          style={{
            marginTop: "10px",
            padding: "8px 12px",
            backgroundColor: "#e3f2fd",
            borderRadius: "4px",
            fontSize: "14px",
            color: "#1976d2",
          }}
        >
          <strong>Active Filter:</strong>{" "}
          {dateFilter.FilterType === DateFilterType.OlderThanDays
            ? `Emails older than ${dateFilter.Days} days`
            : dateFilter.FilterType === DateFilterType.OlderThanMonths
            ? `Emails older than ${dateFilter.Months} months`
            : dateFilter.FilterType === DateFilterType.OlderThanYears
            ? `Emails older than ${dateFilter.Years} years`
            : dateFilter.FilterType === DateFilterType.DateRange
            ? `Emails from ${dateFilter.StartDate?.toLocaleDateString()} to ${dateFilter.EndDate?.toLocaleDateString()}`
            : "All emails"}
        </div>
      )}
    </div>
  );
};

import React from "react";

/**
 * Props for the SearchAndSort component
 */
interface SearchAndSortProps {
  /** Current search term */
  searchTerm: string;
  /** Callback when search term changes */
  onSearchChange: (term: string) => void;
  /** Current sort criteria */
  sortBy: "count" | "size" | "name";
  /** Callback when sort criteria changes */
  onSortChange: (sortBy: "count" | "size" | "name") => void;
  /** Number of filtered results for display */
  resultCount: number;
}

/**
 * Component that provides search and sorting controls for email sender groups.
 * Includes a search input field and dropdown for sorting criteria.
 */
export const SearchAndSort: React.FC<SearchAndSortProps> = ({
  searchTerm,
  onSearchChange,
  sortBy,
  onSortChange,
  resultCount,
}) => {
  return (
    <div
      style={{
        marginBottom: "20px",
        display: "flex",
        gap: "15px",
        alignItems: "center",
        flexWrap: "wrap",
      }}
    >
      {/* Search input */}
      <input
        type="text"
        placeholder="Search by sender name or email..."
        value={searchTerm}
        onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
          onSearchChange(e.target.value)
        }
        style={{
          padding: "8px 12px",
          border: "1px solid #ddd",
          borderRadius: "4px",
          fontSize: "14px",
          minWidth: "300px",
          flex: "1",
          maxWidth: "400px",
        }}
      />

      {/* Sort dropdown */}
      <select
        value={sortBy}
        onChange={(e: React.ChangeEvent<HTMLSelectElement>) =>
          onSortChange(e.target.value as "count" | "size" | "name")
        }
        style={{
          padding: "8px 12px",
          border: "1px solid #ddd",
          borderRadius: "4px",
          fontSize: "14px",
          backgroundColor: "white",
          cursor: "pointer",
        }}
      >
        <option value="count">Sort by Email Count</option>
        <option value="size">Sort by Total Size</option>
        <option value="name">Sort by Sender Name</option>
      </select>

      {/* Results count */}
      <div
        style={{
          fontSize: "14px",
          color: "#666",
          padding: "8px 0",
          whiteSpace: "nowrap",
        }}
      >
        {resultCount} sender{resultCount !== 1 ? "s" : ""} found
      </div>
    </div>
  );
};

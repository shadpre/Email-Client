/**
 * Enumeration of supported date filter types for email filtering.
 * Allows users to filter emails by preset periods or custom date ranges.
 */
export enum DateFilterType {
  /** Show all emails without date filtering */
  All = "All",
  /** Filter emails older than specified number of days */
  OlderThanDays = "OlderThanDays",
  /** Filter emails older than specified number of months */
  OlderThanMonths = "OlderThanMonths",
  /** Filter emails older than specified number of years */
  OlderThanYears = "OlderThanYears",
  /** Filter emails within a custom date range */
  DateRange = "DateRange",
}

/**
 * Date filter configuration for filtering emails by date criteria.
 * Supports both preset periods and custom date ranges for flexible email management.
 * Property names match the backend C# model exactly.
 */
export interface DateFilter {
  /** Type of date filter to apply */
  FilterType: DateFilterType;

  /** Number of days for OlderThanDays filter type */
  Days?: number;

  /** Number of months for OlderThanMonths filter type */
  Months?: number;

  /** Number of years for OlderThanYears filter type */
  Years?: number;

  /** Start date for DateRange filter type */
  StartDate?: Date;

  /** End date for DateRange filter type */
  EndDate?: Date;
}

/**
 * Request model for retrieving emails with optional date filtering.
 * Used to send filter criteria to the backend API.
 * Property names match the backend C# model exactly.
 */
export interface EmailsByFilterRequest {
  /** Optional date filter to apply when retrieving emails */
  DateFilter?: DateFilter | null;
}

/**
 * Utility functions for creating common date filters
 */
export class DateFilterHelpers {
  /**
   * Creates a date filter for emails older than specified days
   */
  static olderThan(days: number): DateFilter {
    return {
      FilterType: DateFilterType.OlderThanDays,
      Days: days,
    };
  }

  /**
   * Creates a date filter for emails older than specified months
   */
  static olderThanMonths(months: number): DateFilter {
    return {
      FilterType: DateFilterType.OlderThanMonths,
      Months: months,
    };
  }

  /**
   * Creates a date filter for emails older than specified years
   */
  static olderThanYears(years: number): DateFilter {
    return {
      FilterType: DateFilterType.OlderThanYears,
      Years: years,
    };
  }

  /**
   * Creates a date filter for a custom date range
   */
  static between(startDate: Date, endDate: Date): DateFilter {
    return {
      FilterType: DateFilterType.DateRange,
      StartDate: startDate,
      EndDate: endDate,
    };
  }

  /**
   * Creates a date filter that includes all emails (no filtering)
   */
  static all(): DateFilter {
    return {
      FilterType: DateFilterType.All,
    };
  }

  /**
   * Common preset filters for quick selection
   */
  static readonly PRESETS = {
    ALL: DateFilterHelpers.all(),
    LAST_7_DAYS: DateFilterHelpers.olderThan(7),
    LAST_30_DAYS: DateFilterHelpers.olderThan(30),
    LAST_90_DAYS: DateFilterHelpers.olderThan(90),
    LAST_6_MONTHS: DateFilterHelpers.olderThanMonths(6),
    LAST_YEAR: DateFilterHelpers.olderThanYears(1),
    LAST_2_YEARS: DateFilterHelpers.olderThanYears(2),
  };
}

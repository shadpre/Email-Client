namespace EmailClient.Api.Models
{
    /// <summary>
    /// Date filter configuration for filtering emails by date ranges.
    /// Supports both preset periods (days/months/years) and custom date ranges.
    /// </summary>
    public class DateFilter
    {
        /// <summary>
        /// Type of date filter to apply
        /// </summary>
        public DateFilterType FilterType { get; set; } = DateFilterType.All;

        /// <summary>
        /// Number of days to filter (used when FilterType is OlderThanDays)
        /// </summary>
        public int? Days { get; set; }

        /// <summary>
        /// Number of months to filter (used when FilterType is OlderThanMonths)
        /// </summary>
        public int? Months { get; set; }

        /// <summary>
        /// Number of years to filter (used when FilterType is OlderThanYears)
        /// </summary>
        public int? Years { get; set; }

        /// <summary>
        /// Custom start date for filtering (used when FilterType is DateRange)
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Custom end date for filtering (used when FilterType is DateRange)
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Creates a date filter for emails older than specified days
        /// </summary>
        /// <param name="days">Number of days</param>
        /// <returns>Configured DateFilter</returns>
        public static DateFilter OlderThan(int days)
        {
            return new DateFilter
            {
                FilterType = DateFilterType.OlderThanDays,
                Days = days
            };
        }

        /// <summary>
        /// Creates a date filter for emails older than specified months
        /// </summary>
        /// <param name="months">Number of months</param>
        /// <returns>Configured DateFilter</returns>
        public static DateFilter OlderThanMonths(int months)
        {
            return new DateFilter
            {
                FilterType = DateFilterType.OlderThanMonths,
                Months = months
            };
        }

        /// <summary>
        /// Creates a date filter for emails older than specified years
        /// </summary>
        /// <param name="years">Number of years</param>
        /// <returns>Configured DateFilter</returns>
        public static DateFilter OlderThanYears(int years)
        {
            return new DateFilter
            {
                FilterType = DateFilterType.OlderThanYears,
                Years = years
            };
        }

        /// <summary>
        /// Creates a date filter for a custom date range
        /// </summary>
        /// <param name="startDate">Start date (inclusive)</param>
        /// <param name="endDate">End date (inclusive)</param>
        /// <returns>Configured DateFilter</returns>
        public static DateFilter Between(DateTime startDate, DateTime endDate)
        {
            return new DateFilter
            {
                FilterType = DateFilterType.DateRange,
                StartDate = startDate,
                EndDate = endDate
            };
        }

        /// <summary>
        /// Creates a date filter that includes all emails (no filtering)
        /// </summary>
        /// <returns>Configured DateFilter</returns>
        public static DateFilter All()
        {
            return new DateFilter
            {
                FilterType = DateFilterType.All
            };
        }
    }

    /// <summary>
    /// Enumeration of available date filter types
    /// </summary>
    public enum DateFilterType
    {
        /// <summary>
        /// No date filtering - include all emails
        /// </summary>
        All,

        /// <summary>
        /// Filter emails older than specified number of days
        /// </summary>
        OlderThanDays,

        /// <summary>
        /// Filter emails older than specified number of months
        /// </summary>
        OlderThanMonths,

        /// <summary>
        /// Filter emails older than specified number of years
        /// </summary>
        OlderThanYears,

        /// <summary>
        /// Filter emails within a custom date range
        /// </summary>
        DateRange
    }
}

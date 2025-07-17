using MimeKit;

namespace EmailClient.Api.Services
{
    /// <summary>
    /// Utility service for parsing and extracting information from email addresses.
    /// Implements DRY principle by centralizing email parsing logic.
    /// </summary>
    public interface IEmailParsingService
    {
        /// <summary>
        /// Extracts the email address from a formatted email string
        /// </summary>
        /// <param name="fromString">Email string that may contain name and address (e.g., "John Doe &lt;john@example.com&gt;")</param>
        /// <returns>The email address portion only</returns>
        string ExtractEmail(string fromString);

        /// <summary>
        /// Extracts the display name from a formatted email string
        /// </summary>
        /// <param name="fromString">Email string that may contain name and address</param>
        /// <returns>The display name if available, otherwise the email address</returns>
        string ExtractName(string fromString);
    }
}

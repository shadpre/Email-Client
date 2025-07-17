using MimeKit;

namespace EmailClient.Api.Services
{
    /// <summary>
    /// Implementation of email parsing utilities.
    /// Safely handles malformed email addresses and provides fallback behavior.
    /// </summary>
    public class EmailParsingService : IEmailParsingService
    {
        /// <inheritdoc />
        public string ExtractEmail(string fromString)
        {
            try
            {
                // Use MimeKit's robust email parsing
                var mailboxAddress = MailboxAddress.Parse(fromString);
                return mailboxAddress.Address;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to parse email from '{fromString}': {ex.Message}");
                // Return the original string as fallback if parsing fails
                return fromString;
            }
        }

        /// <inheritdoc />
        public string ExtractName(string fromString)
        {
            try
            {
                // Use MimeKit's robust email parsing
                var mailboxAddress = MailboxAddress.Parse(fromString);
                
                // Return name if available, otherwise fall back to email address
                return !string.IsNullOrEmpty(mailboxAddress.Name) 
                    ? mailboxAddress.Name 
                    : mailboxAddress.Address;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to parse name from '{fromString}': {ex.Message}");
                // Return the original string as fallback if parsing fails
                return fromString;
            }
        }
    }
}

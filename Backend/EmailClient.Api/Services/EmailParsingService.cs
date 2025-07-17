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
                if (string.IsNullOrWhiteSpace(fromString))
                {
                    return "unknown@example.com";
                }

                // Use MimeKit's robust email parsing
                var mailboxAddress = MailboxAddress.Parse(fromString);
                var email = mailboxAddress.Address;
                
                // Validate the email address
                if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                {
                    return "unknown@example.com";
                }
                
                return email.ToLowerInvariant(); // Normalize to lowercase
            }
            catch
            {
                // Try simple fallback extraction
                var fallback = ExtractEmailFallback(fromString);
                return !string.IsNullOrWhiteSpace(fallback) ? fallback : "unknown@example.com";
            }
        }

        /// <inheritdoc />
        public string ExtractName(string fromString)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fromString))
                {
                    return "Unknown Sender";
                }

                // Use MimeKit's robust email parsing
                var mailboxAddress = MailboxAddress.Parse(fromString);
                
                // Return name if available, otherwise fall back to email address
                var name = !string.IsNullOrWhiteSpace(mailboxAddress.Name) 
                    ? mailboxAddress.Name 
                    : mailboxAddress.Address;
                    
                return !string.IsNullOrWhiteSpace(name) ? name : "Unknown Sender";
            }
            catch
            {
                // Try simple fallback extraction
                var fallback = ExtractEmailFallback(fromString);
                return !string.IsNullOrWhiteSpace(fallback) ? fallback : "Unknown Sender";
            }
        }

        /// <summary>
        /// Simple fallback email extraction when MimeKit parsing fails
        /// </summary>
        /// <param name="fromString">The from string to parse</param>
        /// <returns>Extracted email or empty string if not found</returns>
        private string ExtractEmailFallback(string fromString)
        {
            try
            {
                // Look for email pattern using regex
                var emailPattern = @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}";
                var match = System.Text.RegularExpressions.Regex.Match(fromString, emailPattern);
                return match.Success ? match.Value.ToLowerInvariant() : string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}

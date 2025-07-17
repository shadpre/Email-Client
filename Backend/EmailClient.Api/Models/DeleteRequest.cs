namespace EmailClient.Api.Models
{
    /// <summary>
    /// Request model for deleting specific emails by their unique identifiers.
    /// Used when deleting individual emails or a custom selection of emails.
    /// </summary>
    public class DeleteRequest
    {
        /// <summary>
        /// Collection of email unique identifiers (UIDs) to be deleted from the server.
        /// These UIDs correspond to specific messages on the IMAP server.
        /// </summary>
        public List<uint> EmailUids { get; set; } = new();
    }
}

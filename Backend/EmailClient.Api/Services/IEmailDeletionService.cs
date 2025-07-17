using MailKit;
using MailKit.Search;
using EmailClient.Api.Models;

namespace EmailClient.Api.Services
{
    /// <summary>
    /// Service responsible for email deletion operations on the IMAP server.
    /// Handles both individual email deletion and bulk deletion by sender.
    /// </summary>
    public interface IEmailDeletionService
    {
        /// <summary>
        /// Deletes specific emails by their unique identifiers
        /// </summary>
        /// <param name="uids">Collection of email UIDs to delete</param>
        /// <returns>Number of emails successfully deleted</returns>
        Task<int> DeleteEmailsAsync(List<uint> uids);

        /// <summary>
        /// Deletes all emails from a specific sender
        /// </summary>
        /// <param name="senderEmail">Email address of the sender whose emails should be deleted</param>
        /// <returns>Number of emails successfully deleted</returns>
        Task<int> DeleteEmailsBySenderAsync(string senderEmail);

        /// <summary>
        /// Deletes emails from a specific sender that match the date filter criteria
        /// </summary>
        /// <param name="senderEmail">Email address of the sender whose emails should be deleted</param>
        /// <param name="dateFilter">Optional date filter to apply when deleting emails</param>
        /// <returns>Number of emails successfully deleted</returns>
        Task<int> DeleteEmailsBySenderWithFilterAsync(string senderEmail, DateFilter? dateFilter);
    }
}

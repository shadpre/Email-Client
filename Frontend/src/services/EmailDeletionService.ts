import { ApiClient } from "./ApiClient";
import { DeleteRequest } from "../types";

/**
 * Service for handling email deletion operations on the IMAP server.
 * Provides methods for deleting individual emails and bulk deletion by sender.
 */
export class EmailDeletionService {
  private readonly apiClient = ApiClient.getInstance().getAxiosInstance();

  /**
   * Deletes specific emails by their unique identifiers.
   *
   * @param emailUids - Array of email UIDs to delete from the server
   * @returns Promise resolving to the number of emails successfully deleted
   * @throws Error if deletion fails or server returns an error
   */
  async deleteEmails(emailUids: number[]): Promise<number> {
    try {
      if (!emailUids || emailUids.length === 0) {
        console.warn("No email UIDs provided for deletion");
        return 0;
      }

      console.log(`Requesting deletion of ${emailUids.length} specific emails`);

      const request: DeleteRequest = { emailUids };
      const response = await this.apiClient.delete("/email/delete", {
        data: request,
      });

      const deletedCount = response.data;
      console.log(`Successfully deleted ${deletedCount} emails`);

      return deletedCount;
    } catch (error: any) {
      console.error(
        "Failed to delete emails:",
        error.response?.data?.message || error.message
      );
      throw new Error(
        `Failed to delete emails: ${
          error.response?.data?.message || error.message
        }`
      );
    }
  }

  /**
   * Deletes all emails from a specific sender.
   * This is a bulk operation that permanently removes all emails from the specified sender.
   *
   * @param senderEmail - Email address of the sender whose emails should be deleted
   * @returns Promise resolving to the number of emails successfully deleted
   * @throws Error if deletion fails, sender email is invalid, or server returns an error
   */
  async deleteEmailsBySender(senderEmail: string): Promise<number> {
    try {
      if (!senderEmail || senderEmail.trim() === "") {
        throw new Error("Sender email address is required");
      }

      console.log(
        `Requesting deletion of all emails from sender: ${senderEmail}`
      );

      const response = await this.apiClient.delete(
        `/email/delete-by-sender/${encodeURIComponent(senderEmail)}`
      );

      const deletedCount = response.data;
      console.log(
        `Successfully deleted ${deletedCount} emails from ${senderEmail}`
      );

      return deletedCount;
    } catch (error: any) {
      console.error(
        `Failed to delete emails from ${senderEmail}:`,
        error.response?.data?.message || error.message
      );
      throw new Error(
        `Failed to delete emails from sender: ${
          error.response?.data?.message || error.message
        }`
      );
    }
  }

  /**
   * Confirms deletion with the user before proceeding with bulk operations.
   * Provides a safety mechanism for destructive operations.
   *
   * @param senderEmail - Email address of the sender
   * @param emailCount - Number of emails that will be deleted
   * @returns Promise resolving to true if user confirms, false otherwise
   */
  async confirmBulkDeletion(
    senderEmail: string,
    emailCount: number
  ): Promise<boolean> {
    const message = `Are you sure you want to permanently delete ${emailCount} email(s) from ${senderEmail}?\n\nThis action cannot be undone.`;

    return new Promise((resolve) => {
      const confirmed = window.confirm(message);
      resolve(confirmed);
    });
  }
}

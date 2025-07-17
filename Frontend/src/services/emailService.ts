import { ConnectionService } from "./ConnectionService";
import { EmailRetrievalService } from "./EmailRetrievalService";
import { EmailDeletionService } from "./EmailDeletionService";
import { ImapConfig, SenderGroup, ProcessingStatus } from "../types";

/**
 * Main facade service that provides a unified interface for all email operations.
 * Coordinates between different specialized services following the Facade pattern.
 * This is the primary service that components should interact with.
 */
export class EmailService {
  private readonly connectionService = new ConnectionService();
  private readonly retrievalService = new EmailRetrievalService();
  private readonly deletionService = new EmailDeletionService();

  /**
   * Establishes a connection to an IMAP server.
   *
   * @param config - IMAP server configuration
   * @returns Promise resolving to true if connection successful
   */
  async connect(config: ImapConfig): Promise<boolean> {
    return this.connectionService.connect(config);
  }

  /**
   * Disconnects from the IMAP server.
   */
  async disconnect(): Promise<void> {
    return this.connectionService.disconnect();
  }

  /**
   * Retrieves all emails grouped by sender.
   *
   * @returns Promise resolving to sender groups with email statistics
   */
  async getEmailsBySender(): Promise<SenderGroup[]> {
    return this.retrievalService.getEmailsBySender();
  }

  /**
   * Gets the current processing status for operations.
   *
   * @returns Promise resolving to processing status
   */
  async getProcessingStatus(): Promise<ProcessingStatus> {
    return this.retrievalService.getProcessingStatus();
  }

  /**
   * Deletes specific emails by their UIDs.
   *
   * @param emailUids - Email UIDs to delete
   * @returns Promise resolving to number of deleted emails
   */
  async deleteEmails(emailUids: number[]): Promise<number> {
    return this.deletionService.deleteEmails(emailUids);
  }

  /**
   * Deletes all emails from a specific sender with confirmation.
   *
   * @param senderEmail - Sender's email address
   * @param emailCount - Number of emails (for confirmation dialog)
   * @returns Promise resolving to number of deleted emails, or 0 if cancelled
   */
  async deleteEmailsBySender(
    senderEmail: string,
    emailCount: number
  ): Promise<number> {
    // Show confirmation dialog before proceeding with bulk deletion
    const confirmed = await this.deletionService.confirmBulkDeletion(
      senderEmail,
      emailCount
    );

    if (!confirmed) {
      console.log(`User cancelled deletion of emails from ${senderEmail}`);
      return 0;
    }

    return this.deletionService.deleteEmailsBySender(senderEmail);
  }
}

// Export a singleton instance for use throughout the application
export const emailService = new EmailService();

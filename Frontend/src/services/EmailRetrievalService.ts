import { ApiClient } from "./ApiClient";
import { SenderGroup, ProcessingStatus } from "../types";

/**
 * Service for retrieving and organizing emails from the IMAP server.
 * Handles the complexity of large mailbox processing with progress tracking.
 */
export class EmailRetrievalService {
  private readonly apiClient = ApiClient.getInstance().getAxiosInstance();

  /**
   * Retrieves all emails from the inbox grouped by sender.
   * This operation may take time for large mailboxes and supports progress tracking.
   *
   * @returns Promise resolving to an array of sender groups with email statistics
   * @throws Error if retrieval fails or server returns an error
   */
  async getEmailsBySender(): Promise<SenderGroup[]> {
    try {
      console.log("Requesting emails grouped by sender from server");

      const response = await this.apiClient.get("/email/emails-by-sender");
      const senderGroups: SenderGroup[] = response.data;

      console.log(`Received ${senderGroups.length} sender groups`);

      // Sort by email count (highest first) for better user experience
      return senderGroups.sort((a, b) => b.emailCount - a.emailCount);
    } catch (error: any) {
      console.error(
        "Failed to retrieve emails:",
        error.response?.data?.message || error.message
      );
      throw new Error(
        `Failed to retrieve emails: ${
          error.response?.data?.message || error.message
        }`
      );
    }
  }

  /**
   * Gets the current processing status for long-running email operations.
   * Useful for displaying progress bars during large mailbox processing.
   *
   * @returns Promise resolving to the current processing status
   */
  async getProcessingStatus(): Promise<ProcessingStatus> {
    try {
      const response = await this.apiClient.get("/email/processing-status");
      return response.data;
    } catch (error: any) {
      console.warn("Failed to get processing status:", error.message);

      // Return default status if unable to retrieve from server
      return {
        isProcessing: false,
        totalEmails: 0,
        processedEmails: 0,
        currentBatch: 0,
        totalBatches: 0,
        currentOperation: "Unknown",
        progressPercentage: 0,
      };
    }
  }
}

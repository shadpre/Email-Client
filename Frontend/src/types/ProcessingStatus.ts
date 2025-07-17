/**
 * Represents the current state of email processing operations.
 * Provides real-time feedback during lengthy operations like processing large mailboxes.
 */
export interface ProcessingStatus {
  /** Indicates whether an email processing operation is currently in progress */
  isProcessing: boolean;

  /** Total number of emails to be processed in the current operation */
  totalEmails: number;

  /** Number of emails that have been processed so far */
  processedEmails: number;

  /** Current batch number being processed (for batch operations) */
  currentBatch: number;

  /** Total number of batches in the current operation */
  totalBatches: number;

  /** Human-readable description of the current operation status */
  currentOperation: string;

  /** Calculated progress percentage (0-100) based on processed vs total emails */
  progressPercentage: number;
}

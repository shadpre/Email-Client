import { EmailSummary } from "./EmailSummary";

/**
 * Represents a collection of emails grouped by sender.
 * Provides aggregated statistics and a sample of recent emails for efficient mailbox management.
 */
export interface SenderGroup {
  /** The email address of the sender (used as the primary identifier) */
  senderEmail: string;

  /** The display name of the sender (fallback to email if name not available) */
  senderName: string;

  /** Total number of emails from this sender in the mailbox */
  emailCount: number;

  /** Combined size of all emails from this sender in bytes */
  totalSize: number;

  /**
   * A limited collection of recent emails from this sender for preview purposes.
   * Limited to 10 emails for performance optimization.
   */
  emails: EmailSummary[];
}

/**
 * Represents a summary of an individual email message.
 * Contains essential metadata without the full email content for performance optimization.
 */
export interface EmailSummary {
  /** Unique identifier for the email message on the IMAP server */
  Uid: number;

  /** The email subject line (displays "No Subject" if empty) */
  Subject: string;

  /** The original sender information as received from the email header */
  From: string;

  /** The extracted email address of the sender (parsed from From field) */
  FromEmail: string;

  /** The date and time when the email was sent or received */
  Date: string;

  /** The size of the email in bytes (used for calculating storage usage) */
  Size: number;
}

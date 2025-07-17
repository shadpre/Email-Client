/**
 * Configuration required to establish an IMAP connection to an email server.
 * Contains server details, authentication credentials, and connection preferences.
 */
export interface ImapConfig {
  /** The IMAP server hostname (e.g., "imap.gmail.com", "outlook.office365.com") */
  server: string;

  /** The port number for the IMAP connection (typically 993 for SSL, 143 for non-SSL) */
  port: number;

  /** The username/email address for authentication */
  username: string;

  /** The password for authentication (use App Password for providers like Gmail) */
  password: string;

  /** Whether to use SSL/TLS encryption for the connection (recommended: true) */
  useSsl: boolean;
}

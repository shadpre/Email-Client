/**
 * Request model for deleting specific emails by their unique identifiers.
 * Used when deleting individual emails or a custom selection of emails.
 */
export interface DeleteRequest {
  /**
   * Collection of email unique identifiers (UIDs) to be deleted from the server.
   * These UIDs correspond to specific messages on the IMAP server.
   */
  emailUids: number[];
}

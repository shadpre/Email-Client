import { ApiClient } from "./ApiClient";
import { ImapConfig } from "../types";

/**
 * Service for handling IMAP connection operations.
 * Manages the connection lifecycle to email servers.
 */
export class ConnectionService {
  private readonly apiClient = ApiClient.getInstance().getAxiosInstance();

  /**
   * Establishes a connection to an IMAP server using the provided configuration.
   *
   * @param config - IMAP server configuration including credentials
   * @returns Promise resolving to true if connection successful, false otherwise
   * @throws Error if connection fails with server error details
   */
  async connect(config: ImapConfig): Promise<boolean> {
    try {
      console.log(`Attempting to connect to ${config.server}:${config.port}`);

      await this.apiClient.post("/email/connect", config);

      console.log("Successfully connected to IMAP server");
      return true;
    } catch (error: any) {
      console.error(
        "Connection failed:",
        error.response?.data?.message || error.message
      );

      // Return false for authentication/connection failures, throw for unexpected errors
      if (error.response?.status === 400) {
        return false;
      }

      throw new Error(
        `Connection error: ${error.response?.data?.message || error.message}`
      );
    }
  }

  /**
   * Disconnects from the IMAP server and releases resources.
   *
   * @returns Promise that resolves when disconnection is complete
   */
  async disconnect(): Promise<void> {
    try {
      await this.apiClient.post("/email/disconnect");
      console.log("Successfully disconnected from IMAP server");
    } catch (error: any) {
      console.warn(
        "Disconnect warning:",
        error.response?.data?.message || error.message
      );
      // Don't throw on disconnect errors as the connection might already be closed
    }
  }
}

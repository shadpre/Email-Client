import axios, { AxiosInstance } from "axios";
/**
 * Base HTTP client configuration for the email client API.
 * Provides a centralized axios instance with common configuration.
 */
export class ApiClient {
  private static instance: ApiClient;
  private readonly axiosInstance: AxiosInstance;
  /** Base URL for the email client API */
  private static readonly API_BASE_URL = "http://localhost:5000/api";
  private constructor() {
    this.axiosInstance = axios.create({
      baseURL: ApiClient.API_BASE_URL,
      timeout: 60000, // 60 seconds timeout for large mailbox operations
      headers: {
        "Content-Type": "application/json",
      },
    });
    // Add request interceptor for logging (development only)
    this.axiosInstance.interceptors.request.use(
      (config) => {
        console.log(
          `API Request: ${config.method?.toUpperCase()} ${config.url}`
        );
        return config;
      },
      (error) => {
        console.error("API Request Error:", error);
        return Promise.reject(error);
      }
    );
    // Add response interceptor for error handling
    this.axiosInstance.interceptors.response.use(
      (response) => {
        return response;
      },
      (error) => {
        console.error(
          "API Response Error:",
          error.response?.data || error.message
        );
        return Promise.reject(error);
      }
    );
  }
  /**
   * Singleton pattern to ensure single axios instance across the application
   */
  public static getInstance(): ApiClient {
    if (!ApiClient.instance) {
      ApiClient.instance = new ApiClient();
    }
    return ApiClient.instance;
  }
  /**
   * Gets the configured axios instance for making HTTP requests
   */
  public getAxiosInstance(): AxiosInstance {
    return this.axiosInstance;
  }
}

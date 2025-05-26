/**
 * API Client Configuration
 * 
 * This file configures the base API client for making HTTP requests.
 * It provides a centralized way to handle API requests, responses, and errors.
 */

import { withAuthToken, isAuthError } from './authMiddleware';

// Base API URL - should be moved to environment variables in production
const API_BASE_URL = 'https://api.notesprint.com/v1';

// Default request timeout in milliseconds
const DEFAULT_TIMEOUT = 30000;

// HTTP request methods
type Method = 'GET' | 'POST' | 'PUT' | 'DELETE' | 'PATCH';

// Basic request options type
interface RequestOptions {
  method: Method;
  headers?: Record<string, string>;
  body?: any;
  timeout?: number;
}

// Response type
interface ApiResponse<T> {
  data: T | null;
  error: Error | null;
  status: number;
}

/**
 * Makes an API request to the specified endpoint
 * 
 * @param endpoint - The API endpoint to call (without the base URL)
 * @param options - Request options including method, headers, and body
 * @returns Promise with the response data or error
 */
export async function apiRequest<T>(
  endpoint: string,
  options: RequestOptions
): Promise<ApiResponse<T>> {
  const url = `${API_BASE_URL}${endpoint}`;
  
  // Default headers with auth token
  const baseHeaders = {
    'Content-Type': 'application/json',
    'Accept': 'application/json',
    ...options.headers,
  };
  
  // Add authentication token if available
  const headers = await withAuthToken(baseHeaders);

  // Request configuration
  const config: RequestInit = {
    method: options.method,
    headers,
    body: options.body ? JSON.stringify(options.body) : undefined,
  };

  try {
    // Create AbortController for timeout handling
    const controller = new AbortController();
    const timeoutId = setTimeout(
      () => controller.abort(),
      options.timeout || DEFAULT_TIMEOUT
    );
    
    config.signal = controller.signal;
    
    // Make the request
    const response = await fetch(url, config);
    
    // Clear timeout
    clearTimeout(timeoutId);
    
    // Parse response
    let data = null;
    try {
      data = await response.json();
    } catch (e) {
      // Response is not JSON or is empty
    }
    
    // Check for authentication errors
    if (isAuthError(response.status)) {
      // You could trigger a logout or token refresh here
      console.warn('Authentication error detected');
    }
    
    // Return formatted response
    return {
      data: response.ok ? data : null,
      error: response.ok ? null : new Error(data?.message || 'API request failed'),
      status: response.status,
    };
  } catch (error) {
    // Handle network errors or timeouts
    return {
      data: null,
      error: error instanceof Error ? error : new Error('Unknown error occurred'),
      status: 0,
    };
  }
}

/**
 * Convenience methods for different HTTP verbs
 */
export const api = {
  get: <T>(endpoint: string, headers?: Record<string, string>) => 
    apiRequest<T>(endpoint, { method: 'GET', headers }),
    
  post: <T>(endpoint: string, body: any, headers?: Record<string, string>) => 
    apiRequest<T>(endpoint, { method: 'POST', body, headers }),
    
  put: <T>(endpoint: string, body: any, headers?: Record<string, string>) => 
    apiRequest<T>(endpoint, { method: 'PUT', body, headers }),
    
  patch: <T>(endpoint: string, body: any, headers?: Record<string, string>) => 
    apiRequest<T>(endpoint, { method: 'PATCH', body, headers }),
    
  delete: <T>(endpoint: string, headers?: Record<string, string>) => 
    apiRequest<T>(endpoint, { method: 'DELETE', headers }),
};

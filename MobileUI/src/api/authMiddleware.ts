/**
 * Authentication Middleware
 * 
 * This file provides middleware functionality to inject authentication tokens
 * into API requests and handle authentication-related responses.
 */

import * as SecureStore from 'expo-secure-store';

// Token storage key
const TOKEN_KEY = 'notesprint_auth_token';

/**
 * Middleware to inject authentication token into request headers
 * 
 * @param headers - The current request headers
 * @returns Promise with updated headers including auth token if available
 */
export async function withAuthToken(headers: Record<string, string> = {}): Promise<Record<string, string>> {
  try {
    const token = await SecureStore.getItemAsync(TOKEN_KEY);
    
    if (token) {
      return {
        ...headers,
        'Authorization': `Bearer ${token}`,
      };
    }
    
    return headers;
  } catch (error) {
    console.error('Error retrieving auth token for request:', error);
    return headers;
  }
}

/**
 * Check if a response indicates an authentication error (401 Unauthorized)
 * 
 * @param status - HTTP status code from response
 * @returns Boolean indicating if authentication failed
 */
export function isAuthError(status: number): boolean {
  return status === 401;
}

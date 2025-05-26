/**
 * Authentication Service
 * 
 * This file provides authentication-related API endpoints and functionality.
 */

import { api } from './client';

// Response types
export interface LoginResponse {
  token: string;
  user: {
    id: string;
    email: string;
    username: string;
  };
}

export interface RegisterResponse {
  token: string;
  user: {
    id: string;
    email: string;
    username: string;
  };
}

// Request types
export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  username: string;
  password: string;
}

/**
 * Authentication service with methods for login, registration, etc.
 */
export const authService = {
  /**
   * Log in a user with email and password
   * 
   * @param credentials - User login credentials
   * @returns Promise with login response data
   */
  login: (credentials: LoginRequest) => 
    api.post<LoginResponse>('/auth/login', credentials),
  
  /**
   * Register a new user
   * 
   * @param userData - New user registration data
   * @returns Promise with registration response data
   */
  register: (userData: RegisterRequest) => 
    api.post<RegisterResponse>('/auth/register', userData),
  
  /**
   * Log out the current user
   * 
   * @returns Promise with logout response
   */
  logout: () => 
    api.post('/auth/logout', {}),
  
  /**
   * Check if the current authentication token is valid
   * 
   * @returns Promise with validation response
   */
  validateToken: () => 
    api.get('/auth/validate'),
};

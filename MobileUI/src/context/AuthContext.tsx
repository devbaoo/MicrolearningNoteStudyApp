import React, { createContext, useState, useContext, useEffect } from 'react';
import { router } from 'expo-router';
import * as SecureStore from 'expo-secure-store';

// Define the shape of the auth context
interface AuthContextType {
  isAuthenticated: boolean;
  isLoading: boolean;
  login: (token: string, userData: any) => Promise<void>;
  logout: () => Promise<void>;
  getToken: () => Promise<string | null>;
}

// Create the auth context
const AuthContext = createContext<AuthContextType>({
  isAuthenticated: false,
  isLoading: true,
  login: async () => {},
  logout: async () => {},
  getToken: async () => null,
});

// Token storage key
const TOKEN_KEY = 'notesprint_auth_token';
const USER_DATA_KEY = 'notesprint_user_data';

// Auth provider component
export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isLoading, setIsLoading] = useState(true);

  // Check for existing token on mount
  useEffect(() => {
    const checkAuth = async () => {
      try {
        const token = await SecureStore.getItemAsync(TOKEN_KEY);
        const isLoggedIn = !!token;
        setIsAuthenticated(isLoggedIn);
        
        // Force redirect to login when not authenticated
        if (!isLoggedIn) {
          router.replace('/login');
        }
      } catch (error) {
        console.error('Error checking authentication:', error);
        setIsAuthenticated(false);
        // On error, redirect to login for safety
        router.replace('/login');
      } finally {
        setIsLoading(false);
      }
    };

    checkAuth();
  }, []);
  
  // Add a separate effect to handle initial routing
  useEffect(() => {
    if (!isLoading) {
      if (!isAuthenticated) {
        router.replace('/login');
      }
    }
  }, [isLoading, isAuthenticated]);

  // Store token and set authenticated state
  const login = async (token: string, userData: any) => {
    try {
      await SecureStore.setItemAsync(TOKEN_KEY, token);
      await SecureStore.setItemAsync(USER_DATA_KEY, JSON.stringify(userData));
      setIsAuthenticated(true);
    } catch (error) {
      console.error('Error storing auth token:', error);
      throw error;
    }
  };

  // Remove token and clear authenticated state
  const logout = async () => {
    try {
      await SecureStore.deleteItemAsync(TOKEN_KEY);
      await SecureStore.deleteItemAsync(USER_DATA_KEY);
      setIsAuthenticated(false);
      router.replace('/login');
    } catch (error) {
      console.error('Error removing auth token:', error);
      throw error;
    }
  };

  // Get the current token
  const getToken = async () => {
    try {
      return await SecureStore.getItemAsync(TOKEN_KEY);
    } catch (error) {
      console.error('Error getting auth token:', error);
      return null;
    }
  };

  return (
    <AuthContext.Provider
      value={{
        isAuthenticated,
        isLoading,
        login,
        logout,
        getToken,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

// Hook for using the auth context
export function useAuth() {
  return useContext(AuthContext);
}

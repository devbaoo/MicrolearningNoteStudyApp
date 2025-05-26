/**
 * API Module Exports
 * 
 * This file exports all API-related functionality from the api directory.
 * It provides a clean interface for importing API services throughout the app.
 */

// Export the API client
export * from './client';

// Export API services
export * from './notesService';
export * from './authService';
export * from './authMiddleware';

// Add additional service exports here as they are created
// export * from './usersService';
// etc.

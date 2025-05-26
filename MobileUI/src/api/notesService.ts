/**
 * Notes API Service
 * 
 * This service handles all API requests related to notes in the application.
 * It uses the base API client for making HTTP requests.
 */

import { api } from './client';

// Type definitions for Notes
export interface Note {
  id: string;
  title: string;
  content: string;
  tags: string[];
  createdAt: string;
  updatedAt: string;
}

export interface CreateNoteRequest {
  title: string;
  content: string;
  tags?: string[];
}

export interface UpdateNoteRequest {
  title?: string;
  content?: string;
  tags?: string[];
}

// Notes API endpoints
const NOTES_ENDPOINT = '/notes';

/**
 * Notes API Service with methods for CRUD operations
 */
export const notesService = {
  /**
   * Get all notes for the current user
   */
  getAllNotes: async () => {
    const response = await api.get<Note[]>(NOTES_ENDPOINT);
    return response;
  },

  /**
   * Get a specific note by ID
   */
  getNoteById: async (noteId: string) => {
    const response = await api.get<Note>(`${NOTES_ENDPOINT}/${noteId}`);
    return response;
  },

  /**
   * Create a new note
   */
  createNote: async (noteData: CreateNoteRequest) => {
    const response = await api.post<Note>(NOTES_ENDPOINT, noteData);
    return response;
  },

  /**
   * Update an existing note
   */
  updateNote: async (noteId: string, noteData: UpdateNoteRequest) => {
    const response = await api.put<Note>(`${NOTES_ENDPOINT}/${noteId}`, noteData);
    return response;
  },

  /**
   * Delete a note
   */
  deleteNote: async (noteId: string) => {
    const response = await api.delete<void>(`${NOTES_ENDPOINT}/${noteId}`);
    return response;
  },

  /**
   * Search notes by query
   */
  searchNotes: async (query: string) => {
    const response = await api.get<Note[]>(`${NOTES_ENDPOINT}/search?q=${encodeURIComponent(query)}`);
    return response;
  },
};

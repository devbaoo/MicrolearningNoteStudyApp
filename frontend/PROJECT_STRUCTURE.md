# Microlearning Note Study App - Frontend Structure

This document provides an overview of the frontend project structure for the Microlearning Note Study App, explaining the purpose of each directory and file.

## Root Directory

- `package.json` - Contains project dependencies and scripts
- `next.config.js` - Next.js configuration file
- `tailwind.config.js` - Tailwind CSS configuration
- `tsconfig.json` - TypeScript configuration
- `.env.local` - Environment variables (not tracked in git)
- `.gitignore` - Specifies files to ignore in git
- `README.md` - Project documentation

## Public Directory

The `public` directory contains static assets that are served directly:

- `favicon.ico` - Website favicon
- `logo.png` - Application logo
- `icons/` - SVG icons used throughout the application
  - `brain.svg` - Brain icon for learning features
  - `notes.svg` - Notes icon
  - `plus.svg` - Add/create icon
  - `search.svg` - Search icon
  - `edit.svg` - Edit icon
  - `delete.svg` - Delete icon
  - `archive.svg` - Archive icon

## Source Directory (src)

### App Router (`src/app`)

Next.js 13+ App Router for file-based routing:

- `layout.tsx` - Root layout that wraps all pages
- `page.tsx` - Home page component
- `globals.css` - Global CSS styles

#### Auth Group (`src/app/(auth)`)

Authentication-related pages grouped together:

- `layout.tsx` - Layout for auth pages
- `login/page.tsx` - Login page
- `register/page.tsx` - Registration page

#### Notes Feature (`src/app/notes`)

All notes-related pages:

- `page.tsx` - Main notes listing page
- `create/page.tsx` - Create new note page
- `[id]/page.tsx` - View individual note by ID
- `[id]/edit/page.tsx` - Edit individual note by ID
- `search/page.tsx` - Search notes functionality
- `archived/page.tsx` - View archived notes

### Components (`src/components`)

Reusable React components organized by category:

#### UI Components (`src/components/ui`)

Base UI components used throughout the application:

- `Button.tsx` - Reusable button component
- `Input.tsx` - Form input component
- `Card.tsx` - Card container component
- `Modal.tsx` - Modal/dialog component
- `Loading.tsx` - Loading indicator
- `Pagination.tsx` - Pagination controls
- `TagInput.tsx` - Component for entering and managing tags
- `RichTextEditor.tsx` - Rich text editing component

#### Layout Components (`src/components/layout`)

Components that define the application layout:

- `Header.tsx` - Application header
- `Navigation.tsx` - Navigation menu
- `Footer.tsx` - Application footer
- `Sidebar.tsx` - Sidebar navigation

#### Auth Components (`src/components/auth`)

Authentication-related components:

- `LoginForm.tsx` - Login form component
- `RegisterForm.tsx` - Registration form component
- `AuthGuard.tsx` - Component to protect routes requiring authentication

#### Notes Components (`src/components/notes`)

Components specific to the notes feature:

- `NoteCard.tsx` - Card component for displaying a note preview
- `NoteList.tsx` - Component for displaying a list of notes
- `NoteEditor.tsx` - Form component for creating/editing notes
- `NotePreview.tsx` - Component for previewing a note
- `NoteSearch.tsx` - Search component for notes
- `NoteFilters.tsx` - Filtering options for notes
- `NoteTags.tsx` - Component for managing note tags
- `NoteStats.tsx` - Component for displaying note statistics
- `NoteActions.tsx` - Action buttons for notes (edit, delete, etc.)

### Types (`src/types`)

TypeScript type definitions:

- `index.ts` - Common shared types
- `auth.ts` - Authentication-related types
- `notes.ts` - Note-related types

### Lib (`src/lib`)

Utility functions and constants:

- `utils.ts` - General utility functions
- `constants.ts` - Application constants
- `validation.ts` - Form validation utilities
- `date.ts` - Date formatting and manipulation utilities

### Hooks (`src/hooks`)

Custom React hooks:

- `useLocalStorage.ts` - Hook for working with localStorage
- `useDebounce.ts` - Hook for debouncing values (useful for search)
- `useNotes.ts` - Hook for notes management

### Context (`src/context`)

React Context providers for state management:

- `AuthContext.tsx` - Authentication state management
- `NotesContext.tsx` - Notes state management

### Styles (`src/styles`)

Additional styles beyond Tailwind:

- `components.css` - Custom component styles

### Services (`src/services`)

Service modules for external interactions:

#### API Services (`src/services/api`)

API interaction layer:

- `index.ts` - Main export file for API services
- `config.ts` - API configuration (base URL, headers, etc.)
- `auth.ts` - Authentication API endpoints
- `notes.ts` - Notes API endpoints

## Architecture Overview

This project follows a modern Next.js architecture with:

1. **App Router** - File-based routing with nested layouts
2. **Component-Based Design** - Reusable components organized by function
3. **TypeScript** - Strong typing for better developer experience
4. **Context API** - State management for auth and notes
5. **Custom Hooks** - Encapsulated logic for reuse
6. **API Services** - Centralized API communication
7. **Tailwind CSS** - Utility-first styling

The structure is designed to be modular, maintainable, and scalable as the application grows.

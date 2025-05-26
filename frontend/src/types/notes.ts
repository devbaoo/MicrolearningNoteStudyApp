export interface Tag {
  id: string;
  name: string;
  color?: string;
}

export interface Note {
  id: string;
  title: string;
  content: string;
  tags: Tag[];
  isArchived: boolean;
  isPinned: boolean;
  createdAt: string;
  updatedAt: string;
  userId: string;
}

export interface NoteState {
  notes: Note[];
  currentNote: Note | null;
  isLoading: boolean;
  error: string | null;
}

export interface CreateNotePayload {
  title: string;
  content: string;
  tags?: string[];
}

export interface UpdateNotePayload {
  id: string;
  title?: string;
  content?: string;
  tags?: string[];
  isArchived?: boolean;
  isPinned?: boolean;
}

export interface NoteFilters {
  search?: string;
  tags?: string[];
  archived?: boolean;
  sortBy?: 'createdAt' | 'updatedAt' | 'title';
  sortDirection?: 'asc' | 'desc';
}
import React from 'react';
import { StyleSheet, ScrollView, View, TouchableOpacity } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { Ionicons } from '@expo/vector-icons';
import { router } from 'expo-router';

import { ThemedText } from '@/components/ThemedText';
import { ThemedView } from '@/components/ThemedView';
import { Colors } from '@/constants/Colors';
import { useColorScheme } from '@/hooks/useColorScheme';

export default function NotesScreen() {
  const insets = useSafeAreaInsets();
  const colorScheme = useColorScheme();
  const colors = Colors[colorScheme ?? 'light'];

  // Get color based on weight value
  const getWeightColor = (weight: number) => {
    if (weight >= 4) return '#2196F3'; // Blue
    if (weight >= 2) return '#FFC107'; // Yellow
    return '#F44336'; // Red
  };

  // Handle add note
  const handleAddNote = () => {
    router.push('/(modals)/add-note');
  };

  // Mock notes data
  const notes = [
    {
      id: 1,
      title: 'Introduction to Photosynthesis',
      preview: 'The process by which plants convert light into chemical energy...',
      date: '2 days ago',
      tags: ['Biology', 'Plants'],
      importance: 5, // 1-5 scale
      difficulty: 3, // 1-5 scale
    },
    {
      id: 2,
      title: 'Economic Principles: Inflation',
      preview: 'Inflation is a state of continuous increase in the general price level...',
      date: '1 week ago',
      tags: ['Economics', 'Finance'],
      importance: 4,
      difficulty: 4,
    },
    {
      id: 3,
      title: 'React Native Performance',
      preview: 'React Native uses a virtual DOM to improve performance...',
      date: 'Today',
      tags: ['Programming', 'React'],
      importance: 3,
      difficulty: 5,
    },
  ];

  return (
    <ScrollView
      style={[styles.container, { paddingTop: insets.top }]}
      contentContainerStyle={styles.contentContainer}
      showsVerticalScrollIndicator={false}
    >
      {/* Header */}
      <View style={styles.header}>
        <ThemedText type="title">Notes</ThemedText>
        <View style={styles.headerButtons}>
          <TouchableOpacity style={styles.headerButton}>
            <Ionicons name="search-outline" size={24} color={colors.tint} />
          </TouchableOpacity>
          <TouchableOpacity style={styles.headerButton}>
            <Ionicons name="filter-outline" size={24} color={colors.tint} />
          </TouchableOpacity>
        </View>
      </View>

      {/* Notes List */}
      {notes.map((note) => (
        <ThemedView key={note.id} style={styles.noteCard}>
          <View style={styles.noteTitleRow}>
            <ThemedText type="defaultSemiBold" style={styles.noteTitle}>
              {note.title}
            </ThemedText>
            <View style={styles.weightContainer}>
              <View style={styles.weightItem}>
                <Ionicons 
                  name="star" 
                  size={14} 
                  color={getWeightColor(note.importance)} 
                />
                <ThemedText style={[
                  styles.weightText, 
                  { color: getWeightColor(note.importance) }
                ]}>
                  {note.importance}
                </ThemedText>
              </View>
              <View style={styles.weightItem}>
                <Ionicons 
                  name="speedometer" 
                  size={14} 
                  color={getWeightColor(note.difficulty)} 
                />
                <ThemedText style={[
                  styles.weightText, 
                  { color: getWeightColor(note.difficulty) }
                ]}>
                  {note.difficulty}
                </ThemedText>
              </View>
            </View>
          </View>
          <ThemedText style={styles.notePreview} numberOfLines={2}>
            {note.preview}
          </ThemedText>
          <View style={styles.noteFooter}>
            <ThemedText style={styles.noteDate}>{note.date}</ThemedText>
            <View style={styles.tagContainer}>
              {note.tags.map((tag, index) => (
                <View key={index} style={styles.tag}>
                  <ThemedText style={styles.tagText}>{tag}</ThemedText>
                </View>
              ))}
            </View>
          </View>
        </ThemedView>
      ))}

      {/* Floating Action Button */}
      <TouchableOpacity style={styles.fab} onPress={handleAddNote}>
        <Ionicons name="add" size={24} color="white" />
      </TouchableOpacity>
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
  },
  contentContainer: {
    padding: 16,
    paddingBottom: 80, // Extra space for FAB
  },
  header: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 16,
  },
  headerButtons: {
    flexDirection: 'row',
  },
  headerButton: {
    marginLeft: 16,
  },
  noteCard: {
    borderRadius: 12,
    padding: 16,
    marginBottom: 16,
    shadowColor: "#000",
    shadowOffset: {
      width: 0,
      height: 2,
    },
    shadowOpacity: 0.1,
    shadowRadius: 3.84,
    elevation: 5,
  },
  noteTitleRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'flex-start',
    marginBottom: 8,
  },
  noteTitle: {
    fontSize: 16,
    flex: 1,
    marginRight: 8,
  },
  weightContainer: {
    flexDirection: 'row',
    alignItems: 'center',
  },
  weightItem: {
    flexDirection: 'row',
    alignItems: 'center',
    marginLeft: 8,
  },
  weightText: {
    fontSize: 12,
    fontWeight: 'bold',
    marginLeft: 2,
  },
  notePreview: {
    fontSize: 14,
    opacity: 0.7,
    marginBottom: 12,
  },
  noteFooter: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
  },
  noteDate: {
    fontSize: 12,
    opacity: 0.5,
  },
  tagContainer: {
    flexDirection: 'row',
  },
  tag: {
    backgroundColor: 'rgba(10, 126, 164, 0.1)',
    paddingHorizontal: 8,
    paddingVertical: 2,
    borderRadius: 4,
    marginLeft: 8,
  },
  tagText: {
    fontSize: 10,
    color: '#0a7ea4',
  },
  fab: {
    position: 'absolute',
    bottom: 20,
    right: 20,
    width: 56,
    height: 56,
    borderRadius: 28,
    backgroundColor: '#0a7ea4',
    justifyContent: 'center',
    alignItems: 'center',
    shadowColor: "#000",
    shadowOffset: {
      width: 0,
      height: 4,
    },
    shadowOpacity: 0.3,
    shadowRadius: 4.65,
    elevation: 8,
  },
}); 
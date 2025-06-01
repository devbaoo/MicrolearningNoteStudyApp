import React, { useState } from 'react';
import { StyleSheet, ScrollView, View, TouchableOpacity, TextInput, KeyboardAvoidingView, Platform } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { Ionicons } from '@expo/vector-icons';
import { router } from 'expo-router';
import Slider from '@react-native-community/slider';

import { ThemedText } from '@/components/ThemedText';
import { ThemedView } from '@/components/ThemedView';
import { Colors } from '@/constants/Colors';
import { useColorScheme } from '@/hooks/useColorScheme';

export default function AddNoteScreen() {
  const insets = useSafeAreaInsets();
  const colorScheme = useColorScheme();
  const colors = Colors[colorScheme ?? 'light'];
  
  // State for note content
  const [title, setTitle] = useState('');
  const [content, setContent] = useState('');
  const [tags, setTags] = useState<string[]>([]);
  const [tagInput, setTagInput] = useState('');
  const [importance, setImportance] = useState(3);
  const [difficulty, setDifficulty] = useState(3);
  const [noteMethod, setNoteMethod] = useState('standard'); // standard, cornell, zettelkasten
  
  // Handle adding a tag
  const handleAddTag = () => {
    if (tagInput.trim() && !tags.includes(tagInput.trim())) {
      setTags([...tags, tagInput.trim()]);
      setTagInput('');
    }
  };
  
  // Handle removing a tag
  const handleRemoveTag = (tagToRemove: string) => {
    setTags(tags.filter(tag => tag !== tagToRemove));
  };
  
  // Handle save note
  const handleSaveNote = () => {
    // Here you would save the note to your data store
    console.log({
      title,
      content,
      tags,
      importance,
      difficulty,
      noteMethod,
      createdAt: new Date().toISOString(),
    });
    
    // Navigate back to notes screen
    router.back();
  };
  
  // Handle cancel
  const handleCancel = () => {
    router.back();
  };
  
  // Get color based on weight value
  const getWeightColor = (weight: number) => {
    if (weight >= 4) return '#2196F3'; // Blue
    if (weight >= 2) return '#FFC107'; // Yellow
    return '#F44336'; // Red
  };

  return (
    <KeyboardAvoidingView
      style={{ flex: 1 }}
      behavior={Platform.OS === 'ios' ? 'padding' : 'height'}
    >
      <ScrollView
        style={[styles.container, { paddingTop: insets.top }]}
        contentContainerStyle={styles.contentContainer}
      >
        {/* Header */}
        <View style={styles.header}>
          <TouchableOpacity onPress={handleCancel}>
            <Ionicons name="close" size={24} color={colors.text} />
          </TouchableOpacity>
          <ThemedText type="title">New Note</ThemedText>
          <TouchableOpacity onPress={handleSaveNote}>
            <ThemedText style={{ color: colors.tint, fontWeight: 'bold' }}>Save</ThemedText>
          </TouchableOpacity>
        </View>
        
        {/* Note Method Selector */}
        <View style={styles.methodSelector}>
          <TouchableOpacity 
            style={[
              styles.methodOption, 
              noteMethod === 'standard' && styles.methodOptionSelected
            ]}
            onPress={() => setNoteMethod('standard')}
          >
            <Ionicons 
              name="document-text-outline" 
              size={20} 
              color={noteMethod === 'standard' ? colors.tint : colors.text} 
            />
            <ThemedText 
              style={[
                styles.methodText, 
                noteMethod === 'standard' && { color: colors.tint }
              ]}
            >
              Standard
            </ThemedText>
          </TouchableOpacity>
          
          <TouchableOpacity 
            style={[
              styles.methodOption, 
              noteMethod === 'cornell' && styles.methodOptionSelected
            ]}
            onPress={() => setNoteMethod('cornell')}
          >
            <Ionicons 
              name="grid-outline" 
              size={20} 
              color={noteMethod === 'cornell' ? colors.tint : colors.text} 
            />
            <ThemedText 
              style={[
                styles.methodText, 
                noteMethod === 'cornell' && { color: colors.tint }
              ]}
            >
              Cornell
            </ThemedText>
          </TouchableOpacity>
          
          <TouchableOpacity 
            style={[
              styles.methodOption, 
              noteMethod === 'zettelkasten' && styles.methodOptionSelected
            ]}
            onPress={() => setNoteMethod('zettelkasten')}
          >
            <Ionicons 
              name="git-network-outline" 
              size={20} 
              color={noteMethod === 'zettelkasten' ? colors.tint : colors.text} 
            />
            <ThemedText 
              style={[
                styles.methodText, 
                noteMethod === 'zettelkasten' && { color: colors.tint }
              ]}
            >
              Zettelkasten
            </ThemedText>
          </TouchableOpacity>
        </View>
        
        {/* Title Input */}
        <ThemedView style={styles.inputContainer}>
          <TextInput
            style={styles.titleInput}
            placeholder="Title"
            placeholderTextColor={colorScheme === 'dark' ? '#888' : '#aaa'}
            value={title}
            onChangeText={setTitle}
            maxLength={100}
          />
        </ThemedView>
        
        {/* Content Input */}
        <ThemedView style={[styles.inputContainer, styles.noteContentContainer]}>
          {noteMethod === 'cornell' ? (
            <View style={styles.cornellContainer}>
              <View style={styles.cornellCues}>
                <ThemedText style={styles.cornellLabel}>Cues/Questions</ThemedText>
                <TextInput
                  style={styles.cornellCuesInput}
                  placeholder="Write questions/cues here..."
                  placeholderTextColor={colorScheme === 'dark' ? '#888' : '#aaa'}
                  multiline
                />
              </View>
              <View style={styles.cornellNotes}>
                <ThemedText style={styles.cornellLabel}>Notes</ThemedText>
                <TextInput
                  style={styles.contentInput}
                  placeholder="Start writing your note..."
                  placeholderTextColor={colorScheme === 'dark' ? '#888' : '#aaa'}
                  multiline
                  value={content}
                  onChangeText={setContent}
                />
              </View>
              <View style={styles.cornellSummary}>
                <ThemedText style={styles.cornellLabel}>Summary</ThemedText>
                <TextInput
                  style={styles.cornellSummaryInput}
                  placeholder="Summarize your notes here..."
                  placeholderTextColor={colorScheme === 'dark' ? '#888' : '#aaa'}
                  multiline
                />
              </View>
            </View>
          ) : noteMethod === 'zettelkasten' ? (
            <View style={styles.zettelkastenContainer}>
              <View style={styles.zettelId}>
                <ThemedText style={styles.zettelLabel}>ID: {new Date().getTime().toString().slice(-6)}</ThemedText>
              </View>
              <TextInput
                style={styles.contentInput}
                placeholder="Write one concise idea per note..."
                placeholderTextColor={colorScheme === 'dark' ? '#888' : '#aaa'}
                multiline
                value={content}
                onChangeText={setContent}
              />
              <View style={styles.zettelLinks}>
                <ThemedText style={styles.zettelLabel}>Links to other notes</ThemedText>
                <TouchableOpacity style={styles.addLinkButton}>
                  <Ionicons name="add-circle-outline" size={20} color={colors.tint} />
                  <ThemedText style={{ color: colors.tint, marginLeft: 4 }}>Add Link</ThemedText>
                </TouchableOpacity>
              </View>
            </View>
          ) : (
            <TextInput
              style={styles.contentInput}
              placeholder="Start writing your note..."
              placeholderTextColor={colorScheme === 'dark' ? '#888' : '#aaa'}
              multiline
              value={content}
              onChangeText={setContent}
            />
          )}
        </ThemedView>
        
        {/* Tags Input */}
        <ThemedView style={styles.inputContainer}>
          <View style={styles.tagsInputContainer}>
            <TextInput
              style={styles.tagInput}
              placeholder="Add tags..."
              placeholderTextColor={colorScheme === 'dark' ? '#888' : '#aaa'}
              value={tagInput}
              onChangeText={setTagInput}
              onSubmitEditing={handleAddTag}
              returnKeyType="done"
            />
            <TouchableOpacity onPress={handleAddTag}>
              <Ionicons name="add-circle" size={24} color={colors.tint} />
            </TouchableOpacity>
          </View>
          
          <View style={styles.tagsContainer}>
            {tags.map((tag, index) => (
              <View key={index} style={styles.tag}>
                <ThemedText style={styles.tagText}>{tag}</ThemedText>
                <TouchableOpacity onPress={() => handleRemoveTag(tag)}>
                  <Ionicons name="close-circle" size={16} color="#0a7ea4" />
                </TouchableOpacity>
              </View>
            ))}
          </View>
        </ThemedView>
        
        {/* Importance and Difficulty Sliders */}
        <ThemedView style={styles.inputContainer}>
          <View style={styles.sliderContainer}>
            <View style={styles.sliderHeader}>
              <ThemedText>Importance</ThemedText>
              <ThemedText style={{ color: getWeightColor(importance), fontWeight: 'bold' }}>
                {importance}
              </ThemedText>
            </View>
            <Slider
              style={styles.slider}
              minimumValue={1}
              maximumValue={5}
              step={1}
              value={importance}
              onValueChange={setImportance}
              minimumTrackTintColor={getWeightColor(importance)}
              maximumTrackTintColor="#ddd"
              thumbTintColor={getWeightColor(importance)}
            />
          </View>
          
          <View style={styles.sliderContainer}>
            <View style={styles.sliderHeader}>
              <ThemedText>Difficulty</ThemedText>
              <ThemedText style={{ color: getWeightColor(difficulty), fontWeight: 'bold' }}>
                {difficulty}
              </ThemedText>
            </View>
            <Slider
              style={styles.slider}
              minimumValue={1}
              maximumValue={5}
              step={1}
              value={difficulty}
              onValueChange={setDifficulty}
              minimumTrackTintColor={getWeightColor(difficulty)}
              maximumTrackTintColor="#ddd"
              thumbTintColor={getWeightColor(difficulty)}
            />
          </View>
        </ThemedView>
        
        {/* AI Suggestions */}
        <ThemedView style={styles.aiSuggestions}>
          <View style={styles.aiHeader}>
            <Ionicons name="bulb-outline" size={20} color="#FFC107" />
            <ThemedText style={styles.aiTitle}>AI Suggestions</ThemedText>
          </View>
          <ThemedText style={styles.aiText}>
            Add specific examples to increase knowledge density.
          </ThemedText>
          <ThemedText style={styles.aiText}>
            Consider linking this note to &apos;Economic Principles: Inflation&apos;.
          </ThemedText>
        </ThemedView>
      </ScrollView>
    </KeyboardAvoidingView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
  },
  contentContainer: {
    padding: 16,
    paddingBottom: 32,
  },
  header: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 16,
  },
  methodSelector: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    marginBottom: 16,
  },
  methodOption: {
    flexDirection: 'row',
    alignItems: 'center',
    paddingVertical: 8,
    paddingHorizontal: 12,
    borderRadius: 8,
    backgroundColor: 'rgba(0, 0, 0, 0.05)',
  },
  methodOptionSelected: {
    backgroundColor: 'rgba(10, 126, 164, 0.1)',
  },
  methodText: {
    marginLeft: 4,
  },
  inputContainer: {
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
  titleInput: {
    fontSize: 18,
    fontWeight: 'bold',
    padding: 0,
  },
  noteContentContainer: {
    minHeight: 200,
  },
  contentInput: {
    flex: 1,
    fontSize: 16,
    padding: 0,
    textAlignVertical: 'top',
  },
  cornellContainer: {
    flex: 1,
  },
  cornellCues: {
    borderBottomWidth: 1,
    borderBottomColor: 'rgba(0, 0, 0, 0.1)',
    paddingBottom: 8,
    marginBottom: 8,
  },
  cornellLabel: {
    fontSize: 12,
    opacity: 0.7,
    marginBottom: 4,
  },
  cornellCuesInput: {
    fontSize: 14,
    padding: 0,
    minHeight: 60,
    textAlignVertical: 'top',
  },
  cornellNotes: {
    flex: 1,
    minHeight: 120,
    marginBottom: 8,
  },
  cornellSummary: {
    borderTopWidth: 1,
    borderTopColor: 'rgba(0, 0, 0, 0.1)',
    paddingTop: 8,
  },
  cornellSummaryInput: {
    fontSize: 14,
    padding: 0,
    minHeight: 60,
    textAlignVertical: 'top',
  },
  zettelkastenContainer: {
    flex: 1,
  },
  zettelId: {
    marginBottom: 8,
  },
  zettelLabel: {
    fontSize: 12,
    opacity: 0.7,
    marginBottom: 4,
  },
  zettelLinks: {
    marginTop: 8,
  },
  addLinkButton: {
    flexDirection: 'row',
    alignItems: 'center',
    marginTop: 4,
  },
  tagsInputContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: 8,
  },
  tagInput: {
    flex: 1,
    fontSize: 16,
    padding: 0,
    marginRight: 8,
  },
  tagsContainer: {
    flexDirection: 'row',
    flexWrap: 'wrap',
  },
  tag: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: 'rgba(10, 126, 164, 0.1)',
    paddingHorizontal: 8,
    paddingVertical: 4,
    borderRadius: 4,
    marginRight: 8,
    marginBottom: 8,
  },
  tagText: {
    fontSize: 12,
    color: '#0a7ea4',
    marginRight: 4,
  },
  sliderContainer: {
    marginBottom: 16,
  },
  sliderHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    marginBottom: 8,
  },
  slider: {
    width: '100%',
    height: 40,
  },
  aiSuggestions: {
    borderRadius: 12,
    padding: 16,
    marginBottom: 16,
    backgroundColor: 'rgba(255, 193, 7, 0.05)',
    borderLeftWidth: 3,
    borderLeftColor: '#FFC107',
  },
  aiHeader: {
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: 8,
  },
  aiTitle: {
    fontSize: 16,
    fontWeight: 'bold',
    marginLeft: 8,
  },
  aiText: {
    fontSize: 14,
    marginBottom: 4,
  },
}); 
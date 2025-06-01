import React, { useState } from 'react';
import { StyleSheet, View, TouchableOpacity } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { Ionicons } from '@expo/vector-icons';

import { ThemedText } from '@/components/ThemedText';
import { ThemedView } from '@/components/ThemedView';
import { Colors } from '@/constants/Colors';
import { useColorScheme } from '@/hooks/useColorScheme';

export default function ReviewScreen() {
  const insets = useSafeAreaInsets();
  const colorScheme = useColorScheme();
  const colors = Colors[colorScheme ?? 'light'];
  const [currentCardIndex, setCurrentCardIndex] = useState(0);

  // Mock review items
  const reviewItems = [
    {
      id: 1,
      question: "What is photosynthesis?",
      answer: "Photosynthesis is the process by which plants convert light energy into chemical energy that can later be released to fuel the organism's activities.",
      difficulty: 'medium',
    },
    {
      id: 2,
      question: "What causes inflation?",
      answer: "Inflation can be caused by demand-pull factors (when demand exceeds supply) or cost-push factors (when production costs increase).",
      difficulty: 'hard',
    },
    {
      id: 3,
      question: "How does React Native improve performance?",
      answer: "React Native uses a virtual DOM to minimize direct manipulation of the native UI elements, which improves performance by batching updates.",
      difficulty: 'easy',
    },
  ];

  const currentItem = reviewItems[currentCardIndex];

  // Handle next card
  const handleNextCard = () => {
    if (currentCardIndex < reviewItems.length - 1) {
      setCurrentCardIndex(currentCardIndex + 1);
    }
  };

  // Handle previous card
  const handlePrevCard = () => {
    if (currentCardIndex > 0) {
      setCurrentCardIndex(currentCardIndex - 1);
    }
  };

  return (
    <View style={[styles.container, { paddingTop: insets.top }]}>
      {/* Header */}
      <View style={styles.header}>
        <ThemedText type="title">Review Session</ThemedText>
        <View style={styles.progressContainer}>
          <ThemedText style={styles.progressText}>
            {currentCardIndex + 1}/{reviewItems.length}
          </ThemedText>
        </View>
      </View>

      {/* Review Card */}
      <ThemedView style={styles.reviewCard}>
        <View style={styles.cardHeader}>
          <ThemedText style={styles.cardNumber}>Card {currentCardIndex + 1}</ThemedText>
          <View 
            style={[
              styles.difficultyBadge, 
              { 
                backgroundColor: 
                  currentItem.difficulty === 'easy' 
                    ? 'rgba(76, 175, 80, 0.1)' 
                    : currentItem.difficulty === 'medium'
                    ? 'rgba(255, 152, 0, 0.1)'
                    : 'rgba(244, 67, 54, 0.1)'
              }
            ]}
          >
            <ThemedText 
              style={[
                styles.difficultyText,
                {
                  color: 
                    currentItem.difficulty === 'easy' 
                      ? '#4CAF50' 
                      : currentItem.difficulty === 'medium'
                      ? '#FF9800'
                      : '#F44336'
                }
              ]}
            >
              {currentItem.difficulty.charAt(0).toUpperCase() + currentItem.difficulty.slice(1)}
            </ThemedText>
          </View>
        </View>

        <ThemedText type="subtitle" style={styles.questionText}>
          {currentItem.question}
        </ThemedText>

        <View style={styles.separator} />

        <ThemedText style={styles.answerText}>
          {currentItem.answer}
        </ThemedText>
      </ThemedView>

      {/* Rating Buttons */}
      <View style={styles.ratingContainer}>
        <ThemedText type="subtitle" style={styles.ratingTitle}>
          How well did you remember this?
        </ThemedText>
        
        <View style={styles.ratingButtonsContainer}>
          <TouchableOpacity 
            style={[styles.ratingButton, { backgroundColor: 'rgba(244, 67, 54, 0.1)' }]}
            onPress={handleNextCard}
          >
            <ThemedText style={[styles.ratingButtonText, { color: '#F44336' }]}>
              Forgot
            </ThemedText>
          </TouchableOpacity>
          
          <TouchableOpacity 
            style={[styles.ratingButton, { backgroundColor: 'rgba(255, 152, 0, 0.1)' }]}
            onPress={handleNextCard}
          >
            <ThemedText style={[styles.ratingButtonText, { color: '#FF9800' }]}>
              Hard
            </ThemedText>
          </TouchableOpacity>
          
          <TouchableOpacity 
            style={[styles.ratingButton, { backgroundColor: 'rgba(76, 175, 80, 0.1)' }]}
            onPress={handleNextCard}
          >
            <ThemedText style={[styles.ratingButtonText, { color: '#4CAF50' }]}>
              Good
            </ThemedText>
          </TouchableOpacity>
          
          <TouchableOpacity 
            style={[styles.ratingButton, { backgroundColor: 'rgba(33, 150, 243, 0.1)' }]}
            onPress={handleNextCard}
          >
            <ThemedText style={[styles.ratingButtonText, { color: '#2196F3' }]}>
              Easy
            </ThemedText>
          </TouchableOpacity>
        </View>
      </View>

      {/* Navigation Buttons */}
      <View style={styles.navigationContainer}>
        <TouchableOpacity 
          style={[styles.navButton, { opacity: currentCardIndex === 0 ? 0.5 : 1 }]}
          onPress={handlePrevCard}
          disabled={currentCardIndex === 0}
        >
          <Ionicons name="chevron-back" size={24} color={colors.tint} />
          <ThemedText style={styles.navButtonText}>Previous</ThemedText>
        </TouchableOpacity>
        
        <TouchableOpacity 
          style={[styles.navButton, { opacity: currentCardIndex === reviewItems.length - 1 ? 0.5 : 1 }]}
          onPress={handleNextCard}
          disabled={currentCardIndex === reviewItems.length - 1}
        >
          <ThemedText style={styles.navButtonText}>Next</ThemedText>
          <Ionicons name="chevron-forward" size={24} color={colors.tint} />
        </TouchableOpacity>
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    padding: 16,
  },
  header: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 24,
  },
  progressContainer: {
    backgroundColor: 'rgba(10, 126, 164, 0.1)',
    paddingHorizontal: 12,
    paddingVertical: 6,
    borderRadius: 16,
  },
  progressText: {
    color: '#0a7ea4',
    fontWeight: '600',
  },
  reviewCard: {
    borderRadius: 12,
    padding: 20,
    marginBottom: 24,
    shadowColor: "#000",
    shadowOffset: {
      width: 0,
      height: 2,
    },
    shadowOpacity: 0.1,
    shadowRadius: 3.84,
    elevation: 5,
  },
  cardHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 16,
  },
  cardNumber: {
    fontSize: 14,
    opacity: 0.6,
  },
  difficultyBadge: {
    paddingHorizontal: 8,
    paddingVertical: 4,
    borderRadius: 4,
  },
  difficultyText: {
    fontSize: 12,
    fontWeight: '500',
  },
  questionText: {
    fontSize: 18,
    marginBottom: 20,
  },
  separator: {
    height: 1,
    backgroundColor: 'rgba(0, 0, 0, 0.1)',
    marginVertical: 16,
  },
  answerText: {
    fontSize: 16,
    lineHeight: 24,
  },
  ratingContainer: {
    marginBottom: 24,
  },
  ratingTitle: {
    marginBottom: 16,
    textAlign: 'center',
  },
  ratingButtonsContainer: {
    flexDirection: 'row',
    justifyContent: 'space-between',
  },
  ratingButton: {
    flex: 1,
    paddingVertical: 12,
    marginHorizontal: 4,
    borderRadius: 8,
    alignItems: 'center',
  },
  ratingButtonText: {
    fontWeight: '600',
  },
  navigationContainer: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    marginTop: 'auto',
  },
  navButton: {
    flexDirection: 'row',
    alignItems: 'center',
    padding: 8,
  },
  navButtonText: {
    color: '#0a7ea4',
    fontWeight: '600',
    marginHorizontal: 4,
  },
}); 
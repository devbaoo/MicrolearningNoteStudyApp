import React from 'react';
import { StyleSheet, View } from 'react-native';

import { ThemedText } from './ThemedText';
import { ThemedView } from './ThemedView';

interface KnowledgeHealthCardProps {
  healthPercentage: number;
}

export function KnowledgeHealthCard({ healthPercentage }: KnowledgeHealthCardProps) {
  // Determine color based on health percentage
  const getHealthColor = () => {
    if (healthPercentage >= 80) return '#4CAF50'; // Green
    if (healthPercentage >= 60) return '#FFC107'; // Yellow
    return '#F44336'; // Red
  };
  
  // Generate health message based on percentage
  const getHealthMessage = () => {
    if (healthPercentage >= 80) {
      return "Great job! Your knowledge retention is strong.";
    } else if (healthPercentage >= 60) {
      return "Good progress! Some knowledge areas could use review.";
    } else {
      return "Time to review! Several knowledge areas are at risk of being forgotten.";
    }
  };
  
  return (
    <ThemedView style={styles.card}>
      <ThemedText type="subtitle">Knowledge Health</ThemedText>
      
      <View style={styles.healthContainer}>
        <View style={styles.progressContainer}>
          <View 
            style={[
              styles.progressBar, 
              { width: `${healthPercentage}%`, backgroundColor: getHealthColor() }
            ]} 
          />
        </View>
        <ThemedText type="defaultSemiBold" style={styles.percentage}>{healthPercentage}%</ThemedText>
      </View>
      
      <ThemedText style={styles.message}>
        {getHealthMessage()}
      </ThemedText>
      
      <View style={styles.statsContainer}>
        <View style={styles.statItem}>
          <ThemedText style={styles.statValue}>
            {Math.round(healthPercentage / 10)}
          </ThemedText>
          <ThemedText style={styles.statLabel}>Topics Mastered</ThemedText>
        </View>
        
        <View style={styles.statItem}>
          <ThemedText style={styles.statValue}>
            {Math.round((100 - healthPercentage) / 20)}
          </ThemedText>
          <ThemedText style={styles.statLabel}>Need Review</ThemedText>
        </View>
        
        <View style={styles.statItem}>
          <ThemedText style={styles.statValue}>
            {Math.round(healthPercentage / 25) + 1}
          </ThemedText>
          <ThemedText style={styles.statLabel}>Days Streak</ThemedText>
        </View>
      </View>
    </ThemedView>
  );
}

const styles = StyleSheet.create({
  card: {
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
  healthContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    marginVertical: 12,
  },
  progressContainer: {
    flex: 1,
    height: 12,
    backgroundColor: 'rgba(0, 0, 0, 0.1)',
    borderRadius: 6,
    marginRight: 10,
  },
  progressBar: {
    height: 12,
    borderRadius: 6,
  },
  percentage: {
    fontSize: 16,
    fontWeight: 'bold',
  },
  message: {
    fontSize: 14,
    opacity: 0.7,
    marginBottom: 16,
  },
  statsContainer: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    borderTopWidth: StyleSheet.hairlineWidth,
    borderTopColor: 'rgba(0, 0, 0, 0.1)',
    paddingTop: 16,
  },
  statItem: {
    alignItems: 'center',
    flex: 1,
  },
  statValue: {
    fontSize: 18,
    fontWeight: 'bold',
    marginBottom: 4,
  },
  statLabel: {
    fontSize: 12,
    opacity: 0.7,
  },
}); 
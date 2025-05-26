import React from 'react';
import { StyleSheet, View, TouchableOpacity } from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { router } from 'expo-router';

import { ThemedText } from './ThemedText';
import { ThemedView } from './ThemedView';
import { Colors } from '@/constants/Colors';
import { useColorScheme } from '@/hooks/useColorScheme';

interface HomeStatsSectionProps {
  optimalStudyTime: string;
  atomsCreated: number;
  retentionRate: number;
  knowledgeHealth: number;
}

export function HomeStatsSection({
  optimalStudyTime,
  atomsCreated,
  retentionRate,
  knowledgeHealth,
}: HomeStatsSectionProps) {
  const colorScheme = useColorScheme();
  const colors = Colors[colorScheme ?? 'light'];

  // Determine health status text
  const getHealthStatus = () => {
    if (knowledgeHealth >= 80) return 'Healthy';
    if (knowledgeHealth >= 60) return 'Good';
    return 'Needs Attention';
  };

  // Determine health color
  const getHealthColor = () => {
    if (knowledgeHealth >= 80) return '#2196F3'; // Blue
    if (knowledgeHealth >= 60) return '#FFC107'; // Yellow
    return '#F44336'; // Red
  };

  return (
    <>
      {/* Optimal Study Time Card */}
      <ThemedView style={[styles.card, styles.studyTimeCard]}>
        <View style={styles.studyTimeContent}>
          <View style={styles.alarmIconContainer}>
            <Ionicons name="alarm" size={24} color="#FF5252" />
          </View>
          <View style={styles.studyTimeTextContainer}>
            <ThemedText type="subtitle" style={styles.studyTimeTitle}>
              Peak Study Time
            </ThemedText>
            <ThemedText style={styles.studyTimeValue}>
              {optimalStudyTime}
            </ThemedText>
          </View>
        </View>
      </ThemedView>

      {/* Learning Stats Card */}
      <ThemedView style={styles.card}>
        <View style={styles.headerContainer}>
          <ThemedText type="subtitle">Learning Stats</ThemedText>
          <TouchableOpacity 
            style={styles.viewAllButton}
            onPress={() => router.push('/(tabs)/profile#learningStats')}
          >
            <ThemedText style={[styles.viewAllText, { color: colors.tint }]}>
              View all
            </ThemedText>
          </TouchableOpacity>
        </View>

        <View style={styles.statsGrid}>
          <ThemedView style={styles.statCard}>
            <ThemedText style={styles.statValue}>{atomsCreated}</ThemedText>
            <ThemedText style={styles.statLabel}>Atoms</ThemedText>
          </ThemedView>

          <ThemedView style={styles.statCard}>
            <ThemedText style={styles.statValue}>{retentionRate}%</ThemedText>
            <ThemedText style={styles.statLabel}>Retention</ThemedText>
          </ThemedView>
        </View>

        <View style={styles.knowledgeHealthContainer}>
          <View style={styles.knowledgeHealthHeader}>
            <ThemedText style={styles.knowledgeHealthTitle}>
            Knowledge Health
            </ThemedText>
            <ThemedText style={[styles.knowledgeHealthStatus, { color: getHealthColor() }]}>
              {knowledgeHealth}%
            </ThemedText>
          </View>
          
          <View style={styles.progressContainer}>
            <View 
              style={[
                styles.progressBar, 
                { width: `${knowledgeHealth}%`, backgroundColor: getHealthColor() }
              ]} 
            />
          </View>
        </View>
      </ThemedView>
    </>
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
  studyTimeCard: {
    backgroundColor: '#2196F3', // Blue color
  },
  studyTimeContent: {
    flexDirection: 'row',
    alignItems: 'center',
  },
  alarmIconContainer: {
    width: 40,
    height: 40,
    borderRadius: 8,
    backgroundColor: 'rgba(255, 255, 255, 0.2)',
    justifyContent: 'center',
    alignItems: 'center',
    marginRight: 12,
  },
  studyTimeTextContainer: {
    flex: 1,
  },
  studyTimeTitle: {
    color: 'white',
    marginBottom: 4,
  },
  studyTimeValue: {
    color: 'white',
    opacity: 0.9,
  },
  headerContainer: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 16,
  },
  viewAllButton: {
    padding: 4,
  },
  viewAllText: {
    fontWeight: '600',
  },
  statsGrid: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    marginBottom: 16,
  },
  statCard: {
    width: '48%',
    padding: 16,
    borderRadius: 8,
    alignItems: 'center',
    backgroundColor: 'rgba(0, 0, 0, 0.02)',
  },
  statValue: {
    fontSize: 28,
    fontWeight: 'bold',
    color: '#2196F3', // Blue color
    marginBottom: 4,
  },
  statLabel: {
    fontSize: 14,
    opacity: 0.7,
  },
  knowledgeHealthContainer: {
    marginTop: 8,
  },
  knowledgeHealthHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 8,
  },
  knowledgeHealthTitle: {
    fontSize: 16,
    fontWeight: '500',
  },
  knowledgeHealthStatus: {
    fontSize: 14,
    fontWeight: '600',
  },
  progressContainer: {
    height: 8,
    backgroundColor: 'rgba(0, 0, 0, 0.05)',
    borderRadius: 4,
    overflow: 'hidden',
  },
  progressBar: {
    height: 8,
    borderRadius: 4,
  },
}); 
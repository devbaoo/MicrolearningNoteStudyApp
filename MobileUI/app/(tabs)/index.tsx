import React from 'react';
import { StyleSheet, ScrollView, TouchableOpacity, View } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { Ionicons } from '@expo/vector-icons';
import { router } from 'expo-router';

import { ThemedText } from '@/components/ThemedText';
import { HomeStatsSection } from '@/components/HomeStatsSection';
import { ReviewCard } from '@/components/ReviewCard';
import { SuggestedConnectionsCard } from '@/components/SuggestedConnectionsCard';
import { Colors } from '@/constants/Colors';
import { useColorScheme } from '@/hooks/useColorScheme';

export default function HomeScreen() {
  const insets = useSafeAreaInsets();
  const colorScheme = useColorScheme();
  const colors = Colors[colorScheme ?? 'light'];
  
  // Mock data for today's review
  const todaysReviewAtoms = [
    { id: 1, content: "Photosynthesis is the process by which plants convert light into chemical energy" },
    { id: 2, content: "Inflation is a state of continuous increase in the general price level of the economy" },
    { id: 3, content: "React Native uses a virtual DOM to improve performance" },
  ];
  
  // Mock data for knowledge stats
  const knowledgeStats = {
    optimalStudyTime: "7:30 AM",
    atomsCreated: 247,
    retentionRate: 83,
    knowledgeHealth: 78
  };
  
  // Mock data for suggested connections
  const suggestedConnections = [
    { id: 1, title: "Marketing ROI and Attribution Models", confidence: 87 },
    { id: 2, title: "Supply-Demand and Inflation", confidence: 92 },
  ];

  // Handle atom press
  const handleAtomPress = (atomId: number) => {
    console.log(`Atom pressed: ${atomId}`);
    // Navigate to atom detail screen
    // router.push(`/atom/${atomId}`);
  };

  // Handle start review session
  const handleStartReview = () => {
    console.log('Start review session');
    // Navigate to review session screen
    router.push('/(tabs)/review');
  };

  // Handle connection press
  const handleConnectionPress = (connectionId: number) => {
    console.log(`Connection pressed: ${connectionId}`);
    // Navigate to connection detail screen
    // router.push(`/connection/${connectionId}`);
  };

  return (
    <ScrollView 
      style={[styles.container, { paddingTop: insets.top }]}
      contentContainerStyle={styles.contentContainer}
      showsVerticalScrollIndicator={false}
    >
      {/* Header */}
      <View style={styles.header}>
        <View>
          <ThemedText type="title">NoteSprint</ThemedText>
          <ThemedText style={styles.subtitle}>Your Knowledge Dashboard</ThemedText>
        </View>
        <TouchableOpacity style={styles.profileButton} onPress={() => router.push('/(tabs)/profile')}>
          <Ionicons name="person-circle-outline" size={40} color={colors.tint} />
        </TouchableOpacity>
      </View>

      {/* Stats Section */}
      <HomeStatsSection 
        optimalStudyTime={knowledgeStats.optimalStudyTime}
        atomsCreated={knowledgeStats.atomsCreated}
        retentionRate={knowledgeStats.retentionRate}
        knowledgeHealth={knowledgeStats.knowledgeHealth}
      />

      {/* Today's Review Card */}
      <ReviewCard 
        atoms={todaysReviewAtoms} 
        onStartReview={handleStartReview} 
        onAtomPress={handleAtomPress}
      />

      {/* Suggested Connections Card */}
      <SuggestedConnectionsCard 
        connections={suggestedConnections} 
        onConnectionPress={handleConnectionPress}
      />
    </ScrollView>
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
  subtitle: {
    fontSize: 14,
    opacity: 0.7,
    marginTop: 4,
  },
  profileButton: {
    padding: 4,
  },
});

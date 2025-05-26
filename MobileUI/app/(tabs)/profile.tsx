import React from 'react';
import { StyleSheet, ScrollView, View, TouchableOpacity, Image } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { Ionicons } from '@expo/vector-icons';

import { ThemedText } from '@/components/ThemedText';
import { ThemedView } from '@/components/ThemedView';
import { Colors } from '@/constants/Colors';
import { useColorScheme } from '@/hooks/useColorScheme';

export default function ProfileScreen() {
  const insets = useSafeAreaInsets();
  const colorScheme = useColorScheme();
  const colors = Colors[colorScheme ?? 'light'];
  
  // Mock user data
  const user = {
    name: 'Alex Johnson',
    email: 'alex.johnson@example.com',
    avatar: 'https://i.pravatar.cc/150?img=12',
    streak: 12,
    totalNotes: 47,
    knowledgeItems: 183,
    retention: 82,
  };
  
  

  // Mock detailed learning stats data
  const detailedStats = {
    optimalStudyTime: "7:30 AM",
    atomsCreated: 247,
    retentionRate: 83,
    knowledgeHealth: 78,
    totalStudyTime: "42h 15m",
    averageDailyStudy: "45m",
    streak: 12,
    topicsMastered: 8,
    topicsInProgress: 5,
    topicsToReview: 2,
  };

  // Mock topic stats
  const topicStats = [
    { name: "Biology", progress: 85, atoms: 42 },
    { name: "Economics", progress: 76, atoms: 38 },
    { name: "Programming", progress: 92, atoms: 65 },
    { name: "History", progress: 63, atoms: 31 },
    { name: "Mathematics", progress: 71, atoms: 45 },
  ];

  // Get health color based on percentage
  const getHealthColor = (percentage: number) => {
    if (percentage >= 80) return '#4CAF50'; // Green
    if (percentage >= 60) return '#FFC107'; // Yellow
    return '#F44336'; // Red
  };

  return (
    <ScrollView
      style={[styles.container, { paddingTop: insets.top }]}
      contentContainerStyle={styles.contentContainer}
      showsVerticalScrollIndicator={false}
    >
      {/* Header */}
      <View style={styles.header}>
        <ThemedText type="title">Profile</ThemedText>
        <TouchableOpacity style={styles.headerButton}>
          <Ionicons name="settings-outline" size={24} color={colors.tint} />
        </TouchableOpacity>
      </View>
      
      {/* User Info Card */}
      <ThemedView style={styles.userCard}>
        <View style={styles.userInfo}>
          <Image 
            source={{ uri: user.avatar }} 
            style={styles.avatar}
          />
          <View style={styles.userDetails}>
            <ThemedText type="subtitle">{user.name}</ThemedText>
            <ThemedText style={styles.email}>{user.email}</ThemedText>
            <View style={styles.streakContainer}>
              <Ionicons name="flame" size={16} color="#FF9800" />
              <ThemedText style={styles.streakText}>{user.streak} day streak</ThemedText>
            </View>
          </View>
        </View>
        <TouchableOpacity style={styles.editProfileButton}>
          <ThemedText style={styles.editProfileText}>Edit Profile</ThemedText>
        </TouchableOpacity>
      </ThemedView>
      
      {/* Stats Card */}
      <ThemedView style={styles.statsCard}>
        <ThemedText type="subtitle">Your Stats</ThemedText>
        <View style={styles.statsGrid}>
          <View style={styles.statItem}>
            <ThemedText style={styles.statValue}>{user.totalNotes}</ThemedText>
            <ThemedText style={styles.statLabel}>Notes</ThemedText>
          </View>
          <View style={styles.statItem}>
            <ThemedText style={styles.statValue}>{user.knowledgeItems}</ThemedText>
            <ThemedText style={styles.statLabel}>Knowledge Items</ThemedText>
          </View>
          <View style={styles.statItem}>
            <ThemedText style={styles.statValue}>{user.retention}%</ThemedText>
            <ThemedText style={styles.statLabel}>Retention</ThemedText>
          </View>
          <View style={styles.statItem}>
            <ThemedText style={styles.statValue}>{topicStats.length}</ThemedText>
            <ThemedText style={styles.statLabel}>Topics</ThemedText>
          </View>
        </View>
      </ThemedView>
      
      
      
      {/* Detailed Learning Statistics Section */}
      <View id="learningStats">
        {/* Knowledge Health Card */}
        <ThemedView style={styles.card}>
          <ThemedText type="subtitle">Knowledge Health</ThemedText>
          <View style={styles.healthContainer}>
            <View style={styles.progressContainer}>
              <View 
                style={[
                  styles.progressBar, 
                  { width: `${detailedStats.knowledgeHealth}%`, backgroundColor: getHealthColor(detailedStats.knowledgeHealth) }
                ]} 
              />
            </View>
            <ThemedText type="defaultSemiBold" style={styles.percentage}>{detailedStats.knowledgeHealth}%</ThemedText>
          </View>
          <View style={styles.healthStatsRow}>
            <View style={styles.healthStatItem}>
              <View style={[styles.indicator, { backgroundColor: '#4CAF50' }]} />
              <ThemedText style={styles.healthStatText}>
                {detailedStats.topicsMastered} Topics Mastered
              </ThemedText>
            </View>
            <View style={styles.healthStatItem}>
              <View style={[styles.indicator, { backgroundColor: '#FFC107' }]} />
              <ThemedText style={styles.healthStatText}>
                {detailedStats.topicsInProgress} In Progress
              </ThemedText>
            </View>
            <View style={styles.healthStatItem}>
              <View style={[styles.indicator, { backgroundColor: '#F44336' }]} />
              <ThemedText style={styles.healthStatText}>
                {detailedStats.topicsToReview} Need Review
              </ThemedText>
            </View>
          </View>
        </ThemedView>

        {/* Topics Progress Card */}
        <ThemedView style={styles.card}>
          <ThemedText type="subtitle">Topics Progress</ThemedText>
          {topicStats.map((topic, index) => (
            <View key={index} style={styles.topicItem}>
              <View style={styles.topicHeader}>
                <ThemedText style={styles.topicName}>{topic.name}</ThemedText>
                <View style={styles.topicHeaderRight}>
                  <ThemedText style={styles.topicPercentage}>{topic.progress}%</ThemedText>
                  <ThemedText style={styles.topicAtoms}>{topic.atoms} atoms</ThemedText>
                </View>
              </View>
              <View style={styles.topicProgressContainer}>
                <View 
                  style={[
                    styles.topicProgressBar, 
                    { width: `${topic.progress}%`, backgroundColor: getHealthColor(topic.progress) }
                  ]} 
                />
              </View>
            </View>
          ))}
        </ThemedView>

        {/* Study Habits Card */}
        <ThemedView style={styles.card}>
          <ThemedText type="subtitle">Study Habits</ThemedText>
          <View style={styles.studyHabitsContainer}>
            <View style={styles.studyHabitItem}>
              <Ionicons name="alarm-outline" size={24} color={colors.tint} />
              <View style={styles.studyHabitContent}>
                <ThemedText style={styles.studyHabitLabel}>Optimal study time</ThemedText>
                <ThemedText style={styles.studyHabitValue}>{detailedStats.optimalStudyTime}</ThemedText>
              </View>
            </View>
            <View style={styles.studyHabitItem}>
              <Ionicons name="time-outline" size={24} color={colors.tint} />
              <View style={styles.studyHabitContent}>
                <ThemedText style={styles.studyHabitLabel}>Average daily study</ThemedText>
                <ThemedText style={styles.studyHabitValue}>{detailedStats.averageDailyStudy}</ThemedText>
              </View>
            </View>
            <View style={styles.studyHabitItem}>
              <Ionicons name="hourglass-outline" size={24} color={colors.tint} />
              <View style={styles.studyHabitContent}>
                <ThemedText style={styles.studyHabitLabel}>Total study time</ThemedText>
                <ThemedText style={styles.studyHabitValue}>{detailedStats.totalStudyTime}</ThemedText>
              </View>
            </View>
          </View>
        </ThemedView>
      </View>
      
      {/* Account Options */}
      <ThemedView style={styles.optionsCard}>
        <ThemedText type="subtitle">Account</ThemedText>
        
        <TouchableOpacity style={styles.optionItem}>
          <View style={styles.optionIcon}>
            <Ionicons name="cloud-upload-outline" size={20} color={colors.tint} />
          </View>
          <ThemedText style={styles.optionText}>Sync Settings</ThemedText>
          <Ionicons name="chevron-forward" size={20} color={colors.icon} />
        </TouchableOpacity>
        
        <TouchableOpacity style={styles.optionItem}>
          <View style={styles.optionIcon}>
            <Ionicons name="notifications-outline" size={20} color={colors.tint} />
          </View>
          <ThemedText style={styles.optionText}>Notifications</ThemedText>
          <Ionicons name="chevron-forward" size={20} color={colors.icon} />
        </TouchableOpacity>
        
        <TouchableOpacity style={styles.optionItem}>
          <View style={styles.optionIcon}>
            <Ionicons name="lock-closed-outline" size={20} color={colors.tint} />
          </View>
          <ThemedText style={styles.optionText}>Privacy</ThemedText>
          <Ionicons name="chevron-forward" size={20} color={colors.icon} />
        </TouchableOpacity>
        
        <TouchableOpacity style={styles.optionItem}>
          <View style={styles.optionIcon}>
            <Ionicons name="help-circle-outline" size={20} color={colors.tint} />
          </View>
          <ThemedText style={styles.optionText}>Help & Support</ThemedText>
          <Ionicons name="chevron-forward" size={20} color={colors.icon} />
        </TouchableOpacity>
        
        <TouchableOpacity style={[styles.optionItem, styles.logoutButton]}>
          <Ionicons name="log-out-outline" size={20} color="#F44336" />
          <ThemedText style={styles.logoutText}>Log Out</ThemedText>
        </TouchableOpacity>
      </ThemedView>
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
  headerButton: {
    padding: 4,
  },
  userCard: {
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
  userInfo: {
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: 16,
  },
  avatar: {
    width: 60,
    height: 60,
    borderRadius: 30,
    marginRight: 16,
  },
  userDetails: {
    flex: 1,
  },
  email: {
    fontSize: 14,
    opacity: 0.7,
    marginBottom: 8,
  },
  streakContainer: {
    flexDirection: 'row',
    alignItems: 'center',
  },
  streakText: {
    fontSize: 14,
    marginLeft: 4,
    color: '#FF9800',
  },
  editProfileButton: {
    backgroundColor: 'rgba(10, 126, 164, 0.1)',
    borderRadius: 8,
    padding: 12,
    alignItems: 'center',
  },
  editProfileText: {
    color: '#0a7ea4',
    fontWeight: '600',
  },
  statsCard: {
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
  statsGrid: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    marginTop: 16,
  },
  statItem: {
    width: '50%',
    marginBottom: 16,
    alignItems: 'center',
  },
  statValue: {
    fontSize: 24,
    fontWeight: 'bold',
    marginBottom: 4,
  },
  statLabel: {
    fontSize: 14,
    opacity: 0.7,
  },
  
  progressBar: {
    height: 8,
    borderRadius: 4,
  },
  
  optionsCard: {
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
  optionItem: {
    flexDirection: 'row',
    alignItems: 'center',
    paddingVertical: 16,
    borderBottomWidth: StyleSheet.hairlineWidth,
    borderBottomColor: 'rgba(0, 0, 0, 0.1)',
  },
  optionIcon: {
    marginRight: 16,
  },
  optionText: {
    flex: 1,
    fontSize: 16,
  },
  logoutButton: {
    borderBottomWidth: 0,
    justifyContent: 'center',
  },
  logoutText: {
    color: '#F44336',
    fontSize: 16,
    fontWeight: '600',
    marginLeft: 16,
  },
  // Added styles for detailed learning stats
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
  percentage: {
    fontSize: 16,
    fontWeight: 'bold',
  },
  healthStatsRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    marginTop: 12,
  },
  healthStatItem: {
    flexDirection: 'row',
    alignItems: 'center',
  },
  indicator: {
    width: 10,
    height: 10,
    borderRadius: 5,
    marginRight: 6,
  },
  healthStatText: {
    fontSize: 12,
    opacity: 0.7,
  },
  topicItem: {
    marginTop: 12,
    marginBottom: 8,
  },
  topicHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 8,
  },
  topicName: {
    fontWeight: '500',
  },
  topicHeaderRight: {
    flexDirection: 'row',
    alignItems: 'center',
  },
  topicPercentage: {
    fontSize: 14,
    fontWeight: '600',
    marginRight: 10,
    color: '#2196F3',
  },
  topicAtoms: {
    fontSize: 12,
    opacity: 0.7,
  },
  topicProgressContainer: {
    height: 8,
    backgroundColor: 'rgba(0, 0, 0, 0.1)',
    borderRadius: 4,
    marginBottom: 4,
  },
  topicProgressBar: {
    height: 8,
    borderRadius: 4,
  },
  studyHabitsContainer: {
    marginTop: 12,
  },
  studyHabitItem: {
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: 16,
  },
  studyHabitContent: {
    marginLeft: 12,
  },
  studyHabitLabel: {
    fontSize: 14,
    opacity: 0.7,
    marginBottom: 2,
  },
  studyHabitValue: {
    fontSize: 16,
    fontWeight: '500',
  },
}); 
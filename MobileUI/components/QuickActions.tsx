import React from 'react';
import { StyleSheet, TouchableOpacity, View } from 'react-native';
import { Ionicons } from '@expo/vector-icons';

import { ThemedText } from './ThemedText';
import { ThemedView } from './ThemedView';
import { Colors } from '@/constants/Colors';
import { useColorScheme } from '@/hooks/useColorScheme';

interface QuickAction {
  id: string;
  title: string;
  icon: string;
  color: string;
  backgroundColor: string;
}

interface QuickActionsProps {
  onActionPress: (actionId: string) => void;
}

export function QuickActions({ onActionPress }: QuickActionsProps) {
  const colorScheme = useColorScheme();
  const colors = Colors[colorScheme ?? 'light'];
  
  // Define quick actions
  const actions: QuickAction[] = [
    {
      id: 'new-note',
      title: 'New Note',
      icon: 'create-outline',
      color: colors.tint,
      backgroundColor: 'rgba(10, 126, 164, 0.1)'
    },
    {
      id: 'analytics',
      title: 'Analytics',
      icon: 'analytics-outline',
      color: '#4CAF50',
      backgroundColor: 'rgba(76, 175, 80, 0.1)'
    },
    {
      id: 'knowledge-map',
      title: 'Knowledge Map',
      icon: 'git-network-outline',
      color: '#9C27B0',
      backgroundColor: 'rgba(156, 39, 176, 0.1)'
    },
    {
      id: 'import',
      title: 'Import',
      icon: 'download-outline',
      color: '#FF9800',
      backgroundColor: 'rgba(255, 152, 0, 0.1)'
    }
  ];
  
  return (
    <ThemedView style={styles.container}>
      <ThemedText type="subtitle" style={styles.title}>Quick Actions</ThemedText>
      <View style={styles.actionsContainer}>
        {actions.map((action) => (
          <TouchableOpacity 
            key={action.id} 
            style={styles.action}
            onPress={() => onActionPress(action.id)}
          >
            <View 
              style={[
                styles.iconContainer, 
                { backgroundColor: action.backgroundColor }
              ]}
            >
              <Ionicons name={action.icon as any} size={24} color={action.color} />
            </View>
            <ThemedText style={styles.actionText}>{action.title}</ThemedText>
          </TouchableOpacity>
        ))}
      </View>
    </ThemedView>
  );
}

const styles = StyleSheet.create({
  container: {
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
  title: {
    marginBottom: 16,
  },
  actionsContainer: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    justifyContent: 'space-between',
  },
  action: {
    width: '48%',
    alignItems: 'center',
    padding: 12,
    borderRadius: 12,
    marginBottom: 12,
  },
  iconContainer: {
    width: 50,
    height: 50,
    borderRadius: 25,
    justifyContent: 'center',
    alignItems: 'center',
    marginBottom: 8,
  },
  actionText: {
    fontSize: 12,
    fontWeight: '500',
  },
}); 
import React from 'react';
import { StyleSheet, TouchableOpacity, View } from 'react-native';
import { Ionicons } from '@expo/vector-icons';

import { ThemedText } from './ThemedText';
import { ThemedView } from './ThemedView';
import { Colors } from '@/constants/Colors';
import { useColorScheme } from '@/hooks/useColorScheme';

interface Connection {
  id: number;
  title: string;
  confidence: number;
}

interface SuggestedConnectionsCardProps {
  connections: Connection[];
  onConnectionPress: (connectionId: number) => void;
}

export function SuggestedConnectionsCard({ 
  connections, 
  onConnectionPress 
}: SuggestedConnectionsCardProps) {
  const colorScheme = useColorScheme();
  const colors = Colors[colorScheme ?? 'light'];
  
  return (
    <ThemedView style={styles.card}>
            <ThemedText type="subtitle">Connections</ThemedText>
      
      {connections.length === 0 ? (
        <ThemedText style={styles.emptyText}>
          No connections available
        </ThemedText>
      ) : (
        connections.map((connection) => (
          <TouchableOpacity 
            key={connection.id} 
            style={styles.connectionItem}
            onPress={() => onConnectionPress(connection.id)}
          >
            <View style={styles.connectionContent}>
              <ThemedText style={styles.connectionTitle}>{connection.title}</ThemedText>
              <View style={styles.confidenceContainer}>
                <ThemedText style={[
                  styles.confidenceText,
                  { 
                    color: connection.confidence >= 90 ? '#2196F3' : 
                           connection.confidence >= 70 ? '#FFC107' : 
                           '#F44336' 
                  }
                ]}>
                  {connection.confidence}% match
                </ThemedText>
              </View>
            </View>
            <Ionicons name="git-network-outline" size={24} color={colors.tint} />
          </TouchableOpacity>
        ))
      )}
      
      {connections.length > 0 && (
        <TouchableOpacity style={styles.viewAllButton}>
          <ThemedText style={styles.viewAllText}>View All</ThemedText>
          <Ionicons name="chevron-forward" size={16} color={colors.tint} />
        </TouchableOpacity>
      )}
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
  description: {
    fontSize: 14,
    opacity: 0.7,
    marginBottom: 16,
  },
  connectionItem: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    paddingVertical: 12,
    borderBottomWidth: StyleSheet.hairlineWidth,
    borderBottomColor: 'rgba(0, 0, 0, 0.1)',
  },
  connectionContent: {
    flex: 1,
    marginRight: 12,
  },
  connectionTitle: {
    fontWeight: '500',
    marginBottom: 4,
  },
  confidenceContainer: {
    flexDirection: 'row',
    alignItems: 'center',
  },
  confidenceText: {
    fontSize: 12,
    fontWeight: '500',
    marginRight: 8,
  },
  viewAllButton: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    marginTop: 16,
  },
  viewAllText: {
    color: '#0a7ea4',
    fontWeight: '500',
    marginRight: 4,
  },
  emptyText: {
    textAlign: 'center',
    marginVertical: 20,
    fontStyle: 'italic',
  },
}); 
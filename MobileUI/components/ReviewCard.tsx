import React from 'react';
import { StyleSheet, TouchableOpacity, View } from 'react-native';
import { Ionicons } from '@expo/vector-icons';

import { ThemedText } from './ThemedText';
import { ThemedView } from './ThemedView';
import { Colors } from '@/constants/Colors';
import { useColorScheme } from '@/hooks/useColorScheme';

interface Atom {
  id: number;
  content: string;
}

interface ReviewCardProps {
  atoms: Atom[];
  onStartReview: () => void;
  onAtomPress: (atomId: number) => void;
}

export function ReviewCard({ atoms, onStartReview, onAtomPress }: ReviewCardProps) {
  const colorScheme = useColorScheme();
  const colors = Colors[colorScheme ?? 'light'];
  
  return (
    <ThemedView style={styles.card}>
      <View style={styles.cardHeader}>
        <ThemedText type="subtitle">Today's Review</ThemedText>
        <ThemedText style={styles.count}>{atoms.length} atoms</ThemedText>
      </View>
      
      {atoms.length === 0 ? (
        <ThemedText style={styles.emptyText}>
          Great job! You've completed all your reviews for today.
        </ThemedText>
      ) : (
        <>
          {atoms.map((atom) => (
            <TouchableOpacity 
              key={atom.id} 
              style={styles.reviewItem}
              onPress={() => onAtomPress(atom.id)}
            >
              <Ionicons name="flash-outline" size={20} color={colors.tint} />
              <ThemedText style={styles.reviewText} numberOfLines={2}>
                {atom.content}
              </ThemedText>
              <Ionicons name="chevron-forward" size={20} color={colors.icon} />
            </TouchableOpacity>
          ))}
          
          <TouchableOpacity 
            style={styles.button}
            onPress={onStartReview}
          >
            <ThemedText style={styles.buttonText}>Start Review Session</ThemedText>
          </TouchableOpacity>
        </>
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
  cardHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 12,
  },
  count: {
    fontSize: 14,
    fontWeight: 'bold',
    backgroundColor: 'rgba(10, 126, 164, 0.1)',
    color: '#0a7ea4',
    paddingHorizontal: 10,
    paddingVertical: 4,
    borderRadius: 12,
    textAlign: 'center',
  },
  reviewItem: {
    flexDirection: 'row',
    alignItems: 'center',
    paddingVertical: 12,
    borderBottomWidth: StyleSheet.hairlineWidth,
    borderBottomColor: 'rgba(0, 0, 0, 0.1)',
  },
  reviewText: {
    flex: 1,
    marginHorizontal: 12,
  },
  button: {
    backgroundColor: '#0a7ea4',
    borderRadius: 8,
    padding: 12,
    alignItems: 'center',
    marginTop: 16,
  },
  buttonText: {
    color: 'white',
    fontWeight: 'bold',
  },
  emptyText: {
    textAlign: 'center',
    marginVertical: 20,
    fontStyle: 'italic',
  },
}); 
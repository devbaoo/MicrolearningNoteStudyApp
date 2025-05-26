import React from 'react';
import { StyleSheet, View, TouchableOpacity, Dimensions } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { Ionicons } from '@expo/vector-icons';
// We'll need to install react-native-svg
// import Svg, { Circle, Line, Text as SvgText } from 'react-native-svg';

import { ThemedText } from '@/components/ThemedText';
import { ThemedView } from '@/components/ThemedView';
import { Colors } from '@/constants/Colors';
import { useColorScheme } from '@/hooks/useColorScheme';

const { width } = Dimensions.get('window');
const graphWidth = width - 32; // 16px padding on each side
const graphHeight = 400;

// Mock knowledge graph data
interface Node {
  id: number;
  label: string;
  x: number;
  y: number;
  size: number;
  color: string;
}

interface Edge {
  source: number;
  target: number;
}

const nodes: Node[] = [
  { id: 1, label: 'Photosynthesis', x: graphWidth * 0.5, y: graphHeight * 0.3, size: 30, color: '#4CAF50' },
  { id: 2, label: 'Plants', x: graphWidth * 0.3, y: graphHeight * 0.5, size: 25, color: '#4CAF50' },
  { id: 3, label: 'Light Energy', x: graphWidth * 0.7, y: graphHeight * 0.5, size: 25, color: '#FF9800' },
  { id: 4, label: 'Chemical Energy', x: graphWidth * 0.6, y: graphHeight * 0.7, size: 22, color: '#2196F3' },
  { id: 5, label: 'Chlorophyll', x: graphWidth * 0.4, y: graphHeight * 0.7, size: 22, color: '#4CAF50' },
  { id: 6, label: 'Carbon Cycle', x: graphWidth * 0.2, y: graphHeight * 0.2, size: 20, color: '#9C27B0' },
  { id: 7, label: 'Oxygen', x: graphWidth * 0.8, y: graphHeight * 0.2, size: 20, color: '#2196F3' },
];

// Connections between nodes
const edges: Edge[] = [
  { source: 1, target: 2 },
  { source: 1, target: 3 },
  { source: 1, target: 4 },
  { source: 1, target: 5 },
  { source: 1, target: 6 },
  { source: 1, target: 7 },
  { source: 2, target: 5 },
  { source: 3, target: 4 },
  { source: 6, target: 2 },
];

export default function KnowledgeScreen() {
  const insets = useSafeAreaInsets();
  const colorScheme = useColorScheme();
  const colors = Colors[colorScheme ?? 'light'];
  
  // Find node by ID
  const findNode = (id: number): Node | undefined => nodes.find(node => node.id === id);
  
  return (
    <View style={[styles.container, { paddingTop: insets.top }]}>
      {/* Header */}
      <View style={styles.header}>
        <ThemedText type="title">Knowledge Map</ThemedText>
        <View style={styles.headerButtons}>
          <TouchableOpacity style={styles.headerButton}>
            <Ionicons name="search-outline" size={24} color={colors.tint} />
          </TouchableOpacity>
          <TouchableOpacity style={styles.headerButton}>
            <Ionicons name="options-outline" size={24} color={colors.tint} />
          </TouchableOpacity>
        </View>
      </View>
      
      {/* Graph Filters */}
      <View style={styles.filtersContainer}>
        <TouchableOpacity style={[styles.filterButton, styles.activeFilter]}>
          <ThemedText style={styles.activeFilterText}>All</ThemedText>
        </TouchableOpacity>
        <TouchableOpacity style={styles.filterButton}>
          <ThemedText style={styles.filterText}>Biology</ThemedText>
        </TouchableOpacity>
        <TouchableOpacity style={styles.filterButton}>
          <ThemedText style={styles.filterText}>Physics</ThemedText>
        </TouchableOpacity>
        <TouchableOpacity style={styles.filterButton}>
          <ThemedText style={styles.filterText}>Economics</ThemedText>
        </TouchableOpacity>
      </View>
      
      {/* Knowledge Graph */}
      <ThemedView style={styles.graphContainer}>
        {/* Since we need to install react-native-svg, we'll show a placeholder for now */}
        <View style={styles.graphPlaceholder}>
          <ThemedText style={styles.graphPlaceholderText}>
            Knowledge Graph Visualization
          </ThemedText>
          <ThemedText style={styles.graphPlaceholderSubtext}>
            (Install react-native-svg to see the actual graph)
          </ThemedText>
        </View>
        
        {/* Commented out SVG implementation until we install the dependency
        <Svg width={graphWidth} height={graphHeight}>
          {edges.map((edge, index) => {
            const source = findNode(edge.source);
            const target = findNode(edge.target);
            if (!source || !target) return null;
            return (
              <Line
                key={`edge-${index}`}
                x1={source.x}
                y1={source.y}
                x2={target.x}
                y2={target.y}
                stroke={colorScheme === 'dark' ? 'rgba(255, 255, 255, 0.2)' : 'rgba(0, 0, 0, 0.2)'}
                strokeWidth={1.5}
              />
            );
          })}
          
          {nodes.map((node) => (
            <React.Fragment key={`node-${node.id}`}>
              <Circle
                cx={node.x}
                cy={node.y}
                r={node.size}
                fill={`${node.color}20`}
                stroke={node.color}
                strokeWidth={2}
              />
              <SvgText
                x={node.x}
                y={node.y}
                fontSize={node.size / 3}
                fill={colorScheme === 'dark' ? '#fff' : '#000'}
                textAnchor="middle"
                alignmentBaseline="central"
              >
                {node.label}
              </SvgText>
            </React.Fragment>
          ))}
        </Svg>
        */}
      </ThemedView>
      
      {/* Selected Node Info */}
      <ThemedView style={styles.infoCard}>
        <ThemedText type="subtitle">Photosynthesis</ThemedText>
        <View style={styles.infoStats}>
          <View style={styles.infoStat}>
            <ThemedText style={styles.infoStatValue}>6</ThemedText>
            <ThemedText style={styles.infoStatLabel}>Connections</ThemedText>
          </View>
          <View style={styles.infoStat}>
            <ThemedText style={styles.infoStatValue}>85%</ThemedText>
            <ThemedText style={styles.infoStatLabel}>Retention</ThemedText>
          </View>
          <View style={styles.infoStat}>
            <ThemedText style={styles.infoStatValue}>3</ThemedText>
            <ThemedText style={styles.infoStatLabel}>Notes</ThemedText>
          </View>
        </View>
        <TouchableOpacity style={styles.viewDetailsButton}>
          <ThemedText style={styles.viewDetailsText}>View Details</ThemedText>
          <Ionicons name="chevron-forward" size={16} color={colors.tint} />
        </TouchableOpacity>
      </ThemedView>
      
      {/* Zoom Controls */}
      <View style={styles.zoomControls}>
        <TouchableOpacity style={styles.zoomButton}>
          <Ionicons name="add" size={24} color={colors.tint} />
        </TouchableOpacity>
        <TouchableOpacity style={styles.zoomButton}>
          <Ionicons name="remove" size={24} color={colors.tint} />
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
    marginBottom: 16,
  },
  headerButtons: {
    flexDirection: 'row',
  },
  headerButton: {
    marginLeft: 16,
  },
  filtersContainer: {
    flexDirection: 'row',
    marginBottom: 16,
  },
  filterButton: {
    paddingHorizontal: 12,
    paddingVertical: 6,
    borderRadius: 16,
    marginRight: 8,
  },
  activeFilter: {
    backgroundColor: '#0a7ea4',
  },
  filterText: {
    fontSize: 14,
  },
  activeFilterText: {
    fontSize: 14,
    color: 'white',
  },
  graphContainer: {
    borderRadius: 12,
    padding: 8,
    marginBottom: 16,
    shadowColor: "#000",
    shadowOffset: {
      width: 0,
      height: 2,
    },
    shadowOpacity: 0.1,
    shadowRadius: 3.84,
    elevation: 5,
    alignItems: 'center',
    justifyContent: 'center',
    height: graphHeight,
  },
  graphPlaceholder: {
    flex: 1,
    alignItems: 'center',
    justifyContent: 'center',
    width: '100%',
  },
  graphPlaceholderText: {
    fontSize: 18,
    fontWeight: 'bold',
    marginBottom: 8,
  },
  graphPlaceholderSubtext: {
    fontSize: 14,
    opacity: 0.7,
  },
  infoCard: {
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
  infoStats: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    marginVertical: 16,
  },
  infoStat: {
    alignItems: 'center',
    flex: 1,
  },
  infoStatValue: {
    fontSize: 18,
    fontWeight: 'bold',
    marginBottom: 4,
  },
  infoStatLabel: {
    fontSize: 12,
    opacity: 0.7,
  },
  viewDetailsButton: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
  },
  viewDetailsText: {
    color: '#0a7ea4',
    fontWeight: '500',
    marginRight: 4,
  },
  zoomControls: {
    position: 'absolute',
    right: 16,
    bottom: 16,
    backgroundColor: 'rgba(255, 255, 255, 0.8)',
    borderRadius: 8,
    shadowColor: "#000",
    shadowOffset: {
      width: 0,
      height: 2,
    },
    shadowOpacity: 0.1,
    shadowRadius: 3.84,
    elevation: 5,
  },
  zoomButton: {
    width: 40,
    height: 40,
    alignItems: 'center',
    justifyContent: 'center',
  },
}); 
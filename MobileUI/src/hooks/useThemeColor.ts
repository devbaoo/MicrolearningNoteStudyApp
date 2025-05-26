/**
 * Theme Color Hook
 * 
 * This hook provides access to the current theme colors based on the device's color scheme.
 * It handles the selection of colors based on the current theme (light or dark).
 */

import { Colors } from '@/src/theme/colors';
import { useColorScheme } from '@/src/hooks/useColorScheme';

/**
 * Hook to get the appropriate color for the current theme
 * 
 * @param props - Optional override colors for light and dark themes
 * @param colorName - The name of the color to retrieve from the theme
 * @returns The color value for the current theme
 */
export function useThemeColor(
  props: { light?: string; dark?: string },
  colorName: keyof typeof Colors.light & keyof typeof Colors.dark
) {
  const theme = useColorScheme() ?? 'light';
  const colorFromProps = props[theme];

  if (colorFromProps) {
    return colorFromProps;
  } else {
    return Colors[theme][colorName];
  }
}

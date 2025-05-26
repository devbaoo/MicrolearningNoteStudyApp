import { Stack } from 'expo-router';

export default function ModalsLayout() {
  return (
    <Stack>
      <Stack.Screen name="add-note" options={{ headerShown: false }} />
    </Stack>
  );
} 
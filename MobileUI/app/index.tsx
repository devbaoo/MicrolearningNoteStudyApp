import { Redirect } from 'expo-router';

// This is the root index file that will be loaded first when the app starts
// It simply redirects to the login page
export default function Index() {
  return <Redirect href="/login" />;
}

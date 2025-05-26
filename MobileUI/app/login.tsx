import React, { useState } from 'react';
import { 
  View, 
  StyleSheet, 
  Text, 
  TouchableOpacity, 
  KeyboardAvoidingView, 
  Platform,
  ScrollView,
  Image,
  Alert
} from 'react-native';
import { StatusBar } from 'expo-status-bar';
import { router } from 'expo-router';
import { Ionicons } from '@expo/vector-icons';
import { useAuth } from '@/src/context/AuthContext';

// Components
import { Button } from '@/components/ui/Button';
import { Input } from '@/components/ui/Input';

// Hooks and services
import { useThemeColor } from '@/src/hooks/useThemeColor';
import { authService, LoginRequest } from '@/src/api/authService';

export default function LoginScreen() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [errors, setErrors] = useState<{email?: string; password?: string}>({});
  const { login } = useAuth();
  
  const backgroundColor = useThemeColor({}, 'background');
  const textColor = useThemeColor({}, 'text');
  const tintColor = useThemeColor({}, 'tint');
  
  // Validate form inputs
  const validateForm = () => {
    const newErrors: {email?: string; password?: string} = {};
    let isValid = true;
    
    // Email validation
    if (!email) {
      newErrors.email = 'Email is required';
      isValid = false;
    } else if (!/\S+@\S+\.\S+/.test(email)) {
      newErrors.email = 'Email is invalid';
      isValid = false;
    }
    
    // Password validation
    if (!password) {
      newErrors.password = 'Password is required';
      isValid = false;
    } else if (password.length < 6) {
      newErrors.password = 'Password must be at least 6 characters';
      isValid = false;
    }
    
    setErrors(newErrors);
    return isValid;
  };
  
  // Handle login
  const handleLogin = async () => {
    if (!validateForm()) return;
    
    setIsLoading(true);
    
    try {
      const credentials: LoginRequest = { email, password };
      const response = await authService.login(credentials);
      
      if (response.error) {
        Alert.alert('Login Failed', response.error.message || 'Please check your credentials and try again.');
      } else if (response.data) {
        // Login successful, store token and navigate to the main app
        await login(response.data.token, response.data.user);
        router.replace('/(tabs)');
      }
    } catch (error) {
      Alert.alert('Error', 'An unexpected error occurred. Please try again later.');
      console.error('Login error:', error);
    } finally {
      setIsLoading(false);
    }
  };
  
  // Navigate to registration screen
  const navigateToRegister = () => {
    router.push('/register');
  };
  
  // Navigate to forgot password screen
  const navigateToForgotPassword = () => {
    router.push('/forgot-password');
  };
  
  return (
    <KeyboardAvoidingView
      style={[styles.container, { backgroundColor }]}
      behavior={Platform.OS === 'ios' ? 'padding' : 'height'}
      keyboardVerticalOffset={Platform.OS === 'ios' ? 64 : 0}
    >
      <StatusBar style="auto" />
      
      <ScrollView
        contentContainerStyle={styles.scrollContent}
        showsVerticalScrollIndicator={false}
      >
        <View style={styles.logoContainer}>
          <Image 
            source={require('@/assets/images/logo.png')} 
            style={styles.logo}
            resizeMode="contain"
          />
          <Text style={[styles.appName, { color: textColor }]}>
            NoteSprint
          </Text>
        </View>
        
        <Text style={[styles.title, { color: textColor }]}>
          Welcome Back
        </Text>
        <Text style={[styles.subtitle, { color: textColor }]}>
          Sign in to continue to your notes
        </Text>
        
        <View style={styles.formContainer}>
          <Input
            label="Email"
            placeholder="Enter your email"
            value={email}
            onChangeText={setEmail}
            autoCapitalize="none"
            keyboardType="email-address"
            error={errors.email}
            leftIcon={<Ionicons name="mail-outline" size={20} color={tintColor} />}
          />
          
          <Input
            label="Password"
            placeholder="Enter your password"
            value={password}
            onChangeText={setPassword}
            isPassword
            error={errors.password}
            leftIcon={<Ionicons name="lock-closed-outline" size={20} color={tintColor} />}
          />
          
          <TouchableOpacity 
            style={styles.forgotPassword}
            onPress={navigateToForgotPassword}
          >
            <Text style={[styles.forgotPasswordText, { color: tintColor }]}>
              Forgot Password?
            </Text>
          </TouchableOpacity>
          
          <Button
            title="Sign In"
            onPress={handleLogin}
            isLoading={isLoading}
            fullWidth
            size="large"
            style={styles.loginButton}
          />
        </View>
        
        <View style={styles.footer}>
          <Text style={[styles.footerText, { color: textColor }]}>
            Don't have an account?
          </Text>
          <TouchableOpacity onPress={navigateToRegister}>
            <Text style={[styles.signUpText, { color: tintColor }]}>
              Sign Up
            </Text>
          </TouchableOpacity>
        </View>
        
        <View style={styles.orContainer}>
          <View style={[styles.divider, { backgroundColor: textColor }]} />
          <Text style={[styles.orText, { color: textColor }]}>OR</Text>
          <View style={[styles.divider, { backgroundColor: textColor }]} />
        </View>
        
        <View style={styles.socialContainer}>
          <Button
            title="Continue with Google"
            variant="outline"
            leftIcon={<Ionicons name="logo-google" size={20} color="#DB4437" />}
            style={styles.socialButton}
            onPress={() => Alert.alert('Coming Soon', 'Google login will be available soon!')}
          />
          
          <Button
            title="Continue with Apple"
            variant="outline"
            leftIcon={<Ionicons name="logo-apple" size={20} color={textColor} />}
            style={styles.socialButton}
            onPress={() => Alert.alert('Coming Soon', 'Apple login will be available soon!')}
          />
        </View>
      </ScrollView>
    </KeyboardAvoidingView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
  },
  scrollContent: {
    flexGrow: 1,
    padding: 24,
    paddingTop: Platform.OS === 'ios' ? 60 : 40,
  },
  logoContainer: {
    alignItems: 'center',
    marginBottom: 32,
  },
  logo: {
    width: 80,
    height: 80,
    marginBottom: 16,
  },
  appName: {
    fontSize: 24,
    fontWeight: 'bold',
  },
  title: {
    fontSize: 28,
    fontWeight: 'bold',
    marginBottom: 8,
    textAlign: 'center',
  },
  subtitle: {
    fontSize: 16,
    marginBottom: 32,
    textAlign: 'center',
    opacity: 0.8,
  },
  formContainer: {
    width: '100%',
    marginBottom: 24,
  },
  forgotPassword: {
    alignSelf: 'flex-end',
    marginBottom: 24,
  },
  forgotPasswordText: {
    fontSize: 14,
    fontWeight: '500',
  },
  loginButton: {
    marginTop: 8,
  },
  footer: {
    flexDirection: 'row',
    justifyContent: 'center',
    marginTop: 16,
  },
  footerText: {
    fontSize: 14,
    marginRight: 4,
  },
  signUpText: {
    fontSize: 14,
    fontWeight: '600',
  },
  orContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    marginVertical: 24,
  },
  divider: {
    flex: 1,
    height: 1,
    opacity: 0.2,
  },
  orText: {
    marginHorizontal: 16,
    fontSize: 14,
    fontWeight: '500',
    opacity: 0.6,
  },
  socialContainer: {
    gap: 12,
    marginBottom: 24,
  },
  socialButton: {
    marginVertical: 0,
  },
});

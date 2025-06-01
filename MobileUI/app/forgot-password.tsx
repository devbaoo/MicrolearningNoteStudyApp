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

// Components
import { Button } from '@/components/ui/Button';
import { Input } from '@/components/ui/Input';

// Hooks and services
import { useThemeColor } from '@/src/hooks/useThemeColor';
import { api } from '@/src/api';

export default function ForgotPasswordScreen() {
  const [email, setEmail] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [isSubmitted, setIsSubmitted] = useState(false);
  const [error, setError] = useState('');
  
  const backgroundColor = useThemeColor({}, 'background');
  const textColor = useThemeColor({}, 'text');
  const tintColor = useThemeColor({}, 'tint');
  
  // Validate email
  const validateEmail = () => {
    if (!email) {
      setError('Email is required');
      return false;
    } else if (!/\S+@\S+\.\S+/.test(email)) {
      setError('Email is invalid');
      return false;
    }
    
    setError('');
    return true;
  };
  
  // Handle password reset request
  const handleResetPassword = async () => {
    if (!validateEmail()) return;
    
    setIsLoading(true);
    
    try {
      // This would be replaced with a real API call
      const response = await api.post('/auth/forgot-password', { email });
      
      if (response.error) {
        Alert.alert('Error', response.error.message || 'Failed to send reset link. Please try again.');
      } else {
        setIsSubmitted(true);
      }
    } catch (error) {
      Alert.alert('Error', 'An unexpected error occurred. Please try again later.');
      console.error('Forgot password error:', error);
    } finally {
      setIsLoading(false);
    }
  };
  
  // Navigate back to login screen
  const navigateToLogin = () => {
    router.back();
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
        <TouchableOpacity 
          style={styles.backButton}
          onPress={navigateToLogin}
        >
          <Ionicons name="arrow-back" size={24} color={textColor} />
        </TouchableOpacity>
        
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
        
        {!isSubmitted ? (
          <>
            <Text style={[styles.title, { color: textColor }]}>
              Forgot Password
            </Text>
            <Text style={[styles.subtitle, { color: textColor }]}>
              Enter your email to receive a password reset link
            </Text>
            
            <View style={styles.formContainer}>
              <Input
                label="Email"
                placeholder="Enter your email"
                value={email}
                onChangeText={setEmail}
                autoCapitalize="none"
                keyboardType="email-address"
                error={error}
                leftIcon={<Ionicons name="mail-outline" size={20} color={tintColor} />}
              />
              
              <Button
                title="Send Reset Link"
                onPress={handleResetPassword}
                isLoading={isLoading}
                fullWidth
                size="large"
                style={styles.resetButton}
              />
            </View>
          </>
        ) : (
          <View style={styles.successContainer}>
            <View style={[styles.iconCircle, { backgroundColor: tintColor }]}>
              <Ionicons name="checkmark" size={40} color="#fff" />
            </View>
            
            <Text style={[styles.title, { color: textColor }]}>
              Check Your Email
            </Text>
            
            <Text style={[styles.successMessage, { color: textColor }]}>
              We&apos;ve sent a password reset link to:
            </Text>
            
            <Text style={[styles.emailText, { color: textColor }]}>
              {email}
            </Text>
            
            <Text style={[styles.instructionText, { color: textColor }]}>
              Please check your email and follow the instructions to reset your password.
            </Text>
            
            <Button
              title="Back to Login"
              onPress={navigateToLogin}
              variant="primary"
              fullWidth
              size="large"
              style={styles.backToLoginButton}
            />
            
            <TouchableOpacity 
              style={styles.resendLink}
              onPress={handleResetPassword}
            >
              <Text style={[styles.resendText, { color: tintColor }]}>
                Didn&apos;t receive the email? Resend
              </Text>
            </TouchableOpacity>
          </View>
        )}
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
  backButton: {
    position: 'absolute',
    top: Platform.OS === 'ios' ? 60 : 40,
    left: 24,
    zIndex: 10,
    width: 40,
    height: 40,
    justifyContent: 'center',
    alignItems: 'center',
  },
  logoContainer: {
    alignItems: 'center',
    marginBottom: 32,
    marginTop: 24,
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
  resetButton: {
    marginTop: 16,
  },
  successContainer: {
    alignItems: 'center',
    paddingHorizontal: 16,
  },
  iconCircle: {
    width: 80,
    height: 80,
    borderRadius: 40,
    justifyContent: 'center',
    alignItems: 'center',
    marginBottom: 24,
  },
  successMessage: {
    fontSize: 16,
    marginTop: 16,
    textAlign: 'center',
  },
  emailText: {
    fontSize: 16,
    fontWeight: 'bold',
    marginTop: 8,
    marginBottom: 24,
  },
  instructionText: {
    fontSize: 14,
    textAlign: 'center',
    marginBottom: 32,
    lineHeight: 20,
    opacity: 0.8,
  },
  backToLoginButton: {
    marginBottom: 16,
  },
  resendLink: {
    marginTop: 16,
    padding: 8,
  },
  resendText: {
    fontSize: 14,
    fontWeight: '500',
  },
});

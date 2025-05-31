'use client'

import { useState, useEffect } from 'react'
import Navigation from '@/components/landing/Navigation'
import HeroSection from '@/components/landing/HeroSection'
import FeaturesSection from '@/components/landing/FeaturesSection'
import HowItWorksSection from '@/components/landing/HowItWorksSection'
import TestimonialsSection from '@/components/landing/TestimonialsSection'
import CTASection from '@/components/landing/CTASection'
import Footer from '@/components/landing/Footer'

export default function LandingPage() {
  const [isVisible, setIsVisible] = useState(false)
  const [isLogin, setIsLogin] = useState(true)

  useEffect(() => {
    setIsVisible(true)
  }, [])

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-50 via-white to-indigo-50">
      <Navigation setIsLogin={setIsLogin} />
      <HeroSection isVisible={isVisible} isLogin={isLogin} setIsLogin={setIsLogin} />
      <FeaturesSection />
      <HowItWorksSection />
      <TestimonialsSection />
      <CTASection />
      <Footer />
    </div>
  )
}

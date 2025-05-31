'use client'

import { BookOpen, Brain, Award } from 'lucide-react'

export default function HowItWorksSection() {
  return (
    <section id="how-it-works" className="py-24 bg-gradient-to-br from-gray-50 to-indigo-50">
      <div className="max-w-7xl mx-auto px-6">
        <div className="text-center mb-16">
          <h2 className="text-4xl font-bold text-gray-900 mb-4">
            Master Any Subject in 3 Simple Steps
          </h2>
          <p className="text-xl text-gray-600 max-w-3xl mx-auto">
            Our scientifically-proven approach breaks down complex learning into manageable, effective steps
          </p>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
          <div className="text-center group">
            <div className="w-20 h-20 bg-gradient-to-br from-indigo-500 to-purple-600 rounded-2xl flex items-center justify-center mx-auto mb-6 group-hover:scale-110 transition-transform duration-300">
              <BookOpen className="w-10 h-10 text-white" />
            </div>
            <h3 className="text-2xl font-semibold text-gray-900 mb-4">1. Create Knowledge Atoms</h3>
            <p className="text-gray-600 leading-relaxed">
              Break down complex topics into bite-sized knowledge atoms. Our AI helps you identify key concepts and create effective study materials.
            </p>
          </div>

          <div className="text-center group">
            <div className="w-20 h-20 bg-gradient-to-br from-purple-500 to-pink-600 rounded-2xl flex items-center justify-center mx-auto mb-6 group-hover:scale-110 transition-transform duration-300">
              <Brain className="w-10 h-10 text-white" />
            </div>
            <h3 className="text-2xl font-semibold text-gray-900 mb-4">2. AI-Powered Learning</h3>
            <p className="text-gray-600 leading-relaxed">
              Our intelligent system adapts to your learning style, optimizes review timing, and identifies knowledge gaps for maximum retention.
            </p>
          </div>

          <div className="text-center group">
            <div className="w-20 h-20 bg-gradient-to-br from-pink-500 to-red-500 rounded-2xl flex items-center justify-center mx-auto mb-6 group-hover:scale-110 transition-transform duration-300">
              <Award className="w-10 h-10 text-white" />
            </div>
            <h3 className="text-2xl font-semibold text-gray-900 mb-4">3. Master & Retain</h3>
            <p className="text-gray-600 leading-relaxed">
              Track your progress, visualize knowledge connections, and achieve long-term retention through spaced repetition and active recall.
            </p>
          </div>
        </div>
      </div>
    </section>
  )
}

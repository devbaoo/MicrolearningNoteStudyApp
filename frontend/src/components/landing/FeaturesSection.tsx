'use client'

import {
  Brain,
  Zap,
  Target,
  Network,
  Clock,
  TrendingUp
} from 'lucide-react'

export default function FeaturesSection() {
  const features = [
    {
      icon: Brain,
      title: "AI-Powered Learning",
      description: "Intelligent algorithms adapt to your learning style and optimize retention through personalized spaced repetition."
    },
    {
      icon: Zap,
      title: "Microlearning Atoms",
      description: "Break down complex concepts into digestible knowledge atoms that are easy to understand and remember."
    },
    {
      icon: Network,
      title: "Knowledge Graphs",
      description: "Visualize connections between concepts and discover new learning pathways through interactive knowledge maps."
    },
    {
      icon: Target,
      title: "Smart Review System",
      description: "Never forget what you've learned with our scientifically-backed review scheduling system."
    },
    {
      icon: TrendingUp,
      title: "Progress Analytics",
      description: "Track your learning journey with detailed insights and performance metrics."
    },
    {
      icon: Clock,
      title: "Efficient Study Sessions",
      description: "Maximize learning in minimal time with focused, bite-sized study sessions."
    }
  ]

  return (
    <section id="features" className="py-24 bg-white">
      <div className="max-w-7xl mx-auto px-6">
        <div className="text-center mb-16">
          <h2 className="text-4xl font-bold text-gray-900 mb-4">
            Revolutionizing How You Learn
          </h2>
          <p className="text-xl text-gray-600 max-w-3xl mx-auto">
            Our cutting-edge features work together to create the most effective learning experience possible
          </p>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
          {features.map((feature, index) => (
            <div key={index} className="group p-8 rounded-2xl border border-gray-100 hover:border-indigo-200 hover:shadow-lg transition-all duration-300">
              <div className="w-12 h-12 bg-gradient-to-br from-indigo-500 to-purple-600 rounded-xl flex items-center justify-center mb-6 group-hover:scale-110 transition-transform duration-300">
                <feature.icon className="w-6 h-6 text-white" />
              </div>
              <h3 className="text-xl font-semibold text-gray-900 mb-3">{feature.title}</h3>
              <p className="text-gray-600 leading-relaxed">{feature.description}</p>
            </div>
          ))}
        </div>
      </div>
    </section>
  )
}

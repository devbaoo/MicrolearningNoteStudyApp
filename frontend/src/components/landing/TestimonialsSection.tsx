'use client'

import { useState, useEffect } from 'react'
import { Star } from 'lucide-react'

export default function TestimonialsSection() {
  const [currentTestimonial, setCurrentTestimonial] = useState(0)

  const testimonials = [
    {
      quote: "MicroNotes revolutionized how I study. The spaced repetition system helped me retain 90% more information!",
      author: "Sarah Chen",
      role: "Medical Student",
      avatar: "SC"
    },
    {
      quote: "The knowledge graph feature is incredible. I can see how all my concepts connect - it's like having a map of my brain!",
      author: "David Rodriguez",
      role: "Software Engineer",
      avatar: "DR"
    },
    {
      quote: "I went from struggling with complex topics to mastering them. The microlearning approach just clicks for me.",
      author: "Emma Thompson",
      role: "Graduate Researcher",
      avatar: "ET"
    }
  ]

  useEffect(() => {
    const interval = setInterval(() => {
      setCurrentTestimonial((prev) => (prev + 1) % testimonials.length)
    }, 5000)
    
    return () => clearInterval(interval)
  }, [testimonials.length])

  return (
    <section id="testimonials" className="py-24 bg-white">
      <div className="max-w-7xl mx-auto px-6">
        <div className="text-center mb-16">
          <h2 className="text-4xl font-bold text-gray-900 mb-4">
            Loved by Learners Worldwide
          </h2>
          <p className="text-xl text-gray-600">
            Join thousands who have transformed their learning journey
          </p>
        </div>

        <div className="relative">
          <div className="bg-gradient-to-br from-indigo-50 to-purple-50 rounded-2xl p-12 text-center">
            <div className="flex justify-center mb-6">
              {[...Array(5)].map((_, i) => (
                <Star key={i} className="w-6 h-6 text-yellow-400 fill-current" />
              ))}
            </div>
            
            <blockquote className="text-2xl font-medium text-gray-900 mb-8 max-w-4xl mx-auto leading-relaxed">
              &ldquo;{testimonials[currentTestimonial].quote}&rdquo;
            </blockquote>
            
            <div className="flex items-center justify-center space-x-4">
              <div className="w-12 h-12 bg-gradient-to-br from-indigo-500 to-purple-600 rounded-full flex items-center justify-center">
                <span className="text-white font-semibold">{testimonials[currentTestimonial].avatar}</span>
              </div>
              <div className="text-left">
                <div className="font-semibold text-gray-900">{testimonials[currentTestimonial].author}</div>
                <div className="text-gray-600">{testimonials[currentTestimonial].role}</div>
              </div>
            </div>
          </div>

          {/* Testimonial Navigation */}
          <div className="flex justify-center mt-8 space-x-2">
            {testimonials.map((_, index) => (
              <button
                key={index}
                onClick={() => setCurrentTestimonial(index)}
                className={`w-3 h-3 rounded-full transition-all ${
                  index === currentTestimonial ? 'bg-indigo-600' : 'bg-gray-300'
                }`}
              />
            ))}
          </div>
        </div>
      </div>
    </section>
  )
}

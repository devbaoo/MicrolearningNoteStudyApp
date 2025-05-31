'use client'

import { ArrowRight, Users } from 'lucide-react'
import Link from 'next/link'

export default function CTASection() {
  return (
    <section className="py-24 bg-gradient-to-br from-indigo-600 via-purple-600 to-pink-600">
      <div className="max-w-7xl mx-auto px-6 text-center">
        <h2 className="text-4xl lg:text-5xl font-bold text-white mb-6">
          Ready to Transform Your Learning?
        </h2>
        <p className="text-xl text-indigo-100 mb-12 max-w-3xl mx-auto">
          Join thousands of learners who have already revolutionized their study habits with MicroNotes
        </p>
        
        <div className="flex flex-col sm:flex-row gap-6 justify-center">
          <Link href="/home" className="group bg-white hover:bg-gray-100 text-indigo-600 px-8 py-4 rounded-xl font-semibold transition-all duration-200 shadow-lg hover:shadow-xl flex items-center justify-center space-x-2">
            <span>Start Learning for Free</span>
            <ArrowRight className="w-5 h-5 group-hover:translate-x-1 transition-transform" />
          </Link>
          <button className="bg-transparent border-2 border-white hover:bg-white hover:text-indigo-600 text-white px-8 py-4 rounded-xl font-semibold transition-all duration-200 flex items-center justify-center space-x-2">
            <Users className="w-5 h-5" />
            <span>Join Community</span>
          </button>
        </div>
      </div>
    </section>
  )
}

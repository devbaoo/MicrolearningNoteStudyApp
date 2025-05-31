'use client'

interface NavigationProps {
  setIsLogin: (value: boolean) => void
}

export default function Navigation({ setIsLogin }: NavigationProps) {
  return (
    <nav className="relative z-50 bg-white/80 backdrop-blur-md border-b border-gray-100 sticky top-0">
      <div className="max-w-7xl mx-auto px-6 py-4">
        <div className="flex items-center justify-between">
          {/* Logo */}
          <div className="flex items-center space-x-3">
            <div className="relative w-10 h-10 bg-gradient-to-br from-indigo-500 via-purple-500 to-pink-500 rounded-2xl flex items-center justify-center shadow-lg">
              <div className="relative">
                <div className="w-2 h-2 bg-white rounded-full absolute top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2"></div>
                <div className="w-6 h-6 border border-white/60 rounded-full absolute top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 animate-spin" style={{animationDuration: '8s'}}></div>
                <div className="w-4 h-4 border border-white/40 rounded-full absolute top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 rotate-45 animate-spin" style={{animationDuration: '6s', animationDirection: 'reverse'}}></div>
              </div>
            </div>
            <div>
              <h1 className="text-xl font-bold text-gray-900">MicroNotes</h1>
              <p className="text-xs text-gray-500 -mt-1">Microlearning Platform</p>
            </div>
          </div>

          {/* Navigation Links */}
          <div className="hidden md:flex items-center space-x-8">
            <a href="#features" className="text-gray-600 hover:text-indigo-600 transition-colors">Features</a>
            <a href="#how-it-works" className="text-gray-600 hover:text-indigo-600 transition-colors">How it Works</a>
            <a href="#testimonials" className="text-gray-600 hover:text-indigo-600 transition-colors">Reviews</a>
            <a href="#pricing" className="text-gray-600 hover:text-indigo-600 transition-colors">Pricing</a>
          </div>

          {/* CTA Buttons */}
          <div className="flex items-center space-x-4">
            <button 
              onClick={() => setIsLogin(true)}
              className="text-gray-600 hover:text-indigo-600 font-medium transition-colors"
            >
              Sign In
            </button>
            <button 
              onClick={() => setIsLogin(false)}
              className="bg-indigo-600 hover:bg-indigo-700 text-white px-6 py-2 rounded-lg font-medium transition-all duration-200 shadow-lg hover:shadow-xl"
            >
              Get Started
            </button>
          </div>
        </div>
      </div>
    </nav>
  )
}

'use client'

import { useState } from 'react'
import {
  Search,
  Plus,
  Brain,
  BookOpen,
  Target,
  TrendingUp,
  Calendar,
  Clock,
  Zap,
  BarChart3,
  Network,
  FileText,
  Star,
  ChevronRight,
  Settings,
  Bell,
  ChevronLeft
} from 'lucide-react'

export default function Dashboard() {
  const [searchQuery, setSearchQuery] = useState('')
  const [sidebarCollapsed, setSidebarCollapsed] = useState(false)

  // Mock data - in real app this would come from API/database
  const todayReviewAtoms = [
    {
      id: 1,
      content: "React hooks allow functional components to use state",
      difficulty: 3,
      lastReview: "2 days ago",
      masteryLevel: 60,
      category: "React",
      reviewCount: 8,
      nextReview: "Today"
    },
    {
      id: 2,
      content: "Spaced repetition improves long-term retention",
      difficulty: 2,
      lastReview: "5 days ago",
      masteryLevel: 85,
      category: "Learning Theory",
      reviewCount: 12,
      nextReview: "Today"
    },
    {
      id: 3,
      content: "Knowledge graphs visualize relationships between concepts",
      difficulty: 4,
      lastReview: "1 week ago",
      masteryLevel: 35,
      category: "Data Visualization",
      reviewCount: 4,
      nextReview: "Overdue"
    },
    {
      id: 4,
      content: "CSS Grid provides two-dimensional layout control",
      difficulty: 3,
      lastReview: "3 days ago",
      masteryLevel: 70,
      category: "CSS",
      reviewCount: 6,
      nextReview: "Today"
    },
    {
      id: 5,
      content: "Binary search has O(log n) time complexity",
      difficulty: 4,
      lastReview: "1 week ago",
      masteryLevel: 45,
      category: "Algorithms",
      reviewCount: 5,
      nextReview: "Today"
    },
  ]

  const recentNotes = [
    { id: 1, title: "Machine Learning Fundamentals", atoms: 12, created: "Today" },
    { id: 2, title: "React Best Practices", atoms: 8, created: "Yesterday" },
    { id: 3, title: "Database Design Patterns", atoms: 15, created: "2 days ago" },
  ]

  const stats = {
    knowledgeHealth: 87,
    totalAtoms: 234,
    reviewStreak: 12,
    weeklyProgress: 85
  }

  return (
    <div className="min-h-screen bg-[var(--background)] text-[var(--foreground)]">
      {/* Header */}
      <header className="border-b border-[var(--border)] bg-[var(--card)] sticky top-0 z-50 backdrop-blur-md bg-opacity-95">
        <div className="flex items-center justify-between px-6 py-3">
          {/* Logo and Brand */}
          <div className="flex items-center space-x-3 min-w-0">
            <div className="relative flex items-center space-x-3 group">
              {/* Modern atom-inspired logo */}
              <div className="relative w-10 h-10 bg-gradient-to-br from-indigo-500 via-purple-500 to-pink-500 rounded-2xl flex items-center justify-center shadow-lg group-hover:shadow-xl transition-all duration-300 group-hover:scale-105">
                <div className="relative">
                  {/* Central nucleus */}
                  <div className="w-2 h-2 bg-white rounded-full absolute top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2"></div>
                  {/* Orbital rings */}
                  <div className="w-6 h-6 border border-white/60 rounded-full absolute top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 animate-spin" style={{animationDuration: '8s'}}></div>
                  <div className="w-4 h-4 border border-white/40 rounded-full absolute top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 rotate-45 animate-spin" style={{animationDuration: '6s', animationDirection: 'reverse'}}></div>
                  {/* Electrons */}
                  <div className="w-1 h-1 bg-white rounded-full absolute -top-0.5 left-1/2 transform -translate-x-1/2 animate-pulse"></div>
                  <div className="w-1 h-1 bg-white rounded-full absolute top-1/2 -right-0.5 transform -translate-y-1/2 animate-pulse" style={{animationDelay: '0.5s'}}></div>
                </div>
              </div>
              
              <div className="flex flex-col min-w-0">
                <h1 className="text-primary text-xl font-bold text-[var(--foreground)] group-hover:text-[var(--primary)] transition-all duration-300">
                  MicroNotes
                </h1>
                <p className="text-xs text-[var(--muted)] font-medium tracking-wide">Microlearning application</p>
              </div>
            </div>
          </div>

          {/* Enhanced Search Bar */}
          <div className="flex-1 max-w-xl mx-8">
            <div className="relative group">
              <div className="absolute inset-0 bg-gradient-to-r from-indigo-500/10 via-purple-500/10 to-pink-500/10 rounded-xl blur opacity-0 group-hover:opacity-100 transition-opacity duration-300"></div>
              <div className="relative">
                <Search className="absolute left-4 top-1/2 transform -translate-y-1/2 w-4 h-4 text-[var(--muted)] group-focus-within:text-[var(--primary)] transition-colors" />
                <input
                  type="text"
                  placeholder="Search atoms, notes, or concepts..."
                  value={searchQuery}
                  onChange={(e) => setSearchQuery(e.target.value)}
                  className="w-full pl-12 pr-20 py-2.5 bg-[var(--accent)] border border-[var(--border)] rounded-xl focus:outline-none focus:ring-2 focus:ring-[var(--primary)]/20 focus:border-[var(--primary)] transition-all duration-200 text-sm placeholder:text-[var(--muted)] hover:bg-[var(--accent)]/80"
                />
                {/* Search shortcut hint */}
                <div className="absolute right-3 top-1/2 transform -translate-y-1/2 hidden sm:flex items-center space-x-1">
                  <kbd className="px-1.5 py-0.5 text-xs font-medium text-[var(--muted)] bg-[var(--card)] border border-[var(--border)] rounded shadow-sm">⌘</kbd>
                  <kbd className="px-1.5 py-0.5 text-xs font-medium text-[var(--muted)] bg-[var(--card)] border border-[var(--border)] rounded shadow-sm">K</kbd>
                </div>
              </div>
            </div>
          </div>

          {/* Action Buttons */}
          <div className="flex items-center space-x-1">
            {/* Quick Add Button */}
            <button 
              className="relative p-2.5 hover:bg-[var(--accent)] rounded-xl transition-all duration-200 group"
              title="Create new note"
            >
              <Plus className="w-5 h-5 text-[var(--foreground-secondary)] group-hover:text-[var(--primary)] transition-colors" />
              <div className="absolute -top-1 -right-1 w-2 h-2 bg-[var(--primary)] rounded-full opacity-0 group-hover:opacity-100 transition-opacity"></div>
            </button>
            
            {/* Notifications */}
            <button 
              className="relative p-2.5 hover:bg-[var(--accent)] rounded-xl transition-all duration-200 group"
              title="Notifications"
            >
              <Bell className="w-5 h-5 text-[var(--foreground-secondary)] group-hover:text-[var(--primary)] transition-colors" />
              {/* Enhanced notification badge */}
              <div className="absolute -top-0.5 -right-0.5 min-w-[18px] h-[18px] bg-red-500 rounded-full flex items-center justify-center border-2 border-white shadow-sm">
                <span className="text-[10px] text-white font-semibold px-1">3</span>
              </div>
            </button>
            
            {/* Settings */}
            <button 
              className="p-2.5 hover:bg-[var(--accent)] rounded-xl transition-all duration-200 group"
              title="Settings"
            >
              <Settings className="w-5 h-5 text-[var(--foreground-secondary)] group-hover:text-[var(--primary)] group-hover:rotate-90 transition-all duration-300" />
            </button>
            
            {/* User Profile */}
            <div className="relative ml-2">
              <button className="flex items-center space-x-3 pl-3 pr-4 py-2 hover:bg-[var(--accent)] rounded-xl transition-all duration-200 group">
                <div className="w-8 h-8 bg-gradient-to-br from-indigo-500 to-purple-600 rounded-lg flex items-center justify-center shadow-md group-hover:shadow-lg transition-shadow">
                  <span className="text-white font-semibold text-sm">JD</span>
                </div>
                <div className="hidden lg:block text-left">
                  <p className="text-sm font-medium text-[var(--foreground)] group-hover:text-[var(--primary)] transition-colors leading-tight">John Doe</p>
                  <p className="text-xs text-[var(--muted)] leading-tight">Pro Plan</p>
                </div>
              </button>
            </div>
          </div>
        </div>
        
        {/* Progress bar for daily goals */}
        <div className="px-6 pb-2">
          <div className="flex items-center justify-between text-xs text-[var(--muted)] mb-1">
            <span>Daily Learning Goal</span>
            <span>12/15 atoms reviewed</span>
          </div>
          <div className="w-full bg-[var(--accent)] rounded-full h-1.5">
            <div className="bg-gradient-to-r from-indigo-500 to-purple-600 h-1.5 rounded-full transition-all duration-500" style={{width: '80%'}}></div>
          </div>
        </div>
      </header>

      <div className="flex relative">
        {/* Sidebar */}
        <aside className={`${sidebarCollapsed ? 'w-20' : 'w-64'} bg-[var(--card)] border-r border-[var(--border)] min-h-[calc(100vh-73px)] transition-all duration-300 ease-in-out relative`}>
          {/* Toggle Button - positioned on the border */}
          <button
            onClick={() => setSidebarCollapsed(!sidebarCollapsed)}
            className="absolute -right-3 top-6 w-6 h-6 bg-[var(--secondary)] text-white rounded-full flex items-center justify-center hover:bg-[var(--secondary-hover)] transition-colors shadow-md border-2 border-white z-10"
            title={sidebarCollapsed ? "Expand sidebar" : "Collapse sidebar"}
          >
            {sidebarCollapsed ? (
            <ChevronRight className="w-4 h-4" />
            ) : (
            <ChevronLeft className="w-4 h-4" />
            )}
          </button>
          <div className="p-4">
            {sidebarCollapsed ? (
              <button
                className="w-12 h-12 mx-auto flex items-center justify-center bg-[var(--primary)] text-white rounded-xl hover:bg-[var(--primary-hover)] transition-colors shadow-md"
                title="New Note"
              >
                <Plus className="w-7 h-7" />
              </button>
            ) : (
              <button className="w-full flex items-center justify-center space-x-2 bg-[var(--primary)] text-white px-4 py-2 rounded-lg hover:bg-[var(--primary-hover)] transition-colors">
                <Plus className="w-4 h-4" />
                <span>New Note</span>
              </button>
            )}
          </div>

          <nav className="px-4 space-y-1">
            {!sidebarCollapsed && (
              <div className="mb-4">
                <h3 className="text-xs font-medium text-[var(--muted)] uppercase tracking-wider mb-2">Workspace</h3>
              </div>
            )}
            <div className="space-y-2">
              <a
                href="#"
                className={`flex items-center ${sidebarCollapsed ? 'justify-center h-12 rounded-xl' : 'space-x-3 px-3 py-2 rounded-lg'} bg-[var(--accent)] text-[var(--primary)] transition-all hover:shadow-sm`}
                title={sidebarCollapsed ? "Dashboard" : ""}
              >
                <BarChart3 className={sidebarCollapsed ? "w-6 h-6" : "w-4 h-4"} />
                {!sidebarCollapsed && <span>Dashboard</span>}
              </a>
              <a
                href="#"
                className={`flex items-center ${sidebarCollapsed ? 'justify-center h-12 rounded-xl' : 'space-x-3 px-3 py-2 rounded-lg'} hover:bg-[var(--accent)] transition-all hover:shadow-sm`}
                title={sidebarCollapsed ? "All Notes" : ""}
              >
                <FileText className={sidebarCollapsed ? "w-6 h-6" : "w-4 h-4"} />
                {!sidebarCollapsed && <span>All Notes</span>}
              </a>
              <a
                href="#"
                className={`flex items-center ${sidebarCollapsed ? 'justify-center h-12 rounded-xl' : 'space-x-3 px-3 py-2 rounded-lg'} hover:bg-[var(--accent)] transition-all hover:shadow-sm`}
                title={sidebarCollapsed ? "Knowledge Graph" : ""}
              >
                <Network className={sidebarCollapsed ? "w-6 h-6" : "w-4 h-4"} />
                {!sidebarCollapsed && <span>Knowledge Graph</span>}
              </a>
              <a
                href="#"
                className={`flex items-center ${sidebarCollapsed ? 'justify-center h-12 rounded-xl' : 'space-x-3 px-3 py-2 rounded-lg'} hover:bg-[var(--accent)] transition-all hover:shadow-sm`}
                title={sidebarCollapsed ? "Recall Studio" : ""}
              >
                <Target className={sidebarCollapsed ? "w-6 h-6" : "w-4 h-4"} />
                {!sidebarCollapsed && <span>Recall Studio</span>}
              </a>
            </div>

            {!sidebarCollapsed && (
              <div className="mb-4 mt-6">
                <h3 className="text-xs font-medium text-[var(--muted)] uppercase tracking-wider mb-2">Learning</h3>
              </div>
            )}
            <div className="space-y-2 mt-4">
              <a
                href="#"
                className={`flex items-center ${sidebarCollapsed ? 'justify-center h-12 rounded-xl' : 'space-x-3 px-3 py-2 rounded-lg'} hover:bg-[var(--accent)] transition-all relative hover:shadow-sm`}
                title={sidebarCollapsed ? `Review Queue (${todayReviewAtoms.length})` : ""}
              >
                <Clock className={sidebarCollapsed ? "w-6 h-6" : "w-4 h-4"} />
                {!sidebarCollapsed && (
                  <>
                    <span>Review Queue</span>
                    <span className="ml-auto bg-[var(--primary)] text-white text-xs px-2 py-1 rounded-full">
                      {todayReviewAtoms.length}
                    </span>
                  </>
                )}
                {sidebarCollapsed && (
                  <span className="absolute -top-1 -right-1 bg-[var(--primary)] text-white text-xs w-5 h-5 rounded-full flex items-center justify-center">
                    {todayReviewAtoms.length}
                  </span>
                )}
              </a>
              <a
                href="#"
                className={`flex items-center ${sidebarCollapsed ? 'justify-center h-12 rounded-xl' : 'space-x-3 px-3 py-2 rounded-lg'} hover:bg-[var(--accent)] transition-all hover:shadow-sm`}
                title={sidebarCollapsed ? "Progress" : ""}
              >
                <TrendingUp className={sidebarCollapsed ? "w-6 h-6" : "w-4 h-4"} />
                {!sidebarCollapsed && <span>Progress</span>}
              </a>
              <a
                href="#"
                className={`flex items-center ${sidebarCollapsed ? 'justify-center h-12 rounded-xl' : 'space-x-3 px-3 py-2 rounded-lg'} hover:bg-[var(--accent)] transition-all hover:shadow-sm`}
                title={sidebarCollapsed ? "Favorites" : ""}
              >
                <Star className={sidebarCollapsed ? "w-6 h-6" : "w-4 h-4"} />
                {!sidebarCollapsed && <span>Favorites</span>}
              </a>
            </div>
          </nav>
        </aside>

        {/* Main Content */}
        <main className="flex-1 p-6 bg-[var(--background-secondary)]">
          <div className="max-w-7xl mx-auto">
            {/* Welcome Section */}
            <div className="mb-8">
              <h2 className="text-2xl font-semibold mb-2">Good morning, John!</h2>
              <p className="text-[var(--foreground-secondary)]">
                You have {todayReviewAtoms.length} atoms to review today. Let&apos;s strengthen your knowledge!
              </p>
            </div>

            {/* Stats Grid */}
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
              <div className="bg-[var(--card)] p-6 rounded-xl border border-[var(--border)] hover:shadow-md transition-shadow">
                <div className="flex items-center justify-between">
                  <div>
                    <p className="text-sm text-[var(--foreground-secondary)] mb-1">Knowledge Health</p>
                    <p className="text-2xl font-semibold text-[var(--success)]">{stats.knowledgeHealth}%</p>
                  </div>
                  <div className="p-3 bg-[var(--success-light)] rounded-lg">
                    <Brain className="w-6 h-6 text-[var(--success)]" />
                  </div>
                </div>
              </div>

              <div className="bg-[var(--card)] p-6 rounded-xl border border-[var(--border)] hover:shadow-md transition-shadow">
                <div className="flex items-center justify-between">
                  <div>
                    <p className="text-sm text-[var(--foreground-secondary)] mb-1">Total Atoms</p>
                    <p className="text-2xl font-semibold">{stats.totalAtoms}</p>
                  </div>
                  <div className="p-3 bg-[var(--primary-light)] rounded-lg">
                    <Zap className="w-6 h-6 text-[var(--primary)]" />
                  </div>
                </div>
              </div>

              <div className="bg-[var(--card)] p-6 rounded-xl border border-[var(--border)] hover:shadow-md transition-shadow">
                <div className="flex items-center justify-between">
                  <div>
                    <p className="text-sm text-[var(--foreground-secondary)] mb-1">Review Streak</p>
                    <p className="text-2xl font-semibold text-[var(--warning)]">{stats.reviewStreak} days</p>
                  </div>
                  <div className="p-3 bg-[var(--warning-light)] rounded-lg">
                    <Calendar className="w-6 h-6 text-[var(--warning)]" />
                  </div>
                </div>
              </div>

              <div className="bg-[var(--card)] p-6 rounded-xl border border-[var(--border)] hover:shadow-md transition-shadow">
                <div className="flex items-center justify-between">
                  <div>
                    <p className="text-sm text-[var(--foreground-secondary)] mb-1">Weekly Progress</p>
                    <p className="text-2xl font-semibold text-[var(--primary)]">{stats.weeklyProgress}%</p>
                  </div>
                  <div className="p-3 bg-[var(--primary-light)] rounded-lg">
                    <TrendingUp className="w-6 h-6 text-[var(--primary)]" />
                  </div>
                </div>
              </div>
            </div>

            {/* Main Content Grid */}
            <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
              {/* Today's Review Atoms */}
              <div className="lg:col-span-2">
                <div className="bg-white rounded-2xl border border-gray-100 overflow-hidden shadow-lg">
                  {/* Header */}
                  <div className="px-8 py-6 bg-gradient-to-br from-indigo-50 via-white to-purple-50 border-b border-gray-100">
                    <div className="flex items-center justify-between">
                      <div className="flex items-center space-x-4">
                        <div className="w-12 h-12 bg-gradient-to-br from-indigo-500 to-purple-600 rounded-xl flex items-center justify-center shadow-lg">
                          <Clock className="w-6 h-6 text-white" />
                        </div>
                        <div>
                          <h2 className="text-xl font-bold text-gray-900">Today&apos;s Review</h2>
                          <p className="text-sm text-gray-600 mt-1">
                            {todayReviewAtoms.length} knowledge atoms ready for review
                          </p>
                        </div>
                      </div>
                      <button className="px-4 py-2 text-indigo-600 hover:text-indigo-700 font-medium text-sm bg-white rounded-lg border border-indigo-200 hover:border-indigo-300 hover:bg-indigo-50 transition-all duration-200 shadow-sm">
                        View All
                      </button>
                    </div>
                  </div>

                  {/* Content */}
                  <div className="p-8">
                    <div className="space-y-4">
                      {todayReviewAtoms.map((atom) => (
                        <div key={atom.id} className="group relative bg-gray-50 hover:bg-white rounded-2xl p-6 transition-all duration-300 cursor-pointer border border-gray-100 hover:border-indigo-200 hover:shadow-lg">
                          <div className="flex items-start space-x-6">
                            {/* Mastery Circle */}
                            <div className="flex-shrink-0">
                              <div className="relative w-16 h-16">
                                <svg className="w-16 h-16 transform -rotate-90" viewBox="0 0 36 36">
                                  <path
                                    d="M18 2.0845 a 15.9155 15.9155 0 0 1 0 31.831 a 15.9155 15.9155 0 0 1 0 -31.831"
                                    fill="none"
                                    stroke="#e5e7eb"
                                    strokeWidth="2"
                                  />
                                  <path
                                    d="M18 2.0845 a 15.9155 15.9155 0 0 1 0 31.831 a 15.9155 15.9155 0 0 1 0 -31.831"
                                    fill="none"
                                    stroke={
                                      atom.masteryLevel >= 80 ? '#10b981' :
                                      atom.masteryLevel >= 60 ? '#f59e0b' :
                                      atom.masteryLevel >= 40 ? '#6366f1' :
                                      '#ef4444'
                                    }
                                    strokeWidth="2"
                                    strokeDasharray={`${atom.masteryLevel}, 100`}
                                    className="transition-all duration-500"
                                  />
                                </svg>
                                <div className="absolute inset-0 flex items-center justify-center">
                                  <span className="text-sm font-bold text-gray-700">{atom.masteryLevel}%</span>
                                </div>
                              </div>
                            </div>

                            {/* Content */}
                            <div className="flex-1 min-w-0">
                              <div className="flex items-start justify-between mb-3">
                                <div className="flex-1 pr-4">
                                  <h3 className="text-base font-semibold text-gray-900 leading-relaxed mb-2">
                                    {atom.content}
                                  </h3>
                                  <div className="flex items-center space-x-3">
                                    <span className={`inline-flex items-center px-3 py-1 rounded-full text-xs font-medium ${
                                      atom.category === 'React' ? 'bg-blue-100 text-blue-700' :
                                      atom.category === 'Learning Theory' ? 'bg-green-100 text-green-700' :
                                      atom.category === 'Data Visualization' ? 'bg-purple-100 text-purple-700' :
                                      atom.category === 'CSS' ? 'bg-pink-100 text-pink-700' :
                                      atom.category === 'Algorithms' ? 'bg-orange-100 text-orange-700' :
                                      'bg-gray-100 text-gray-700'
                                    }`}>
                                      {atom.category}
                                    </span>
                                    {atom.nextReview === 'Overdue' && (
                                      <span className="inline-flex items-center px-3 py-1 rounded-full text-xs font-medium bg-red-100 text-red-700">
                                        <div className="w-1.5 h-1.5 bg-red-500 rounded-full mr-1.5"></div>
                                        Overdue
                                      </span>
                                    )}
                                  </div>
                                </div>
                                <ChevronRight className="w-5 h-5 text-gray-400 group-hover:text-indigo-500 transition-colors flex-shrink-0" />
                              </div>

                              <div className="flex items-center space-x-6 text-sm text-gray-500">
                                <div className="flex items-center space-x-1">
                                  <Clock className="w-4 h-4" />
                                  <span>{atom.lastReview}</span>
                                </div>
                                <div className="flex items-center space-x-1">
                                  <Target className="w-4 h-4" />
                                  <span>Level {atom.difficulty}/5</span>
                                </div>
                                <div className="flex items-center space-x-1">
                                  <BarChart3 className="w-4 h-4" />
                                  <span>{atom.reviewCount} reviews</span>
                                </div>
                              </div>
                            </div>
                          </div>
                        </div>
                      ))}
                    </div>

                    {/* Action Button */}
                    <div className="mt-8 pt-6 border-t border-gray-100">
                      <button className="w-full bg-gradient-to-r from-indigo-600 to-purple-600 hover:from-indigo-700 hover:to-purple-700 text-white font-semibold py-4 px-6 rounded-xl transition-all duration-200 shadow-lg hover:shadow-xl flex items-center justify-center space-x-3 group">
                        <Zap className="w-5 h-5 group-hover:scale-110 transition-transform" />
                        <span>Start Review Session</span>
                        <div className="w-2 h-2 bg-white rounded-full opacity-75 group-hover:opacity-100 transition-opacity"></div>
                      </button>
                    </div>
                  </div>
                </div>
              </div>

              {/* Sidebar Content */}
              <div className="space-y-6">
                {/* Recent Notes */}
                <div className="bg-[var(--card)] rounded-xl border border-[var(--border)] overflow-hidden">
                  <div className="p-4 border-b border-[var(--border)]">
                    <h3 className="font-semibold">Recent Notes</h3>
                  </div>
                  <div className="p-4 space-y-3">
                    {recentNotes.map((note) => (
                      <div key={note.id} className="flex items-center space-x-3 p-3 hover:bg-[var(--accent)] rounded-lg transition-colors cursor-pointer">
                        <BookOpen className="w-4 h-4 text-[var(--primary)] flex-shrink-0" />
                        <div className="flex-1 min-w-0">
                          <p className="text-sm font-medium truncate">{note.title}</p>
                          <p className="text-xs text-[var(--foreground-secondary)]">
                            {note.atoms} atoms • {note.created}
                          </p>
                        </div>
                      </div>
                    ))}
                  </div>
                </div>

                {/* Quick Actions */}
                <div className="bg-[var(--card)] rounded-xl border border-[var(--border)] overflow-hidden">
                  <div className="p-4 border-b border-[var(--border)]">
                    <h3 className="font-semibold">Quick Actions</h3>
                  </div>
                  <div className="p-4 space-y-2">
                    <button className="w-full flex items-center space-x-3 p-3 hover:bg-[var(--accent)] rounded-lg transition-colors text-left">
                      <Plus className="w-4 h-4 text-[var(--primary)]" />
                      <span className="text-sm">Create New Note</span>
                    </button>
                    <button className="w-full flex items-center space-x-3 p-3 hover:bg-[var(--accent)] rounded-lg transition-colors text-left">
                      <Network className="w-4 h-4 text-[var(--primary)]" />
                      <span className="text-sm">Explore Knowledge Graph</span>
                    </button>
                    <button className="w-full flex items-center space-x-3 p-3 hover:bg-[var(--accent)] rounded-lg transition-colors text-left">
                      <Target className="w-4 h-4 text-[var(--primary)]" />
                      <span className="text-sm">Practice Recall</span>
                    </button>
                    <button className="w-full flex items-center space-x-3 p-3 hover:bg-[var(--accent)] rounded-lg transition-colors text-left">
                      <BarChart3 className="w-4 h-4 text-[var(--primary)]" />
                      <span className="text-sm">View Analytics</span>
                    </button>
                  </div>
                </div>
              </div>
            </div>

            {/* Learning Activity Section */}
            <div className="mt-8">
              <div className="bg-[var(--card)] rounded-xl border border-[var(--border)] overflow-hidden">
                <div className="p-6 border-b border-[var(--border)]">
                  <div className="flex items-center justify-between">
                    <div>
                      <h3 className="text-lg font-semibold">Learning Activity</h3>
                      <p className="text-sm text-[var(--foreground-secondary)] mt-1">
                        167 study sessions in the last year
                      </p>
                    </div>
                    <button className="text-[var(--primary)] hover:text-[var(--primary-hover)] text-sm font-medium">
                      Activity Settings
                    </button>
                  </div>
                </div>
                <div className="p-6">
                  <div className="overflow-x-auto">
                    {/* Month labels */}
                    <div className="flex mb-2 text-xs text-[var(--foreground-secondary)]">
                      <div className="w-8"></div> {/* Space for day labels */}
                      {['Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec', 'Jan', 'Feb', 'Mar', 'Apr', 'May'].map((month) => (
                        <div key={month} className="flex-1 text-center min-w-[52px]">
                          {month}
                        </div>
                      ))}
                    </div>

                    {/* Contribution grid */}
                    <div className="flex">
                      {/* Day labels */}
                      <div className="flex flex-col text-xs text-[var(--foreground-secondary)] mr-2">
                        <div className="h-3 mb-1"></div> {/* Spacer for alignment */}
                        <div className="h-3 mb-1">Mon</div>
                        <div className="h-3 mb-1"></div>
                        <div className="h-3 mb-1">Wed</div>
                        <div className="h-3 mb-1"></div>
                        <div className="h-3 mb-1">Fri</div>
                        <div className="h-3 mb-1"></div>
                      </div>

                      {/* Grid of contribution squares */}
                      <div className="flex-1 grid grid-cols-53 gap-1">
                        {(() => {
                          const squares = [];
                          // Use a seed-based approach for consistent rendering
                          const seedData = [0.1, 0.3, 0.7, 0.9, 0.2, 0.5, 0.8, 0.0, 0.4, 0.6];
                          
                          for (let week = 0; week < 53; week++) {
                            for (let day = 0; day < 7; day++) {
                              const seedIndex = (week * 7 + day) % seedData.length;
                              const activity = seedData[seedIndex];
                              let bgColor = 'bg-[var(--muted-light)]'; // No activity

                              if (activity > 0.8) {
                                bgColor = 'bg-[var(--primary)]'; // High activity
                              } else if (activity > 0.6) {
                                bgColor = 'bg-purple-500'; // Medium-high activity
                              } else if (activity > 0.4) {
                                bgColor = 'bg-purple-400'; // Medium activity
                              } else if (activity > 0.2) {
                                bgColor = 'bg-purple-300'; // Low activity
                              }

                              const studySessions = Math.floor(activity * 8); // 0-8 sessions per day
                              const tooltipText = studySessions === 0 ? 'No study sessions on this day' :
                                                studySessions === 1 ? '1 study session on this day' :
                                                `${studySessions} study sessions on this day`;

                              squares.push(
                                <div
                                  key={`${week}-${day}`}
                                  className={`w-3 h-3 rounded-sm ${bgColor} hover:ring-1 hover:ring-[var(--primary)] transition-all cursor-pointer`}
                                  title={tooltipText}
                                />
                              );
                            }
                          }
                          return squares;
                        })()}
                      </div>
                    </div>

                    {/* Legend */}
                    <div className="flex items-center justify-between mt-4 text-xs text-[var(--foreground-secondary)]">
                      <span>Learn how we track study sessions</span>
                      <div className="flex items-center space-x-2">
                        <span>Less</span>
                        <div className="flex space-x-1">
                          <div className="w-3 h-3 bg-[var(--muted-light)] rounded-sm"></div>
                          <div className="w-3 h-3 bg-purple-300 rounded-sm"></div>
                          <div className="w-3 h-3 bg-purple-400 rounded-sm"></div>
                          <div className="w-3 h-3 bg-purple-500 rounded-sm"></div>
                          <div className="w-3 h-3 bg-[var(--primary)] rounded-sm"></div>
                        </div>
                        <span>More</span>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            {/* Learning Insights */}
            <div className="mt-8">
              <div className="bg-[var(--card)] rounded-xl border border-[var(--border)] overflow-hidden">
                <div className="p-6 border-b border-[var(--border)]">
                  <h3 className="text-lg font-semibold">Learning Insights</h3>
                  <p className="text-sm text-[var(--foreground-secondary)] mt-1">
                    AI-powered recommendations to optimize your learning
                  </p>
                </div>
                <div className="p-6">
                  <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                    <div className="text-center">
                      <div className="w-12 h-12 bg-[var(--success-light)] rounded-lg flex items-center justify-center mx-auto mb-3">
                        <TrendingUp className="w-6 h-6 text-[var(--success)]" />
                      </div>
                      <h4 className="font-semibold mb-2">Optimal Study Time</h4>
                      <p className="text-sm text-[var(--foreground-secondary)]">
                        Your peak learning performance is between 9-11 AM
                      </p>
                    </div>

                    <div className="text-center">
                      <div className="w-12 h-12 bg-[var(--warning-light)] rounded-lg flex items-center justify-center mx-auto mb-3">
                        <Target className="w-6 h-6 text-[var(--warning)]" />
                      </div>
                      <h4 className="font-semibold mb-2">Focus Areas</h4>
                      <p className="text-sm text-[var(--foreground-secondary)]">
                        Strengthen connections in Machine Learning concepts
                      </p>
                    </div>

                    <div className="text-center">
                      <div className="w-12 h-12 bg-[var(--primary-light)] rounded-lg flex items-center justify-center mx-auto mb-3">
                        <Network className="w-6 h-6 text-[var(--primary)]" />
                      </div>
                      <h4 className="font-semibold mb-2">Knowledge Gaps</h4>
                      <p className="text-sm text-[var(--foreground-secondary)]">
                        3 unconnected atoms detected in your knowledge graph
                      </p>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </main>
      </div>
    </div>
  )
}
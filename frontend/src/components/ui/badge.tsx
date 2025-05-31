import { ReactNode } from 'react'

interface BadgeProps {
  children: ReactNode
  variant?: 'default' | 'primary' | 'success' | 'warning' | 'error' | 'secondary'
  size?: 'sm' | 'md'
  className?: string
}

export function Badge({ 
  children, 
  variant = 'default', 
  size = 'sm', 
  className = '' 
}: BadgeProps) {
  const baseClasses = 'inline-flex items-center font-medium rounded-full'
  
  const variantClasses = {
    default: 'bg-[var(--muted-light)] text-[var(--foreground)]',
    primary: 'bg-[var(--primary)] text-white',
    success: 'bg-[var(--success)] text-white',
    warning: 'bg-[var(--warning)] text-white',
    error: 'bg-[var(--error)] text-white',
    secondary: 'bg-[var(--accent)] text-[var(--foreground)]'
  }
  
  const sizeClasses = {
    sm: 'px-2 py-1 text-xs',
    md: 'px-3 py-1.5 text-sm'
  }
  
  return (
    <span className={`${baseClasses} ${variantClasses[variant]} ${sizeClasses[size]} ${className}`}>
      {children}
    </span>
  )
}

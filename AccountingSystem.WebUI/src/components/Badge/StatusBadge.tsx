interface StatusBadgeProps {
  children: string
  tone?: 'neutral' | 'success' | 'warning' | 'danger'
}

export function StatusBadge({ children, tone = 'neutral' }: StatusBadgeProps) {
  return <span className={`status-badge status-${tone}`}>{children}</span>
}

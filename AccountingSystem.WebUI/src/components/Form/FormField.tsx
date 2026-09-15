import type { ReactNode } from 'react'

interface FormFieldProps {
  label: string
  htmlFor?: string
  children: ReactNode
}

export function FormField({ label, htmlFor, children }: FormFieldProps) {
  return (
    <label className="form-field" htmlFor={htmlFor}>
      <span>{label}</span>
      {children}
    </label>
  )
}

import type { ReactNode } from 'react'

export interface DataTableColumn<T> {
  header: string
  render: (item: T) => ReactNode
  className?: string
}

interface DataTableProps<T> {
  data: T[]
  columns: DataTableColumn<T>[]
  getRowKey: (item: T, index: number) => string | number
  emptyText?: string
}

export function DataTable<T>({
  data,
  columns,
  getRowKey,
  emptyText = 'Brak danych'
}: DataTableProps<T>) {
  if (data.length === 0) {
    return <div className="state state-empty">{emptyText}</div>
  }

  return (
    <div className="table-shell">
      <table className="data-table">
        <thead>
          <tr>
            {columns.map((column) => (
              <th key={column.header} className={column.className}>
                {column.header}
              </th>
            ))}
          </tr>
        </thead>
        <tbody>
          {data.map((item, index) => (
            <tr key={getRowKey(item, index)}>
              {columns.map((column) => (
                <td key={column.header} className={column.className}>
                  {column.render(item)}
                </td>
              ))}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}

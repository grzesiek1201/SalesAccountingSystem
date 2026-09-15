import { Navigate, Route, Routes } from 'react-router-dom'
import { AppLayout } from '../components/Layout/AppLayout'
import { Customers } from '../pages/Customers/Customers'
import { Dashboard } from '../pages/Dashboard/Dashboard'
import { Products } from '../pages/Products/Products'
import { SalesDocumentsPage } from '../pages/Sales/SalesDocumentsPage'

export function AppRoutes() {
  return (
    <Routes>
      <Route element={<AppLayout />}>
        <Route path="/" element={<Navigate to="/dashboard" replace />} />
        <Route path="/dashboard" element={<Dashboard />} />
        <Route path="/customers" element={<Customers />} />
        <Route path="/products" element={<Products />} />
        <Route path="/sales" element={<Navigate to="/sales/quotations" replace />} />
        <Route
          path="/sales/quotations"
          element={<SalesDocumentsPage type="quotation" />}
        />
        <Route path="/sales/orders" element={<SalesDocumentsPage type="order" />} />
        <Route
          path="/sales/invoices"
          element={<SalesDocumentsPage type="invoice" />}
        />
      </Route>
      <Route path="*" element={<Navigate to="/dashboard" replace />} />
    </Routes>
  )
}

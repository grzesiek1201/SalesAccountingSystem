import { NavLink, Outlet, useLocation } from 'react-router-dom'
import { ApiStatusBadge } from './ApiStatusBadge'

const mainNavigation = [
  { to: '/dashboard', label: 'Dashboard' },
  { to: '/customers', label: 'Klienci' },
  { to: '/products', label: 'Produkty' },
  { to: '/sales/quotations', label: 'Sprzedaż' }
]

export function AppLayout() {
  const location = useLocation()

  return (
    <div className="app-shell">
      <aside className="sidebar">
        <div className="brand">
          <span className="brand-mark">AS</span>
          <div>
            <strong>AccountingSystem</strong>
            <span>Panel operacyjny</span>
          </div>
        </div>

        <nav className="sidebar-nav" aria-label="Główna nawigacja">
          {mainNavigation.map((item) => (
            <NavLink
              key={item.to}
              to={item.to}
              className={({ isActive }) =>
                isActive ||
                (item.to.startsWith('/sales') &&
                  location.pathname.startsWith('/sales'))
                  ? 'nav-item active'
                  : 'nav-item'
              }
            >
              {item.label}
            </NavLink>
          ))}
        </nav>
      </aside>

      <div className="content-shell">
        <header className="topbar">
          <div>
            <span className="eyebrow">Frontend</span>
            <h1>AccountingSystem.WebUI</h1>
          </div>
          <ApiStatusBadge />
        </header>

        <main className="main-content">
          <Outlet />
        </main>
      </div>
    </div>
  )
}

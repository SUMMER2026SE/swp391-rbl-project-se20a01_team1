import { NavLink, Outlet } from 'react-router-dom';

const navItems = [
  { to: '/', label: 'Tìm trọ', end: true, icon: HomeIcon },
  { to: '/admin/kyc', label: 'Duyệt KYC', end: false, icon: ShieldIcon },
  { to: '/admin/rooming-houses', label: 'Duyệt khu trọ', end: false, icon: BuildingIcon },
] as const;

export function AppShell() {
  return (
    <div className="min-h-screen flex flex-col">
      <header className="sticky top-0 z-50 border-b border-slate-200/80 bg-white/80 backdrop-blur-xl">
        <div className="mx-auto flex h-16 max-w-6xl items-center justify-between gap-4 px-4 sm:px-6">
          <NavLink to="/" className="flex items-center gap-3 group">
            <div className="flex h-10 w-10 items-center justify-center rounded-xl bg-gradient-to-br from-brand-500 to-brand-700 text-white shadow-lg shadow-brand-600/25 transition group-hover:scale-105">
              <span className="text-sm font-black">SR</span>
            </div>
            <div className="hidden sm:block">
              <p className="text-sm font-bold text-slate-900 leading-tight">Smart Rental</p>
              <p className="text-[11px] text-slate-500">Interval 1 · Demo</p>
            </div>
          </NavLink>

          <nav className="flex items-center gap-1 rounded-2xl bg-slate-100/80 p-1 ring-1 ring-slate-200/60">
            {navItems.map(({ to, label, end, icon: Icon }) => (
              <NavLink
                key={to}
                to={to}
                end={end}
                className={({ isActive }) =>
                  `flex items-center gap-2 rounded-xl px-3 py-2 text-xs font-semibold transition-all sm:px-4 sm:text-sm ${
                    isActive
                      ? 'bg-white text-brand-700 shadow-sm ring-1 ring-slate-200/80'
                      : 'text-slate-600 hover:text-slate-900'
                  }`
                }
              >
                <Icon className="h-4 w-4 shrink-0" />
                <span className="hidden md:inline">{label}</span>
              </NavLink>
            ))}
          </nav>
        </div>
      </header>

      <main className="mx-auto w-full max-w-6xl flex-1 px-4 py-8 sm:px-6 sm:py-10">
        <Outlet />
      </main>

      <footer className="mt-auto border-t border-slate-200/60 bg-white/50 py-6 text-center text-xs text-slate-500">
        Smart Rental Platform · Chỉ giao diện demo — API qua Swagger giữ nguyên
      </footer>
    </div>
  );
}

function HomeIcon({ className }: { className?: string }) {
  return (
    <svg className={className} fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
      <path strokeLinecap="round" strokeLinejoin="round" d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6" />
    </svg>
  );
}

function ShieldIcon({ className }: { className?: string }) {
  return (
    <svg className={className} fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
      <path strokeLinecap="round" strokeLinejoin="round" d="M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z" />
    </svg>
  );
}

function BuildingIcon({ className }: { className?: string }) {
  return (
    <svg className={className} fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
      <path strokeLinecap="round" strokeLinejoin="round" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
    </svg>
  );
}

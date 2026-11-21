import React from 'react';
import { NavLink, Outlet } from 'react-router-dom';
import {
  BadgeCheck,
  BookOpen,
  ChartNoAxesCombined,
  ClipboardList,
  FolderKanban,
  LayoutDashboard,
  ShieldAlert,
} from 'lucide-react';
import { Navbar } from '../common/Navbar';
import { Footer } from '../common/Footer';
import { useAuth } from '../../context/AuthContext';

const adminLinks = [
  { to: '/admin', label: 'Overview', icon: LayoutDashboard, end: true },
  { to: '/admin/categories', label: 'Threat categories', icon: ShieldAlert },
  { to: '/admin/modules', label: 'Learning modules', icon: BookOpen },
  { to: '/admin/quizzes', label: 'Quizzes', icon: ClipboardList },
  { to: '/admin/scenarios', label: 'Scenarios', icon: FolderKanban },
  { to: '/admin/badges', label: 'Safety badges', icon: BadgeCheck },
];

export const AdminLayout: React.FC = () => {
  const { user } = useAuth();

  return (
    <div className="min-h-screen flex flex-col bg-[#F7F7F5] text-[#191919] font-sans antialiased">
      <Navbar transparentOnHero={false} />
      <div className="pt-20 flex-grow">
        <div className="border-b border-gray-200 bg-white">
          <div className="mx-auto max-w-[1440px] px-5 py-5 sm:px-8 lg:px-10">
            <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
              <div className="flex items-center gap-3">
                <div className="flex h-10 w-10 items-center justify-center rounded-xl bg-[#191919] text-white">
                  <ChartNoAxesCombined size={19} aria-hidden="true" />
                </div>
                <div>
                  <div className="flex items-center gap-2">
                    <h1 className="font-serif text-xl sm:text-2xl">Administration</h1>
                    <span className="rounded-full bg-amber-50 px-2.5 py-1 text-[10px] font-bold uppercase tracking-wider text-amber-800">
                      Admin
                    </span>
                  </div>
                  <p className="mt-0.5 text-xs text-[#191919]/55">
                    DigitalShield platform operations
                  </p>
                </div>
              </div>
              <div className="text-xs text-[#191919]/60">
                Signed in as <span className="font-semibold text-[#191919]">{user?.name}</span>
                {user?.email && <span className="hidden sm:inline"> · {user.email}</span>}
              </div>
            </div>
          </div>
        </div>

        <div className="mx-auto grid max-w-[1440px] gap-6 px-5 py-6 sm:px-8 lg:grid-cols-[230px_minmax(0,1fr)] lg:gap-9 lg:px-10 lg:py-9">
          <aside aria-label="Administration navigation">
            <nav className="flex gap-2 overflow-x-auto pb-2 lg:sticky lg:top-28 lg:flex-col lg:overflow-visible lg:pb-0">
              {adminLinks.map(({ to, label, icon: Icon, end }) => (
                <NavLink
                  key={to}
                  to={to}
                  end={end}
                  className={({ isActive }) =>
                    `flex shrink-0 items-center gap-3 rounded-xl px-3.5 py-2.5 text-xs font-medium transition-colors ${
                      isActive
                        ? 'bg-[#191919] text-white shadow-sm'
                        : 'text-[#191919]/65 hover:bg-white hover:text-[#191919]'
                    }`
                  }
                >
                  <Icon size={16} aria-hidden="true" />
                  {label}
                </NavLink>
              ))}
            </nav>
          </aside>

          <main className="min-w-0">
            <Outlet />
          </main>
        </div>
      </div>
      <Footer />
    </div>
  );
};

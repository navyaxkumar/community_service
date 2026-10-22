import React from 'react';
import { Outlet, NavLink } from 'react-router-dom';
import { Navbar } from '../common/Navbar';
import { Footer } from '../common/Footer';
import { useAuth } from '../../context/AuthContext';

export const AppLayout: React.FC = () => {
  const { user } = useAuth();

  const tabClass = ({ isActive }: { isActive: boolean }) =>
    `px-4 py-2 text-xs sm:text-sm font-medium border-b-2 transition-colors ${
      isActive
        ? 'border-[#191919] text-[#191919]'
        : 'border-transparent text-[#191919]/60 hover:text-[#191919] hover:border-gray-200'
    }`;

  return (
    <div className="min-h-screen flex flex-col bg-[#FAFAFA] text-[#191919] font-sans antialiased selection:bg-[#191919] selection:text-white">
      <Navbar transparentOnHero={false} />

      {/* Sub-header User Banner */}
      <div className="pt-20 bg-white border-b border-gray-200">
        <div className="max-w-7xl mx-auto px-6 sm:px-10 md:px-14 pt-6 pb-0">
          <div className="flex flex-col sm:flex-row sm:items-center justify-between pb-4 gap-2">
            <div>
              <span className="text-[11px] uppercase tracking-wider font-semibold text-[#191919]/50 block">
                Signed-in Account
              </span>
              <h1 className="text-xl sm:text-2xl font-serif text-[#191919]">
                {user?.name || 'Welcome Back'}
              </h1>
            </div>
            <div className="text-xs text-[#191919]/60">
              Role: <span className="font-semibold text-[#191919]">{user?.role}</span> &bull; {user?.email}
            </div>
          </div>

          {/* Navigation Tabs */}
          <div className="flex space-x-1 sm:space-x-3 overflow-x-auto">
            <NavLink to="/dashboard" className={tabClass}>
              Dashboard
            </NavLink>
            <NavLink to="/profile" className={tabClass}>
              Profile
            </NavLink>
            <NavLink to="/scam-checker" className={tabClass}>
              Scam Checker
            </NavLink>
            <NavLink to="/learn" className={tabClass}>
              Learning Library
            </NavLink>
            <NavLink to="/quizzes" className={tabClass}>
              Quizzes
            </NavLink>
          </div>
        </div>
      </div>

      <main className="flex-grow max-w-7xl mx-auto w-full px-6 sm:px-10 md:px-14 py-8">
        <Outlet />
      </main>

      <Footer />
    </div>
  );
};

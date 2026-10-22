import React from 'react';
import { Outlet, NavLink } from 'react-router-dom';
import { Navbar } from '../common/Navbar';
import { Footer } from '../common/Footer';
import { useAuth } from '../../context/AuthContext';

export const AdminLayout: React.FC = () => {
  const { user } = useAuth();

  return (
    <div className="min-h-screen flex flex-col bg-[#F8F9FA] text-[#191919] font-sans antialiased selection:bg-[#191919] selection:text-white">
      <Navbar transparentOnHero={false} />

      {/* Admin Notice Banner */}
      <div className="pt-20 bg-[#191919] text-white">
        <div className="max-w-7xl mx-auto px-6 sm:px-10 md:px-14 py-6">
          <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-3">
            <div className="flex items-center space-x-3">
              <span className="px-2 py-0.5 bg-red-600 text-white text-[10px] uppercase font-bold tracking-widest rounded">
                Admin
              </span>
              <div>
                <h1 className="text-xl sm:text-2xl font-serif">Administrative Portal</h1>
                <p className="text-xs text-white/60 mt-0.5">
                  Platform curriculum management, category rules, and system oversight.
                </p>
              </div>
            </div>
            <div className="text-xs text-white/70">
              Authenticated as <span className="font-semibold text-white">{user?.name}</span> ({user?.email})
            </div>
          </div>
        </div>
      </div>

      {/* Admin Navigation Strip */}
      <div className="bg-white border-b border-gray-200">
        <div className="max-w-7xl mx-auto px-6 sm:px-10 md:px-14 flex space-x-4 overflow-x-auto">
          <NavLink
            to="/admin"
            end
            className={({ isActive }) =>
              `py-3 px-1 text-xs font-medium border-b-2 transition-colors ${
                isActive
                  ? 'border-[#191919] text-[#191919]'
                  : 'border-transparent text-[#191919]/60 hover:text-[#191919]'
              }`
            }
          >
            Overview & Curriculum Management
          </NavLink>
        </div>
      </div>

      <main className="flex-grow max-w-7xl mx-auto w-full px-6 sm:px-10 md:px-14 py-8">
        <Outlet />
      </main>

      <Footer />
    </div>
  );
};

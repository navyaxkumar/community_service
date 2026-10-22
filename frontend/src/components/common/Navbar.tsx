import React, { useState } from 'react';
import { Link, useLocation } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';

interface NavbarProps {
  transparentOnHero?: boolean;
}

export const Navbar: React.FC<NavbarProps> = ({ transparentOnHero = false }) => {
  const { isAuthenticated, isAdmin, user, logout } = useAuth();
  const [mobileMenuOpen, setMobileMenuOpen] = useState(false);
  const location = useLocation();

  const isHome = location.pathname === '/';
  const headerBgClass = transparentOnHero && isHome
    ? 'bg-transparent'
    : 'bg-white/95 backdrop-blur-md border-b border-gray-100';

  const isActive = (path: string) => {
    if (path === '/') return location.pathname === '/';
    return location.pathname.startsWith(path);
  };

  const navLinkClass = (path: string) =>
    `text-sm font-medium transition-colors duration-150 ${
      isActive(path)
        ? 'text-[#191919] font-semibold'
        : 'text-[#191919]/70 hover:text-[#191919]'
    }`;

  return (
    <header className={`fixed top-0 left-0 right-0 z-50 transition-colors duration-200 ${headerBgClass}`}>
      <div className="max-w-7xl mx-auto px-6 sm:px-10 md:px-14 py-4 flex items-center justify-between">
        {/* Brand / Logo */}
        <Link
          to="/"
          className="flex items-center gap-[10px] group focus:outline-none focus-visible:ring-2 focus-visible:ring-[#191919] rounded-md py-1 pr-2"
          aria-label="DigitalShield Home"
        >
          <svg
            className="w-5 h-5 text-[#191919] transition-transform duration-200 group-hover:scale-105"
            viewBox="0 0 24 24"
            fill="none"
            xmlns="http://www.w3.org/2000/svg"
            aria-hidden="true"
          >
            <path
              d="M12 2L4 5.5V11.5C4 16.5 7.4 21.1 12 22.3C16.6 21.1 20 16.5 20 11.5V5.5L12 2Z"
              stroke="currentColor"
              strokeWidth="2"
              strokeLinecap="round"
              strokeLinejoin="round"
            />
            <path
              d="M12 6.5V17.5M8 11.5H16"
              stroke="currentColor"
              strokeWidth="1.75"
              strokeLinecap="round"
              strokeLinejoin="round"
              className="opacity-70"
            />
          </svg>
          <span className="font-semibold text-base tracking-tight text-[#191919]">
            DigitalShield
          </span>
        </Link>

        {/* Center Desktop Navigation */}
        <nav className="hidden md:flex items-center space-x-7" aria-label="Main Navigation">
          <Link to="/" className={navLinkClass('/')}>
            Home
          </Link>
          <Link to="/features" className={navLinkClass('/features')}>
            Features
          </Link>
          <Link to="/scam-checker" className={navLinkClass('/scam-checker')}>
            Scam Checker
          </Link>
          <Link to="/learn" className={navLinkClass('/learn')}>
            Learn
          </Link>
          <Link to="/quizzes" className={navLinkClass('/quizzes')}>
            Quizzes
          </Link>
          {isAuthenticated && (
            <Link to="/dashboard" className={navLinkClass('/dashboard')}>
              Dashboard
            </Link>
          )}
          {isAdmin && (
            <Link to="/admin" className={navLinkClass('/admin')}>
              Admin
            </Link>
          )}
        </nav>

        {/* Right Desktop Actions */}
        <div className="hidden md:flex items-center space-x-4">
          {isAuthenticated ? (
            <div className="flex items-center space-x-3">
              <Link
                to="/profile"
                className="text-xs font-medium text-[#191919]/80 hover:text-[#191919] px-3 py-1.5 rounded-full border border-gray-200 hover:bg-gray-50 transition-colors flex items-center space-x-1.5"
                title="View Profile"
              >
                <span className="w-2 h-2 rounded-full bg-emerald-500" />
                <span>{user?.name?.split(' ')[0] || 'Account'}</span>
              </Link>
              <button
                type="button"
                onClick={logout}
                className="text-xs font-medium text-[#191919]/60 hover:text-red-600 transition-colors py-1 px-2"
              >
                Sign Out
              </button>
            </div>
          ) : (
            <div className="flex items-center space-x-3">
              <Link
                to="/login"
                className="text-xs font-medium text-[#191919]/70 hover:text-[#191919] transition-colors py-1 px-2"
              >
                Sign In
              </Link>
              <Link
                to="/scam-checker"
                className="bg-[#191919] text-white text-xs sm:text-sm font-medium px-4 sm:px-5 py-2 sm:py-2.5 rounded-full hover:bg-black/85 active:scale-95 transition-all duration-200 shadow-sm"
              >
                Check for Scams
              </Link>
            </div>
          )}
        </div>

        {/* Mobile Hamburger Button */}
        <div className="md:hidden flex items-center space-x-3">
          {!isAuthenticated && (
            <Link
              to="/scam-checker"
              className="bg-[#191919] text-white text-xs font-medium px-3.5 py-1.5 rounded-full shadow-sm"
            >
              Check
            </Link>
          )}
          <button
            type="button"
            onClick={() => setMobileMenuOpen(!mobileMenuOpen)}
            className="p-2 text-[#191919] hover:bg-gray-100 rounded-lg transition-colors focus:outline-none focus:ring-2 focus:ring-[#191919]/20"
            aria-label="Toggle Navigation Menu"
            aria-expanded={mobileMenuOpen}
          >
            {mobileMenuOpen ? (
              <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M6 18L18 6M6 6l12 12" />
              </svg>
            ) : (
              <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M4 6h16M4 12h16M4 18h16" />
              </svg>
            )}
          </button>
        </div>
      </div>

      {/* Mobile Drawer Menu */}
      {mobileMenuOpen && (
        <div className="md:hidden bg-white border-b border-gray-200 px-6 py-5 space-y-4 shadow-xl">
          <div className="flex flex-col space-y-3">
            <Link
              to="/"
              onClick={() => setMobileMenuOpen(false)}
              className={navLinkClass('/')}
            >
              Home
            </Link>
            <Link
              to="/features"
              onClick={() => setMobileMenuOpen(false)}
              className={navLinkClass('/features')}
            >
              Features
            </Link>
            <Link
              to="/scam-checker"
              onClick={() => setMobileMenuOpen(false)}
              className={navLinkClass('/scam-checker')}
            >
              Scam Checker
            </Link>
            <Link
              to="/learn"
              onClick={() => setMobileMenuOpen(false)}
              className={navLinkClass('/learn')}
            >
              Learn Library
            </Link>
            <Link
              to="/quizzes"
              onClick={() => setMobileMenuOpen(false)}
              className={navLinkClass('/quizzes')}
            >
              Interactive Quizzes
            </Link>

            {isAuthenticated && (
              <>
                <div className="h-px bg-gray-100 my-2" />
                <Link
                  to="/dashboard"
                  onClick={() => setMobileMenuOpen(false)}
                  className={navLinkClass('/dashboard')}
                >
                  Dashboard
                </Link>
                <Link
                  to="/profile"
                  onClick={() => setMobileMenuOpen(false)}
                  className={navLinkClass('/profile')}
                >
                  My Profile ({user?.name})
                </Link>
                {isAdmin && (
                  <Link
                    to="/admin"
                    onClick={() => setMobileMenuOpen(false)}
                    className={navLinkClass('/admin')}
                  >
                    Admin Portal
                  </Link>
                )}
                <button
                  type="button"
                  onClick={() => {
                    logout();
                    setMobileMenuOpen(false);
                  }}
                  className="text-left text-sm font-medium text-red-600 pt-2"
                >
                  Sign Out
                </button>
              </>
            )}

            {!isAuthenticated && (
              <div className="pt-3 border-t border-gray-100 flex flex-col space-y-2">
                <Link
                  to="/login"
                  onClick={() => setMobileMenuOpen(false)}
                  className="w-full text-center py-2 text-xs font-medium text-[#191919] border border-gray-300 rounded-lg hover:bg-gray-50"
                >
                  Sign In
                </Link>
                <Link
                  to="/register"
                  onClick={() => setMobileMenuOpen(false)}
                  className="w-full text-center py-2 text-xs font-medium text-white bg-[#191919] rounded-lg hover:bg-black/85"
                >
                  Create Free Account
                </Link>
              </div>
            )}
          </div>
        </div>
      )}
    </header>
  );
};

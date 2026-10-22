import React from 'react';
import { Link } from 'react-router-dom';

export const Footer: React.FC = () => {
  return (
    <footer className="bg-white border-t border-gray-200 mt-auto text-[#191919]">
      <div className="max-w-7xl mx-auto px-6 sm:px-10 md:px-14 py-12 md:py-16">
        <div className="grid grid-cols-1 md:grid-cols-4 gap-8 lg:gap-12 pb-12 border-b border-gray-100">
          {/* Column 1: Brand & Mission */}
          <div className="md:col-span-1 space-y-3">
            <Link to="/" className="flex items-center gap-2.5">
              <svg
                className="w-5 h-5 text-[#191919]"
                viewBox="0 0 24 24"
                fill="none"
                xmlns="http://www.w3.org/2000/svg"
              >
                <path
                  d="M12 2L4 5.5V11.5C4 16.5 7.4 21.1 12 22.3C16.6 21.1 20 16.5 20 11.5V5.5L12 2Z"
                  stroke="currentColor"
                  strokeWidth="2"
                  strokeLinecap="round"
                  strokeLinejoin="round"
                />
              </svg>
              <span className="font-semibold text-base tracking-tight text-[#191919]">
                DigitalShield
              </span>
            </Link>
            <p className="text-xs text-[#191919]/65 leading-relaxed">
              Everyday digital safety and fraud awareness platform helping users recognize online scams, verify suspicious communications, and build safe digital habits.
            </p>
          </div>

          {/* Column 2: Platform Tools */}
          <div>
            <h4 className="text-xs uppercase tracking-wider font-semibold text-[#191919]/50 mb-3">
              Tools & Features
            </h4>
            <ul className="space-y-2 text-xs">
              <li>
                <Link to="/scam-checker" className="text-[#191919]/70 hover:text-[#191919] transition-colors">
                  Scam Checker Engine
                </Link>
              </li>
              <li>
                <Link to="/features" className="text-[#191919]/70 hover:text-[#191919] transition-colors">
                  Capability Overview
                </Link>
              </li>
              <li>
                <Link to="/learn" className="text-[#191919]/70 hover:text-[#191919] transition-colors">
                  Learning Library
                </Link>
              </li>
              <li>
                <Link to="/quizzes" className="text-[#191919]/70 hover:text-[#191919] transition-colors">
                  Interactive Quizzes
                </Link>
              </li>
            </ul>
          </div>

          {/* Column 3: Protected Resources */}
          <div>
            <h4 className="text-xs uppercase tracking-wider font-semibold text-[#191919]/50 mb-3">
              Account & Training
            </h4>
            <ul className="space-y-2 text-xs">
              <li>
                <Link to="/dashboard" className="text-[#191919]/70 hover:text-[#191919] transition-colors">
                  User Dashboard
                </Link>
              </li>
              <li>
                <Link to="/profile" className="text-[#191919]/70 hover:text-[#191919] transition-colors">
                  Profile & Progress
                </Link>
              </li>
              <li>
                <Link to="/login" className="text-[#191919]/70 hover:text-[#191919] transition-colors">
                  Account Sign In
                </Link>
              </li>
              <li>
                <Link to="/register" className="text-[#191919]/70 hover:text-[#191919] transition-colors">
                  Create Free Account
                </Link>
              </li>
            </ul>
          </div>

          {/* Column 4: Important Advisory */}
          <div>
            <h4 className="text-xs uppercase tracking-wider font-semibold text-[#191919]/50 mb-3">
              Educational Advisory
            </h4>
            <p className="text-[11px] text-[#191919]/60 leading-relaxed">
              Automated fraud analysis provides heuristic risk indicators and educational guidance. It is an aid to human judgment and does not guarantee complete security or replace direct verification with trusted institutions.
            </p>
          </div>
        </div>

        {/* Bottom Bar */}
        <div className="pt-8 flex flex-col sm:flex-row items-center justify-between text-xs text-[#191919]/50 space-y-3 sm:space-y-0">
          <span>&copy; {new Date().getFullYear()} DigitalShield. Built for everyday digital safety.</span>
          <div className="flex space-x-6 text-xs">
            <Link to="/features" className="hover:text-[#191919] transition-colors">
              Platform Features
            </Link>
            <Link to="/scam-checker" className="hover:text-[#191919] transition-colors">
              Scam Checker
            </Link>
            <Link to="/learn" className="hover:text-[#191919] transition-colors">
              Learning Modules
            </Link>
          </div>
        </div>
      </div>
    </footer>
  );
};

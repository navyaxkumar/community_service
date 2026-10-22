import React from 'react';

interface NavbarProps {
  onNavigate: (destination: string) => void;
}

export const Navbar: React.FC<NavbarProps> = ({ onNavigate }) => {
  return (
    <header className="fixed top-0 left-0 right-0 z-50 px-6 sm:px-10 md:px-14 py-4 sm:py-5 bg-transparent">
      <nav
        className="w-full max-w-7xl mx-auto flex items-center justify-between"
        aria-label="Main Navigation"
      >
        {/* Left: Custom SVG Shield Mark + Wordmark */}
        <a
          href="#home"
          onClick={(e) => {
            e.preventDefault();
            onNavigate('home');
          }}
          className="flex items-center gap-[10px] group focus:outline-none focus-visible:ring-2 focus-visible:ring-[#191919] rounded-md py-1 pr-2"
          aria-label="DigitalShield Home"
        >
          {/* Custom inline SVG shield mark */}
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
        </a>

        {/* Center: Navigation Links (hidden below md breakpoint) */}
        <div className="hidden md:flex items-center space-x-8">
          <button
            type="button"
            onClick={() => onNavigate('home')}
            className="text-sm text-[#191919]/70 hover:text-[#191919] transition-colors duration-200 focus:outline-none focus-visible:underline"
          >
            Home
          </button>
          <button
            type="button"
            onClick={() => onNavigate('learning')}
            className="text-sm text-[#191919]/70 hover:text-[#191919] transition-colors duration-200 focus:outline-none focus-visible:underline"
          >
            Learn
          </button>
          <button
            type="button"
            onClick={() => onNavigate('scam-checker')}
            className="text-sm text-[#191919]/70 hover:text-[#191919] transition-colors duration-200 focus:outline-none focus-visible:underline"
          >
            Scam Checker
          </button>
          <button
            type="button"
            onClick={() => onNavigate('quizzes')}
            className="text-sm text-[#191919]/70 hover:text-[#191919] transition-colors duration-200 focus:outline-none focus-visible:underline"
          >
            Quizzes
          </button>
        </div>

        {/* Right: Navbar CTA */}
        <div className="flex items-center">
          <button
            type="button"
            onClick={() => onNavigate('scam-checker')}
            className="bg-[#191919] text-white text-xs sm:text-sm font-medium px-4 sm:px-5 py-2 sm:py-2.5 rounded-full hover:bg-black/85 active:scale-95 transition-all duration-200 shadow-sm focus:outline-none focus:ring-2 focus:ring-[#191919]/30"
          >
            Check for Scams
          </button>
        </div>
      </nav>
    </header>
  );
};

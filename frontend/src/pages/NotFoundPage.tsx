import React from 'react';
import { Link } from 'react-router-dom';

export const NotFoundPage: React.FC = () => {
  return (
    <div className="pt-36 pb-24 max-w-md mx-auto px-6 text-center space-y-6">
      <div className="w-16 h-16 mx-auto rounded-full bg-gray-100 flex items-center justify-center text-gray-400 font-mono text-xl font-bold">
        404
      </div>

      <div className="space-y-2">
        <h1 className="text-3xl font-serif text-[#191919]">Page Not Found</h1>
        <p className="text-xs text-[#191919]/65 leading-relaxed">
          The requested URL path does not exist on DigitalShield or may have been relocated.
        </p>
      </div>

      <div className="pt-2 flex flex-col sm:flex-row items-center justify-center gap-3">
        <Link
          to="/"
          className="w-full sm:w-auto bg-[#191919] text-white text-xs font-medium px-6 py-2.5 rounded-full hover:bg-black/85 transition-colors"
        >
          Return Home
        </Link>
        <Link
          to="/scam-checker"
          className="w-full sm:w-auto border border-gray-300 text-xs font-medium px-6 py-2.5 rounded-full hover:bg-gray-50 transition-colors"
        >
          Scam Checker
        </Link>
      </div>
    </div>
  );
};

import React from 'react';
import { Link } from 'react-router-dom';

export const AccessDeniedPage: React.FC = () => {
  return (
    <div className="pt-36 pb-24 max-w-md mx-auto px-6 text-center space-y-6">
      <div className="w-16 h-16 mx-auto rounded-full bg-red-100 text-red-700 flex items-center justify-center font-mono text-xl font-bold">
        403
      </div>

      <div className="space-y-2">
        <h1 className="text-3xl font-serif text-[#191919]">Access Denied</h1>
        <p className="text-xs text-[#191919]/65 leading-relaxed">
          You do not have administrative privileges required to access this resource. Backend security boundaries enforce role authorization on all sensitive endpoints.
        </p>
      </div>

      <div className="pt-2 flex flex-col sm:flex-row items-center justify-center gap-3">
        <Link
          to="/dashboard"
          className="w-full sm:w-auto bg-[#191919] text-white text-xs font-medium px-6 py-2.5 rounded-full hover:bg-black/85 transition-colors"
        >
          Go to User Dashboard
        </Link>
        <Link
          to="/"
          className="w-full sm:w-auto border border-gray-300 text-xs font-medium px-6 py-2.5 rounded-full hover:bg-gray-50 transition-colors"
        >
          Return Home
        </Link>
      </div>
    </div>
  );
};

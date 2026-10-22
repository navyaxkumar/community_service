import React from 'react';
import { ArrowRight } from 'lucide-react';

interface FeatureRowProps {
  number: string;
  label: string;
  subtitle?: string;
  destination: string;
  onNavigate: (destination: string) => void;
}

export const FeatureRow: React.FC<FeatureRowProps> = ({
  number,
  label,
  subtitle,
  destination,
  onNavigate,
}) => {
  return (
    <button
      type="button"
      onClick={() => onNavigate(destination)}
      className="group w-full text-left bg-[#F4F3F3] hover:bg-[#EAEAEA] active:bg-[#E2E1E1] transition-all duration-200 cursor-pointer px-4 sm:px-6 py-3.5 sm:py-4 flex items-center justify-between rounded-sm focus:outline-none focus:ring-2 focus:ring-[#191919]/30"
      aria-label={`${number} ${label} - ${subtitle || 'Explore'}`}
    >
      <div className="flex items-center min-w-0 pr-3">
        <span className="text-xs sm:text-sm font-normal text-[#191919]/40 tabular-nums">
          {number}
        </span>
        <span className="mx-2 text-xs sm:text-sm text-[#191919]/30 select-none">
          /
        </span>
        <span className="text-xs sm:text-sm font-medium text-[#191919] truncate">
          {label}
        </span>
      </div>

      <ArrowRight
        className="w-4 h-4 text-gray-400 group-hover:text-[#191919]/80 group-hover:translate-x-0.5 transition-all duration-200 shrink-0"
        aria-hidden="true"
      />
    </button>
  );
};

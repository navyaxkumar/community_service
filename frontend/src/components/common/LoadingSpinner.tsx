import React from 'react';

interface LoadingSpinnerProps {
  label?: string;
  size?: 'sm' | 'md' | 'lg';
}

export const LoadingSpinner: React.FC<LoadingSpinnerProps> = ({
  label = 'Loading...',
  size = 'md',
}) => {
  const sizeClasses = {
    sm: 'w-4 h-4 border-2',
    md: 'w-7 h-7 border-2',
    lg: 'w-10 h-10 border-3',
  }[size];

  return (
    <div
      className="flex flex-col items-center justify-center p-8 space-y-3"
      role="status"
      aria-live="polite"
    >
      <div
        className={`${sizeClasses} border-[#191919]/20 border-t-[#191919] rounded-full animate-spin`}
      />
      <span className="text-xs font-medium text-[#191919]/60">{label}</span>
    </div>
  );
};

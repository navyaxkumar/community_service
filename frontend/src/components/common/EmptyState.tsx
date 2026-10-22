import React from 'react';

interface EmptyStateProps {
  title: string;
  description: string;
  actionText?: string;
  onAction?: () => void;
}

export const EmptyState: React.FC<EmptyStateProps> = ({
  title,
  description,
  actionText,
  onAction,
}) => {
  return (
    <div className="text-center py-12 px-4 border border-dashed border-gray-200 rounded-2xl bg-[#F4F3F3]/40">
      <div className="w-12 h-12 mx-auto mb-3 text-gray-400 flex items-center justify-center rounded-full bg-gray-100">
        <svg
          className="w-6 h-6 text-gray-500"
          fill="none"
          stroke="currentColor"
          viewBox="0 0 24 24"
        >
          <path
            strokeLinecap="round"
            strokeLinejoin="round"
            strokeWidth="1.5"
            d="M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0v5a2 2 0 01-2 2H6a2 2 0 01-2-2v-5m16 0h-2.586a1 1 0 00-.707.293l-2.414 2.414a1 1 0 01-.707.293h-3.172a1 1 0 01-.707-.293l-2.414-2.414A1 1 0 006.586 13H4"
          />
        </svg>
      </div>
      <h3 className="text-base font-medium text-[#191919]">{title}</h3>
      <p className="text-xs text-[#191919]/60 max-w-sm mx-auto mt-1 leading-relaxed">
        {description}
      </p>
      {actionText && onAction && (
        <button
          type="button"
          onClick={onAction}
          className="mt-4 bg-[#191919] text-white text-xs font-medium px-4 py-2 rounded-full hover:bg-black/85 transition-colors"
        >
          {actionText}
        </button>
      )}
    </div>
  );
};

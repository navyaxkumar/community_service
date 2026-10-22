import React from 'react';

interface ErrorMessageProps {
  title?: string;
  message: string;
  onRetry?: () => void;
}

export const ErrorMessage: React.FC<ErrorMessageProps> = ({
  title = 'Something went wrong',
  message,
  onRetry,
}) => {
  return (
    <div
      className="p-5 border border-red-200 bg-red-50/60 rounded-xl my-4 flex items-start justify-between"
      role="alert"
    >
      <div className="flex items-start space-x-3">
        <svg
          className="w-5 h-5 text-red-600 mt-0.5 shrink-0"
          fill="none"
          stroke="currentColor"
          viewBox="0 0 24 24"
        >
          <path
            strokeLinecap="round"
            strokeLinejoin="round"
            strokeWidth="2"
            d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"
          />
        </svg>
        <div>
          <h4 className="text-sm font-semibold text-red-900">{title}</h4>
          <p className="text-xs text-red-700 mt-1 leading-relaxed">{message}</p>
        </div>
      </div>
      {onRetry && (
        <button
          type="button"
          onClick={onRetry}
          className="text-xs font-medium text-red-800 bg-red-100 hover:bg-red-200 px-3 py-1.5 rounded-md transition-colors shrink-0 ml-4"
        >
          Retry
        </button>
      )}
    </div>
  );
};

import React, { useEffect, useState } from 'react';
import { useParams, useLocation, Link } from 'react-router-dom';
import { ArrowRight } from 'lucide-react';
import { quizService } from '../services/quizService';
import { QuizAttempt } from '../types/quiz';
import { LoadingSpinner } from '../components/common/LoadingSpinner';

export const QuizResultsPage: React.FC = () => {
  const { quizId } = useParams<{ quizId: string }>();
  const id = parseInt(quizId || '0', 10);
  const location = useLocation();

  const stateAttempt = (location.state as { attempt?: QuizAttempt; quizTitle?: string })?.attempt;
  const stateTitle = (location.state as { attempt?: QuizAttempt; quizTitle?: string })?.quizTitle;

  const [attempt, setAttempt] = useState<QuizAttempt | null>(stateAttempt || null);
  const [isLoading, setIsLoading] = useState<boolean>(!stateAttempt);

  useEffect(() => {
    if (!stateAttempt && id > 0) {
      // Fetch latest attempt for this user and quiz
      quizService
        .getMyAttempts()
        .then((attempts) => {
          const matched = attempts.filter((a) => a.quizId === id).sort((a, b) => b.id - a.id);
          if (matched.length > 0) {
            setAttempt(matched[0]);
          }
        })
        .catch(() => {})
        .finally(() => setIsLoading(false));
    }
  }, [id, stateAttempt]);

  if (isLoading) {
    return (
      <div className="pt-32 pb-24 max-w-2xl mx-auto px-6">
        <LoadingSpinner label="Compiling verified assessment results..." />
      </div>
    );
  }

  const score = attempt?.score ?? 0;
  const total = attempt?.totalQuestions ?? 3;
  const percentage = Math.round((score / Math.max(total, 1)) * 100);
  const isPassed = percentage >= 66;

  return (
    <div className="pt-28 pb-24 max-w-2xl mx-auto px-6 sm:px-10">
      <div className="bg-white border border-gray-200 rounded-3xl p-8 sm:p-10 shadow-sm text-center space-y-6">
        {/* Badge Icon */}
        <div
          className={`w-16 h-16 mx-auto rounded-full flex items-center justify-center ${
            isPassed ? 'bg-emerald-100 text-emerald-700' : 'bg-amber-100 text-amber-700'
          }`}
        >
          {isPassed ? (
            <svg className="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2.5" d="M5 13l4 4L19 7" />
            </svg>
          ) : (
            <svg className="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2.5" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
            </svg>
          )}
        </div>

        <div>
          <span className="text-xs uppercase tracking-widest text-[#191919]/50 font-semibold block mb-1">
            Official Evaluation Completed
          </span>
          <h1 className="text-3xl font-serif text-[#191919]">
            {isPassed ? 'Strong Detection Instincts!' : 'Review Recommended'}
          </h1>
          <p className="text-xs text-[#191919]/60 mt-1">
            {stateTitle || `Quiz Assessment #${id}`}
          </p>
        </div>

        {/* Score Ring / Block */}
        <div className="py-6 border-y border-gray-100 flex items-center justify-center space-x-8">
          <div>
            <div className="text-4xl font-serif font-bold text-[#191919]">{score} / {total}</div>
            <div className="text-[11px] uppercase tracking-wider text-[#191919]/50 mt-1">Correct Answers</div>
          </div>
          <div className="h-10 w-px bg-gray-200" />
          <div>
            <div className={`text-4xl font-serif font-bold ${isPassed ? 'text-emerald-700' : 'text-amber-700'}`}>
              {percentage}%
            </div>
            <div className="text-[11px] uppercase tracking-wider text-[#191919]/50 mt-1">Overall Accuracy</div>
          </div>
        </div>

        <p className="text-xs sm:text-sm text-[#191919]/70 leading-relaxed max-w-md mx-auto">
          {isPassed
            ? 'You demonstrated reliable awareness of common pressure tactics, suspicious domain structures, and payment redirects.'
            : 'Some subtle deception triggers were missed. We recommend revisiting the learning modules to review how fraud schemes craft fake urgency.'}
        </p>

        {/* Recommendation Cards */}
        <div className="pt-2 text-left bg-[#F4F3F3] p-5 rounded-2xl border border-gray-100 text-xs space-y-2">
          <strong className="block text-[#191919] font-semibold uppercase tracking-wider text-[11px]">
            Recommended Next Steps:
          </strong>
          <ul className="text-[#191919]/70 space-y-1.5 list-disc pl-5">
            <li>Re-read lesson guidelines on OTP security and UPI payment redirects.</li>
            <li>Use the Scam Checker whenever you receive unexpected SMS or emails.</li>
            <li>Track all completed modules and badges on your personal Dashboard.</li>
          </ul>
        </div>

        {/* Actions */}
        <div className="pt-4 flex flex-col sm:flex-row items-center justify-center gap-3">
          <Link
            to={`/quizzes/${id}`}
            className="w-full sm:w-auto px-6 py-2.5 rounded-full border border-gray-300 text-xs font-medium text-[#191919] hover:bg-gray-50 transition-colors"
          >
            Retake Quiz
          </Link>
          <Link
            to="/dashboard"
            className="w-full sm:w-auto bg-[#191919] text-white px-7 py-2.5 rounded-full text-xs font-medium hover:bg-black/85 transition-colors shadow-sm inline-flex items-center justify-center gap-1.5"
          >
            Go to Dashboard <ArrowRight className="w-3.5 h-3.5" />
          </Link>
          <Link
            to="/learn"
            className="w-full sm:w-auto px-6 py-2.5 text-xs font-medium text-[#191919]/70 hover:text-[#191919] transition-colors"
          >
            Browse Modules
          </Link>
        </div>
      </div>
    </div>
  );
};

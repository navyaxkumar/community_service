import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { ArrowRight, CheckCircle2 } from 'lucide-react';
import { useAuth } from '../context/AuthContext';
import { useAudience } from '../context/AudienceContext';
import { quizService } from '../services/quizService';
import { fraudService } from '../services/fraudService';
import { Quiz, QuizAttempt } from '../types/quiz';
import { FraudCategory } from '../types/fraud';
import { getAudienceMeta } from '../types/audience';
import { LoadingSpinner } from '../components/common/LoadingSpinner';
import { ErrorMessage } from '../components/common/ErrorMessage';
import { EmptyState } from '../components/common/EmptyState';
import { AudienceSelector } from '../components/common/AudienceSelector';

// Fallback preview quizzes for unauthenticated public preview
const PUBLIC_PREVIEW_QUIZZES: Quiz[] = [
  {
    id: 1,
    title: 'Phishing Detection & Suspicious Link Assessment',
    description: 'Test your ability to spot deceptive URLs, spoofed corporate branding, and credential harvesting traps.',
    isPublished: true,
    fraudCategoryId: 1,
    targetAudience: 'All',
    difficultyLevel: 'Beginner',
    createdAt: new Date().toISOString(),
  },
  {
    id: 2,
    title: 'Social Engineering & Emergency Authority Scams',
    description: 'Evaluate scenario interactions with fake law enforcement, banking officers, and urgent tech support claims.',
    isPublished: true,
    fraudCategoryId: 2,
    targetAudience: 'Seniors',
    difficultyLevel: 'Beginner',
    createdAt: new Date().toISOString(),
  },
  {
    id: 3,
    title: 'Digital Payments & UPI Redirection Defense',
    description: 'Test your understanding of instant settlement mechanisms, deceptive QR codes, and refund bypasses.',
    isPublished: true,
    fraudCategoryId: 3,
    targetAudience: 'YoungAdults',
    difficultyLevel: 'Intermediate',
    createdAt: new Date().toISOString(),
  },
];

export const QuizzesPage: React.FC = () => {
  const { isAuthenticated } = useAuth();
  const { audience, isHighContrast, fontSize } = useAudience();

  const [quizzes, setQuizzes] = useState<Quiz[]>([]);
  const [categories, setCategories] = useState<FraudCategory[]>([]);
  const [userAttempts, setUserAttempts] = useState<QuizAttempt[]>([]);

  const [selectedCategoryId, setSelectedCategoryId] = useState<number | 'all'>('all');
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchQuizzes = async () => {
    setIsLoading(true);
    setError(null);

    try {
      if (isAuthenticated) {
        const [fetchedQuizzes, fetchedCategories, fetchedAttempts] = await Promise.all([
          quizService.getPublishedQuizzes(audience).catch(() => PUBLIC_PREVIEW_QUIZZES),
          fraudService.getCategories().catch(() => []),
          quizService.getMyAttempts().catch(() => []),
        ]);

        setQuizzes(fetchedQuizzes.length > 0 ? fetchedQuizzes : PUBLIC_PREVIEW_QUIZZES);
        setCategories(fetchedCategories);
        setUserAttempts(fetchedAttempts);
      } else {
        const filteredPreview =
          audience === 'All'
            ? PUBLIC_PREVIEW_QUIZZES
            : PUBLIC_PREVIEW_QUIZZES.filter(
                (q) => q.targetAudience === audience || q.targetAudience === 'All'
              );
        setQuizzes(filteredPreview.length > 0 ? filteredPreview : PUBLIC_PREVIEW_QUIZZES);
      }
    } catch (err: any) {
      setError(err.message || 'Failed to load quizzes catalogue.');
      setQuizzes(PUBLIC_PREVIEW_QUIZZES);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    fetchQuizzes();
  }, [isAuthenticated, audience]);

  const filteredQuizzes = quizzes.filter((quiz) => {
    return selectedCategoryId === 'all' || quiz.fraudCategoryId === selectedCategoryId;
  });

  const fontClass =
    fontSize === 'extra-large'
      ? 'text-lg sm:text-xl'
      : fontSize === 'large'
      ? 'text-base sm:text-lg'
      : 'text-sm';

  return (
    <div className={`pt-28 pb-24 max-w-5xl mx-auto px-6 sm:px-10 ${isHighContrast ? 'bg-white' : ''}`}>
      {/* Header */}
      <div className="max-w-3xl mb-8">
        <span className="text-xs uppercase tracking-widest text-[#191919]/60 font-semibold block mb-2">
          Interactive Assessment · All Ages
        </span>
        <h1 className="text-3xl sm:text-4xl font-serif text-[#191919]">
          Knowledge & Scenario Quizzes
        </h1>
        <p className={`text-[#191919]/75 mt-2 leading-relaxed ${fontClass}`}>
          Test your detection instincts through realistic fraud simulations. Submissions are authoritatively evaluated by the backend engine to build real-world digital safety muscle memory.
        </p>
      </div>

      {/* Audience Selector & Persona Toolbar */}
      <AudienceSelector className="mb-8" />

      {!isAuthenticated && (
        <div className="mb-8 p-4 bg-[#F4F3F3] border border-gray-200 rounded-xl flex items-center justify-between text-xs text-[#191919]/75">
          <span>
            <strong>Note:</strong> Browsing quiz catalog in public preview.{' '}
            <Link to="/login" className="underline font-semibold text-[#191919]">
              Sign In
            </Link>{' '}
            to submit verified quiz attempts, earn badges, and track your detection accuracy.
          </span>
          <Link
            to="/login"
            className="px-3 py-1.5 bg-[#191919] text-white rounded-full text-xs font-medium hover:bg-black/85 transition-colors shrink-0 ml-4"
          >
            Sign In
          </Link>
        </div>
      )}

      {/* Category Filter */}
      {categories.length > 0 && (
        <div className="flex items-center space-x-2 overflow-x-auto pb-4 mb-6">
          <button
            type="button"
            onClick={() => setSelectedCategoryId('all')}
            className={`text-xs px-3 py-1.5 rounded-full transition-colors whitespace-nowrap ${
              selectedCategoryId === 'all'
                ? 'bg-[#191919] text-white'
                : 'bg-gray-100 text-[#191919]/70 hover:bg-gray-200'
            }`}
          >
            All Categories
          </button>
          {categories.map((cat) => (
            <button
              key={cat.id}
              type="button"
              onClick={() => setSelectedCategoryId(cat.id)}
              className={`text-xs px-3 py-1.5 rounded-full transition-colors whitespace-nowrap ${
                selectedCategoryId === cat.id
                  ? 'bg-[#191919] text-white'
                  : 'bg-gray-100 text-[#191919]/70 hover:bg-gray-200'
              }`}
            >
              {cat.name}
            </button>
          ))}
        </div>
      )}

      {/* Loading State */}
      {isLoading && <LoadingSpinner label="Loading quiz assessments catalog..." />}

      {/* Error State */}
      {error && !isLoading && <ErrorMessage message={error} onRetry={fetchQuizzes} />}

      {/* Empty State */}
      {!isLoading && filteredQuizzes.length === 0 && (
        <EmptyState
          title="No quizzes match your filter"
          description="Try choosing 'All Audiences' or selecting another category to see more assessments."
          actionText="Clear Filter"
          onAction={() => setSelectedCategoryId('all')}
        />
      )}

      {/* Quizzes Grid */}
      {!isLoading && filteredQuizzes.length > 0 && (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {filteredQuizzes.map((quiz) => {
            const attempts = userAttempts.filter((a) => a.quizId === quiz.id);
            const latestAttempt = attempts.length > 0 ? attempts[attempts.length - 1] : null;
            const meta = getAudienceMeta(quiz.targetAudience);

            return (
              <div
                key={quiz.id}
                className={`rounded-2xl p-6 transition-all duration-200 flex flex-col justify-between ${
                  isHighContrast
                    ? 'bg-white border-2 border-black shadow-sm'
                    : 'bg-white border border-gray-200 hover:border-gray-400 shadow-sm'
                }`}
              >
                <div className="space-y-3">
                  <div className="flex items-center justify-between gap-1 flex-wrap">
                    <span
                      className={`text-[10px] uppercase font-semibold px-2 py-0.5 rounded-full border ${meta.themeColor}`}
                    >
                      {meta.badge}
                    </span>
                    {quiz.difficultyLevel && (
                      <span className="text-[10px] uppercase font-medium px-1.5 py-0.5 rounded bg-gray-100 text-gray-700">
                        {quiz.difficultyLevel}
                      </span>
                    )}
                  </div>

                  <h3 className="text-lg font-serif text-[#191919] leading-snug">
                    {quiz.title}
                  </h3>

                  <p className={`text-[#191919]/70 leading-relaxed line-clamp-3 ${fontClass}`}>
                    {quiz.description || 'Practice your threat discernment through multiple-choice simulations.'}
                  </p>
                </div>

                <div className="pt-6 mt-6 border-t border-gray-100 flex items-center justify-between">
                  <div>
                    {latestAttempt ? (
                      <span className="text-[11px] font-mono text-emerald-700 flex items-center gap-1 font-semibold">
                        <CheckCircle2 className="w-3.5 h-3.5" />
                        Score: {latestAttempt.score} / {latestAttempt.totalQuestions}
                      </span>
                    ) : (
                      <span className="text-[11px] font-mono text-[#191919]/50">
                        Unattempted
                      </span>
                    )}
                  </div>

                  <Link
                    to={`/quizzes/${quiz.id}`}
                    className={`inline-flex items-center gap-1.5 font-semibold text-[#191919] hover:underline ${
                      audience === 'EarlyLearners'
                        ? 'px-3 py-1.5 bg-emerald-600 text-white rounded-lg text-xs hover:bg-emerald-700 shadow-sm no-underline'
                        : isHighContrast
                        ? 'px-3 py-1.5 bg-black text-white rounded-lg text-xs hover:bg-gray-800'
                        : 'text-xs'
                    }`}
                  >
                    <span>{latestAttempt ? 'Retake Quiz' : 'Start Quiz'}</span>
                    <ArrowRight className="w-3.5 h-3.5" />
                  </Link>
                </div>
              </div>
            );
          })}
        </div>
      )}
    </div>
  );
};

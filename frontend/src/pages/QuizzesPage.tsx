import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { ArrowRight } from 'lucide-react';
import { useAuth } from '../context/AuthContext';
import { quizService } from '../services/quizService';
import { fraudService } from '../services/fraudService';
import { Quiz, QuizAttempt } from '../types/quiz';
import { FraudCategory } from '../types/fraud';
import { LoadingSpinner } from '../components/common/LoadingSpinner';
import { ErrorMessage } from '../components/common/ErrorMessage';
import { EmptyState } from '../components/common/EmptyState';

// Fallback preview quizzes for unauthenticated public preview
const PUBLIC_PREVIEW_QUIZZES: Quiz[] = [
  {
    id: 1,
    title: 'Phishing Detection & Suspicious Link Assessment',
    description: 'Test your ability to spot deceptive URLs, spoofed corporate branding, and credential harvesting traps.',
    isPublished: true,
    fraudCategoryId: 1,
    createdAt: new Date().toISOString(),
  },
  {
    id: 2,
    title: 'Social Engineering & Emergency Authority Scams',
    description: 'Evaluate scenario interactions with fake law enforcement, banking officers, and urgent tech support claims.',
    isPublished: true,
    fraudCategoryId: 2,
    createdAt: new Date().toISOString(),
  },
  {
    id: 3,
    title: 'Digital Payments & UPI Redirection Defense',
    description: 'Test your understanding of instant settlement mechanisms, deceptive QR codes, and refund bypasses.',
    isPublished: true,
    fraudCategoryId: 3,
    createdAt: new Date().toISOString(),
  },
];

export const QuizzesPage: React.FC = () => {
  const { isAuthenticated } = useAuth();

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
          quizService.getPublishedQuizzes().catch(() => PUBLIC_PREVIEW_QUIZZES),
          fraudService.getCategories().catch(() => []),
          quizService.getMyAttempts().catch(() => []),
        ]);

        setQuizzes(fetchedQuizzes.length > 0 ? fetchedQuizzes : PUBLIC_PREVIEW_QUIZZES);
        setCategories(fetchedCategories);
        setUserAttempts(fetchedAttempts);
      } else {
        setQuizzes(PUBLIC_PREVIEW_QUIZZES);
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
  }, [isAuthenticated]);

  const filteredQuizzes = quizzes.filter((quiz) => {
    return selectedCategoryId === 'all' || quiz.fraudCategoryId === selectedCategoryId;
  });

  return (
    <div className="pt-28 pb-24 max-w-5xl mx-auto px-6 sm:px-10">
      {/* Header */}
      <div className="max-w-2xl mb-12">
        <span className="text-xs uppercase tracking-widest text-[#191919]/50 font-semibold block mb-2">
          Interactive Assessment
        </span>
        <h1 className="text-3xl sm:text-4xl font-serif text-[#191919]">
          Knowledge & Scenario Quizzes
        </h1>
        <p className="text-sm text-[#191919]/70 mt-2 leading-relaxed">
          Test your detection instincts through realistic fraud simulations. Submissions are authoritatively evaluated by the backend engine to build real-world digital safety muscle memory.
        </p>
      </div>

      {!isAuthenticated && (
        <div className="mb-8 p-4 bg-[#F4F3F3] border border-gray-200 rounded-xl flex items-center justify-between text-xs text-[#191919]/75">
          <span>
            <strong>Note:</strong> Browsing quiz catalog. <Link to="/login" className="underline font-semibold text-[#191919]">Sign In</Link> to take quizzes, submit answers for scoring, and record achievements.
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
            className={`text-xs px-3.5 py-1.5 rounded-full transition-colors whitespace-nowrap ${
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
              className={`text-xs px-3.5 py-1.5 rounded-full transition-colors whitespace-nowrap ${
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

      {/* Loading */}
      {isLoading && (
        <LoadingSpinner label="Loading published quizzes..." />
      )}

      {/* Error */}
      {error && !isLoading && (
        <ErrorMessage message={error} onRetry={fetchQuizzes} />
      )}

      {/* Empty */}
      {!isLoading && filteredQuizzes.length === 0 && (
        <EmptyState
          title="No quizzes available"
          description="There are currently no published quizzes matching this category filter."
          actionText="View All Quizzes"
          onAction={() => setSelectedCategoryId('all')}
        />
      )}

      {/* Quizzes List */}
      {!isLoading && filteredQuizzes.length > 0 && (
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          {filteredQuizzes.map((quiz) => {
            const attempts = userAttempts.filter((a) => a.quizId === quiz.id);
            const bestScore = attempts.length > 0 ? Math.max(...attempts.map((a) => a.score)) : null;

            return (
              <div
                key={quiz.id}
                className="bg-white border border-gray-200 hover:border-gray-400 rounded-2xl p-6 sm:p-7 shadow-sm transition-all duration-200 flex flex-col justify-between"
              >
                <div className="space-y-3">
                  <div className="flex items-center justify-between">
                    <span className="text-[11px] font-mono uppercase text-[#191919]/50">
                      Quiz #{quiz.id}
                    </span>
                    {isAuthenticated && bestScore !== null && (
                      <span className="text-[10px] uppercase font-semibold px-2 py-0.5 rounded-full bg-emerald-100 text-emerald-800">
                        Best Score: {bestScore} pts
                      </span>
                    )}
                  </div>

                  <h3 className="text-xl font-serif text-[#191919] leading-snug">
                    {quiz.title}
                  </h3>

                  <p className="text-xs text-[#191919]/70 leading-relaxed">
                    {quiz.description || 'Practice identifying real-world deception signals in this guided scenario check.'}
                  </p>
                </div>

                <div className="pt-6 mt-6 border-t border-gray-100 flex items-center justify-between">
                  <span className="text-[11px] text-[#191919]/50">
                    {attempts.length > 0 ? `${attempts.length} previous attempt(s)` : 'Multiple-choice scenarios'}
                  </span>
                  <Link
                    to={`/quizzes/${quiz.id}`}
                    className="inline-flex items-center gap-1.5 text-xs font-semibold text-[#191919] hover:underline"
                  >
                    {attempts.length > 0 ? 'Retake Quiz' : 'Start Quiz'} <ArrowRight className="w-3.5 h-3.5" />
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

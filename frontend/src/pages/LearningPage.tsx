import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { ArrowRight } from 'lucide-react';
import { useAuth } from '../context/AuthContext';
import { learningService } from '../services/learningService';
import { fraudService } from '../services/fraudService';
import { progressService } from '../services/progressService';
import { LearningModule } from '../types/learning';
import { FraudCategory } from '../types/fraud';
import { UserProgress } from '../types/progress';
import { LoadingSpinner } from '../components/common/LoadingSpinner';
import { ErrorMessage } from '../components/common/ErrorMessage';
import { EmptyState } from '../components/common/EmptyState';

// Fallback seed curriculum overview for unauthenticated public preview
const PUBLIC_PREVIEW_MODULES: LearningModule[] = [
  {
    id: 1,
    title: 'Recognizing Email Phishing & Spoofed Domains',
    description: 'Learn how cybercriminals duplicate corporate branding and construct deceptive sender addresses to trick you into entering credentials.',
    content: 'Phishing remains the most prevalent entry point for account takeover...',
    order: 1,
    isPublished: true,
    fraudCategoryId: 1,
    createdAt: new Date().toISOString(),
  },
  {
    id: 2,
    title: 'OTP Scams & Remote Access Fraud',
    description: 'Understand why legitimate service representatives never ask you to install remote support apps or read back SMS verification codes.',
    content: 'One-time passcodes represent the second factor protecting your accounts...',
    order: 2,
    isPublished: true,
    fraudCategoryId: 2,
    createdAt: new Date().toISOString(),
  },
  {
    id: 3,
    title: 'Digital Payment QR Code & Wire Diversion',
    description: 'How to avoid deceptive merchant QR codes that execute payments instead of receiving refunds, and protect peer-to-peer transfers.',
    content: 'UPI and digital wallets enable instant settlements without intermediate recall...',
    order: 3,
    isPublished: true,
    fraudCategoryId: 3,
    createdAt: new Date().toISOString(),
  },
  {
    id: 4,
    title: 'Fake Job Offers & Task Advance Traps',
    description: 'Deconstruct recruitment scams that promise lucrative part-time pay for rating videos or completing advance deposit tasks.',
    content: 'Employment fraud preys upon career transitions and flexible work aspirations...',
    order: 4,
    isPublished: true,
    fraudCategoryId: 4,
    createdAt: new Date().toISOString(),
  },
];

export const LearningPage: React.FC = () => {
  const { isAuthenticated } = useAuth();

  const [modules, setModules] = useState<LearningModule[]>([]);
  const [categories, setCategories] = useState<FraudCategory[]>([]);
  const [userProgress, setUserProgress] = useState<Map<number, UserProgress>>(new Map());

  const [searchQuery, setSearchQuery] = useState('');
  const [selectedCategoryId, setSelectedCategoryId] = useState<number | 'all'>('all');

  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchData = async () => {
    setIsLoading(true);
    setError(null);

    try {
      if (isAuthenticated) {
        const [fetchedModules, fetchedCategories, fetchedProgress] = await Promise.all([
          learningService.getPublishedModules().catch(() => PUBLIC_PREVIEW_MODULES),
          fraudService.getCategories().catch(() => []),
          progressService.getMyProgress().catch(() => []),
        ]);

        setModules(fetchedModules.length > 0 ? fetchedModules : PUBLIC_PREVIEW_MODULES);
        setCategories(fetchedCategories);

        const progressMap = new Map<number, UserProgress>();
        fetchedProgress.forEach((p) => progressMap.set(p.learningModuleId, p));
        setUserProgress(progressMap);
      } else {
        // Public preview mode
        setModules(PUBLIC_PREVIEW_MODULES);
      }
    } catch (err: any) {
      setError(err.message || 'Failed to load learning modules');
      setModules(PUBLIC_PREVIEW_MODULES);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, [isAuthenticated]);

  // Filter modules
  const filteredModules = modules.filter((mod) => {
    const matchesSearch =
      mod.title.toLowerCase().includes(searchQuery.toLowerCase()) ||
      (mod.description && mod.description.toLowerCase().includes(searchQuery.toLowerCase()));

    const matchesCategory =
      selectedCategoryId === 'all' || mod.fraudCategoryId === selectedCategoryId;

    return matchesSearch && matchesCategory;
  });

  return (
    <div className="pt-28 pb-24 max-w-5xl mx-auto px-6 sm:px-10">
      {/* Header */}
      <div className="max-w-2xl mb-12">
        <span className="text-xs uppercase tracking-widest text-[#191919]/50 font-semibold block mb-2">
          Structured Curriculum
        </span>
        <h1 className="text-3xl sm:text-4xl font-serif text-[#191919]">
          Digital Fraud Learning Library
        </h1>
        <p className="text-sm text-[#191919]/70 mt-2 leading-relaxed">
          Explore comprehensive guides to identify deceptive tactics, master verification workflows, and safeguard personal and financial data.
        </p>
      </div>

      {!isAuthenticated && (
        <div className="mb-8 p-4 bg-[#F4F3F3] border border-gray-200 rounded-xl flex items-center justify-between text-xs text-[#191919]/75">
          <span>
            <strong>Note:</strong> Browsing curriculum overview. <Link to="/login" className="underline font-semibold text-[#191919]">Sign In</Link> to save your completion progress and take associated module quizzes.
          </span>
          <Link
            to="/login"
            className="px-3 py-1.5 bg-[#191919] text-white rounded-full text-xs font-medium hover:bg-black/85 transition-colors shrink-0 ml-4"
          >
            Sign In
          </Link>
        </div>
      )}

      {/* Filter and Search Bar */}
      <div className="flex flex-col sm:flex-row items-stretch sm:items-center justify-between gap-4 mb-8">
        {/* Search */}
        <div className="relative flex-grow max-w-md">
          <input
            type="text"
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            placeholder="Search modules by keyword..."
            className="w-full pl-9 pr-4 py-2 text-xs sm:text-sm border border-gray-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-[#191919]"
          />
          <svg
            className="w-4 h-4 text-gray-400 absolute left-3 top-2.5"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
          >
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
          </svg>
        </div>

        {/* Category Pills */}
        {categories.length > 0 && (
          <div className="flex items-center space-x-2 overflow-x-auto pb-1 sm:pb-0">
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
      </div>

      {/* Loading State */}
      {isLoading && (
        <LoadingSpinner label="Loading learning modules catalog..." />
      )}

      {/* Error State */}
      {error && !isLoading && (
        <ErrorMessage message={error} onRetry={fetchData} />
      )}

      {/* Empty State */}
      {!isLoading && filteredModules.length === 0 && (
        <EmptyState
          title="No modules match your search"
          description="Try clearing your search query or choosing a different fraud category filter."
          actionText="Clear Search"
          onAction={() => {
            setSearchQuery('');
            setSelectedCategoryId('all');
          }}
        />
      )}

      {/* Modules Grid */}
      {!isLoading && filteredModules.length > 0 && (
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          {filteredModules.map((mod) => {
            const prog = userProgress.get(mod.id);
            const isCompleted = prog?.isCompleted ?? false;

            return (
              <div
                key={mod.id}
                className="bg-white border border-gray-200 hover:border-gray-400 rounded-2xl p-6 sm:p-7 shadow-sm transition-all duration-200 flex flex-col justify-between"
              >
                <div className="space-y-3">
                  <div className="flex items-center justify-between">
                    <span className="text-[11px] font-mono uppercase text-[#191919]/50">
                      Module #{mod.order || mod.id}
                    </span>
                    {isAuthenticated && (
                      <span
                        className={`text-[10px] uppercase font-semibold px-2 py-0.5 rounded-full ${
                          isCompleted
                            ? 'bg-emerald-100 text-emerald-800'
                            : prog
                            ? 'bg-amber-100 text-amber-800'
                            : 'bg-gray-100 text-gray-600'
                        }`}
                      >
                        {isCompleted ? 'Completed' : prog ? 'In Progress' : 'Not Started'}
                      </span>
                    )}
                  </div>

                  <h3 className="text-xl font-serif text-[#191919] leading-snug">
                    {mod.title}
                  </h3>

                  <p className="text-xs text-[#191919]/70 leading-relaxed line-clamp-3">
                    {mod.description || mod.content.substring(0, 140) + '...'}
                  </p>
                </div>

                <div className="pt-6 mt-6 border-t border-gray-100 flex items-center justify-between">
                  <span className="text-[11px] text-[#191919]/50">
                    Est. 5 min read
                  </span>
                  <Link
                    to={`/learn/${mod.id}`}
                    className="inline-flex items-center gap-1.5 text-xs font-semibold text-[#191919] hover:underline"
                  >
                    Start Reading <ArrowRight className="w-3.5 h-3.5" />
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

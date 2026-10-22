import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { ArrowRight } from 'lucide-react';
import { useAuth } from '../context/AuthContext';
import { learningService } from '../services/learningService';
import { progressService } from '../services/progressService';
import { quizService } from '../services/quizService';
import { badgeService } from '../services/badgeService';
import { LearningModule } from '../types/learning';
import { UserProgress } from '../types/progress';
import { Quiz, QuizAttempt } from '../types/quiz';
import { Badge } from '../types/badge';
import { LoadingSpinner } from '../components/common/LoadingSpinner';
import { ErrorMessage } from '../components/common/ErrorMessage';
import { EmptyState } from '../components/common/EmptyState';

export const DashboardPage: React.FC = () => {
  const { user } = useAuth();

  const [modules, setModules] = useState<LearningModule[]>([]);
  const [progressList, setProgressList] = useState<UserProgress[]>([]);
  const [attempts, setAttempts] = useState<QuizAttempt[]>([]);
  const [quizzes, setQuizzes] = useState<Quiz[]>([]);
  const [badges, setBadges] = useState<Badge[]>([]);

  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const loadDashboardData = async () => {
      setIsLoading(true);
      setError(null);

      try {
        const [mods, progs, atts, qzs, bdgs] = await Promise.all([
          learningService.getPublishedModules().catch(() => []),
          progressService.getMyProgress().catch(() => []),
          quizService.getMyAttempts().catch(() => []),
          quizService.getPublishedQuizzes().catch(() => []),
          badgeService.getActiveBadges().catch(() => []),
        ]);

        setModules(mods);
        setProgressList(progs);
        setAttempts(atts);
        setQuizzes(qzs);
        setBadges(bdgs);
      } catch (err: any) {
        setError(err.message || 'Failed to aggregate user progress metrics.');
      } finally {
        setIsLoading(false);
      }
    };

    loadDashboardData();
  }, []);

  if (isLoading) {
    return (
      <div className="py-16">
        <LoadingSpinner label="Loading personal dashboard metrics..." size="lg" />
      </div>
    );
  }

  // Calculate metrics
  const completedModulesCount = progressList.filter((p) => p.isCompleted).length;
  const totalModulesCount = modules.length;
  const completionPercentage =
    totalModulesCount > 0 ? Math.round((completedModulesCount / totalModulesCount) * 100) : 0;

  // Next recommended module
  const completedModuleIds = new Set(progressList.filter((p) => p.isCompleted).map((p) => p.learningModuleId));
  const nextModule = modules.find((m) => !completedModuleIds.has(m.id));

  // Quiz metrics
  const totalAttemptsCount = attempts.length;
  const bestScore = totalAttemptsCount > 0 ? Math.max(...attempts.map((a) => a.score)) : 0;

  return (
    <div className="space-y-8">
      {/* Welcome Banner */}
      <div>
        <span className="text-xs uppercase tracking-widest text-[#191919]/50 font-semibold block mb-1">
          Personal Safety Overview
        </span>
        <h2 className="text-2xl sm:text-3xl font-serif text-[#191919]">
          Welcome back, {user?.name}
        </h2>
        <p className="text-xs sm:text-sm text-[#191919]/65 mt-1">
          Here is your current digital safety progress, scenario quiz scores, and earned badges.
        </p>
      </div>

      {error && <ErrorMessage message={error} />}

      {/* Top Stat Cards */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        {/* Card 1 */}
        <div className="p-5 bg-white border border-gray-200 rounded-2xl shadow-sm">
          <span className="text-[11px] uppercase tracking-wider text-[#191919]/50 font-medium block">
            Lessons Completed
          </span>
          <div className="mt-2 text-3xl font-serif text-[#191919]">
            {completedModulesCount} <span className="text-sm font-sans text-[#191919]/50">/ {totalModulesCount}</span>
          </div>
          <div className="mt-2 w-full h-1 bg-gray-100 rounded-full overflow-hidden">
            <div
              className="h-full bg-emerald-600 rounded-full"
              style={{ width: `${completionPercentage}%` }}
            />
          </div>
        </div>

        {/* Card 2 */}
        <div className="p-5 bg-white border border-gray-200 rounded-2xl shadow-sm">
          <span className="text-[11px] uppercase tracking-wider text-[#191919]/50 font-medium block">
            Curriculum Progress
          </span>
          <div className="mt-2 text-3xl font-serif text-[#191919]">
            {completionPercentage}%
          </div>
          <p className="text-[11px] text-[#191919]/50 mt-1">
            {totalModulesCount - completedModulesCount} module(s) remaining
          </p>
        </div>

        {/* Card 3 */}
        <div className="p-5 bg-white border border-gray-200 rounded-2xl shadow-sm">
          <span className="text-[11px] uppercase tracking-wider text-[#191919]/50 font-medium block">
            Quiz Assessments
          </span>
          <div className="mt-2 text-3xl font-serif text-[#191919]">
            {totalAttemptsCount}
          </div>
          <p className="text-[11px] text-[#191919]/50 mt-1">
            Best score recorded: {bestScore} pts
          </p>
        </div>

        {/* Card 4 */}
        <div className="p-5 bg-white border border-gray-200 rounded-2xl shadow-sm">
          <span className="text-[11px] uppercase tracking-wider text-[#191919]/50 font-medium block">
            Safety Badges
          </span>
          <div className="mt-2 text-3xl font-serif text-[#191919]">
            {completedModulesCount > 0 ? '1' : '0'} <span className="text-sm font-sans text-[#191919]/50">/ {badges.length || 1}</span>
          </div>
          <p className="text-[11px] text-[#191919]/50 mt-1">
            {completedModulesCount > 0 ? 'Awareness Starter unlocked' : 'Complete 1 lesson to unlock'}
          </p>
        </div>
      </div>

      {/* Recommended Next Action */}
      {nextModule && (
        <div className="p-6 bg-white border border-gray-200 rounded-2xl shadow-sm flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4">
          <div>
            <span className="text-[11px] uppercase tracking-widest text-[#191919]/50 font-semibold block mb-1">
              Recommended Next Lesson
            </span>
            <h3 className="text-lg font-serif text-[#191919]">
              {nextModule.title}
            </h3>
            <p className="text-xs text-[#191919]/70 mt-0.5">
              {nextModule.description || 'Continue building your fraud detection instincts.'}
            </p>
          </div>
          <Link
            to={`/learn/${nextModule.id}`}
            className="inline-flex items-center gap-1.5 bg-[#191919] text-white text-xs font-medium px-5 py-2.5 rounded-full hover:bg-black/85 transition-colors shrink-0"
          >
            Start Module <ArrowRight className="w-3.5 h-3.5" />
          </Link>
        </div>
      )}

      {/* Two Column Layout: Module Status & Quiz History */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
        {/* Module Progress List */}
        <div className="bg-white border border-gray-200 rounded-2xl p-6 shadow-sm space-y-4">
          <div className="flex items-center justify-between border-b border-gray-100 pb-4">
            <h3 className="text-base font-serif text-[#191919]">Learning Modules Status</h3>
            <Link to="/learn" className="text-xs font-semibold text-[#191919] hover:underline">
              View All
            </Link>
          </div>

          {modules.length === 0 ? (
            <EmptyState
              title="No modules found"
              description="Learning modules are being updated by administrators."
            />
          ) : (
            <div className="space-y-3">
              {modules.slice(0, 5).map((mod) => {
                const isCompleted = completedModuleIds.has(mod.id);

                return (
                  <div
                    key={mod.id}
                    className="p-3.5 bg-[#F4F3F3] rounded-xl flex items-center justify-between text-xs"
                  >
                    <div className="pr-3 truncate">
                      <span className="font-medium text-[#191919] block truncate">
                        {mod.title}
                      </span>
                      <span className="text-[11px] text-[#191919]/50">
                        Module #{mod.order || mod.id}
                      </span>
                    </div>
                    <div className="flex items-center space-x-3 shrink-0">
                      <span
                        className={`text-[10px] font-semibold px-2 py-0.5 rounded-full ${
                          isCompleted
                            ? 'bg-emerald-100 text-emerald-800'
                            : 'bg-gray-200 text-gray-700'
                        }`}
                      >
                        {isCompleted ? 'Completed' : 'Pending'}
                      </span>
                      <Link
                        to={`/learn/${mod.id}`}
                        className="text-[#191919] hover:underline font-medium"
                      >
                        &rarr;
                      </Link>
                    </div>
                  </div>
                );
              })}
            </div>
          )}
        </div>

        {/* Quiz History */}
        <div className="bg-white border border-gray-200 rounded-2xl p-6 shadow-sm space-y-4">
          <div className="flex items-center justify-between border-b border-gray-100 pb-4">
            <h3 className="text-base font-serif text-[#191919]">Recent Quiz Assessments</h3>
            <Link to="/quizzes" className="text-xs font-semibold text-[#191919] hover:underline">
              Take New Quiz
            </Link>
          </div>

          {attempts.length === 0 ? (
            <EmptyState
              title="No quiz attempts recorded"
              description="Test your instinct by taking your first interactive fraud quiz."
              actionText="Browse Quizzes"
              onAction={() => (window.location.href = '/quizzes')}
            />
          ) : (
            <div className="space-y-3">
              {attempts.slice(0, 5).map((att) => {
                const quiz = quizzes.find((q) => q.id === att.quizId);
                const pct = Math.round((att.score / Math.max(att.totalQuestions, 1)) * 100);

                return (
                  <div
                    key={att.id}
                    className="p-3.5 bg-[#F4F3F3] rounded-xl flex items-center justify-between text-xs"
                  >
                    <div>
                      <span className="font-medium text-[#191919] block">
                        {quiz?.title || `Quiz Assessment #${att.quizId}`}
                      </span>
                      <span className="text-[11px] text-[#191919]/50">
                        {new Date(att.startedAt).toLocaleDateString()} &bull; Score: {att.score}/{att.totalQuestions}
                      </span>
                    </div>
                    <div className="flex items-center space-x-2">
                      <span
                        className={`font-mono text-xs px-2 py-0.5 rounded font-semibold ${
                          pct >= 66
                            ? 'bg-emerald-100 text-emerald-800'
                            : 'bg-amber-100 text-amber-800'
                        }`}
                      >
                        {pct}%
                      </span>
                      <Link
                        to={`/quizzes/${att.quizId}/results`}
                        state={{ attempt: att, quizTitle: quiz?.title }}
                        className="text-[#191919] font-medium hover:underline ml-1"
                      >
                        View
                      </Link>
                    </div>
                  </div>
                );
              })}
            </div>
          )}
        </div>
      </div>

      {/* Badges Showcase */}
      <div className="bg-white border border-gray-200 rounded-2xl p-6 shadow-sm space-y-4">
        <h3 className="text-base font-serif text-[#191919]">Safety Recognition Badges</h3>
        <div className="grid grid-cols-1 sm:grid-cols-3 gap-4 text-xs">
          <div
            className={`p-4 rounded-xl border ${
              completedModulesCount > 0
                ? 'border-emerald-200 bg-emerald-50/50'
                : 'border-dashed border-gray-200 bg-gray-50/50 opacity-60'
            }`}
          >
            <div className="font-semibold text-[#191919] flex items-center gap-2">
              <span className="w-5 h-5 rounded-full bg-emerald-600 text-white flex items-center justify-center text-[10px]">
                &#10003;
              </span>
              <span>Awareness Starter</span>
            </div>
            <p className="text-[11px] text-[#191919]/65 mt-1 leading-relaxed">
              Awarded for completing your first digital-safety learning module.
            </p>
          </div>

          <div className="p-4 rounded-xl border border-dashed border-gray-200 bg-gray-50/50 opacity-60">
            <div className="font-semibold text-[#191919] flex items-center gap-2">
              <span className="w-5 h-5 rounded-full bg-gray-300 text-gray-700 flex items-center justify-center text-[10px]">
                2
              </span>
              <span>Phishing Spotter</span>
            </div>
            <p className="text-[11px] text-[#191919]/65 mt-1 leading-relaxed">
              Earn 80%+ on the Phishing Detection assessment.
            </p>
          </div>

          <div className="p-4 rounded-xl border border-dashed border-gray-200 bg-gray-50/50 opacity-60">
            <div className="font-semibold text-[#191919] flex items-center gap-2">
              <span className="w-5 h-5 rounded-full bg-gray-300 text-gray-700 flex items-center justify-center text-[10px]">
                3
              </span>
              <span>Shield Master</span>
            </div>
            <p className="text-[11px] text-[#191919]/65 mt-1 leading-relaxed">
              Complete all published curriculum modules and scenario checks.
            </p>
          </div>
        </div>
      </div>
    </div>
  );
};

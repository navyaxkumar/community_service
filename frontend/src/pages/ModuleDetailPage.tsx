import React, { useEffect, useState } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import { Volume2, VolumeX, Sparkles, ShieldAlert, CheckCircle2, BookOpen } from 'lucide-react';
import { useAuth } from '../context/AuthContext';
import { useAudience } from '../context/AudienceContext';
import { learningService } from '../services/learningService';
import { progressService } from '../services/progressService';
import { quizService } from '../services/quizService';
import { LearningModule } from '../types/learning';
import { UserProgress } from '../types/progress';
import { Quiz } from '../types/quiz';
import { getAudienceMeta } from '../types/audience';
import { LoadingSpinner } from '../components/common/LoadingSpinner';
import { ErrorMessage } from '../components/common/ErrorMessage';

export const ModuleDetailPage: React.FC = () => {
  const { moduleId } = useParams<{ moduleId: string }>();
  const id = parseInt(moduleId || '0', 10);
  const navigate = useNavigate();
  const { isAuthenticated } = useAuth();
  const { isHighContrast, fontSize, readAloud, isSpeaking, stopReadAloud } = useAudience();

  const [module, setModule] = useState<LearningModule | null>(null);
  const [allModules, setAllModules] = useState<LearningModule[]>([]);
  const [progress, setProgress] = useState<UserProgress | null>(null);
  const [associatedQuiz, setAssociatedQuiz] = useState<Quiz | null>(null);

  const [isLoading, setIsLoading] = useState(true);
  const [isUpdatingProgress, setIsUpdatingProgress] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchModuleData = async () => {
      setIsLoading(true);
      setError(null);

      try {
        const [mod, modulesList] = await Promise.all([
          learningService.getModuleById(id).catch(() => null),
          learningService.getPublishedModules().catch(() => []),
        ]);

        if (!mod) {
          setError(`Learning module #${id} could not be found or has not been published.`);
          setIsLoading(false);
          return;
        }

        setModule(mod);
        setAllModules(modulesList);

        if (isAuthenticated) {
          const [prog, quizzes] = await Promise.all([
            progressService.getModuleProgress(id).catch(() => null),
            quizService.getPublishedQuizzes().catch(() => []),
          ]);
          setProgress(prog);

          if (mod.fraudCategoryId) {
            const matchedQuiz = quizzes.find((q) => q.fraudCategoryId === mod.fraudCategoryId);
            if (matchedQuiz) setAssociatedQuiz(matchedQuiz);
          }
        }
      } catch (err: any) {
        setError(err.message || 'Failed to load module details.');
      } finally {
        setIsLoading(false);
      }
    };

    if (id > 0) {
      fetchModuleData();
    } else {
      setError('Invalid module identifier.');
      setIsLoading(false);
    }
  }, [id, isAuthenticated]);

  const handleToggleCompleted = async () => {
    if (!isAuthenticated) {
      navigate('/login');
      return;
    }

    setIsUpdatingProgress(true);
    const newCompleted = !(progress?.isCompleted ?? false);

    try {
      if (progress) {
        const updated = await progressService.updateProgress(id, {
          isCompleted: newCompleted,
          progressPercentage: newCompleted ? 100 : 50,
        });
        setProgress(updated);
      } else {
        const created = await progressService.createProgress({
          learningModuleId: id,
          isCompleted: newCompleted,
          progressPercentage: newCompleted ? 100 : 50,
        });
        setProgress(created);
      }
    } catch (err: any) {
      setError(err.message || 'Failed to update module completion state.');
    } finally {
      setIsUpdatingProgress(false);
    }
  };

  if (isLoading) {
    return (
      <div className="pt-32 pb-24 max-w-3xl mx-auto px-6">
        <LoadingSpinner label="Loading lesson content..." />
      </div>
    );
  }

  if (error || !module) {
    return (
      <div className="pt-32 pb-24 max-w-2xl mx-auto px-6">
        <ErrorMessage
          title="Module Not Found"
          message={error || 'The requested module does not exist.'}
          onRetry={() => window.location.reload()}
        />
        <div className="mt-4">
          <Link to="/learn" className="text-xs font-semibold text-[#191919] underline">
            &larr; Return to Learning Library
          </Link>
        </div>
      </div>
    );
  }

  // Calculate previous and next module navigation
  const currentIndex = allModules.findIndex((m) => m.id === id);
  const prevModule = currentIndex > 0 ? allModules[currentIndex - 1] : null;
  const nextModule = currentIndex >= 0 && currentIndex < allModules.length - 1 ? allModules[currentIndex + 1] : null;

  // Split plain text content into structured paragraphs safely without dangerouslySetInnerHTML
  const cleanBody = module.content
    .replace(/<!--.*?-->/gs, '')
    .replace(/^---[\s\S]*?---\s*/g, '')
    .trim();

  const paragraphs = cleanBody
    .split(/\n\n+/)
    .map((p) => p.trim())
    .filter(Boolean);

  const meta = getAudienceMeta(module.targetAudience);

  const fontClass =
    fontSize === 'extra-large'
      ? 'text-lg sm:text-xl'
      : fontSize === 'large'
      ? 'text-base sm:text-lg'
      : 'text-sm sm:text-base';

  return (
    <article className={`pt-28 pb-24 max-w-3xl mx-auto px-6 sm:px-10 ${isHighContrast ? 'bg-white' : ''}`}>
      {/* Back Link */}
      <div className="mb-6 flex items-center justify-between">
        <Link
          to="/learn"
          className="text-xs font-medium text-[#191919]/60 hover:text-[#191919] transition-colors"
        >
          &larr; Back to Learning Library
        </Link>

        {/* Audio Narration Toolbar */}
        <div className="flex items-center gap-2">
          {isSpeaking ? (
            <button
              type="button"
              onClick={stopReadAloud}
              className="inline-flex items-center gap-1.5 px-3 py-1.5 text-xs font-semibold bg-rose-100 text-rose-800 rounded-lg hover:bg-rose-200 transition-colors animate-pulse"
            >
              <VolumeX className="w-4 h-4" />
              <span>Stop Reading</span>
            </button>
          ) : (
            <button
              type="button"
              onClick={() => readAloud(module.title + '. ' + cleanBody)}
              className="inline-flex items-center gap-1.5 px-3 py-1.5 text-xs font-semibold bg-indigo-50 text-indigo-800 rounded-lg hover:bg-indigo-100 transition-colors"
              title="Listen to full lesson audio narration"
            >
              <Volume2 className="w-4 h-4" />
              <span>Listen Aloud</span>
            </button>
          )}
        </div>
      </div>

      {/* Module Meta Header */}
      <header className="mb-8 pb-8 border-b border-gray-100">
        <div className="flex items-center justify-between mb-3 flex-wrap gap-2">
          <div className="flex items-center gap-2">
            <span className="text-xs font-mono uppercase text-[#191919]/50">
              Lesson #{module.order || module.id}
            </span>
            <span
              className={`text-[10px] uppercase font-semibold px-2 py-0.5 rounded-full border ${meta.themeColor}`}
            >
              {meta.label} ({meta.ageRange})
            </span>
            {module.difficultyLevel && (
              <span className="text-[10px] uppercase font-medium px-2 py-0.5 rounded bg-gray-100 text-gray-700">
                {module.difficultyLevel}
              </span>
            )}
            <span className="text-[11px] text-[#191919]/50">
              · {module.estimatedMinutes || 5} min read
            </span>
          </div>

          {isAuthenticated && (
            <span
              className={`text-xs px-2.5 py-0.5 rounded-full font-medium ${
                progress?.isCompleted
                  ? 'bg-emerald-100 text-emerald-800'
                  : 'bg-gray-100 text-gray-700'
              }`}
            >
              {progress?.isCompleted ? '✓ Completed' : 'In Progress'}
            </span>
          )}
        </div>

        <h1 className="text-3xl sm:text-4xl font-serif text-[#191919] leading-tight">
          {module.title}
        </h1>

        {module.description && (
          <p className={`text-[#191919]/70 mt-3 leading-relaxed ${fontClass}`}>
            {module.description}
          </p>
        )}
      </header>

      {/* Learning Objectives Callout if present */}
      {module.learningObjectives && (
        <div className="mb-8 p-4 bg-amber-50/60 border border-amber-200/60 rounded-xl text-xs sm:text-sm text-amber-950 flex items-start gap-3">
          <Sparkles className="w-4 h-4 text-amber-600 shrink-0 mt-0.5" />
          <div>
            <strong className="font-semibold block mb-0.5">Key Learning Objective</strong>
            <p className="leading-relaxed">{module.learningObjectives}</p>
          </div>
        </div>
      )}

      {/* Safe Text Content */}
      <div className={`space-y-6 leading-relaxed font-sans ${fontClass} ${isHighContrast ? 'text-black font-medium' : 'text-[#191919]/85'}`}>
        {paragraphs.map((para, i) => (
          <p key={i} className="leading-relaxed">
            {para}
          </p>
        ))}
      </div>

      {/* Safety Summary Checklist / Tips */}
      <div
        className={`mt-12 p-6 sm:p-8 rounded-2xl space-y-4 ${
          isHighContrast
            ? 'bg-black text-white border-2 border-black'
            : 'bg-[#F4F3F3] border border-gray-200 text-[#191919]'
        }`}
      >
        <div className="flex items-center gap-2">
          <ShieldAlert className={`w-5 h-5 ${isHighContrast ? 'text-amber-400' : 'text-amber-600'}`} />
          <h3 className="text-base font-serif font-medium">
            {module.safetyTips ? 'Safety Rules for this Topic' : 'Core Digital Safety Rules'}
          </h3>
        </div>

        {module.safetyTips ? (
          <p className={`text-xs sm:text-sm leading-relaxed ${isHighContrast ? 'text-gray-200' : 'text-[#191919]/80'}`}>
            {module.safetyTips}
          </p>
        ) : (
          <ul className={`text-xs sm:text-sm space-y-2 list-disc pl-5 ${isHighContrast ? 'text-gray-200' : 'text-[#191919]/75'}`}>
            <li>Never trust unsolicited communications creating artificial panic or urgency.</li>
            <li>Always verify sender identity through official, independently researched websites.</li>
            <li>No authentic organization requires personal passcodes, OTPs, or passwords over chat or calls.</li>
          </ul>
        )}
      </div>

      {/* Progress & Quiz Action Bar */}
      <div className="mt-10 pt-6 border-t border-gray-200 flex flex-col sm:flex-row items-center justify-between gap-4">
        <div>
          {isAuthenticated ? (
            <button
              type="button"
              onClick={handleToggleCompleted}
              disabled={isUpdatingProgress}
              className={`text-xs font-medium px-5 py-2.5 rounded-full transition-colors flex items-center gap-2 ${
                progress?.isCompleted
                  ? 'bg-emerald-700 text-white hover:bg-emerald-800'
                  : 'bg-[#191919] text-white hover:bg-black/85'
              }`}
            >
              <CheckCircle2 className="w-4 h-4" />
              <span>
                {isUpdatingProgress
                  ? 'Updating...'
                  : progress?.isCompleted
                  ? 'Mark as Incomplete'
                  : 'Mark Lesson Completed'}
              </span>
            </button>
          ) : (
            <Link
              to="/login"
              className="text-xs text-[#191919]/60 hover:text-[#191919] underline"
            >
              Sign in to track progress
            </Link>
          )}
        </div>

        {associatedQuiz && (
          <Link
            to={`/quizzes/${associatedQuiz.id}`}
            className="inline-flex items-center gap-2 px-4 py-2 bg-indigo-50 border border-indigo-200 text-indigo-900 rounded-full text-xs font-semibold hover:bg-indigo-100 transition-colors"
          >
            <BookOpen className="w-3.5 h-3.5 text-indigo-600" />
            <span>Take Associated Quiz &rarr;</span>
          </Link>
        )}
      </div>

      {/* Prev / Next Navigation */}
      <div className="mt-8 grid grid-cols-2 gap-4">
        {prevModule ? (
          <Link
            to={`/learn/${prevModule.id}`}
            className="p-4 border border-gray-200 hover:border-gray-400 rounded-xl text-left transition-colors"
          >
            <span className="text-[10px] text-[#191919]/50 block mb-1">Previous Lesson</span>
            <span className="text-xs font-medium text-[#191919] line-clamp-1">&larr; {prevModule.title}</span>
          </Link>
        ) : (
          <div />
        )}
        {nextModule ? (
          <Link
            to={`/learn/${nextModule.id}`}
            className="p-4 border border-gray-200 hover:border-gray-400 rounded-xl text-right transition-colors"
          >
            <span className="text-[10px] text-[#191919]/50 block mb-1">Next Lesson</span>
            <span className="text-xs font-medium text-[#191919] line-clamp-1">{nextModule.title} &rarr;</span>
          </Link>
        ) : (
          <div />
        )}
      </div>
    </article>
  );
};

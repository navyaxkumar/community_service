import React, { useEffect, useState } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import { ArrowRight } from 'lucide-react';
import { useAuth } from '../context/AuthContext';
import { learningService } from '../services/learningService';
import { progressService } from '../services/progressService';
import { quizService } from '../services/quizService';
import { LearningModule } from '../types/learning';
import { UserProgress } from '../types/progress';
import { Quiz } from '../types/quiz';
import { LoadingSpinner } from '../components/common/LoadingSpinner';
import { ErrorMessage } from '../components/common/ErrorMessage';

export const ModuleDetailPage: React.FC = () => {
  const { moduleId } = useParams<{ moduleId: string }>();
  const id = parseInt(moduleId || '0', 10);
  const navigate = useNavigate();
  const { isAuthenticated } = useAuth();

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
  const paragraphs = module.content
    .split(/\n\n+/)
    .map((p) => p.trim())
    .filter(Boolean);

  return (
    <article className="pt-28 pb-24 max-w-3xl mx-auto px-6 sm:px-10">
      {/* Back Link */}
      <div className="mb-6">
        <Link
          to="/learn"
          className="text-xs font-medium text-[#191919]/60 hover:text-[#191919] transition-colors"
        >
          &larr; Back to Learning Library
        </Link>
      </div>

      {/* Module Meta */}
      <header className="mb-10 pb-8 border-b border-gray-100">
        <div className="flex items-center justify-between mb-3">
          <span className="text-xs font-mono uppercase text-[#191919]/50">
            Module #{module.order || module.id}
          </span>
          {isAuthenticated && (
            <span
              className={`text-xs px-2.5 py-0.5 rounded-full font-medium ${
                progress?.isCompleted
                  ? 'bg-emerald-100 text-emerald-800'
                  : 'bg-gray-100 text-gray-700'
              }`}
            >
              {progress?.isCompleted ? 'Completed' : 'In Progress'}
            </span>
          )}
        </div>

        <h1 className="text-3xl sm:text-4xl font-serif text-[#191919] leading-tight">
          {module.title}
        </h1>

        {module.description && (
          <p className="text-sm md:text-base text-[#191919]/70 mt-3 leading-relaxed">
            {module.description}
          </p>
        )}
      </header>

      {/* Safe Text Content */}
      <div className="space-y-5 text-sm md:text-base text-[#191919]/80 leading-relaxed font-sans">
        {paragraphs.map((para, i) => (
          <p key={i}>{para}</p>
        ))}
      </div>

      {/* Safety Summary Checklist */}
      <div className="mt-12 p-6 sm:p-8 bg-[#F4F3F3] border border-gray-200 rounded-2xl space-y-4">
        <h3 className="text-base font-serif font-medium text-[#191919]">
          Key Takeaways & Safe Habits
        </h3>
        <ul className="text-xs sm:text-sm text-[#191919]/70 space-y-2 list-disc pl-5">
          <li>Never trust unsolicited communications creating artificial panic or urgency.</li>
          <li>Always verify sender identity through official, independently researched websites.</li>
          <li>No authentic financial institution requires personal passcodes, OTPs, or passwords to resolve an issue.</li>
        </ul>
      </div>

      {/* Progress & Quiz Action Bar */}
      <div className="mt-10 pt-6 border-t border-gray-200 flex flex-col sm:flex-row items-center justify-between gap-4">
        <div>
          {isAuthenticated ? (
            <button
              type="button"
              onClick={handleToggleCompleted}
              disabled={isUpdatingProgress}
              className={`text-xs font-medium px-5 py-2.5 rounded-full transition-colors ${
                progress?.isCompleted
                  ? 'bg-emerald-700 text-white hover:bg-emerald-800'
                  : 'bg-[#191919] text-white hover:bg-black/85'
              }`}
            >
              {isUpdatingProgress
                ? 'Updating...'
                : progress?.isCompleted
                ? 'Completed (Click to Mark Incomplete)'
                : 'Mark Lesson as Completed'}
            </button>
          ) : (
            <Link
              to="/login"
              className="text-xs font-medium text-[#191919] underline hover:no-underline"
            >
              Sign in to record your lesson completion
            </Link>
          )}
        </div>

        {associatedQuiz && (
          <Link
            to={`/quizzes/${associatedQuiz.id}`}
            className="inline-flex items-center gap-1.5 text-xs font-semibold text-[#191919] bg-white border border-gray-300 hover:bg-gray-50 px-4 py-2.5 rounded-full transition-colors"
          >
            Take Associated Quiz <ArrowRight className="w-3.5 h-3.5" />
          </Link>
        )}
      </div>

      {/* Prev / Next Navigation */}
      <nav className="mt-12 pt-8 border-t border-gray-100 flex items-center justify-between text-xs font-medium">
        {prevModule ? (
          <Link
            to={`/learn/${prevModule.id}`}
            className="text-[#191919]/70 hover:text-[#191919] flex items-center gap-1"
          >
            &larr; Prev: {prevModule.title.substring(0, 25)}...
          </Link>
        ) : (
          <span />
        )}

        {nextModule && (
          <Link
            to={`/learn/${nextModule.id}`}
            className="text-[#191919]/70 hover:text-[#191919] flex items-center gap-1"
          >
            Next: {nextModule.title.substring(0, 25)}... &rarr;
          </Link>
        )}
      </nav>
    </article>
  );
};

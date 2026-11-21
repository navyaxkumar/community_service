import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import {
  ArrowUpRight,
  BadgeCheck,
  BookOpen,
  ClipboardList,
  FolderKanban,
  Plus,
  ShieldAlert,
} from 'lucide-react';
import { badgeService } from '../services/badgeService';
import { fraudService } from '../services/fraudService';
import { learningService } from '../services/learningService';
import { quizService } from '../services/quizService';
import { scenarioService } from '../services/scenarioService';
import { Badge, Scenario } from '../types/badge';
import { FraudCategory } from '../types/fraud';
import { LearningModule } from '../types/learning';
import { Quiz } from '../types/quiz';
import { ErrorMessage } from '../components/common/ErrorMessage';
import { LoadingSpinner } from '../components/common/LoadingSpinner';

type RecentItem = {
  id: number;
  title: string;
  type: string;
  createdAt: string;
  href: string;
};

const formatDate = (date: string) =>
  new Date(date).toLocaleDateString(undefined, { month: 'short', day: 'numeric', year: 'numeric' });

export const AdminPage: React.FC = () => {
  const [categories, setCategories] = useState<FraudCategory[]>([]);
  const [modules, setModules] = useState<LearningModule[]>([]);
  const [quizzes, setQuizzes] = useState<Quiz[]>([]);
  const [scenarios, setScenarios] = useState<Scenario[]>([]);
  const [badges, setBadges] = useState<Badge[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const loadOverview = async () => {
    setIsLoading(true);
    setError(null);
    try {
      const [categoryData, moduleData, quizData, scenarioData, badgeData] = await Promise.all([
        fraudService.getCategories(),
        learningService.getPublishedModules(),
        quizService.getPublishedQuizzes(),
        scenarioService.getPublishedScenarios(),
        badgeService.getActiveBadges(),
      ]);
      setCategories(categoryData);
      setModules(moduleData);
      setQuizzes(quizData);
      setScenarios(scenarioData);
      setBadges(badgeData);
    } catch (loadError) {
      setError(loadError instanceof Error ? loadError.message : 'Could not load the admin overview.');
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    void loadOverview();
  }, []);

  const recentItems: RecentItem[] = [
    ...modules.map((item) => ({
      id: item.id,
      title: item.title,
      type: 'Learning module',
      createdAt: item.createdAt,
      href: '/admin/modules',
    })),
    ...quizzes.map((item) => ({
      id: item.id,
      title: item.title,
      type: 'Quiz',
      createdAt: item.createdAt,
      href: '/admin/quizzes',
    })),
    ...scenarios.map((item) => ({
      id: item.id,
      title: item.title,
      type: 'Scenario',
      createdAt: item.createdAt,
      href: '/admin/scenarios',
    })),
  ]
    .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
    .slice(0, 6);

  const stats = [
    { label: 'Threat categories', value: categories.length, icon: ShieldAlert, href: '/admin/categories' },
    { label: 'Learning modules', value: modules.length, icon: BookOpen, href: '/admin/modules' },
    { label: 'Published quizzes', value: quizzes.length, icon: ClipboardList, href: '/admin/quizzes' },
    { label: 'Scenarios', value: scenarios.length, icon: FolderKanban, href: '/admin/scenarios' },
    { label: 'Active badges', value: badges.length, icon: BadgeCheck, href: '/admin/badges' },
  ];

  if (isLoading) {
    return <LoadingSpinner label="Loading platform overview..." size="lg" />;
  }

  return (
    <div className="space-y-7">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-end sm:justify-between">
        <div>
          <p className="text-[11px] font-semibold uppercase tracking-[0.18em] text-[#191919]/45">
            Platform overview
          </p>
          <h2 className="mt-1 font-serif text-2xl sm:text-3xl">Good to see you.</h2>
          <p className="mt-2 max-w-2xl text-sm leading-6 text-[#191919]/60">
            Review your live educational content and keep the safety library current.
          </p>
        </div>
        <button
          type="button"
          onClick={() => void loadOverview()}
          className="self-start rounded-full border border-gray-300 bg-white px-4 py-2 text-xs font-medium hover:border-[#191919] sm:self-auto"
        >
          Refresh data
        </button>
      </div>

      {error && <ErrorMessage title="Overview could not be refreshed" message={error} onRetry={() => void loadOverview()} />}

      <section aria-label="Content totals" className="grid gap-3 sm:grid-cols-2 xl:grid-cols-5">
        {stats.map(({ label, value, icon: Icon, href }) => (
          <Link
            key={label}
            to={href}
            className="group rounded-2xl border border-gray-200 bg-white p-5 transition hover:-translate-y-0.5 hover:border-gray-400 hover:shadow-sm"
          >
            <div className="flex items-center justify-between">
              <span className="flex h-9 w-9 items-center justify-center rounded-xl bg-[#F3F3F1] text-[#191919]">
                <Icon size={17} aria-hidden="true" />
              </span>
              <ArrowUpRight size={16} className="text-[#191919]/35 transition group-hover:text-[#191919]" />
            </div>
            <p className="mt-5 text-3xl font-semibold tracking-tight">{value}</p>
            <p className="mt-1 text-xs text-[#191919]/55">{label}</p>
          </Link>
        ))}
      </section>

      <div className="grid gap-5 xl:grid-cols-[minmax(0,1.5fr)_minmax(280px,0.8fr)]">
        <section className="overflow-hidden rounded-2xl border border-gray-200 bg-white">
          <div className="flex items-center justify-between border-b border-gray-100 px-5 py-4 sm:px-6">
            <div>
              <h3 className="font-serif text-lg">Recently added content</h3>
              <p className="mt-1 text-xs text-[#191919]/50">Latest items available in the learning library</p>
            </div>
            <Link to="/admin/modules" className="text-xs font-semibold underline underline-offset-4">
              View library
            </Link>
          </div>
          {recentItems.length ? (
            <div className="divide-y divide-gray-100">
              {recentItems.map((item) => (
                <Link
                  key={`${item.type}-${item.id}`}
                  to={item.href}
                  className="flex items-center justify-between gap-4 px-5 py-4 transition hover:bg-[#FAFAF8] sm:px-6"
                >
                  <div className="min-w-0">
                    <p className="truncate text-sm font-medium">{item.title}</p>
                    <p className="mt-1 text-[11px] text-[#191919]/50">{item.type}</p>
                  </div>
                  <time className="shrink-0 text-[11px] text-[#191919]/45">{formatDate(item.createdAt)}</time>
                </Link>
              ))}
            </div>
          ) : (
            <p className="px-6 py-10 text-center text-sm text-[#191919]/55">No published content to show yet.</p>
          )}
        </section>

        <section className="rounded-2xl border border-gray-200 bg-white p-5 sm:p-6">
          <h3 className="font-serif text-lg">Create content</h3>
          <p className="mt-1 text-xs leading-5 text-[#191919]/55">
            Add practical material to the public safety library.
          </p>
          <div className="mt-5 space-y-2">
            {[
              { label: 'Learning module', href: '/admin/modules', icon: BookOpen },
              { label: 'Threat category', href: '/admin/categories', icon: ShieldAlert },
              { label: 'Quiz', href: '/admin/quizzes', icon: ClipboardList },
              { label: 'Scenario', href: '/admin/scenarios', icon: FolderKanban },
              { label: 'Safety badge', href: '/admin/badges', icon: BadgeCheck },
            ].map(({ label, href, icon: Icon }) => (
              <Link
                key={label}
                to={href}
                className="flex items-center justify-between rounded-xl border border-gray-100 px-3.5 py-3 text-xs font-medium transition hover:border-gray-300 hover:bg-[#FAFAF8]"
              >
                <span className="flex items-center gap-2.5">
                  <Icon size={15} aria-hidden="true" className="text-[#191919]/60" />
                  {label}
                </span>
                <Plus size={15} aria-hidden="true" />
              </Link>
            ))}
          </div>
          <div className="mt-5 rounded-xl bg-[#F6F5F2] p-4 text-[11px] leading-5 text-[#191919]/60">
            Counts reflect content returned by the live API. User and activity analytics are not exposed by the current backend.
          </div>
        </section>
      </div>
    </div>
  );
};

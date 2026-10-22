import React, { useEffect, useState } from 'react';
import { learningService } from '../services/learningService';
import { fraudService } from '../services/fraudService';
import { quizService } from '../services/quizService';
import { scenarioService } from '../services/scenarioService';
import { badgeService } from '../services/badgeService';
import { LearningModule } from '../types/learning';
import { FraudCategory } from '../types/fraud';
import { Quiz } from '../types/quiz';
import { Scenario } from '../types/badge';
import { Badge } from '../types/badge';
import { LoadingSpinner } from '../components/common/LoadingSpinner';
import { ErrorMessage } from '../components/common/ErrorMessage';

export const AdminPage: React.FC = () => {
  const [categories, setCategories] = useState<FraudCategory[]>([]);
  const [modules, setModules] = useState<LearningModule[]>([]);
  const [quizzes, setQuizzes] = useState<Quiz[]>([]);
  const [scenarios, setScenarios] = useState<Scenario[]>([]);
  const [badges, setBadges] = useState<Badge[]>([]);

  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // New Category Form State
  const [newCatName, setNewCatName] = useState('');
  const [newCatDesc, setNewCatDesc] = useState('');
  const [isCreatingCategory, setIsCreatingCategory] = useState(false);
  const [catActionMessage, setCatActionMessage] = useState<string | null>(null);

  // New Module Form State
  const [showModuleModal, setShowModuleModal] = useState(false);
  const [modTitle, setModTitle] = useState('');
  const [modDesc, setModDesc] = useState('');
  const [modContent, setModContent] = useState('');
  const [modCatId, setModCatId] = useState<number | undefined>(undefined);
  const [isCreatingModule, setIsCreatingModule] = useState(false);
  const [modActionMessage, setModActionMessage] = useState<string | null>(null);

  const loadAdminData = async () => {
    setIsLoading(true);
    setError(null);

    try {
      const [cats, mods, qzs, scens, bdgs] = await Promise.all([
        fraudService.getCategories().catch(() => []),
        learningService.getPublishedModules().catch(() => []),
        quizService.getPublishedQuizzes().catch(() => []),
        scenarioService.getPublishedScenarios().catch(() => []),
        badgeService.getActiveBadges().catch(() => []),
      ]);

      setCategories(cats);
      setModules(mods);
      setQuizzes(qzs);
      setScenarios(scens);
      setBadges(bdgs);
    } catch (err: any) {
      setError(err.message || 'Failed to load administrative resources.');
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    loadAdminData();
  }, []);

  const handleCreateCategory = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!newCatName.trim()) return;

    setIsCreatingCategory(true);
    setCatActionMessage(null);

    try {
      const created = await fraudService.createCategory({
        name: newCatName.trim(),
        description: newCatDesc.trim() || undefined,
        isActive: true,
      });

      setCategories((prev) => [...prev, created]);
      setNewCatName('');
      setNewCatDesc('');
      setCatActionMessage(`Successfully created category "${created.name}".`);
    } catch (err: any) {
      setError(err.message || 'Failed to create category.');
    } finally {
      setIsCreatingCategory(false);
    }
  };

  const handleCreateModule = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!modTitle.trim() || !modContent.trim()) return;

    setIsCreatingModule(true);
    setModActionMessage(null);

    try {
      const created = await learningService.createModule({
        title: modTitle.trim(),
        description: modDesc.trim() || undefined,
        content: modContent.trim(),
        order: modules.length + 1,
        isPublished: true,
        fraudCategoryId: modCatId || (categories[0]?.id ?? 1),
      });

      setModules((prev) => [...prev, created]);
      setModTitle('');
      setModDesc('');
      setModContent('');
      setShowModuleModal(false);
      setModActionMessage(`Successfully published module "${created.title}".`);
    } catch (err: any) {
      setError(err.message || 'Failed to publish module.');
    } finally {
      setIsCreatingModule(false);
    }
  };

  if (isLoading) {
    return (
      <div className="py-16">
        <LoadingSpinner label="Loading administrative platform data..." size="lg" />
      </div>
    );
  }

  return (
    <div className="space-y-10">
      {/* Header */}
      <div>
        <span className="text-xs uppercase tracking-widest text-[#191919]/50 font-semibold block mb-1">
          Server-Enforced Authorization
        </span>
        <h2 className="text-2xl sm:text-3xl font-serif text-[#191919]">
          Platform Administration & Content Oversight
        </h2>
        <p className="text-xs sm:text-sm text-[#191919]/65 mt-1 leading-relaxed">
          Manage verified threat vectors, publish learning curriculum modules, and monitor platform entity totals through authoritative ASP.NET Core API endpoints.
        </p>
      </div>

      {error && <ErrorMessage message={error} onRetry={loadAdminData} />}

      {/* Aggregate Overview Stat Strip */}
      <div className="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-5 gap-4">
        <div className="p-4 bg-white border border-gray-200 rounded-xl shadow-sm text-center">
          <span className="text-[10px] uppercase font-bold text-[#191919]/50 block">Threat Categories</span>
          <span className="text-2xl font-serif font-bold text-[#191919] mt-1 block">{categories.length}</span>
        </div>
        <div className="p-4 bg-white border border-gray-200 rounded-xl shadow-sm text-center">
          <span className="text-[10px] uppercase font-bold text-[#191919]/50 block">Published Modules</span>
          <span className="text-2xl font-serif font-bold text-[#191919] mt-1 block">{modules.length}</span>
        </div>
        <div className="p-4 bg-white border border-gray-200 rounded-xl shadow-sm text-center">
          <span className="text-[10px] uppercase font-bold text-[#191919]/50 block">Active Quizzes</span>
          <span className="text-2xl font-serif font-bold text-[#191919] mt-1 block">{quizzes.length}</span>
        </div>
        <div className="p-4 bg-white border border-gray-200 rounded-xl shadow-sm text-center">
          <span className="text-[10px] uppercase font-bold text-[#191919]/50 block">Scenarios</span>
          <span className="text-2xl font-serif font-bold text-[#191919] mt-1 block">{scenarios.length}</span>
        </div>
        <div className="p-4 bg-white border border-gray-200 rounded-xl shadow-sm text-center col-span-2 sm:col-span-1">
          <span className="text-[10px] uppercase font-bold text-[#191919]/50 block">Safety Badges</span>
          <span className="text-2xl font-serif font-bold text-[#191919] mt-1 block">{badges.length}</span>
        </div>
      </div>

      {/* Section 1: Learning Modules Management */}
      <div className="bg-white border border-gray-200 rounded-2xl p-6 sm:p-8 shadow-sm space-y-6">
        <div className="flex flex-col sm:flex-row sm:items-center justify-between pb-4 border-b border-gray-100 gap-3">
          <div>
            <h3 className="text-lg font-serif text-[#191919]">Learning Modules Management</h3>
            <p className="text-xs text-[#191919]/60">
              Curriculum items stored in SQL Server and published via <code className="font-mono text-[#191919]">/api/learning-modules</code>.
            </p>
          </div>
          <button
            type="button"
            onClick={() => setShowModuleModal(!showModuleModal)}
            className="text-xs font-medium px-4 py-2 bg-[#191919] text-white rounded-full hover:bg-black/85 transition-colors shrink-0"
          >
            {showModuleModal ? 'Close Form' : '+ Create New Module'}
          </button>
        </div>

        {modActionMessage && (
          <div className="p-3.5 bg-emerald-50 border border-emerald-200 text-emerald-800 text-xs rounded-xl">
            {modActionMessage}
          </div>
        )}

        {/* Create Module Inline Form */}
        {showModuleModal && (
          <form onSubmit={handleCreateModule} className="p-5 bg-[#F4F3F3] rounded-xl space-y-4 border border-gray-200">
            <h4 className="text-xs uppercase font-bold tracking-wider text-[#191919]">
              Publish New Learning Module
            </h4>
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <div>
                <label className="block text-[11px] font-semibold text-[#191919]/70 uppercase mb-1">
                  Module Title
                </label>
                <input
                  type="text"
                  value={modTitle}
                  onChange={(e) => setModTitle(e.target.value)}
                  placeholder="e.g. Recognizing Social Engineering Calls"
                  required
                  className="w-full text-xs p-3 bg-white border border-gray-200 rounded-lg"
                />
              </div>
              <div>
                <label className="block text-[11px] font-semibold text-[#191919]/70 uppercase mb-1">
                  Threat Category
                </label>
                <select
                  value={modCatId || ''}
                  onChange={(e) => setModCatId(parseInt(e.target.value, 10))}
                  className="w-full text-xs p-3 bg-white border border-gray-200 rounded-lg"
                >
                  <option value="">Select Category...</option>
                  {categories.map((c) => (
                    <option key={c.id} value={c.id}>
                      {c.name}
                    </option>
                  ))}
                </select>
              </div>
            </div>

            <div>
              <label className="block text-[11px] font-semibold text-[#191919]/70 uppercase mb-1">
                Summary Description
              </label>
              <input
                type="text"
                value={modDesc}
                onChange={(e) => setModDesc(e.target.value)}
                placeholder="Brief summary of the lesson..."
                className="w-full text-xs p-3 bg-white border border-gray-200 rounded-lg"
              />
            </div>

            <div>
              <label className="block text-[11px] font-semibold text-[#191919]/70 uppercase mb-1">
                Lesson Content
              </label>
              <textarea
                rows={5}
                value={modContent}
                onChange={(e) => setModContent(e.target.value)}
                placeholder="Detailed educational text, safe habits, and examples..."
                required
                className="w-full text-xs p-3 bg-white border border-gray-200 rounded-lg resize-y"
              />
            </div>

            <div className="flex justify-end gap-2">
              <button
                type="button"
                onClick={() => setShowModuleModal(false)}
                className="px-4 py-2 text-xs font-medium text-gray-600 hover:text-black"
              >
                Cancel
              </button>
              <button
                type="submit"
                disabled={isCreatingModule}
                className="px-5 py-2 text-xs font-medium bg-[#191919] text-white rounded-full hover:bg-black/85 disabled:opacity-50"
              >
                {isCreatingModule ? 'Publishing...' : 'Publish Module to API'}
              </button>
            </div>
          </form>
        )}

        {/* Modules Table */}
        <div className="overflow-x-auto">
          <table className="w-full text-left text-xs">
            <thead>
              <tr className="border-b border-gray-200 text-[#191919]/50 font-semibold uppercase tracking-wider text-[10px]">
                <th className="py-3 px-3">Order</th>
                <th className="py-3 px-3">Title</th>
                <th className="py-3 px-3">Category</th>
                <th className="py-3 px-3">Status</th>
                <th className="py-3 px-3">Created</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {modules.map((m) => {
                const cat = categories.find((c) => c.id === m.fraudCategoryId);
                return (
                  <tr key={m.id} className="hover:bg-gray-50/50">
                    <td className="py-3 px-3 font-mono text-gray-500">#{m.order || m.id}</td>
                    <td className="py-3 px-3 font-medium text-[#191919]">{m.title}</td>
                    <td className="py-3 px-3 text-gray-600">{cat?.name || 'General'}</td>
                    <td className="py-3 px-3">
                      <span className="px-2 py-0.5 rounded-full text-[10px] font-semibold bg-emerald-100 text-emerald-800">
                        Published
                      </span>
                    </td>
                    <td className="py-3 px-3 text-gray-500">
                      {new Date(m.createdAt).toLocaleDateString()}
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        </div>
      </div>

      {/* Section 2: Threat Categories Management */}
      <div className="bg-white border border-gray-200 rounded-2xl p-6 sm:p-8 shadow-sm space-y-6">
        <div className="pb-4 border-b border-gray-100">
          <h3 className="text-lg font-serif text-[#191919]">Fraud Categories</h3>
          <p className="text-xs text-[#191919]/60">
            Active threat categories configured in the ASP.NET Core database.
          </p>
        </div>

        {catActionMessage && (
          <div className="p-3.5 bg-emerald-50 border border-emerald-200 text-emerald-800 text-xs rounded-xl">
            {catActionMessage}
          </div>
        )}

        {/* Add Category Form */}
        <form onSubmit={handleCreateCategory} className="grid grid-cols-1 sm:grid-cols-12 gap-3 items-end">
          <div className="sm:col-span-5">
            <label className="block text-[11px] font-semibold text-[#191919]/70 uppercase mb-1">
              Category Name
            </label>
            <input
              type="text"
              value={newCatName}
              onChange={(e) => setNewCatName(e.target.value)}
              placeholder="e.g. Identity & Document Theft"
              required
              className="w-full text-xs p-3 border border-gray-200 rounded-lg"
            />
          </div>
          <div className="sm:col-span-5">
            <label className="block text-[11px] font-semibold text-[#191919]/70 uppercase mb-1">
              Description (Optional)
            </label>
            <input
              type="text"
              value={newCatDesc}
              onChange={(e) => setNewCatDesc(e.target.value)}
              placeholder="Brief explanation of threat pattern..."
              className="w-full text-xs p-3 border border-gray-200 rounded-lg"
            />
          </div>
          <div className="sm:col-span-2">
            <button
              type="submit"
              disabled={isCreatingCategory}
              className="w-full py-3 bg-[#191919] text-white text-xs font-medium rounded-lg hover:bg-black/85 disabled:opacity-50"
            >
              {isCreatingCategory ? 'Adding...' : '+ Add Category'}
            </button>
          </div>
        </form>

        {/* Categories Chips */}
        <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-3 pt-2">
          {categories.map((cat) => (
            <div key={cat.id} className="p-4 bg-[#F4F3F3] rounded-xl border border-gray-200/70 text-xs">
              <div className="flex items-center justify-between mb-1">
                <span className="font-semibold text-[#191919]">{cat.name}</span>
                <span className="font-mono text-[10px] text-gray-500">#{cat.id}</span>
              </div>
              <p className="text-[11px] text-[#191919]/65">
                {cat.description || 'Active fraud category in database.'}
              </p>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
};

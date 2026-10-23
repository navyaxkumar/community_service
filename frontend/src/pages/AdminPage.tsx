import React, { useEffect, useState } from 'react';
import { adminService } from '../services/adminService';
import { learningService } from '../services/learningService';
import { quizService } from '../services/quizService';
import { scenarioService } from '../services/scenarioService';
import { fraudService } from '../services/fraudService';
import { badgeService } from '../services/badgeService';

import { AdminDashboardStats } from '../types/admin';
import { LearningModule, CreateLearningModuleRequest } from '../types/learning';
import { Quiz, QuizManagementDetail, CreateQuizRequest, CreateQuizQuestionRequest } from '../types/quiz';
import { Scenario, CreateScenarioRequest, Badge, CreateBadgeRequest } from '../types/badge';
import { FraudCategory, CreateFraudCategoryRequest } from '../types/fraud';
import { AUDIENCE_SEGMENTS } from '../types/audience';

import { LoadingSpinner } from '../components/common/LoadingSpinner';
import { ErrorMessage } from '../components/common/ErrorMessage';

type AdminTab = 'overview' | 'modules' | 'quizzes' | 'scenarios' | 'categories' | 'badges';

export const AdminPage: React.FC = () => {
  const [activeTab, setActiveTab] = useState<AdminTab>('overview');

  // Database Data States
  const [stats, setStats] = useState<AdminDashboardStats | null>(null);
  const [modules, setModules] = useState<LearningModule[]>([]);
  const [quizzes, setQuizzes] = useState<Quiz[]>([]);
  const [scenarios, setScenarios] = useState<Scenario[]>([]);
  const [categories, setCategories] = useState<FraudCategory[]>([]);
  const [badges, setBadges] = useState<Badge[]>([]);

  // Page-level Loading & Error States
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [actionSuccess, setActionSuccess] = useState<string | null>(null);
  const [actionError, setActionError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  // Search & Filters
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState<'all' | 'published' | 'draft'>('all');
  const [categoryFilter, setCategoryFilter] = useState<number | 'all'>('all');
  const [audienceFilter, setAudienceFilter] = useState<string>('all');
  const [currentPage, setCurrentPage] = useState(1);
  const pageSize = 8;

  // Modals
  const [showModuleModal, setShowModuleModal] = useState(false);
  const [editingModule, setEditingModule] = useState<LearningModule | null>(null);

  const [showQuizModal, setShowQuizModal] = useState(false);
  const [editingQuiz, setEditingQuiz] = useState<QuizManagementDetail | null>(null);

  const [showScenarioModal, setShowScenarioModal] = useState(false);
  const [editingScenario, setEditingScenario] = useState<Scenario | null>(null);

  const [showCategoryModal, setShowCategoryModal] = useState(false);
  const [editingCategory, setEditingCategory] = useState<FraudCategory | null>(null);

  const [showBadgeModal, setShowBadgeModal] = useState(false);
  const [editingBadge, setEditingBadge] = useState<Badge | null>(null);

  // Preview Modal
  const [previewItem, setPreviewItem] = useState<{
    type: 'module' | 'quiz' | 'scenario';
    data: any;
  } | null>(null);

  // Confirmation Dialog
  const [confirmDialog, setConfirmDialog] = useState<{
    title: string;
    message: string;
    confirmLabel: string;
    onConfirm: () => Promise<void>;
  } | null>(null);

  // Load All Admin Data
  const loadAdminData = async () => {
    setIsLoading(true);
    setError(null);

    try {
      const [
        statsData,
        modulesData,
        quizzesData,
        scenariosData,
        categoriesData,
        badgesData,
      ] = await Promise.all([
        adminService.getStats().catch(() => null),
        learningService.getAllModulesAdmin().catch(() => []),
        quizService.getAllQuizzesAdmin().catch(() => []),
        scenarioService.getAllScenariosAdmin().catch(() => []),
        fraudService.getAllCategoriesAdmin().catch(() => []),
        badgeService.getAllBadgesAdmin().catch(() => []),
      ]);

      setStats(statsData);
      setModules(modulesData);
      setQuizzes(quizzesData);
      setScenarios(scenariosData);
      setCategories(categoriesData);
      setBadges(badgesData);
    } catch (err: any) {
      setError(err.message || 'Failed to load administrative resources.');
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    loadAdminData();
  }, []);

  // Reset pagination on tab change or filter
  useEffect(() => {
    setCurrentPage(1);
    setSearchTerm('');
    setStatusFilter('all');
    setCategoryFilter('all');
  }, [activeTab]);

  const showSuccessNotification = (msg: string) => {
    setActionSuccess(msg);
    setActionError(null);
    setTimeout(() => setActionSuccess(null), 5000);
  };

  const showErrorNotification = (msg: string) => {
    setActionError(msg);
    setActionSuccess(null);
  };

  // -------------------------------------------------------------
  // MODULE ACTIONS
  // -------------------------------------------------------------
  const handleSaveModule = async (e: React.FormEvent, form: CreateLearningModuleRequest, editId?: number) => {
    e.preventDefault();
    setIsSubmitting(true);
    setActionError(null);

    try {
      if (editId) {
        const updated = await learningService.updateModule(editId, form);
        setModules((prev) => prev.map((m) => (m.id === editId ? updated : m)));
        showSuccessNotification(`Module "${updated.title}" updated successfully.`);
      } else {
        const created = await learningService.createModule(form);
        setModules((prev) => [created, ...prev]);
        showSuccessNotification(`Module "${created.title}" created successfully.`);
      }
      setShowModuleModal(false);
      setEditingModule(null);
      await refreshStats();
    } catch (err: any) {
      showErrorNotification(err.message || 'Failed to save module.');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleTogglePublishModule = (module: LearningModule) => {
    const newStatus = !module.isPublished;
    setConfirmDialog({
      title: newStatus ? 'Publish Learning Module?' : 'Unpublish Learning Module?',
      message: newStatus
        ? `Are you sure you want to publish "${module.title}"? It will become visible to all active learners.`
        : `Are you sure you want to unpublish "${module.title}"? Learners will no longer be able to access it.`,
      confirmLabel: newStatus ? 'Publish Now' : 'Unpublish',
      onConfirm: async () => {
        try {
          const updated = await learningService.setPublishStatus(module.id, newStatus);
          setModules((prev) => prev.map((m) => (m.id === module.id ? updated : m)));
          showSuccessNotification(
            `Module "${updated.title}" is now ${newStatus ? 'published' : 'draft'}.`
          );
          if (previewItem && previewItem.data.id === module.id) {
            setPreviewItem({ type: 'module', data: updated });
          }
          await refreshStats();
        } catch (err: any) {
          showErrorNotification(err.message || 'Failed to update publication status.');
        } finally {
          setConfirmDialog(null);
        }
      },
    });
  };

  const handleDeleteModule = (module: LearningModule) => {
    setConfirmDialog({
      title: 'Delete Learning Module?',
      message: `Are you sure you want to permanently delete "${module.title}"? This action cannot be undone. If users have learning progress for this module, deletion will be safely rejected.`,
      confirmLabel: 'Delete Permanently',
      onConfirm: async () => {
        try {
          await learningService.deleteModule(module.id);
          setModules((prev) => prev.filter((m) => m.id !== module.id));
          showSuccessNotification(`Module "${module.title}" was deleted.`);
          await refreshStats();
        } catch (err: any) {
          showErrorNotification(err.message || 'Failed to delete module.');
        } finally {
          setConfirmDialog(null);
        }
      },
    });
  };

  // -------------------------------------------------------------
  // QUIZ ACTIONS
  // -------------------------------------------------------------
  const handleOpenEditQuiz = async (quiz: Quiz) => {
    setIsSubmitting(true);
    try {
      const details = await quizService.getQuizManagementDetail(quiz.id);
      setEditingQuiz(details);
      setShowQuizModal(true);
    } catch (err: any) {
      showErrorNotification(err.message || 'Failed to load quiz details for editing.');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleSaveQuiz = async (e: React.FormEvent, form: CreateQuizRequest, editId?: number) => {
    e.preventDefault();
    setIsSubmitting(true);
    setActionError(null);

    try {
      if (editId) {
        const updated = await quizService.updateQuiz(editId, form);
        setQuizzes((prev) => prev.map((q) => (q.id === editId ? updated : q)));
        showSuccessNotification(`Quiz "${updated.title}" updated successfully.`);
      } else {
        const created = await quizService.createQuiz(form);
        setQuizzes((prev) => [created, ...prev]);
        showSuccessNotification(`Quiz "${created.title}" created successfully.`);
      }
      setShowQuizModal(false);
      setEditingQuiz(null);
      await refreshStats();
    } catch (err: any) {
      showErrorNotification(err.message || 'Failed to save quiz.');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleTogglePublishQuiz = (quiz: Quiz) => {
    const newStatus = !quiz.isPublished;
    setConfirmDialog({
      title: newStatus ? 'Publish Quiz?' : 'Unpublish Quiz?',
      message: newStatus
        ? `Are you sure you want to publish "${quiz.title}"? The system will verify that all questions have at least two choices and one valid correct answer.`
        : `Are you sure you want to unpublish "${quiz.title}"?`,
      confirmLabel: newStatus ? 'Publish Now' : 'Unpublish',
      onConfirm: async () => {
        try {
          const updated = await quizService.setPublishStatus(quiz.id, newStatus);
          setQuizzes((prev) => prev.map((q) => (q.id === quiz.id ? updated : q)));
          showSuccessNotification(
            `Quiz "${updated.title}" is now ${newStatus ? 'published' : 'draft'}.`
          );
          await refreshStats();
        } catch (err: any) {
          showErrorNotification(err.message || 'Failed to update quiz publication status.');
        } finally {
          setConfirmDialog(null);
        }
      },
    });
  };

  const handleDeleteQuiz = (quiz: Quiz) => {
    setConfirmDialog({
      title: 'Delete Quiz?',
      message: `Are you sure you want to permanently delete "${quiz.title}"? If users have recorded quiz attempts for this quiz, deletion will be safely rejected to preserve audit history.`,
      confirmLabel: 'Delete Permanently',
      onConfirm: async () => {
        try {
          await quizService.deleteQuiz(quiz.id);
          setQuizzes((prev) => prev.filter((q) => q.id !== quiz.id));
          showSuccessNotification(`Quiz "${quiz.title}" was deleted.`);
          await refreshStats();
        } catch (err: any) {
          showErrorNotification(err.message || 'Failed to delete quiz.');
        } finally {
          setConfirmDialog(null);
        }
      },
    });
  };

  // -------------------------------------------------------------
  // SCENARIO ACTIONS
  // -------------------------------------------------------------
  const handleSaveScenario = async (e: React.FormEvent, form: CreateScenarioRequest, editId?: number) => {
    e.preventDefault();
    setIsSubmitting(true);
    setActionError(null);

    try {
      if (editId) {
        const updated = await scenarioService.updateScenario(editId, form);
        setScenarios((prev) => prev.map((s) => (s.id === editId ? updated : s)));
        showSuccessNotification(`Scenario "${updated.title}" updated successfully.`);
      } else {
        const created = await scenarioService.createScenario(form);
        setScenarios((prev) => [created, ...prev]);
        showSuccessNotification(`Scenario "${created.title}" created successfully.`);
      }
      setShowScenarioModal(false);
      setEditingScenario(null);
      await refreshStats();
    } catch (err: any) {
      showErrorNotification(err.message || 'Failed to save scenario.');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleTogglePublishScenario = (scenario: Scenario) => {
    const newStatus = !scenario.isPublished;
    setConfirmDialog({
      title: newStatus ? 'Publish Scenario?' : 'Unpublish Scenario?',
      message: newStatus
        ? `Are you sure you want to publish "${scenario.title}"?`
        : `Are you sure you want to unpublish "${scenario.title}"?`,
      confirmLabel: newStatus ? 'Publish Now' : 'Unpublish',
      onConfirm: async () => {
        try {
          const updated = await scenarioService.setPublishStatus(scenario.id, newStatus);
          setScenarios((prev) => prev.map((s) => (s.id === scenario.id ? updated : s)));
          showSuccessNotification(
            `Scenario "${updated.title}" is now ${newStatus ? 'published' : 'draft'}.`
          );
          await refreshStats();
        } catch (err: any) {
          showErrorNotification(err.message || 'Failed to update scenario publication status.');
        } finally {
          setConfirmDialog(null);
        }
      },
    });
  };

  const handleDeleteScenario = (scenario: Scenario) => {
    setConfirmDialog({
      title: 'Delete Scenario?',
      message: `Are you sure you want to permanently delete "${scenario.title}"?`,
      confirmLabel: 'Delete Permanently',
      onConfirm: async () => {
        try {
          await scenarioService.deleteScenario(scenario.id);
          setScenarios((prev) => prev.filter((s) => s.id !== scenario.id));
          showSuccessNotification(`Scenario "${scenario.title}" was deleted.`);
          await refreshStats();
        } catch (err: any) {
          showErrorNotification(err.message || 'Failed to delete scenario.');
        } finally {
          setConfirmDialog(null);
        }
      },
    });
  };

  // -------------------------------------------------------------
  // CATEGORY ACTIONS
  // -------------------------------------------------------------
  const handleSaveCategory = async (e: React.FormEvent, form: CreateFraudCategoryRequest, editId?: number, isActive: boolean = true) => {
    e.preventDefault();
    setIsSubmitting(true);
    setActionError(null);

    try {
      if (editId) {
        const updated = await fraudService.updateCategory(editId, {
          name: form.name,
          description: form.description,
          isActive,
        });
        setCategories((prev) => prev.map((c) => (c.id === editId ? updated : c)));
        showSuccessNotification(`Category "${updated.name}" updated successfully.`);
      } else {
        const created = await fraudService.createCategory(form);
        setCategories((prev) => [created, ...prev]);
        showSuccessNotification(`Category "${created.name}" created successfully.`);
      }
      setShowCategoryModal(false);
      setEditingCategory(null);
      await refreshStats();
    } catch (err: any) {
      showErrorNotification(err.message || 'Failed to save fraud category.');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDeleteCategory = (cat: FraudCategory) => {
    setConfirmDialog({
      title: 'Delete Fraud Category?',
      message: `Are you sure you want to delete "${cat.name}"? If there are learning modules, quizzes, or scenarios assigned to this category, deletion will be rejected. You can deactivate it instead.`,
      confirmLabel: 'Delete',
      onConfirm: async () => {
        try {
          await fraudService.deleteCategory(cat.id);
          setCategories((prev) => prev.filter((c) => c.id !== cat.id));
          showSuccessNotification(`Category "${cat.name}" was deleted.`);
          await refreshStats();
        } catch (err: any) {
          showErrorNotification(err.message || 'Failed to delete category.');
        } finally {
          setConfirmDialog(null);
        }
      },
    });
  };

  // -------------------------------------------------------------
  // BADGE ACTIONS
  // -------------------------------------------------------------
  const handleSaveBadge = async (e: React.FormEvent, form: CreateBadgeRequest, editId?: number) => {
    e.preventDefault();
    setIsSubmitting(true);
    setActionError(null);

    try {
      if (editId) {
        const updated = await badgeService.updateBadge(editId, form);
        setBadges((prev) => prev.map((b) => (b.id === editId ? updated : b)));
        showSuccessNotification(`Badge "${updated.name}" updated successfully.`);
      } else {
        const created = await badgeService.createBadge(form);
        setBadges((prev) => [created, ...prev]);
        showSuccessNotification(`Badge "${created.name}" created successfully.`);
      }
      setShowBadgeModal(false);
      setEditingBadge(null);
      await refreshStats();
    } catch (err: any) {
      showErrorNotification(err.message || 'Failed to save badge.');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDeleteBadge = (badge: Badge) => {
    setConfirmDialog({
      title: 'Delete Badge?',
      message: `Are you sure you want to delete "${badge.name}"?`,
      confirmLabel: 'Delete',
      onConfirm: async () => {
        try {
          await badgeService.deleteBadge(badge.id);
          setBadges((prev) => prev.filter((b) => b.id !== badge.id));
          showSuccessNotification(`Badge "${badge.name}" was deleted.`);
          await refreshStats();
        } catch (err: any) {
          showErrorNotification(err.message || 'Failed to delete badge.');
        } finally {
          setConfirmDialog(null);
        }
      },
    });
  };

  const refreshStats = async () => {
    try {
      const latestStats = await adminService.getStats();
      setStats(latestStats);
    } catch {
      // Non-critical background refresh
    }
  };

  // Preview loader
  const handleOpenPreview = async (type: 'module' | 'quiz' | 'scenario', item: any) => {
    if (type === 'quiz') {
      try {
        const details = await quizService.getQuizManagementDetail(item.id);
        setPreviewItem({ type: 'quiz', data: details });
      } catch (err: any) {
        showErrorNotification(err.message || 'Failed to load preview details.');
      }
    } else {
      setPreviewItem({ type, data: item });
    }
  };

  if (isLoading) {
    return (
      <div className="py-24">
        <LoadingSpinner label="Connecting to secure administrative data plane..." size="lg" />
      </div>
    );
  }

  return (
    <div className="space-y-8 max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      {/* Page Header */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 border-b border-gray-200 pb-6">
        <div>
          <div className="flex items-center gap-2">
            <span className="px-2.5 py-0.5 rounded-full text-[10px] font-bold uppercase tracking-wider bg-red-100 text-red-900 border border-red-200">
              Admin CMS
            </span>
            <span className="text-xs text-gray-500 font-mono">Role: Administrator</span>
          </div>
          <h1 className="text-3xl font-serif text-[#191919] mt-2">
            Platform Content Management System
          </h1>
          <p className="text-xs sm:text-sm text-[#191919]/70 mt-1">
            Author, review, publish, and audit digital safety curriculum, real threat vectors, interactive quizzes, and credentials.
          </p>
        </div>
        <div className="flex items-center gap-2 shrink-0">
          <button
            type="button"
            onClick={loadAdminData}
            className="text-xs px-4 py-2 border border-gray-300 rounded-full hover:bg-white transition-colors flex items-center gap-1.5 font-medium text-[#191919]"
          >
            <span>&#x21bb;</span> Refresh All Data
          </button>
        </div>
      </div>

      {/* Global Notifications */}
      {actionSuccess && (
        <div className="p-4 bg-emerald-50 border border-emerald-200 text-emerald-900 rounded-xl text-xs flex items-center justify-between">
          <span>&check; {actionSuccess}</span>
          <button type="button" onClick={() => setActionSuccess(null)} className="font-bold ml-2">
            &times;
          </button>
        </div>
      )}

      {actionError && (
        <div className="p-4 bg-red-50 border border-red-200 text-red-900 rounded-xl text-xs flex items-center justify-between">
          <span>&cross; {actionError}</span>
          <button type="button" onClick={() => setActionError(null)} className="font-bold ml-2">
            &times;
          </button>
        </div>
      )}

      {error && <ErrorMessage message={error} onRetry={loadAdminData} />}

      {/* Navigation Tabs */}
      <div className="flex items-center gap-2 overflow-x-auto border-b border-gray-200 pb-1 scrollbar-none">
        <button
          type="button"
          onClick={() => setActiveTab('overview')}
          className={`text-xs font-semibold px-4 py-2.5 rounded-t-lg transition-colors border-b-2 whitespace-nowrap ${
            activeTab === 'overview'
              ? 'border-[#191919] text-[#191919] bg-white'
              : 'border-transparent text-gray-500 hover:text-black hover:bg-gray-100'
          }`}
        >
          Overview & Counts
        </button>
        <button
          type="button"
          onClick={() => setActiveTab('modules')}
          className={`text-xs font-semibold px-4 py-2.5 rounded-t-lg transition-colors border-b-2 whitespace-nowrap ${
            activeTab === 'modules'
              ? 'border-[#191919] text-[#191919] bg-white'
              : 'border-transparent text-gray-500 hover:text-black hover:bg-gray-100'
          }`}
        >
          Learning Modules ({modules.length})
        </button>
        <button
          type="button"
          onClick={() => setActiveTab('quizzes')}
          className={`text-xs font-semibold px-4 py-2.5 rounded-t-lg transition-colors border-b-2 whitespace-nowrap ${
            activeTab === 'quizzes'
              ? 'border-[#191919] text-[#191919] bg-white'
              : 'border-transparent text-gray-500 hover:text-black hover:bg-gray-100'
          }`}
        >
          Quizzes ({quizzes.length})
        </button>
        <button
          type="button"
          onClick={() => setActiveTab('scenarios')}
          className={`text-xs font-semibold px-4 py-2.5 rounded-t-lg transition-colors border-b-2 whitespace-nowrap ${
            activeTab === 'scenarios'
              ? 'border-[#191919] text-[#191919] bg-white'
              : 'border-transparent text-gray-500 hover:text-black hover:bg-gray-100'
          }`}
        >
          Scenarios ({scenarios.length})
        </button>
        <button
          type="button"
          onClick={() => setActiveTab('categories')}
          className={`text-xs font-semibold px-4 py-2.5 rounded-t-lg transition-colors border-b-2 whitespace-nowrap ${
            activeTab === 'categories'
              ? 'border-[#191919] text-[#191919] bg-white'
              : 'border-transparent text-gray-500 hover:text-black hover:bg-gray-100'
          }`}
        >
          Threat Categories ({categories.length})
        </button>
        <button
          type="button"
          onClick={() => setActiveTab('badges')}
          className={`text-xs font-semibold px-4 py-2.5 rounded-t-lg transition-colors border-b-2 whitespace-nowrap ${
            activeTab === 'badges'
              ? 'border-[#191919] text-[#191919] bg-white'
              : 'border-transparent text-gray-500 hover:text-black hover:bg-gray-100'
          }`}
        >
          Safety Badges ({badges.length})
        </button>
      </div>

      {/* TAB CONTENT 1: OVERVIEW */}
      {activeTab === 'overview' && (
        <div className="space-y-8">
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-5">
            {/* Modules Stat */}
            <div className="bg-white border border-gray-200 rounded-2xl p-6 shadow-sm">
              <span className="text-xs uppercase font-bold text-gray-400 block tracking-wider">Learning Modules</span>
              <div className="flex items-baseline justify-between mt-2">
                <span className="text-3xl font-serif font-bold text-[#191919]">{stats?.totalModules ?? modules.length}</span>
                <span className="text-xs font-medium text-emerald-700 bg-emerald-50 px-2 py-0.5 rounded-full border border-emerald-200">
                  {stats?.publishedModules ?? modules.filter(m => m.isPublished).length} Published
                </span>
              </div>
              <p className="text-xs text-gray-500 mt-3">
                {stats?.draftModules ?? modules.filter(m => !m.isPublished).length} unpublished draft modules
              </p>
            </div>

            {/* Quizzes Stat */}
            <div className="bg-white border border-gray-200 rounded-2xl p-6 shadow-sm">
              <span className="text-xs uppercase font-bold text-gray-400 block tracking-wider">Quizzes</span>
              <div className="flex items-baseline justify-between mt-2">
                <span className="text-3xl font-serif font-bold text-[#191919]">{stats?.totalQuizzes ?? quizzes.length}</span>
                <span className="text-xs font-medium text-emerald-700 bg-emerald-50 px-2 py-0.5 rounded-full border border-emerald-200">
                  {stats?.publishedQuizzes ?? quizzes.filter(q => q.isPublished).length} Published
                </span>
              </div>
              <p className="text-xs text-gray-500 mt-3">
                {stats?.totalQuizQuestions ?? 0} total questions configured
              </p>
            </div>

            {/* Scenarios Stat */}
            <div className="bg-white border border-gray-200 rounded-2xl p-6 shadow-sm">
              <span className="text-xs uppercase font-bold text-gray-400 block tracking-wider">Scenarios</span>
              <div className="flex items-baseline justify-between mt-2">
                <span className="text-3xl font-serif font-bold text-[#191919]">{stats?.totalScenarios ?? scenarios.length}</span>
                <span className="text-xs font-medium text-emerald-700 bg-emerald-50 px-2 py-0.5 rounded-full border border-emerald-200">
                  {stats?.publishedScenarios ?? scenarios.filter(s => s.isPublished).length} Published
                </span>
              </div>
              <p className="text-xs text-gray-500 mt-3">
                {stats?.draftScenarios ?? scenarios.filter(s => !s.isPublished).length} draft scenarios
              </p>
            </div>

            {/* Fraud Categories Stat */}
            <div className="bg-white border border-gray-200 rounded-2xl p-6 shadow-sm">
              <span className="text-xs uppercase font-bold text-gray-400 block tracking-wider">Threat Categories</span>
              <div className="flex items-baseline justify-between mt-2">
                <span className="text-3xl font-serif font-bold text-[#191919]">{stats?.totalCategories ?? categories.length}</span>
                <span className="text-xs font-medium text-blue-700 bg-blue-50 px-2 py-0.5 rounded-full border border-blue-200">
                  {stats?.activeCategories ?? categories.filter(c => c.isActive).length} Active
                </span>
              </div>
              <p className="text-xs text-gray-500 mt-3">
                {stats?.inactiveCategories ?? categories.filter(c => !c.isActive).length} archived categories
              </p>
            </div>
          </div>

          {/* User Activity & Database Integrity Row */}
          <div className="grid grid-cols-1 sm:grid-cols-3 gap-5">
            <div className="bg-white border border-gray-200 rounded-2xl p-6 shadow-sm">
              <span className="text-xs uppercase font-bold text-gray-400 block tracking-wider">Total Registered Learners</span>
              <span className="text-2xl font-serif font-bold text-[#191919] mt-2 block">{stats?.totalUsers ?? '—'}</span>
              <p className="text-xs text-gray-500 mt-1">Users persisting learning progress</p>
            </div>
            <div className="bg-white border border-gray-200 rounded-2xl p-6 shadow-sm">
              <span className="text-xs uppercase font-bold text-gray-400 block tracking-wider">Historical Quiz Attempts</span>
              <span className="text-2xl font-serif font-bold text-[#191919] mt-2 block">{stats?.totalQuizAttempts ?? '—'}</span>
              <p className="text-xs text-gray-500 mt-1">Stored audit results (protected against deletion)</p>
            </div>
            <div className="bg-white border border-gray-200 rounded-2xl p-6 shadow-sm">
              <span className="text-xs uppercase font-bold text-gray-400 block tracking-wider">Earnable Safety Badges</span>
              <span className="text-2xl font-serif font-bold text-[#191919] mt-2 block">{stats?.totalBadges ?? badges.length}</span>
              <p className="text-xs text-gray-500 mt-1">{badges.filter(b => b.isActive).length} active milestone awards</p>
            </div>
          </div>
        </div>
      )}

      {/* TAB CONTENT 2: LEARNING MODULES */}
      {activeTab === 'modules' && (
        <ContentSection
          title="Learning Curriculum Modules"
          description="Author and publish digital security lessons. Drafts remain hidden from ordinary learners until published."
          createButtonLabel="+ Create New Module"
          onCreate={() => {
            setEditingModule(null);
            setShowModuleModal(true);
          }}
          searchTerm={searchTerm}
          onSearchChange={setSearchTerm}
          statusFilter={statusFilter}
          onStatusFilterChange={setStatusFilter}
          categories={categories}
          categoryFilter={categoryFilter}
          onCategoryFilterChange={setCategoryFilter}
          audienceFilter={audienceFilter}
          onAudienceFilterChange={setAudienceFilter}
        >
          {(() => {
            const filtered = modules.filter((m) => {
              const matchSearch =
                m.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
                (m.description || '').toLowerCase().includes(searchTerm.toLowerCase()) ||
                m.content.toLowerCase().includes(searchTerm.toLowerCase());
              const matchStatus =
                statusFilter === 'all'
                  ? true
                  : statusFilter === 'published'
                  ? m.isPublished
                  : !m.isPublished;
              const matchCat = categoryFilter === 'all' ? true : m.fraudCategoryId === categoryFilter;
              const matchAudience =
                audienceFilter === 'all'
                  ? true
                  : (m.targetAudience || 'All').toLowerCase() === audienceFilter.toLowerCase();
              return matchSearch && matchStatus && matchCat && matchAudience;
            });

            if (filtered.length === 0) {
              return <EmptyState message="No learning modules match the selected filter or search term." />;
            }

            const paginated = filtered.slice((currentPage - 1) * pageSize, currentPage * pageSize);

            return (
              <>
                <div className="overflow-x-auto">
                  <table className="w-full text-left text-xs">
                    <thead>
                      <tr className="border-b border-gray-200 text-gray-400 font-semibold uppercase text-[10px] tracking-wider">
                        <th className="py-3 px-3">Order</th>
                        <th className="py-3 px-3">Title & Summary</th>
                        <th className="py-3 px-3">Audience & Level</th>
                        <th className="py-3 px-3">Category</th>
                        <th className="py-3 px-3">Status</th>
                        <th className="py-3 px-3 text-right">Actions</th>
                      </tr>
                    </thead>
                    <tbody className="divide-y divide-gray-100">
                      {paginated.map((m) => {
                        const cat = categories.find((c) => c.id === m.fraudCategoryId);
                        return (
                          <tr key={m.id} className="hover:bg-gray-50/50">
                            <td className="py-3 px-3 font-mono text-gray-400">#{m.order}</td>
                            <td className="py-3 px-3">
                              <span className="font-semibold text-[#191919] block">{m.title}</span>
                              {m.description && (
                                <span className="text-[11px] text-gray-500 line-clamp-1">{m.description}</span>
                              )}
                            </td>
                            <td className="py-3 px-3">
                              <div className="flex flex-col gap-1 items-start">
                                <AudienceBadge audience={m.targetAudience} />
                                <DifficultyBadge difficulty={m.difficultyLevel} />
                              </div>
                            </td>
                            <td className="py-3 px-3">
                              <span className="px-2 py-0.5 rounded bg-gray-100 text-gray-700 text-[10px] font-medium">
                                {cat?.name || 'General'}
                              </span>
                            </td>
                            <td className="py-3 px-3">
                              <StatusBadge isPublished={m.isPublished} />
                            </td>
                            <td className="py-3 px-3 text-right space-x-1.5 whitespace-nowrap">
                              <button
                                type="button"
                                onClick={() => handleOpenPreview('module', m)}
                                className="px-2.5 py-1 text-[11px] border border-gray-300 rounded hover:bg-white transition-colors"
                              >
                                Preview
                              </button>
                              <button
                                type="button"
                                onClick={() => {
                                  setEditingModule(m);
                                  setShowModuleModal(true);
                                }}
                                className="px-2.5 py-1 text-[11px] bg-gray-100 text-[#191919] rounded hover:bg-gray-200 transition-colors font-medium"
                              >
                                Edit
                              </button>
                              <button
                                type="button"
                                onClick={() => handleTogglePublishModule(m)}
                                className={`px-2.5 py-1 text-[11px] rounded font-medium transition-colors ${
                                  m.isPublished
                                    ? 'bg-amber-100 text-amber-900 hover:bg-amber-200'
                                    : 'bg-emerald-100 text-emerald-900 hover:bg-emerald-200'
                                }`}
                              >
                                {m.isPublished ? 'Unpublish' : 'Publish'}
                              </button>
                              <button
                                type="button"
                                onClick={() => handleDeleteModule(m)}
                                className="px-2.5 py-1 text-[11px] text-red-600 hover:text-red-800 transition-colors"
                              >
                                Delete
                              </button>
                            </td>
                          </tr>
                        );
                      })}
                    </tbody>
                  </table>
                </div>
                <Pagination
                  currentPage={currentPage}
                  totalCount={filtered.length}
                  pageSize={pageSize}
                  onPageChange={setCurrentPage}
                />
              </>
            );
          })()}
        </ContentSection>
      )}

      {/* TAB CONTENT 3: QUIZZES */}
      {activeTab === 'quizzes' && (
        <ContentSection
          title="Interactive Quizzes"
          description="Build evaluation assessments. Validates that every question has at least two choices and one correct answer before publishing."
          createButtonLabel="+ Create New Quiz"
          onCreate={() => {
            setEditingQuiz(null);
            setShowQuizModal(true);
          }}
          searchTerm={searchTerm}
          onSearchChange={setSearchTerm}
          statusFilter={statusFilter}
          onStatusFilterChange={setStatusFilter}
          categories={categories}
          categoryFilter={categoryFilter}
          onCategoryFilterChange={setCategoryFilter}
          audienceFilter={audienceFilter}
          onAudienceFilterChange={setAudienceFilter}
        >
          {(() => {
            const filtered = quizzes.filter((q) => {
              const matchSearch =
                q.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
                (q.description || '').toLowerCase().includes(searchTerm.toLowerCase());
              const matchStatus =
                statusFilter === 'all'
                  ? true
                  : statusFilter === 'published'
                  ? q.isPublished
                  : !q.isPublished;
              const matchCat = categoryFilter === 'all' ? true : q.fraudCategoryId === categoryFilter;
              const matchAudience =
                audienceFilter === 'all'
                  ? true
                  : (q.targetAudience || 'All').toLowerCase() === audienceFilter.toLowerCase();
              return matchSearch && matchStatus && matchCat && matchAudience;
            });

            if (filtered.length === 0) {
              return <EmptyState message="No quizzes match the selected filter or search term." />;
            }

            const paginated = filtered.slice((currentPage - 1) * pageSize, currentPage * pageSize);

            return (
              <>
                <div className="overflow-x-auto">
                  <table className="w-full text-left text-xs">
                    <thead>
                      <tr className="border-b border-gray-200 text-gray-400 font-semibold uppercase text-[10px] tracking-wider">
                        <th className="py-3 px-3">Title & Overview</th>
                        <th className="py-3 px-3">Audience & Level</th>
                        <th className="py-3 px-3">Category</th>
                        <th className="py-3 px-3">Status</th>
                        <th className="py-3 px-3 text-right">Actions</th>
                      </tr>
                    </thead>
                    <tbody className="divide-y divide-gray-100">
                      {paginated.map((q) => {
                        const cat = categories.find((c) => c.id === q.fraudCategoryId);
                        return (
                          <tr key={q.id} className="hover:bg-gray-50/50">
                            <td className="py-3 px-3">
                              <span className="font-semibold text-[#191919] block">{q.title}</span>
                              {q.description && (
                                <span className="text-[11px] text-gray-500 line-clamp-1">{q.description}</span>
                              )}
                            </td>
                            <td className="py-3 px-3">
                              <div className="flex flex-col gap-1 items-start">
                                <AudienceBadge audience={q.targetAudience} />
                                <DifficultyBadge difficulty={q.difficultyLevel} />
                              </div>
                            </td>
                            <td className="py-3 px-3">
                              <span className="px-2 py-0.5 rounded bg-gray-100 text-gray-700 text-[10px] font-medium">
                                {cat?.name || 'General'}
                              </span>
                            </td>
                            <td className="py-3 px-3">
                              <StatusBadge isPublished={q.isPublished} />
                            </td>
                            <td className="py-3 px-3 text-right space-x-1.5 whitespace-nowrap">
                              <button
                                type="button"
                                onClick={() => handleOpenPreview('quiz', q)}
                                className="px-2.5 py-1 text-[11px] border border-gray-300 rounded hover:bg-white transition-colors"
                              >
                                Preview
                              </button>
                              <button
                                type="button"
                                onClick={() => handleOpenEditQuiz(q)}
                                className="px-2.5 py-1 text-[11px] bg-gray-100 text-[#191919] rounded hover:bg-gray-200 transition-colors font-medium"
                              >
                                Edit Questions
                              </button>
                              <button
                                type="button"
                                onClick={() => handleTogglePublishQuiz(q)}
                                className={`px-2.5 py-1 text-[11px] rounded font-medium transition-colors ${
                                  q.isPublished
                                    ? 'bg-amber-100 text-amber-900 hover:bg-amber-200'
                                    : 'bg-emerald-100 text-emerald-900 hover:bg-emerald-200'
                                }`}
                              >
                                {q.isPublished ? 'Unpublish' : 'Publish'}
                              </button>
                              <button
                                type="button"
                                onClick={() => handleDeleteQuiz(q)}
                                className="px-2.5 py-1 text-[11px] text-red-600 hover:text-red-800 transition-colors"
                              >
                                Delete
                              </button>
                            </td>
                          </tr>
                        );
                      })}
                    </tbody>
                  </table>
                </div>
                <Pagination
                  currentPage={currentPage}
                  totalCount={filtered.length}
                  pageSize={pageSize}
                  onPageChange={setCurrentPage}
                />
              </>
            );
          })()}
        </ContentSection>
      )}

      {/* TAB CONTENT 4: SCENARIOS */}
      {activeTab === 'scenarios' && (
        <ContentSection
          title="Interactive Scam Scenarios"
          description="Real-world simulated cases that challenge learners to identify deception and choose the correct action."
          createButtonLabel="+ Create New Scenario"
          onCreate={() => {
            setEditingScenario(null);
            setShowScenarioModal(true);
          }}
          searchTerm={searchTerm}
          onSearchChange={setSearchTerm}
          statusFilter={statusFilter}
          onStatusFilterChange={setStatusFilter}
          categories={categories}
          categoryFilter={categoryFilter}
          onCategoryFilterChange={setCategoryFilter}
          audienceFilter={audienceFilter}
          onAudienceFilterChange={setAudienceFilter}
        >
          {(() => {
            const filtered = scenarios.filter((s) => {
              const matchSearch =
                s.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
                s.situation.toLowerCase().includes(searchTerm.toLowerCase()) ||
                s.correctAction.toLowerCase().includes(searchTerm.toLowerCase());
              const matchStatus =
                statusFilter === 'all'
                  ? true
                  : statusFilter === 'published'
                  ? s.isPublished
                  : !s.isPublished;
              const matchCat = categoryFilter === 'all' ? true : s.fraudCategoryId === categoryFilter;
              const matchAudience =
                audienceFilter === 'all'
                  ? true
                  : (s.targetAudience || 'All').toLowerCase() === audienceFilter.toLowerCase();
              return matchSearch && matchStatus && matchCat && matchAudience;
            });

            if (filtered.length === 0) {
              return <EmptyState message="No scenarios match the selected filter or search term." />;
            }

            const paginated = filtered.slice((currentPage - 1) * pageSize, currentPage * pageSize);

            return (
              <>
                <div className="overflow-x-auto">
                  <table className="w-full text-left text-xs">
                    <thead>
                      <tr className="border-b border-gray-200 text-gray-400 font-semibold uppercase text-[10px] tracking-wider">
                        <th className="py-3 px-3">Title & Situation</th>
                        <th className="py-3 px-3">Audience & Level</th>
                        <th className="py-3 px-3">Correct Action</th>
                        <th className="py-3 px-3">Category</th>
                        <th className="py-3 px-3">Status</th>
                        <th className="py-3 px-3 text-right">Actions</th>
                      </tr>
                    </thead>
                    <tbody className="divide-y divide-gray-100">
                      {paginated.map((s) => {
                        const cat = categories.find((c) => c.id === s.fraudCategoryId);
                        return (
                          <tr key={s.id} className="hover:bg-gray-50/50">
                            <td className="py-3 px-3 max-w-xs">
                              <span className="font-semibold text-[#191919] block">{s.title}</span>
                              <span className="text-[11px] text-gray-500 line-clamp-1">{s.situation}</span>
                            </td>
                            <td className="py-3 px-3">
                              <div className="flex flex-col gap-1 items-start">
                                <AudienceBadge audience={s.targetAudience} />
                                <DifficultyBadge difficulty={s.difficultyLevel} />
                              </div>
                            </td>
                            <td className="py-3 px-3 max-w-xs">
                              <span className="text-[11px] text-emerald-800 line-clamp-2">{s.correctAction}</span>
                            </td>
                            <td className="py-3 px-3">
                              <span className="px-2 py-0.5 rounded bg-gray-100 text-gray-700 text-[10px] font-medium">
                                {cat?.name || 'General'}
                              </span>
                            </td>
                            <td className="py-3 px-3">
                              <StatusBadge isPublished={s.isPublished} />
                            </td>
                            <td className="py-3 px-3 text-right space-x-1.5 whitespace-nowrap">
                              <button
                                type="button"
                                onClick={() => handleOpenPreview('scenario', s)}
                                className="px-2.5 py-1 text-[11px] border border-gray-300 rounded hover:bg-white transition-colors"
                              >
                                Preview
                              </button>
                              <button
                                type="button"
                                onClick={() => {
                                  setEditingScenario(s);
                                  setShowScenarioModal(true);
                                }}
                                className="px-2.5 py-1 text-[11px] bg-gray-100 text-[#191919] rounded hover:bg-gray-200 transition-colors font-medium"
                              >
                                Edit
                              </button>
                              <button
                                type="button"
                                onClick={() => handleTogglePublishScenario(s)}
                                className={`px-2.5 py-1 text-[11px] rounded font-medium transition-colors ${
                                  s.isPublished
                                    ? 'bg-amber-100 text-amber-900 hover:bg-amber-200'
                                    : 'bg-emerald-100 text-emerald-900 hover:bg-emerald-200'
                                }`}
                              >
                                {s.isPublished ? 'Unpublish' : 'Publish'}
                              </button>
                              <button
                                type="button"
                                onClick={() => handleDeleteScenario(s)}
                                className="px-2.5 py-1 text-[11px] text-red-600 hover:text-red-800 transition-colors"
                              >
                                Delete
                              </button>
                            </td>
                          </tr>
                        );
                      })}
                    </tbody>
                  </table>
                </div>
                <Pagination
                  currentPage={currentPage}
                  totalCount={filtered.length}
                  pageSize={pageSize}
                  onPageChange={setCurrentPage}
                />
              </>
            );
          })()}
        </ContentSection>
      )}

      {/* TAB CONTENT 5: THREAT CATEGORIES */}
      {activeTab === 'categories' && (
        <div className="bg-white border border-gray-200 rounded-2xl p-6 sm:p-8 shadow-sm space-y-6">
          <div className="flex flex-col sm:flex-row sm:items-center justify-between pb-4 border-b border-gray-100 gap-3">
            <div>
              <h3 className="text-lg font-serif text-[#191919]">Fraud Threat Categories</h3>
              <p className="text-xs text-gray-500">
                Primary classification taxonomy linking learning modules, scenarios, and scam patterns.
              </p>
            </div>
            <button
              type="button"
              onClick={() => {
                setEditingCategory(null);
                setShowCategoryModal(true);
              }}
              className="px-4 py-2 bg-[#191919] text-white text-xs font-semibold rounded-full hover:bg-black/85 transition-colors shrink-0"
            >
              + Create Category
            </button>
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-4">
            {categories.map((cat) => (
              <div key={cat.id} className="p-5 bg-[#F4F3F3] rounded-xl border border-gray-200/80 space-y-3">
                <div className="flex items-start justify-between">
                  <div>
                    <span className="font-semibold text-sm text-[#191919] block">{cat.name}</span>
                    <span className="text-[10px] font-mono text-gray-400">ID #{cat.id}</span>
                  </div>
                  <span
                    className={`px-2 py-0.5 rounded-full text-[10px] font-semibold ${
                      cat.isActive ? 'bg-emerald-100 text-emerald-800' : 'bg-gray-200 text-gray-600'
                    }`}
                  >
                    {cat.isActive ? 'Active' : 'Archived'}
                  </span>
                </div>
                <p className="text-xs text-gray-600 leading-relaxed min-h-[36px]">
                  {cat.description || 'No description provided.'}
                </p>
                <div className="pt-2 border-t border-gray-200 flex justify-end gap-2">
                  <button
                    type="button"
                    onClick={() => {
                      setEditingCategory(cat);
                      setShowCategoryModal(true);
                    }}
                    className="text-xs px-2.5 py-1 bg-white border border-gray-300 rounded font-medium text-gray-700 hover:text-black"
                  >
                    Edit
                  </button>
                  <button
                    type="button"
                    onClick={() => handleDeleteCategory(cat)}
                    className="text-xs px-2.5 py-1 text-red-600 hover:text-red-800"
                  >
                    Delete
                  </button>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* TAB CONTENT 6: BADGES */}
      {activeTab === 'badges' && (
        <div className="bg-white border border-gray-200 rounded-2xl p-6 sm:p-8 shadow-sm space-y-6">
          <div className="flex flex-col sm:flex-row sm:items-center justify-between pb-4 border-b border-gray-100 gap-3">
            <div>
              <h3 className="text-lg font-serif text-[#191919]">Safety Badges & Milestones</h3>
              <p className="text-xs text-gray-500">
                Incentive rewards unlocked by learners as they complete modules and score well on quizzes.
              </p>
            </div>
            <button
              type="button"
              onClick={() => {
                setEditingBadge(null);
                setShowBadgeModal(true);
              }}
              className="px-4 py-2 bg-[#191919] text-white text-xs font-semibold rounded-full hover:bg-black/85 transition-colors shrink-0"
            >
              + Create Safety Badge
            </button>
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-4">
            {badges.map((b) => (
              <div key={b.id} className="p-5 bg-[#F4F3F3] rounded-xl border border-gray-200/80 space-y-3">
                <div className="flex items-start justify-between">
                  <div className="flex items-center gap-2">
                    <span className="text-2xl">{b.icon || '🛡️'}</span>
                    <div>
                      <span className="font-semibold text-sm text-[#191919] block">{b.name}</span>
                      <span className="text-[10px] text-gray-500">{b.requiredPoints} pts required</span>
                    </div>
                  </div>
                  <span
                    className={`px-2 py-0.5 rounded-full text-[10px] font-semibold ${
                      b.isActive ? 'bg-emerald-100 text-emerald-800' : 'bg-gray-200 text-gray-600'
                    }`}
                  >
                    {b.isActive ? 'Active' : 'Inactive'}
                  </span>
                </div>
                <p className="text-xs text-gray-600 leading-relaxed min-h-[36px]">
                  {b.description || 'No description provided.'}
                </p>
                <div className="pt-2 border-t border-gray-200 flex justify-end gap-2">
                  <button
                    type="button"
                    onClick={() => {
                      setEditingBadge(b);
                      setShowBadgeModal(true);
                    }}
                    className="text-xs px-2.5 py-1 bg-white border border-gray-300 rounded font-medium text-gray-700 hover:text-black"
                  >
                    Edit
                  </button>
                  <button
                    type="button"
                    onClick={() => handleDeleteBadge(b)}
                    className="text-xs px-2.5 py-1 text-red-600 hover:text-red-800"
                  >
                    Delete
                  </button>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* ------------------------------------------------------------- */}
      {/* MODAL: CREATE / EDIT MODULE */}
      {/* ------------------------------------------------------------- */}
      {showModuleModal && (
        <ModuleModal
          module={editingModule}
          categories={categories}
          isSubmitting={isSubmitting}
          onClose={() => {
            setShowModuleModal(false);
            setEditingModule(null);
          }}
          onSave={handleSaveModule}
        />
      )}

      {/* ------------------------------------------------------------- */}
      {/* MODAL: CREATE / EDIT QUIZ */}
      {/* ------------------------------------------------------------- */}
      {showQuizModal && (
        <QuizModal
          quiz={editingQuiz}
          categories={categories}
          isSubmitting={isSubmitting}
          onClose={() => {
            setShowQuizModal(false);
            setEditingQuiz(null);
          }}
          onSave={handleSaveQuiz}
        />
      )}

      {/* ------------------------------------------------------------- */}
      {/* MODAL: CREATE / EDIT SCENARIO */}
      {/* ------------------------------------------------------------- */}
      {showScenarioModal && (
        <ScenarioModal
          scenario={editingScenario}
          categories={categories}
          isSubmitting={isSubmitting}
          onClose={() => {
            setShowScenarioModal(false);
            setEditingScenario(null);
          }}
          onSave={handleSaveScenario}
        />
      )}

      {/* ------------------------------------------------------------- */}
      {/* MODAL: CREATE / EDIT CATEGORY */}
      {/* ------------------------------------------------------------- */}
      {showCategoryModal && (
        <CategoryModal
          category={editingCategory}
          isSubmitting={isSubmitting}
          onClose={() => {
            setShowCategoryModal(false);
            setEditingCategory(null);
          }}
          onSave={handleSaveCategory}
        />
      )}

      {/* ------------------------------------------------------------- */}
      {/* MODAL: CREATE / EDIT BADGE */}
      {/* ------------------------------------------------------------- */}
      {showBadgeModal && (
        <BadgeModal
          badge={editingBadge}
          isSubmitting={isSubmitting}
          onClose={() => {
            setShowBadgeModal(false);
            setEditingBadge(null);
          }}
          onSave={handleSaveBadge}
        />
      )}

      {/* ------------------------------------------------------------- */}
      {/* MODAL: PREVIEW AS LEARNER */}
      {/* ------------------------------------------------------------- */}
      {previewItem && (
        <PreviewModal
          item={previewItem}
          categories={categories}
          onClose={() => setPreviewItem(null)}
          onPublishModule={handleTogglePublishModule}
          onPublishQuiz={handleTogglePublishQuiz}
          onPublishScenario={handleTogglePublishScenario}
        />
      )}

      {/* ------------------------------------------------------------- */}
      {/* CONFIRMATION DIALOG */}
      {/* ------------------------------------------------------------- */}
      {confirmDialog && (
        <div className="fixed inset-0 z-50 bg-black/40 backdrop-blur-sm flex items-center justify-center p-4">
          <div className="bg-white rounded-2xl max-w-md w-full p-6 shadow-xl border border-gray-200 space-y-4">
            <h4 className="text-base font-serif font-bold text-[#191919]">{confirmDialog.title}</h4>
            <p className="text-xs text-gray-600 leading-relaxed">{confirmDialog.message}</p>
            <div className="flex justify-end gap-2 pt-2">
              <button
                type="button"
                onClick={() => setConfirmDialog(null)}
                className="px-4 py-2 text-xs font-medium text-gray-600 hover:text-black"
              >
                Cancel
              </button>
              <button
                type="button"
                onClick={confirmDialog.onConfirm}
                className="px-4 py-2 text-xs font-semibold bg-[#191919] text-white rounded-full hover:bg-black/85"
              >
                {confirmDialog.confirmLabel}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

// -----------------------------------------------------------------------------
// REUSABLE SUB-COMPONENTS
// -----------------------------------------------------------------------------

const StatusBadge: React.FC<{ isPublished: boolean }> = ({ isPublished }) => (
  <span
    className={`px-2 py-0.5 rounded-full text-[10px] font-semibold flex items-center gap-1 w-fit ${
      isPublished
        ? 'bg-emerald-100 text-emerald-800 border border-emerald-200'
        : 'bg-amber-100 text-amber-900 border border-amber-200'
    }`}
  >
    <span className={`w-1.5 h-1.5 rounded-full ${isPublished ? 'bg-emerald-600' : 'bg-amber-600'}`} />
    {isPublished ? 'Published' : 'Draft'}
  </span>
);

const AudienceBadge: React.FC<{ audience?: string }> = ({ audience }) => {
  const seg = (audience || 'All').toLowerCase();
  let color = 'bg-gray-100 text-gray-700 border-gray-200';
  let label = audience || 'All';

  if (seg.includes('early') || seg.includes('5')) {
    color = 'bg-emerald-50 text-emerald-800 border-emerald-200';
    label = 'Kids (5–7)';
  } else if (seg.includes('child') || seg.includes('8')) {
    color = 'bg-sky-50 text-sky-800 border-sky-200';
    label = 'Kids (8–12)';
  } else if (seg.includes('teen')) {
    color = 'bg-purple-50 text-purple-800 border-purple-200';
    label = 'Teens (13–17)';
  } else if (seg.includes('young') || seg.includes('student')) {
    color = 'bg-indigo-50 text-indigo-800 border-indigo-200';
    label = 'Students (18–24)';
  } else if (seg.includes('senior')) {
    color = 'bg-amber-50 text-amber-900 border-amber-200';
    label = 'Seniors (65+)';
  } else if (seg.includes('pro')) {
    color = 'bg-slate-100 text-slate-800 border-slate-300';
    label = 'Pros';
  } else if (seg.includes('adult')) {
    color = 'bg-blue-50 text-blue-800 border-blue-200';
    label = 'Adults';
  }

  return (
    <span className={`inline-flex items-center px-1.5 py-0.5 rounded text-[10px] font-semibold border ${color}`}>
      {label}
    </span>
  );
};

const DifficultyBadge: React.FC<{ difficulty?: string }> = ({ difficulty }) => {
  const diff = (difficulty || 'Beginner').toLowerCase();
  let color = 'bg-green-50 text-green-700';
  if (diff === 'intermediate') color = 'bg-blue-50 text-blue-700';
  if (diff === 'advanced') color = 'bg-purple-50 text-purple-700';

  return (
    <span className={`inline-flex items-center px-1.5 py-0.5 rounded text-[10px] font-medium ${color}`}>
      {difficulty || 'Beginner'}
    </span>
  );
};

const EmptyState: React.FC<{ message: string }> = ({ message }) => (
  <div className="py-12 text-center text-xs text-gray-400 bg-gray-50/50 rounded-xl border border-dashed border-gray-200">
    {message}
  </div>
);

const ContentSection: React.FC<{
  title: string;
  description: string;
  createButtonLabel: string;
  onCreate: () => void;
  searchTerm: string;
  onSearchChange: (v: string) => void;
  statusFilter: 'all' | 'published' | 'draft';
  onStatusFilterChange: (v: 'all' | 'published' | 'draft') => void;
  categories: FraudCategory[];
  categoryFilter: number | 'all';
  onCategoryFilterChange: (v: number | 'all') => void;
  audienceFilter?: string;
  onAudienceFilterChange?: (v: string) => void;
  children: React.ReactNode;
}> = ({
  title,
  description,
  createButtonLabel,
  onCreate,
  searchTerm,
  onSearchChange,
  statusFilter,
  onStatusFilterChange,
  categories,
  categoryFilter,
  onCategoryFilterChange,
  audienceFilter,
  onAudienceFilterChange,
  children,
}) => (
  <div className="bg-white border border-gray-200 rounded-2xl p-6 sm:p-8 shadow-sm space-y-6">
    <div className="flex flex-col sm:flex-row sm:items-center justify-between pb-4 border-b border-gray-100 gap-3">
      <div>
        <h3 className="text-lg font-serif text-[#191919]">{title}</h3>
        <p className="text-xs text-gray-500">{description}</p>
      </div>
      <button
        type="button"
        onClick={onCreate}
        className="px-4 py-2 bg-[#191919] text-white text-xs font-semibold rounded-full hover:bg-black/85 transition-colors shrink-0"
      >
        {createButtonLabel}
      </button>
    </div>

    {/* Filters Row */}
    <div className="grid grid-cols-1 sm:grid-cols-12 gap-3 items-center">
      <div className={onAudienceFilterChange ? 'sm:col-span-4' : 'sm:col-span-5'}>
        <input
          type="text"
          value={searchTerm}
          onChange={(e) => onSearchChange(e.target.value)}
          placeholder="Search by title, description, or keyword..."
          className="w-full text-xs p-2.5 bg-gray-50 border border-gray-200 rounded-lg focus:bg-white focus:outline-none focus:ring-1 focus:ring-black"
        />
      </div>
      <div className={onAudienceFilterChange ? 'sm:col-span-2' : 'sm:col-span-3'}>
        <select
          value={statusFilter}
          onChange={(e) => onStatusFilterChange(e.target.value as any)}
          className="w-full text-xs p-2.5 bg-gray-50 border border-gray-200 rounded-lg focus:bg-white"
        >
          <option value="all">All Statuses</option>
          <option value="published">Published</option>
          <option value="draft">Drafts</option>
        </select>
      </div>
      <div className={onAudienceFilterChange ? 'sm:col-span-3' : 'sm:col-span-4'}>
        <select
          value={categoryFilter}
          onChange={(e) =>
            onCategoryFilterChange(e.target.value === 'all' ? 'all' : parseInt(e.target.value, 10))
          }
          className="w-full text-xs p-2.5 bg-gray-50 border border-gray-200 rounded-lg focus:bg-white"
        >
          <option value="all">All Categories</option>
          {categories.map((c) => (
            <option key={c.id} value={c.id}>
              {c.name}
            </option>
          ))}
        </select>
      </div>
      {onAudienceFilterChange && (
        <div className="sm:col-span-3">
          <select
            value={audienceFilter || 'all'}
            onChange={(e) => onAudienceFilterChange(e.target.value)}
            className="w-full text-xs p-2.5 bg-gray-50 border border-gray-200 rounded-lg focus:bg-white font-medium"
          >
            <option value="all">All Audiences</option>
            {AUDIENCE_SEGMENTS.map((seg) => (
              <option key={seg.id} value={seg.id}>
                {seg.label} ({seg.ageRange})
              </option>
            ))}
          </select>
        </div>
      )}
    </div>

    {children}
  </div>
);

const Pagination: React.FC<{
  currentPage: number;
  totalCount: number;
  pageSize: number;
  onPageChange: (p: number) => void;
}> = ({ currentPage, totalCount, pageSize, onPageChange }) => {
  const totalPages = Math.ceil(totalCount / pageSize);
  if (totalPages <= 1) return null;

  return (
    <div className="flex items-center justify-between pt-4 border-t border-gray-100 text-xs text-gray-500">
      <span>
        Showing {(currentPage - 1) * pageSize + 1} to {Math.min(currentPage * pageSize, totalCount)} of {totalCount} items
      </span>
      <div className="flex items-center gap-1">
        <button
          type="button"
          disabled={currentPage === 1}
          onClick={() => onPageChange(currentPage - 1)}
          className="px-3 py-1 border border-gray-200 rounded hover:bg-gray-50 disabled:opacity-40"
        >
          Previous
        </button>
        <span className="px-3 py-1 font-mono">
          {currentPage} / {totalPages}
        </span>
        <button
          type="button"
          disabled={currentPage === totalPages}
          onClick={() => onPageChange(currentPage + 1)}
          className="px-3 py-1 border border-gray-200 rounded hover:bg-gray-50 disabled:opacity-40"
        >
          Next
        </button>
      </div>
    </div>
  );
};

// -----------------------------------------------------------------------------
// MODULE FORM MODAL
// -----------------------------------------------------------------------------
const ModuleModal: React.FC<{
  module: LearningModule | null;
  categories: FraudCategory[];
  isSubmitting: boolean;
  onClose: () => void;
  onSave: (e: React.FormEvent, form: CreateLearningModuleRequest, editId?: number) => Promise<void>;
}> = ({ module, categories, isSubmitting, onClose, onSave }) => {
  const [title, setTitle] = useState(module?.title || '');
  const [description, setDescription] = useState(module?.description || '');
  const [content, setContent] = useState(module?.content || '');
  const [order, setOrder] = useState<number>(module?.order ?? 1);
  const [isPublished, setIsPublished] = useState<boolean>(module?.isPublished ?? false);
  const [categoryId, setCategoryId] = useState<number | undefined>(module?.fraudCategoryId);
  const [targetAudience, setTargetAudience] = useState<string>(module?.targetAudience || 'All');
  const [difficultyLevel, setDifficultyLevel] = useState<string>(module?.difficultyLevel || 'Beginner');
  const [estimatedMinutes, setEstimatedMinutes] = useState<number>(module?.estimatedMinutes ?? 5);
  const [learningObjectives, setLearningObjectives] = useState<string>(module?.learningObjectives || '');
  const [safetyTips, setSafetyTips] = useState<string>(module?.safetyTips || '');

  return (
    <div className="fixed inset-0 z-50 bg-black/40 backdrop-blur-sm flex items-center justify-center p-4 overflow-y-auto">
      <div className="bg-white rounded-2xl max-w-2xl w-full p-6 sm:p-8 shadow-xl border border-gray-200 space-y-6 my-8">
        <div className="flex items-center justify-between border-b border-gray-100 pb-4">
          <h3 className="text-lg font-serif text-[#191919]">
            {module ? `Edit Learning Module #${module.id}` : 'Create Learning Module'}
          </h3>
          <button type="button" onClick={onClose} className="text-gray-400 hover:text-black font-bold">
            &times;
          </button>
        </div>

        <form
          onSubmit={(e) =>
            onSave(
              e,
              {
                title: title.trim(),
                description: description.trim() || undefined,
                content: content.trim(),
                order,
                isPublished,
                fraudCategoryId: categoryId,
                targetAudience,
                difficultyLevel,
                estimatedMinutes,
                learningObjectives: learningObjectives.trim() || undefined,
                safetyTips: safetyTips.trim() || undefined,
              },
              module?.id
            )
          }
          className="space-y-4"
        >
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div>
              <label className="block text-[11px] font-semibold uppercase text-gray-500 mb-1">Module Title *</label>
              <input
                type="text"
                required
                value={title}
                onChange={(e) => setTitle(e.target.value)}
                placeholder="e.g. Recognizing Social Engineering"
                className="w-full text-xs p-3 border border-gray-200 rounded-lg"
              />
            </div>
            <div>
              <label className="block text-[11px] font-semibold uppercase text-gray-500 mb-1">Threat Category</label>
              <select
                value={categoryId ?? ''}
                onChange={(e) => setCategoryId(e.target.value ? parseInt(e.target.value, 10) : undefined)}
                className="w-full text-xs p-3 border border-gray-200 rounded-lg"
              >
                <option value="">No Specific Category</option>
                {categories.map((c) => (
                  <option key={c.id} value={c.id}>
                    {c.name}
                  </option>
                ))}
              </select>
            </div>
          </div>

          {/* Age-Adaptive & Level Settings */}
          <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
            <div>
              <label className="block text-[11px] font-semibold uppercase text-gray-500 mb-1">Target Audience</label>
              <select
                value={targetAudience}
                onChange={(e) => setTargetAudience(e.target.value)}
                className="w-full text-xs p-3 border border-gray-200 rounded-lg font-medium"
              >
                <option value="All">All Audiences (Universal)</option>
                <option value="EarlyLearners">Early Learners (Ages 5–7)</option>
                <option value="Children">Children (Ages 8–12)</option>
                <option value="Teens">Teenagers (Ages 13–17)</option>
                <option value="YoungAdults">Students (Ages 18–24)</option>
                <option value="Adults">Everyday Adults</option>
                <option value="Seniors">Senior Citizens (65+)</option>
                <option value="Professionals">Workplace & Professionals</option>
              </select>
            </div>
            <div>
              <label className="block text-[11px] font-semibold uppercase text-gray-500 mb-1">Difficulty Level</label>
              <select
                value={difficultyLevel}
                onChange={(e) => setDifficultyLevel(e.target.value)}
                className="w-full text-xs p-3 border border-gray-200 rounded-lg"
              >
                <option value="Beginner">Beginner</option>
                <option value="Intermediate">Intermediate</option>
                <option value="Advanced">Advanced</option>
              </select>
            </div>
            <div>
              <label className="block text-[11px] font-semibold uppercase text-gray-500 mb-1">Estimated Reading (Mins)</label>
              <input
                type="number"
                min={1}
                max={120}
                value={estimatedMinutes}
                onChange={(e) => setEstimatedMinutes(parseInt(e.target.value, 10) || 5)}
                className="w-full text-xs p-3 border border-gray-200 rounded-lg"
              />
            </div>
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div>
              <label className="block text-[11px] font-semibold uppercase text-gray-500 mb-1">Display Order</label>
              <input
                type="number"
                min={0}
                value={order}
                onChange={(e) => setOrder(parseInt(e.target.value, 10) || 0)}
                className="w-full text-xs p-3 border border-gray-200 rounded-lg"
              />
            </div>
            <div className="flex items-center gap-2 pt-6">
              <input
                id="mod-pub"
                type="checkbox"
                checked={isPublished}
                onChange={(e) => setIsPublished(e.target.checked)}
                className="rounded border-gray-300 text-black focus:ring-black"
              />
              <label htmlFor="mod-pub" className="text-xs text-gray-700 cursor-pointer font-medium">
                Publish immediately to learners
              </label>
            </div>
          </div>

          <div>
            <label className="block text-[11px] font-semibold uppercase text-gray-500 mb-1">Short Description</label>
            <input
              type="text"
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              placeholder="Summary of digital habit and key lesson takeaway..."
              className="w-full text-xs p-3 border border-gray-200 rounded-lg"
            />
          </div>

          <div>
            <label className="block text-[11px] font-semibold uppercase text-gray-500 mb-1">Lesson Content *</label>
            <textarea
              rows={6}
              required
              value={content}
              onChange={(e) => setContent(e.target.value)}
              placeholder="Detailed educational text, safe steps, and concrete examples..."
              className="w-full text-xs p-3 border border-gray-200 rounded-lg font-sans"
            />
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div>
              <label className="block text-[11px] font-semibold uppercase text-gray-500 mb-1">Learning Objectives</label>
              <textarea
                rows={3}
                value={learningObjectives}
                onChange={(e) => setLearningObjectives(e.target.value)}
                placeholder="Key goals (e.g. Spot urgent tone; Verify before clicking)..."
                className="w-full text-xs p-3 border border-gray-200 rounded-lg font-sans"
              />
            </div>
            <div>
              <label className="block text-[11px] font-semibold uppercase text-gray-500 mb-1">Safety Checklist / Tips</label>
              <textarea
                rows={3}
                value={safetyTips}
                onChange={(e) => setSafetyTips(e.target.value)}
                placeholder="Actionable safe habits (e.g. Ask a trusted adult; Never share OTPs)..."
                className="w-full text-xs p-3 border border-gray-200 rounded-lg font-sans"
              />
            </div>
          </div>

          <div className="flex justify-end gap-2 pt-4 border-t border-gray-100">
            <button type="button" onClick={onClose} className="px-4 py-2 text-xs font-medium text-gray-600 hover:text-black">
              Cancel
            </button>
            <button
              type="submit"
              disabled={isSubmitting}
              className="px-5 py-2 text-xs font-semibold bg-[#191919] text-white rounded-full hover:bg-black/85 disabled:opacity-50"
            >
              {isSubmitting ? 'Saving...' : module ? 'Update Module' : 'Create Module'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

// -----------------------------------------------------------------------------
// QUIZ FORM MODAL (WITH QUESTIONS & CHOICES BUILDER)
// -----------------------------------------------------------------------------
const QuizModal: React.FC<{
  quiz: QuizManagementDetail | null;
  categories: FraudCategory[];
  isSubmitting: boolean;
  onClose: () => void;
  onSave: (e: React.FormEvent, form: CreateQuizRequest, editId?: number) => Promise<void>;
}> = ({ quiz, categories, isSubmitting, onClose, onSave }) => {
  const [title, setTitle] = useState(quiz?.title || '');
  const [description, setDescription] = useState(quiz?.description || '');
  const [isPublished, setIsPublished] = useState<boolean>(quiz?.isPublished ?? false);
  const [categoryId, setCategoryId] = useState<number | undefined>(quiz?.fraudCategoryId);
  const [targetAudience, setTargetAudience] = useState<string>(quiz?.targetAudience || 'All');
  const [difficultyLevel, setDifficultyLevel] = useState<string>(quiz?.difficultyLevel || 'Beginner');

  // Dynamic Question & Option List
  const [questions, setQuestions] = useState<CreateQuizQuestionRequest[]>(
    quiz?.questions?.map((q) => ({
      questionText: q.questionText,
      explanation: q.explanation || '',
      order: q.order,
      options: q.options.map((o) => ({
        optionText: o.optionText,
        isCorrect: o.isCorrect,
        order: o.order,
      })),
    })) || [
      {
        questionText: '',
        explanation: '',
        order: 1,
        options: [
          { optionText: '', isCorrect: true, order: 1 },
          { optionText: '', isCorrect: false, order: 2 },
        ],
      },
    ]
  );

  const handleAddQuestion = () => {
    setQuestions((prev) => [
      ...prev,
      {
        questionText: '',
        explanation: '',
        order: prev.length + 1,
        options: [
          { optionText: '', isCorrect: true, order: 1 },
          { optionText: '', isCorrect: false, order: 2 },
        ],
      },
    ]);
  };

  const handleRemoveQuestion = (idx: number) => {
    setQuestions((prev) => prev.filter((_, i) => i !== idx));
  };

  const handleAddOption = (qIdx: number) => {
    setQuestions((prev) =>
      prev.map((q, i) =>
        i === qIdx
          ? {
              ...q,
              options: [
                ...q.options,
                { optionText: '', isCorrect: false, order: q.options.length + 1 },
              ],
            }
          : q
      )
    );
  };

  const handleRemoveOption = (qIdx: number, oIdx: number) => {
    setQuestions((prev) =>
      prev.map((q, i) =>
        i === qIdx ? { ...q, options: q.options.filter((_, oi) => oi !== oIdx) } : q
      )
    );
  };

  const handleSetCorrectOption = (qIdx: number, oIdx: number) => {
    setQuestions((prev) =>
      prev.map((q, i) =>
        i === qIdx
          ? {
              ...q,
              options: q.options.map((opt, oi) => ({
                ...opt,
                isCorrect: oi === oIdx,
              })),
            }
          : q
      )
    );
  };

  return (
    <div className="fixed inset-0 z-50 bg-black/40 backdrop-blur-sm flex items-center justify-center p-4 overflow-y-auto">
      <div className="bg-white rounded-2xl max-w-3xl w-full p-6 sm:p-8 shadow-xl border border-gray-200 space-y-6 my-8 max-h-[90vh] overflow-y-auto">
        <div className="flex items-center justify-between border-b border-gray-100 pb-4">
          <h3 className="text-lg font-serif text-[#191919]">
            {quiz ? `Edit Quiz #${quiz.id}` : 'Create New Assessment Quiz'}
          </h3>
          <button type="button" onClick={onClose} className="text-gray-400 hover:text-black font-bold">
            &times;
          </button>
        </div>

        <form
          onSubmit={(e) =>
            onSave(
              e,
              {
                title: title.trim(),
                description: description.trim() || undefined,
                isPublished,
                fraudCategoryId: categoryId,
                targetAudience,
                difficultyLevel,
                questions,
              },
              quiz?.id
            )
          }
          className="space-y-6"
        >
          {/* Metadata */}
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div>
              <label className="block text-[11px] font-semibold uppercase text-gray-500 mb-1">Quiz Title *</label>
              <input
                type="text"
                required
                value={title}
                onChange={(e) => setTitle(e.target.value)}
                placeholder="e.g. Phishing Identification Challenge"
                className="w-full text-xs p-3 border border-gray-200 rounded-lg"
              />
            </div>
            <div>
              <label className="block text-[11px] font-semibold uppercase text-gray-500 mb-1">Category</label>
              <select
                value={categoryId ?? ''}
                onChange={(e) => setCategoryId(e.target.value ? parseInt(e.target.value, 10) : undefined)}
                className="w-full text-xs p-3 border border-gray-200 rounded-lg"
              >
                <option value="">No Specific Category</option>
                {categories.map((c) => (
                  <option key={c.id} value={c.id}>
                    {c.name}
                  </option>
                ))}
              </select>
            </div>
          </div>

          {/* Age-Adaptive & Difficulty */}
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div>
              <label className="block text-[11px] font-semibold uppercase text-gray-500 mb-1">Target Audience</label>
              <select
                value={targetAudience}
                onChange={(e) => setTargetAudience(e.target.value)}
                className="w-full text-xs p-3 border border-gray-200 rounded-lg font-medium"
              >
                <option value="All">All Audiences (Universal)</option>
                <option value="EarlyLearners">Early Learners (Ages 5–7)</option>
                <option value="Children">Children (Ages 8–12)</option>
                <option value="Teens">Teenagers (Ages 13–17)</option>
                <option value="YoungAdults">Students (Ages 18–24)</option>
                <option value="Adults">Everyday Adults</option>
                <option value="Seniors">Senior Citizens (65+)</option>
                <option value="Professionals">Workplace & Professionals</option>
              </select>
            </div>
            <div>
              <label className="block text-[11px] font-semibold uppercase text-gray-500 mb-1">Difficulty Level</label>
              <select
                value={difficultyLevel}
                onChange={(e) => setDifficultyLevel(e.target.value)}
                className="w-full text-xs p-3 border border-gray-200 rounded-lg"
              >
                <option value="Beginner">Beginner</option>
                <option value="Intermediate">Intermediate</option>
                <option value="Advanced">Advanced</option>
              </select>
            </div>
          </div>

          <div className="flex items-center justify-between">
            <div className="w-2/3">
              <label className="block text-[11px] font-semibold uppercase text-gray-500 mb-1">Description</label>
              <input
                type="text"
                value={description}
                onChange={(e) => setDescription(e.target.value)}
                placeholder="Test your knowledge on spotting fraudulent messages..."
                className="w-full text-xs p-3 border border-gray-200 rounded-lg"
              />
            </div>
            <div className="flex items-center gap-2 pt-4">
              <input
                id="quiz-pub"
                type="checkbox"
                checked={isPublished}
                onChange={(e) => setIsPublished(e.target.checked)}
                className="rounded border-gray-300 text-black focus:ring-black"
              />
              <label htmlFor="quiz-pub" className="text-xs text-gray-700 cursor-pointer font-medium">
                Publish immediately
              </label>
            </div>
          </div>

          {/* Questions Builder */}
          <div className="space-y-4 pt-4 border-t border-gray-100">
            <div className="flex items-center justify-between">
              <h4 className="text-xs uppercase font-bold tracking-wider text-[#191919]">
                Questions & Answer Options ({questions.length})
              </h4>
              <button
                type="button"
                onClick={handleAddQuestion}
                className="text-xs px-3 py-1.5 bg-gray-100 text-[#191919] rounded-lg hover:bg-gray-200 font-medium"
              >
                + Add Question
              </button>
            </div>

            {questions.map((q, qIdx) => (
              <div key={qIdx} className="p-4 bg-[#F4F3F3] rounded-xl border border-gray-200 space-y-3">
                <div className="flex items-center justify-between">
                  <span className="font-semibold text-xs text-[#191919]">Question #{qIdx + 1}</span>
                  {questions.length > 1 && (
                    <button
                      type="button"
                      onClick={() => handleRemoveQuestion(qIdx)}
                      className="text-[11px] text-red-600 hover:text-red-800"
                    >
                      Remove Question
                    </button>
                  )}
                </div>

                <input
                  type="text"
                  required
                  value={q.questionText}
                  onChange={(e) =>
                    setQuestions((prev) =>
                      prev.map((item, i) => (i === qIdx ? { ...item, questionText: e.target.value } : item))
                    )
                  }
                  placeholder="Enter the question text..."
                  className="w-full text-xs p-2.5 bg-white border border-gray-200 rounded-lg"
                />

                <input
                  type="text"
                  value={q.explanation || ''}
                  onChange={(e) =>
                    setQuestions((prev) =>
                      prev.map((item, i) => (i === qIdx ? { ...item, explanation: e.target.value } : item))
                    )
                  }
                  placeholder="Explanation shown after user answers (optional)..."
                  className="w-full text-xs p-2.5 bg-white border border-gray-200 rounded-lg text-gray-600"
                />

                {/* Options */}
                <div className="space-y-2 pt-2">
                  <span className="text-[10px] uppercase font-bold text-gray-400 block tracking-wider">
                    Answer Options (Select the correct answer):
                  </span>
                  {q.options.map((opt, oIdx) => (
                    <div key={oIdx} className="flex items-center gap-2">
                      <input
                        type="radio"
                        name={`correct-opt-${qIdx}`}
                        checked={opt.isCorrect}
                        onChange={() => handleSetCorrectOption(qIdx, oIdx)}
                        className="text-emerald-600 focus:ring-emerald-500"
                      />
                      <input
                        type="text"
                        required
                        value={opt.optionText}
                        onChange={(e) =>
                          setQuestions((prev) =>
                            prev.map((item, i) =>
                              i === qIdx
                                ? {
                                    ...item,
                                    options: item.options.map((o, oi) =>
                                      oi === oIdx ? { ...o, optionText: e.target.value } : o
                                    ),
                                  }
                                : item
                            )
                          )
                        }
                        placeholder={`Option ${oIdx + 1}`}
                        className={`w-full text-xs p-2 rounded-lg border ${
                          opt.isCorrect ? 'border-emerald-300 bg-emerald-50/50' : 'border-gray-200 bg-white'
                        }`}
                      />
                      {q.options.length > 2 && (
                        <button
                          type="button"
                          onClick={() => handleRemoveOption(qIdx, oIdx)}
                          className="text-gray-400 hover:text-red-600 font-bold px-1"
                        >
                          &times;
                        </button>
                      )}
                    </div>
                  ))}
                  <button
                    type="button"
                    onClick={() => handleAddOption(qIdx)}
                    className="text-[11px] text-gray-500 hover:text-black mt-1 font-medium"
                  >
                    + Add Another Choice
                  </button>
                </div>
              </div>
            ))}
          </div>

          <div className="flex justify-end gap-2 pt-4 border-t border-gray-100">
            <button type="button" onClick={onClose} className="px-4 py-2 text-xs font-medium text-gray-600 hover:text-black">
              Cancel
            </button>
            <button
              type="submit"
              disabled={isSubmitting}
              className="px-5 py-2 text-xs font-semibold bg-[#191919] text-white rounded-full hover:bg-black/85 disabled:opacity-50"
            >
              {isSubmitting ? 'Saving...' : quiz ? 'Update Quiz' : 'Create Quiz'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

// -----------------------------------------------------------------------------
// SCENARIO FORM MODAL
// -----------------------------------------------------------------------------
const ScenarioModal: React.FC<{
  scenario: Scenario | null;
  categories: FraudCategory[];
  isSubmitting: boolean;
  onClose: () => void;
  onSave: (e: React.FormEvent, form: CreateScenarioRequest, editId?: number) => Promise<void>;
}> = ({ scenario, categories, isSubmitting, onClose, onSave }) => {
  const [title, setTitle] = useState(scenario?.title || '');
  const [description, setDescription] = useState(scenario?.description || '');
  const [situation, setSituation] = useState(scenario?.situation || '');
  const [correctAction, setCorrectAction] = useState(scenario?.correctAction || '');
  const [isPublished, setIsPublished] = useState<boolean>(scenario?.isPublished ?? false);
  const [categoryId, setCategoryId] = useState<number | undefined>(scenario?.fraudCategoryId);
  const [targetAudience, setTargetAudience] = useState<string>(scenario?.targetAudience || 'All');
  const [difficultyLevel, setDifficultyLevel] = useState<string>(scenario?.difficultyLevel || 'Beginner');
  const [safetyTips, setSafetyTips] = useState<string>(scenario?.safetyTips || '');

  return (
    <div className="fixed inset-0 z-50 bg-black/40 backdrop-blur-sm flex items-center justify-center p-4 overflow-y-auto">
      <div className="bg-white rounded-2xl max-w-2xl w-full p-6 sm:p-8 shadow-xl border border-gray-200 space-y-6 my-8">
        <div className="flex items-center justify-between border-b border-gray-100 pb-4">
          <h3 className="text-lg font-serif text-[#191919]">
            {scenario ? `Edit Scenario #${scenario.id}` : 'Create Security Scenario'}
          </h3>
          <button type="button" onClick={onClose} className="text-gray-400 hover:text-black font-bold">
            &times;
          </button>
        </div>

        <form
          onSubmit={(e) =>
            onSave(
              e,
              {
                title: title.trim(),
                description: description.trim() || undefined,
                situation: situation.trim(),
                correctAction: correctAction.trim(),
                isPublished,
                fraudCategoryId: categoryId,
                targetAudience,
                difficultyLevel,
                safetyTips: safetyTips.trim() || undefined,
              },
              scenario?.id
            )
          }
          className="space-y-4"
        >
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div>
              <label className="block text-[11px] font-semibold uppercase text-gray-500 mb-1">Scenario Title *</label>
              <input
                type="text"
                required
                value={title}
                onChange={(e) => setTitle(e.target.value)}
                placeholder="e.g. Urgent Courier Redelivery SMS"
                className="w-full text-xs p-3 border border-gray-200 rounded-lg"
              />
            </div>
            <div>
              <label className="block text-[11px] font-semibold uppercase text-gray-500 mb-1">Threat Category</label>
              <select
                value={categoryId ?? ''}
                onChange={(e) => setCategoryId(e.target.value ? parseInt(e.target.value, 10) : undefined)}
                className="w-full text-xs p-3 border border-gray-200 rounded-lg"
              >
                <option value="">No Specific Category</option>
                {categories.map((c) => (
                  <option key={c.id} value={c.id}>
                    {c.name}
                  </option>
                ))}
              </select>
            </div>
          </div>

          {/* Age-Adaptive & Level Settings */}
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div>
              <label className="block text-[11px] font-semibold uppercase text-gray-500 mb-1">Target Audience</label>
              <select
                value={targetAudience}
                onChange={(e) => setTargetAudience(e.target.value)}
                className="w-full text-xs p-3 border border-gray-200 rounded-lg font-medium"
              >
                <option value="All">All Audiences (Universal)</option>
                <option value="EarlyLearners">Early Learners (Ages 5–7)</option>
                <option value="Children">Children (Ages 8–12)</option>
                <option value="Teens">Teenagers (Ages 13–17)</option>
                <option value="YoungAdults">Students (Ages 18–24)</option>
                <option value="Adults">Everyday Adults</option>
                <option value="Seniors">Senior Citizens (65+)</option>
                <option value="Professionals">Workplace & Professionals</option>
              </select>
            </div>
            <div>
              <label className="block text-[11px] font-semibold uppercase text-gray-500 mb-1">Difficulty Level</label>
              <select
                value={difficultyLevel}
                onChange={(e) => setDifficultyLevel(e.target.value)}
                className="w-full text-xs p-3 border border-gray-200 rounded-lg"
              >
                <option value="Beginner">Beginner</option>
                <option value="Intermediate">Intermediate</option>
                <option value="Advanced">Advanced</option>
              </select>
            </div>
          </div>

          <div className="flex items-center gap-2">
            <input
              id="scen-pub"
              type="checkbox"
              checked={isPublished}
              onChange={(e) => setIsPublished(e.target.checked)}
              className="rounded border-gray-300 text-black focus:ring-black"
            />
            <label htmlFor="scen-pub" className="text-xs text-gray-700 cursor-pointer font-medium">
              Publish immediately to learners
            </label>
          </div>

          <div>
            <label className="block text-[11px] font-semibold uppercase text-gray-500 mb-1">Context / Background</label>
            <input
              type="text"
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              placeholder="Short premise of the incoming contact..."
              className="w-full text-xs p-3 border border-gray-200 rounded-lg"
            />
          </div>

          <div>
            <label className="block text-[11px] font-semibold uppercase text-gray-500 mb-1">
              Realistic Situation / Attack Prompt *
            </label>
            <textarea
              rows={4}
              required
              value={situation}
              onChange={(e) => setSituation(e.target.value)}
              placeholder="Describe the suspicious message or scenario the learner encounters..."
              className="w-full text-xs p-3 border border-gray-200 rounded-lg font-sans"
            />
          </div>

          <div>
            <label className="block text-[11px] font-semibold uppercase text-gray-500 mb-1">
              Correct / Recommended Action *
            </label>
            <textarea
              rows={3}
              required
              value={correctAction}
              onChange={(e) => setCorrectAction(e.target.value)}
              placeholder="Explain the safest habit: e.g. Never click the link; check official tracking directly..."
              className="w-full text-xs p-3 border border-gray-200 rounded-lg font-sans text-emerald-950 bg-emerald-50/30"
            />
          </div>

          <div>
            <label className="block text-[11px] font-semibold uppercase text-gray-500 mb-1">Safety Checklist / Tips</label>
            <textarea
              rows={2}
              value={safetyTips}
              onChange={(e) => setSafetyTips(e.target.value)}
              placeholder="Recommended defensive actions (e.g. Call official bank number; Don't press redelivery links)..."
              className="w-full text-xs p-3 border border-gray-200 rounded-lg font-sans"
            />
          </div>

          <div className="flex justify-end gap-2 pt-4 border-t border-gray-100">
            <button type="button" onClick={onClose} className="px-4 py-2 text-xs font-medium text-gray-600 hover:text-black">
              Cancel
            </button>
            <button
              type="submit"
              disabled={isSubmitting}
              className="px-5 py-2 text-xs font-semibold bg-[#191919] text-white rounded-full hover:bg-black/85 disabled:opacity-50"
            >
              {isSubmitting ? 'Saving...' : scenario ? 'Update Scenario' : 'Create Scenario'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

// -----------------------------------------------------------------------------
// CATEGORY MODAL
// -----------------------------------------------------------------------------
const CategoryModal: React.FC<{
  category: FraudCategory | null;
  isSubmitting: boolean;
  onClose: () => void;
  onSave: (e: React.FormEvent, form: CreateFraudCategoryRequest, editId?: number, isActive?: boolean) => Promise<void>;
}> = ({ category, isSubmitting, onClose, onSave }) => {
  const [name, setName] = useState(category?.name || '');
  const [description, setDescription] = useState(category?.description || '');
  const [isActive, setIsActive] = useState<boolean>(category?.isActive ?? true);

  return (
    <div className="fixed inset-0 z-50 bg-black/40 backdrop-blur-sm flex items-center justify-center p-4">
      <div className="bg-white rounded-2xl max-w-md w-full p-6 shadow-xl border border-gray-200 space-y-4">
        <div className="flex items-center justify-between border-b border-gray-100 pb-3">
          <h3 className="text-base font-serif text-[#191919]">
            {category ? `Edit Category #${category.id}` : 'Create Threat Category'}
          </h3>
          <button type="button" onClick={onClose} className="text-gray-400 hover:text-black font-bold">
            &times;
          </button>
        </div>

        <form
          onSubmit={(e) =>
            onSave(
              e,
              { name: name.trim(), description: description.trim() || undefined, isActive },
              category?.id,
              isActive
            )
          }
          className="space-y-4"
        >
          <div>
            <label className="block text-[11px] font-semibold uppercase text-gray-500 mb-1">Category Name *</label>
            <input
              type="text"
              required
              value={name}
              onChange={(e) => setName(e.target.value)}
              placeholder="e.g. Identity & Document Theft"
              className="w-full text-xs p-3 border border-gray-200 rounded-lg"
            />
          </div>

          <div>
            <label className="block text-[11px] font-semibold uppercase text-gray-500 mb-1">Description</label>
            <textarea
              rows={3}
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              placeholder="Brief explanation of this fraud vector..."
              className="w-full text-xs p-3 border border-gray-200 rounded-lg"
            />
          </div>

          {category && (
            <div className="flex items-center gap-2">
              <input
                id="cat-act"
                type="checkbox"
                checked={isActive}
                onChange={(e) => setIsActive(e.target.checked)}
                className="rounded border-gray-300 text-black focus:ring-black"
              />
              <label htmlFor="cat-act" className="text-xs text-gray-700 cursor-pointer font-medium">
                Category is actively visible in curricula
              </label>
            </div>
          )}

          <div className="flex justify-end gap-2 pt-2">
            <button type="button" onClick={onClose} className="px-4 py-2 text-xs font-medium text-gray-600 hover:text-black">
              Cancel
            </button>
            <button
              type="submit"
              disabled={isSubmitting}
              className="px-5 py-2 text-xs font-semibold bg-[#191919] text-white rounded-full hover:bg-black/85 disabled:opacity-50"
            >
              {isSubmitting ? 'Saving...' : category ? 'Update Category' : 'Create Category'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

// -----------------------------------------------------------------------------
// BADGE MODAL
// -----------------------------------------------------------------------------
const BadgeModal: React.FC<{
  badge: Badge | null;
  isSubmitting: boolean;
  onClose: () => void;
  onSave: (e: React.FormEvent, form: CreateBadgeRequest, editId?: number) => Promise<void>;
}> = ({ badge, isSubmitting, onClose, onSave }) => {
  const [name, setName] = useState(badge?.name || '');
  const [description, setDescription] = useState(badge?.description || '');
  const [icon, setIcon] = useState(badge?.icon || '🛡️');
  const [requiredPoints, setRequiredPoints] = useState<number>(badge?.requiredPoints ?? 100);
  const [isActive, setIsActive] = useState<boolean>(badge?.isActive ?? true);

  return (
    <div className="fixed inset-0 z-50 bg-black/40 backdrop-blur-sm flex items-center justify-center p-4">
      <div className="bg-white rounded-2xl max-w-md w-full p-6 shadow-xl border border-gray-200 space-y-4">
        <div className="flex items-center justify-between border-b border-gray-100 pb-3">
          <h3 className="text-base font-serif text-[#191919]">
            {badge ? `Edit Safety Badge #${badge.id}` : 'Create Safety Badge'}
          </h3>
          <button type="button" onClick={onClose} className="text-gray-400 hover:text-black font-bold">
            &times;
          </button>
        </div>

        <form
          onSubmit={(e) =>
            onSave(
              e,
              {
                name: name.trim(),
                description: description.trim() || undefined,
                icon: icon.trim() || undefined,
                requiredPoints,
                isActive,
              },
              badge?.id
            )
          }
          className="space-y-4"
        >
          <div className="grid grid-cols-4 gap-3">
            <div className="col-span-1">
              <label className="block text-[11px] font-semibold uppercase text-gray-500 mb-1">Icon</label>
              <input
                type="text"
                value={icon}
                onChange={(e) => setIcon(e.target.value)}
                className="w-full text-center text-lg p-2.5 border border-gray-200 rounded-lg"
              />
            </div>
            <div className="col-span-3">
              <label className="block text-[11px] font-semibold uppercase text-gray-500 mb-1">Badge Name *</label>
              <input
                type="text"
                required
                value={name}
                onChange={(e) => setName(e.target.value)}
                placeholder="e.g. Phishing Sentinel"
                className="w-full text-xs p-3 border border-gray-200 rounded-lg"
              />
            </div>
          </div>

          <div>
            <label className="block text-[11px] font-semibold uppercase text-gray-500 mb-1">Required Points</label>
            <input
              type="number"
              min={0}
              value={requiredPoints}
              onChange={(e) => setRequiredPoints(parseInt(e.target.value, 10) || 0)}
              className="w-full text-xs p-3 border border-gray-200 rounded-lg"
            />
          </div>

          <div>
            <label className="block text-[11px] font-semibold uppercase text-gray-500 mb-1">Description</label>
            <textarea
              rows={2}
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              placeholder="Awarded when the user achieves safety score..."
              className="w-full text-xs p-3 border border-gray-200 rounded-lg"
            />
          </div>

          <div className="flex items-center gap-2">
            <input
              id="bdg-act"
              type="checkbox"
              checked={isActive}
              onChange={(e) => setIsActive(e.target.checked)}
              className="rounded border-gray-300 text-black focus:ring-black"
            />
            <label htmlFor="bdg-act" className="text-xs text-gray-700 cursor-pointer font-medium">
              Badge is active and earnable
            </label>
          </div>

          <div className="flex justify-end gap-2 pt-2">
            <button type="button" onClick={onClose} className="px-4 py-2 text-xs font-medium text-gray-600 hover:text-black">
              Cancel
            </button>
            <button
              type="submit"
              disabled={isSubmitting}
              className="px-5 py-2 text-xs font-semibold bg-[#191919] text-white rounded-full hover:bg-black/85 disabled:opacity-50"
            >
              {isSubmitting ? 'Saving...' : badge ? 'Update Badge' : 'Create Badge'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

// -----------------------------------------------------------------------------
// PREVIEW MODAL AS LEARNER
// -----------------------------------------------------------------------------
const PreviewModal: React.FC<{
  item: { type: 'module' | 'quiz' | 'scenario'; data: any };
  categories: FraudCategory[];
  onClose: () => void;
  onPublishModule: (m: LearningModule) => void;
  onPublishQuiz: (q: Quiz) => void;
  onPublishScenario: (s: Scenario) => void;
}> = ({ item, categories, onClose, onPublishModule, onPublishQuiz, onPublishScenario }) => {
  const cat = categories.find((c) => c.id === item.data.fraudCategoryId);
  const isDraft = !item.data.isPublished;
  const [previewPersona, setPreviewPersona] = useState<string>(item.data.targetAudience || 'All');

  return (
    <div className="fixed inset-0 z-50 bg-black/50 backdrop-blur-sm flex items-center justify-center p-4 overflow-y-auto">
      <div className="bg-white rounded-2xl max-w-2xl w-full p-6 sm:p-8 shadow-2xl border border-gray-200 space-y-6 my-8 max-h-[90vh] overflow-y-auto relative">
        {/* Draft Notice Banner */}
        {isDraft ? (
          <div className="p-3 bg-amber-50 border border-amber-200 text-amber-900 rounded-xl text-xs flex items-center justify-between">
            <span className="font-semibold uppercase tracking-wider flex items-center gap-1.5">
              <span className="w-2 h-2 rounded-full bg-amber-500 animate-pulse" />
              Draft Preview — Not Publicly Visible to Learners
            </span>
            <span className="text-[10px] text-amber-700">Internal Review Mode</span>
          </div>
        ) : (
          <div className="p-3 bg-emerald-50 border border-emerald-200 text-emerald-900 rounded-xl text-xs flex items-center justify-between">
            <span className="font-semibold uppercase tracking-wider flex items-center gap-1.5">
              <span className="w-2 h-2 rounded-full bg-emerald-600" />
              Live Published Content — Visible to All Active Users
            </span>
          </div>
        )}

        {/* Persona Switcher for Preview */}
        <div className="flex flex-wrap items-center justify-between gap-2 p-3 bg-gray-50 border border-gray-200 rounded-xl">
          <div className="text-xs">
            <span className="font-semibold text-gray-700">Preview Persona:</span>
            <span className="ml-1 text-gray-500">Test how different audiences experience this item</span>
          </div>
          <select
            value={previewPersona}
            onChange={(e) => setPreviewPersona(e.target.value)}
            className="text-xs p-1.5 bg-white border border-gray-200 rounded-lg font-medium"
          >
            <option value="All">Standard Learner View</option>
            <option value="EarlyLearners">Early Learner (Ages 5–7)</option>
            <option value="Children">Child (Ages 8–12)</option>
            <option value="Teens">Teenager (Ages 13–17)</option>
            <option value="YoungAdults">Student (Ages 18–24)</option>
            <option value="Adults">Everyday Adult</option>
            <option value="Seniors">Senior Citizen (65+)</option>
            <option value="Professionals">Enterprise Professional</option>
          </select>
        </div>

        {/* Header & Metadata */}
        <div className="flex items-start justify-between border-b border-gray-100 pb-4">
          <div>
            <div className="flex items-center gap-2 mb-1.5">
              <span className="text-[10px] uppercase font-bold text-gray-400 tracking-wider">
                {item.type.toUpperCase()} PREVIEW &bull; {cat?.name || 'General Category'}
              </span>
              <AudienceBadge audience={item.data.targetAudience} />
              <DifficultyBadge difficulty={item.data.difficultyLevel} />
              {item.data.estimatedMinutes && (
                <span className="text-[10px] text-gray-500 font-medium">
                  &bull; ~{item.data.estimatedMinutes} min read
                </span>
              )}
            </div>
            <h2 className="text-2xl font-serif text-[#191919] mt-1">{item.data.title}</h2>
            {item.data.description && <p className="text-xs text-gray-500 mt-1">{item.data.description}</p>}
          </div>
          <button type="button" onClick={onClose} className="text-gray-400 hover:text-black font-bold text-lg">
            &times;
          </button>
        </div>

        {/* Persona Perspective Highlights */}
        {previewPersona === 'EarlyLearners' && (
          <div className="p-3 bg-emerald-50 border border-emerald-200 rounded-xl text-xs text-emerald-950 flex items-start gap-2">
            <span className="text-base leading-none">🧸</span>
            <div>
              <strong>Early Learner (5–7) Presentation:</strong>
              <p className="mt-0.5 text-emerald-800">
                Audio read-aloud is activated automatically. Vocabulary is simplified to concrete rules: "Stop, don't tap, show a trusted grown-up."
              </p>
            </div>
          </div>
        )}
        {previewPersona === 'Seniors' && (
          <div className="p-3 bg-amber-50 border border-amber-200 rounded-xl text-xs text-amber-950 flex items-start gap-2">
            <span className="text-base leading-none">👓</span>
            <div>
              <strong>Senior Citizen (65+) Presentation:</strong>
              <p className="mt-0.5 text-amber-800">
                High contrast mode supported with 120% font scaling. Includes official bank branch verification guidance and zero-shame reporting advice.
              </p>
            </div>
          </div>
        )}
        {previewPersona === 'Professionals' && (
          <div className="p-3 bg-slate-100 border border-slate-300 rounded-xl text-xs text-slate-900 flex items-start gap-2">
            <span className="text-base leading-none">💼</span>
            <div>
              <strong>Enterprise Professional Presentation:</strong>
              <p className="mt-0.5 text-slate-700">
                Focuses on vendor fraud, spear-phishing, MFA bypass, and formal incident response escalation to IT security officers.
              </p>
            </div>
          </div>
        )}

        {/* Dynamic preview content */}
        {item.type === 'module' && (
          <div className="space-y-4">
            <div className="prose prose-sm max-w-none text-xs text-gray-800 leading-relaxed whitespace-pre-line p-4 bg-gray-50/50 rounded-xl border border-gray-100">
              {item.data.content}
            </div>

            {/* Learning Objectives */}
            {item.data.learningObjectives && (
              <div className="p-4 bg-blue-50/60 border border-blue-200 rounded-xl text-xs space-y-1">
                <span className="font-bold text-[11px] uppercase tracking-wider text-blue-900 block">
                  🎯 Core Learning Objectives:
                </span>
                <p className="text-blue-950 whitespace-pre-line leading-relaxed">{item.data.learningObjectives}</p>
              </div>
            )}

            {/* Safety Tips Checklist */}
            {item.data.safetyTips && (
              <div className="p-4 bg-emerald-50/60 border border-emerald-200 rounded-xl text-xs space-y-1">
                <span className="font-bold text-[11px] uppercase tracking-wider text-emerald-900 block">
                  🛡️ Actionable Safety Habits:
                </span>
                <p className="text-emerald-950 whitespace-pre-line leading-relaxed">{item.data.safetyTips}</p>
              </div>
            )}
          </div>
        )}

        {item.type === 'quiz' && (
          <div className="space-y-4">
            <span className="text-xs font-semibold text-gray-700 block">
              Quiz Evaluation Questions ({item.data.questions?.length ?? 0}):
            </span>
            {item.data.questions?.map((q: any, i: number) => (
              <div key={i} className="p-4 bg-gray-50 rounded-xl border border-gray-200 space-y-2 text-xs">
                <span className="font-semibold text-[#191919] block">
                  {i + 1}. {q.questionText}
                </span>
                <div className="space-y-1.5 pt-1">
                  {q.options?.map((opt: any, oi: number) => (
                    <div
                      key={oi}
                      className={`p-2 rounded-lg border text-[11px] flex items-center justify-between ${
                        opt.isCorrect
                          ? 'bg-emerald-50 border-emerald-300 text-emerald-900 font-semibold'
                          : 'bg-white border-gray-200 text-gray-700'
                      }`}
                    >
                      <span>{opt.optionText}</span>
                      {opt.isCorrect && <span className="text-[10px] text-emerald-700 font-bold">&check; Correct Answer</span>}
                    </div>
                  ))}
                </div>
                {q.explanation && (
                  <p className="text-[11px] text-gray-500 pt-1 italic">
                    <strong>Explanation:</strong> {q.explanation}
                  </p>
                )}
              </div>
            ))}
          </div>
        )}

        {item.type === 'scenario' && (
          <div className="space-y-4 text-xs">
            <div className="p-4 bg-gray-50 rounded-xl border border-gray-200 space-y-1">
              <span className="font-bold text-[11px] uppercase tracking-wider text-gray-500 block">The Scenario:</span>
              <p className="text-gray-800 leading-relaxed whitespace-pre-line">{item.data.situation}</p>
            </div>
            <div className="p-4 bg-emerald-50 rounded-xl border border-emerald-200 space-y-1">
              <span className="font-bold text-[11px] uppercase tracking-wider text-emerald-800 block">Recommended Action:</span>
              <p className="text-emerald-950 leading-relaxed whitespace-pre-line">{item.data.correctAction}</p>
            </div>
            {item.data.safetyTips && (
              <div className="p-4 bg-blue-50/60 border border-blue-200 rounded-xl text-xs space-y-1">
                <span className="font-bold text-[11px] uppercase tracking-wider text-blue-900 block">
                  🛡️ Safe Habit Checklist:
                </span>
                <p className="text-blue-950 whitespace-pre-line leading-relaxed">{item.data.safetyTips}</p>
              </div>
            )}
          </div>
        )}

        <div className="flex items-center justify-between pt-4 border-t border-gray-100">
          <button type="button" onClick={onClose} className="text-xs text-gray-500 hover:text-black font-medium">
            Close Preview
          </button>
          <div className="flex items-center gap-2">
            {isDraft ? (
              <button
                type="button"
                onClick={() => {
                  onClose();
                  if (item.type === 'module') onPublishModule(item.data);
                  if (item.type === 'quiz') onPublishQuiz(item.data);
                  if (item.type === 'scenario') onPublishScenario(item.data);
                }}
                className="px-4 py-2 bg-emerald-600 text-white rounded-full text-xs font-semibold hover:bg-emerald-700"
              >
                Publish Live Now
              </button>
            ) : (
              <span className="text-xs text-emerald-700 font-medium">&check; Live in Public Curriculum</span>
            )}
          </div>
        </div>
      </div>
    </div>
  );
};

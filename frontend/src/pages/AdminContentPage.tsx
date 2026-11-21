import React, { FormEvent, useEffect, useMemo, useState } from 'react';
import { Badge, SaveBadgeRequest, SaveScenarioRequest, Scenario } from '../types/badge';
import { FraudCategory } from '../types/fraud';
import { LearningModule } from '../types/learning';
import { Quiz, QuizManagementDetail, QuizQuestionRequest, SaveQuizRequest } from '../types/quiz';
import { badgeService } from '../services/badgeService';
import { fraudService } from '../services/fraudService';
import { learningService } from '../services/learningService';
import { quizService } from '../services/quizService';
import { scenarioService } from '../services/scenarioService';
import { ErrorMessage } from '../components/common/ErrorMessage';
import { LoadingSpinner } from '../components/common/LoadingSpinner';

export type AdminResource = 'categories' | 'modules' | 'quizzes' | 'scenarios' | 'badges';
type AdminItem = FraudCategory | LearningModule | Quiz | Scenario | Badge;

interface AdminContentPageProps {
  resource: AdminResource;
}

interface ContentForm {
  name: string;
  title: string;
  description: string;
  content: string;
  order: string;
  isPublished: boolean;
  fraudCategoryId: string;
  situation: string;
  correctAction: string;
  icon: string;
  requiredPoints: string;
  isActive: boolean;
  questions: QuizQuestionRequest[];
}

const initialForm: ContentForm = {
  name: '',
  title: '',
  description: '',
  content: '',
  order: '1',
  isPublished: true,
  fraudCategoryId: '',
  situation: '',
  correctAction: '',
  icon: '',
  requiredPoints: '0',
  isActive: true,
  questions: [{
    questionText: '',
    explanation: '',
    order: 1,
    options: ['', '', '', ''].map((optionText, index) => ({
      optionText,
      isCorrect: index === 0,
      order: index + 1,
    })),
  }],
};

const resourceCopy: Record<AdminResource, { title: string; description: string; singular: string }> = {
  categories: {
    title: 'Threat categories',
    description: 'Organize the scam patterns and threats covered across the platform.',
    singular: 'category',
  },
  modules: {
    title: 'Learning modules',
    description: 'Write and maintain practical lessons for the DigitalShield learning library.',
    singular: 'module',
  },
  quizzes: {
    title: 'Quizzes',
    description: 'Build short knowledge checks that reinforce safer online habits.',
    singular: 'quiz',
  },
  scenarios: {
    title: 'Scenarios',
    description: 'Create realistic situations with clear, safe actions for learners to practise.',
    singular: 'scenario',
  },
  badges: {
    title: 'Safety badges',
    description: 'Recognize learning milestones with active platform badges.',
    singular: 'badge',
  },
};

const errorText = (error: unknown, fallback: string) =>
  error instanceof Error ? error.message : fallback;

const itemTitle = (resource: AdminResource, item: AdminItem) =>
  resource === 'categories' || resource === 'badges'
    ? (item as FraudCategory | Badge).name
    : (item as LearningModule | Quiz | Scenario).title;

const itemDescription = (item: AdminItem) => item.description || '—';

const itemStatus = (item: AdminItem) =>
  'isActive' in item ? item.isActive : item.isPublished;

const itemCategoryId = (item: AdminItem) =>
  'fraudCategoryId' in item ? item.fraudCategoryId : undefined;

export const AdminContentPage: React.FC<AdminContentPageProps> = ({ resource }) => {
  const [items, setItems] = useState<AdminItem[]>([]);
  const [categories, setCategories] = useState<FraudCategory[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [search, setSearch] = useState('');
  const [formOpen, setFormOpen] = useState(false);
  const [editingItem, setEditingItem] = useState<AdminItem | null>(null);
  const [form, setForm] = useState<ContentForm>(initialForm);
  const [isSaving, setIsSaving] = useState(false);
  const [notice, setNotice] = useState<string | null>(null);
  const [quizDetail, setQuizDetail] = useState<QuizManagementDetail | null>(null);
  const [isLoadingQuiz, setIsLoadingQuiz] = useState(false);

  const loadItems = async () => {
    setIsLoading(true);
    setError(null);
    try {
      const categoryData = await fraudService.getCategories();
      setCategories(categoryData);
      switch (resource) {
        case 'categories':
          setItems(categoryData);
          break;
        case 'modules':
          setItems(await learningService.getAllForManagement());
          break;
        case 'quizzes':
          setItems(await quizService.getAllForManagement());
          break;
        case 'scenarios':
          setItems(await scenarioService.getAllForManagement());
          break;
        case 'badges':
          setItems(await badgeService.getAllForManagement());
          break;
      }
    } catch (loadError) {
      setError(errorText(loadError, `Could not load ${resourceCopy[resource].title.toLowerCase()}.`));
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    void loadItems();
  }, [resource]);

  const filteredItems = useMemo(() => {
    const query = search.trim().toLowerCase();
    return items.filter((item) => {
      const categoryId = itemCategoryId(item);
      const categoryName = categories.find((category) => category.id === categoryId)?.name || '';
      return !query || `${itemTitle(resource, item)} ${itemDescription(item)} ${categoryName}`.toLowerCase().includes(query);
    });
  }, [categories, items, resource, search]);

  const openCreateForm = () => {
    setEditingItem(null);
    setForm(initialForm);
    setError(null);
    setNotice(null);
    setFormOpen(true);
  };

  const openEditForm = async (item: AdminItem) => {
    setQuizDetail(null);
    let questions = initialForm.questions;
    if (resource === 'quizzes') {
      setIsLoadingQuiz(true);
      try {
        const detail = await quizService.getManagementDetails(item.id);
        questions = detail.questions.map((question) => ({
          questionText: question.questionText,
          explanation: question.explanation,
          order: question.order,
          options: question.options.map((option) => ({
            optionText: option.optionText,
            isCorrect: option.isCorrect,
            order: option.order,
          })),
        }));
      } catch (loadError) {
        setError(errorText(loadError, 'Could not load quiz details for editing.'));
        setIsLoadingQuiz(false);
        return;
      }
      setIsLoadingQuiz(false);
    }

    setEditingItem(item);
    setError(null);
    setNotice(null);
    setForm({
      ...initialForm,
      name: 'name' in item ? item.name : '',
      title: 'title' in item ? item.title : '',
      description: item.description || '',
      content: 'content' in item ? item.content : '',
      order: 'order' in item ? String(item.order) : '1',
      isPublished: 'isPublished' in item ? item.isPublished : true,
      fraudCategoryId: itemCategoryId(item) ? String(itemCategoryId(item)) : '',
      situation: 'situation' in item ? item.situation : '',
      correctAction: 'correctAction' in item ? item.correctAction : '',
      icon: 'icon' in item ? item.icon || '' : '',
      requiredPoints: 'requiredPoints' in item ? String(item.requiredPoints) : '0',
      isActive: 'isActive' in item ? item.isActive : true,
      questions,
    });
    setFormOpen(true);
  };

  const setField = <K extends keyof ContentForm>(field: K, value: ContentForm[K]) => {
    setForm((current) => ({ ...current, [field]: value }));
  };

  const updateQuestion = (questionIndex: number, patch: Partial<QuizQuestionRequest>) => {
    setForm((current) => ({
      ...current,
      questions: current.questions.map((question, index) =>
        index === questionIndex ? { ...question, ...patch } : question
      ),
    }));
  };

  const showQuizDetail = async (id: number) => {
    setIsLoadingQuiz(true);
    setError(null);
    try {
      setQuizDetail(await quizService.getManagementDetails(id));
    } catch (detailError) {
      setError(errorText(detailError, 'Could not load quiz details.'));
    } finally {
      setIsLoadingQuiz(false);
    }
  };

  const submitForm = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setIsSaving(true);
    setError(null);
    setNotice(null);
    try {
      switch (resource) {
        case 'categories': {
          const payload = {
            name: form.name.trim(),
            description: form.description.trim() || undefined,
            isActive: form.isActive,
          };
          if (editingItem) {
            await fraudService.updateCategory(editingItem.id, payload);
          } else {
            await fraudService.createCategory(payload);
          }
          break;
        }
        case 'modules': {
          const payload = {
            title: form.title.trim(),
            description: form.description.trim() || undefined,
            content: form.content.trim(),
            order: Number(form.order),
            isPublished: form.isPublished,
            fraudCategoryId: form.fraudCategoryId ? Number(form.fraudCategoryId) : undefined,
          };
          if (editingItem) {
            await learningService.updateModule(editingItem.id, payload);
          } else {
            await learningService.createModule(payload);
          }
          break;
        }
        case 'quizzes': {
          const payload: SaveQuizRequest = {
            title: form.title.trim(),
            description: form.description.trim() || undefined,
            isPublished: form.isPublished,
            fraudCategoryId: form.fraudCategoryId ? Number(form.fraudCategoryId) : undefined,
            questions: form.questions.map((question, questionIndex) => ({
              questionText: question.questionText.trim(),
              explanation: question.explanation?.trim() || undefined,
              order: questionIndex + 1,
              options: question.options.map((option, optionIndex) => ({
                optionText: option.optionText.trim(),
                isCorrect: option.isCorrect,
                order: optionIndex + 1,
              })),
            })),
          };
          if (editingItem) {
            await quizService.updateQuiz(editingItem.id, payload);
          } else {
            await quizService.createQuiz(payload);
          }
          break;
        }
        case 'scenarios': {
          const payload: SaveScenarioRequest = {
            title: form.title.trim(),
            description: form.description.trim() || undefined,
            situation: form.situation.trim(),
            correctAction: form.correctAction.trim(),
            isPublished: form.isPublished,
            fraudCategoryId: form.fraudCategoryId ? Number(form.fraudCategoryId) : undefined,
          };
          if (editingItem) {
            await scenarioService.updateScenario(editingItem.id, payload);
          } else {
            await scenarioService.createScenario(payload);
          }
          break;
        }
        case 'badges': {
          const payload: SaveBadgeRequest = {
            name: form.name.trim(),
            description: form.description.trim() || undefined,
            icon: form.icon.trim() || undefined,
            requiredPoints: Number(form.requiredPoints),
            isActive: form.isActive,
          };
          if (editingItem) {
            await badgeService.updateBadge(editingItem.id, payload);
          } else {
            await badgeService.createBadge(payload);
          }
          break;
        }
      }
      setFormOpen(false);
      setEditingItem(null);
      setNotice(`${resourceCopy[resource].singular} ${editingItem ? 'updated' : 'created'} successfully.`);
      await loadItems();
    } catch (saveError) {
      setError(errorText(saveError, `Could not save this ${resourceCopy[resource].singular}.`));
    } finally {
      setIsSaving(false);
    }
  };

  const copy = resourceCopy[resource];

  if (isLoading) {
    return <LoadingSpinner label={`Loading ${copy.title.toLowerCase()}...`} size="lg" />;
  }

  return (
    <div className="space-y-6">
      <header className="flex flex-col gap-4 sm:flex-row sm:items-end sm:justify-between">
        <div>
          <p className="text-[11px] font-semibold uppercase tracking-[0.18em] text-[#191919]/45">Content management</p>
          <h2 className="mt-1 font-serif text-2xl sm:text-3xl">{copy.title}</h2>
          <p className="mt-2 max-w-2xl text-sm leading-6 text-[#191919]/60">{copy.description}</p>
        </div>
        <button
          type="button"
          onClick={openCreateForm}
          className="rounded-full bg-[#191919] px-5 py-2.5 text-xs font-medium text-white transition hover:bg-black/80"
        >
          + Add {copy.singular}
        </button>
      </header>

      {notice && (
        <div className="rounded-xl border border-emerald-200 bg-emerald-50 px-4 py-3 text-xs text-emerald-800" role="status">
          {notice}
        </div>
      )}
      {error && <ErrorMessage title="Content action failed" message={error} onRetry={() => void loadItems()} />}

      <section className="overflow-hidden rounded-2xl border border-gray-200 bg-white">
        <div className="flex flex-col gap-3 border-b border-gray-100 p-4 sm:flex-row sm:items-center sm:justify-between sm:px-5">
          <div className="text-xs text-[#191919]/55">
            <span className="font-semibold text-[#191919]">{filteredItems.length}</span> {filteredItems.length === 1 ? 'item' : 'items'}
            <span className="hidden sm:inline"> · Live content returned by the API</span>
          </div>
          <input
            type="search"
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            placeholder={`Search ${copy.title.toLowerCase()}...`}
            aria-label={`Search ${copy.title.toLowerCase()}`}
            className="w-full rounded-xl border border-gray-200 px-3.5 py-2.5 text-xs outline-none transition focus:border-[#191919] sm:max-w-xs"
          />
        </div>
        {filteredItems.length ? (
          <div className="overflow-x-auto">
            <table className="w-full min-w-[640px] text-left text-xs">
              <thead className="bg-[#FAFAF8] text-[10px] uppercase tracking-wider text-[#191919]/45">
                <tr>
                  <th className="px-5 py-3 font-semibold">Name</th>
                  <th className="px-5 py-3 font-semibold">Category</th>
                  <th className="px-5 py-3 font-semibold">Status</th>
                  <th className="px-5 py-3 font-semibold">Created</th>
                  <th className="px-5 py-3 text-right font-semibold">Action</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-100">
                {filteredItems.map((item) => {
                  const categoryId = itemCategoryId(item);
                  const categoryName = categories.find((category) => category.id === categoryId)?.name;
                  return (
                    <tr key={item.id} className="transition hover:bg-[#FAFAF8]">
                      <td className="max-w-[320px] px-5 py-4">
                        <p className="truncate font-medium text-[#191919]">{itemTitle(resource, item)}</p>
                        <p className="mt-1 truncate text-[11px] text-[#191919]/45">{itemDescription(item)}</p>
                      </td>
                      <td className="px-5 py-4 text-[#191919]/60">{categoryName || (categoryId ? 'Unassigned' : '—')}</td>
                      <td className="px-5 py-4">
                        <span className={`inline-flex items-center gap-1.5 rounded-full px-2.5 py-1 text-[10px] font-semibold ${itemStatus(item) ? 'bg-emerald-50 text-emerald-800' : 'bg-gray-100 text-gray-600'}`}>
                          <span className={`h-1.5 w-1.5 rounded-full ${itemStatus(item) ? 'bg-emerald-500' : 'bg-gray-400'}`} />
                          {itemStatus(item) ? (resource === 'categories' || resource === 'badges' ? 'Active' : 'Published') : (resource === 'categories' || resource === 'badges' ? 'Inactive' : 'Draft')}
                        </span>
                      </td>
                      <td className="px-5 py-4 text-[#191919]/55">
                        {new Date(item.createdAt).toLocaleDateString(undefined, { month: 'short', day: 'numeric', year: 'numeric' })}
                      </td>
                      <td className="px-5 py-4 text-right">
                        {resource === 'quizzes' ? (
                          <div className="flex justify-end gap-2">
                            <button
                              type="button"
                              onClick={() => void showQuizDetail(item.id)}
                              className="rounded-full border border-gray-200 px-3 py-1.5 text-[11px] font-medium hover:border-[#191919]"
                            >
                              Preview
                            </button>
                            <button
                              type="button"
                              onClick={() => void openEditForm(item)}
                              className="rounded-full border border-gray-200 px-3 py-1.5 text-[11px] font-medium hover:border-[#191919]"
                            >
                              Edit
                            </button>
                          </div>
                        ) : (
                          <button
                            type="button"
                            onClick={() => openEditForm(item)}
                            className="rounded-full border border-gray-200 px-3 py-1.5 text-[11px] font-medium hover:border-[#191919]"
                          >
                            Edit
                          </button>
                        )}
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>
        ) : (
          <div className="px-6 py-14 text-center">
            <p className="font-serif text-lg">{search ? 'No matching items' : `No ${copy.title.toLowerCase()} yet`}</p>
            <p className="mt-1 text-xs text-[#191919]/55">
              {search ? 'Try another search term.' : `Create your first ${copy.singular} to get started.`}
            </p>
          </div>
        )}
      </section>

      {formOpen && (
        <div className="fixed inset-0 z-[60] flex items-center justify-center overflow-y-auto bg-black/40 p-4" role="presentation">
          <div role="dialog" aria-modal="true" aria-labelledby="admin-form-title" className="my-auto max-h-[92vh] w-full max-w-2xl overflow-y-auto rounded-2xl bg-white shadow-2xl">
            <div className="flex items-start justify-between border-b border-gray-100 px-5 py-4 sm:px-7">
              <div>
                <h3 id="admin-form-title" className="font-serif text-xl">
                  {editingItem ? 'Edit' : 'Create'} {copy.singular}
                </h3>
                <p className="mt-1 text-xs text-[#191919]/55">Changes are saved to the DigitalShield API.</p>
              </div>
              <button type="button" onClick={() => setFormOpen(false)} aria-label="Close form" className="rounded-full px-2 py-1 text-lg text-[#191919]/50 hover:bg-gray-100">×</button>
            </div>
            <form onSubmit={(event) => void submitForm(event)} className="space-y-4 p-5 sm:p-7">
              {(resource === 'categories' || resource === 'badges') ? (
                <Field label={resource === 'categories' ? 'Category name' : 'Badge name'}>
                  <input required value={form.name} onChange={(event) => setField('name', event.target.value)} className={inputClass} />
                </Field>
              ) : (
                <Field label={resource === 'scenarios' ? 'Scenario title' : resource === 'quizzes' ? 'Quiz title' : 'Module title'}>
                  <input required value={form.title} onChange={(event) => setField('title', event.target.value)} className={inputClass} />
                </Field>
              )}
              <Field label="Description">
                <textarea rows={2} value={form.description} onChange={(event) => setField('description', event.target.value)} className={inputClass} />
              </Field>
              {resource !== 'categories' && resource !== 'badges' && (
                <Field label="Threat category">
                  <select value={form.fraudCategoryId} onChange={(event) => setField('fraudCategoryId', event.target.value)} className={inputClass}>
                    <option value="">No category</option>
                    {categories.map((category) => <option key={category.id} value={category.id}>{category.name}</option>)}
                  </select>
                </Field>
              )}
              {resource === 'modules' && (
                <>
                  <Field label="Learning content">
                    <textarea required rows={7} value={form.content} onChange={(event) => setField('content', event.target.value)} className={inputClass} />
                  </Field>
                  <Field label="Display order">
                    <input type="number" min="0" required value={form.order} onChange={(event) => setField('order', event.target.value)} className={inputClass} />
                  </Field>
                </>
              )}
              {resource === 'quizzes' && (
                <>
                  <div className="border-t border-gray-100 pt-4">
                    <h4 className="text-sm font-semibold">Questions & answer choices</h4>
                    <p className="mt-1 text-[11px] text-[#191919]/55">Choose one correct answer for each question.</p>
                  </div>
                  {form.questions.map((question, questionIndex) => (
                    <fieldset key={questionIndex} className="space-y-3 rounded-xl border border-gray-200 p-4">
                      <div className="flex items-center justify-between">
                        <legend className="text-xs font-semibold">Question {questionIndex + 1}</legend>
                        {form.questions.length > 1 && (
                          <button type="button" onClick={() => setField('questions', form.questions.filter((_, index) => index !== questionIndex))} className="text-[11px] text-red-700 hover:underline">
                            Remove
                          </button>
                        )}
                      </div>
                      <textarea required rows={2} aria-label={`Question ${questionIndex + 1}`} placeholder="Question" value={question.questionText} onChange={(event) => updateQuestion(questionIndex, { questionText: event.target.value })} className={inputClass} />
                      <textarea rows={2} aria-label={`Explanation for question ${questionIndex + 1}`} placeholder="Answer explanation (optional)" value={question.explanation || ''} onChange={(event) => updateQuestion(questionIndex, { explanation: event.target.value })} className={inputClass} />
                      {question.options.map((option, optionIndex) => (
                        <div key={optionIndex} className="flex gap-2">
                          <input required aria-label={`Question ${questionIndex + 1}, answer ${optionIndex + 1}`} value={option.optionText} onChange={(event) => {
                            const options = question.options.map((item, index) =>
                              index === optionIndex ? { ...item, optionText: event.target.value } : item
                            );
                            updateQuestion(questionIndex, { options });
                          }} className={inputClass} />
                          <button type="button" aria-label={`Mark question ${questionIndex + 1}, answer ${optionIndex + 1} as correct`} onClick={() => {
                            const options = question.options.map((item, index) => ({ ...item, isCorrect: index === optionIndex }));
                            updateQuestion(questionIndex, { options });
                          }} className={`shrink-0 rounded-xl border px-3 text-[11px] font-medium ${option.isCorrect ? 'border-emerald-600 bg-emerald-50 text-emerald-800' : 'border-gray-200 text-gray-500'}`}>
                            {option.isCorrect ? 'Correct' : 'Mark correct'}
                          </button>
                        </div>
                      ))}
                    </fieldset>
                  ))}
                  <button type="button" onClick={() => setField('questions', [...form.questions, {
                    questionText: '',
                    order: form.questions.length + 1,
                    options: ['', '', '', ''].map((optionText, index) => ({ optionText, isCorrect: index === 0, order: index + 1 })),
                  }])} className="rounded-full border border-gray-300 px-4 py-2 text-xs font-medium hover:border-[#191919]">
                    + Add question
                  </button>
                </>
              )}
              {resource === 'scenarios' && (
                <>
                  <Field label="Situation">
                    <textarea required rows={4} value={form.situation} onChange={(event) => setField('situation', event.target.value)} className={inputClass} />
                  </Field>
                  <Field label="Recommended safe action">
                    <textarea required rows={4} value={form.correctAction} onChange={(event) => setField('correctAction', event.target.value)} className={inputClass} />
                  </Field>
                </>
              )}
              {resource === 'badges' && (
                <>
                  <Field label="Icon name or identifier">
                    <input value={form.icon} onChange={(event) => setField('icon', event.target.value)} className={inputClass} />
                  </Field>
                  <Field label="Points required">
                    <input type="number" min="0" required value={form.requiredPoints} onChange={(event) => setField('requiredPoints', event.target.value)} className={inputClass} />
                  </Field>
                </>
              )}
              {(resource === 'modules' || resource === 'quizzes' || resource === 'scenarios') ? (
                <label className="flex items-center gap-2 text-xs text-[#191919]/75">
                  <input type="checkbox" checked={form.isPublished} onChange={(event) => setField('isPublished', event.target.checked)} className="accent-[#191919]" />
                  Publish this content
                </label>
              ) : (
                <label className="flex items-center gap-2 text-xs text-[#191919]/75">
                  <input type="checkbox" checked={form.isActive} onChange={(event) => setField('isActive', event.target.checked)} className="accent-[#191919]" />
                  Active
                </label>
              )}
              <div className="flex justify-end gap-2 border-t border-gray-100 pt-4">
                <button type="button" onClick={() => setFormOpen(false)} className="rounded-full px-4 py-2 text-xs text-[#191919]/65 hover:bg-gray-100">Cancel</button>
                <button type="submit" disabled={isSaving} className="rounded-full bg-[#191919] px-5 py-2 text-xs font-medium text-white disabled:opacity-50">
                  {isSaving ? 'Saving…' : editingItem ? 'Save changes' : `Create ${copy.singular}`}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {(quizDetail || isLoadingQuiz) && (
        <div className="fixed inset-0 z-[60] flex items-center justify-center overflow-y-auto bg-black/40 p-4">
          <div role="dialog" aria-modal="true" aria-labelledby="quiz-detail-title" className="my-auto max-h-[90vh] w-full max-w-2xl overflow-y-auto rounded-2xl bg-white p-5 shadow-2xl sm:p-7">
            {isLoadingQuiz ? <LoadingSpinner label="Loading quiz..." /> : quizDetail && (
              <>
                <div className="flex items-start justify-between">
                  <div>
                    <p className="text-[10px] font-semibold uppercase tracking-widest text-[#191919]/45">Quiz details</p>
                    <h3 id="quiz-detail-title" className="mt-1 font-serif text-xl">{quizDetail.title}</h3>
                    <p className="mt-1 text-xs text-[#191919]/55">{quizDetail.description}</p>
                  </div>
                  <button type="button" onClick={() => setQuizDetail(null)} aria-label="Close quiz details" className="rounded-full px-2 py-1 text-lg text-[#191919]/50 hover:bg-gray-100">×</button>
                </div>
                <div className="mt-5 space-y-4">
                  {quizDetail.questions.map((question, index) => (
                    <div key={question.id} className="rounded-xl border border-gray-200 p-4">
                      <p className="text-[10px] font-semibold uppercase tracking-wider text-[#191919]/45">Question {index + 1}</p>
                      <p className="mt-1 text-sm font-medium">{question.questionText}</p>
                      <ul className="mt-3 space-y-2 text-xs text-[#191919]/65">
                        {question.options.map((option) => <li key={option.id} className={`rounded-lg px-3 py-2 ${option.isCorrect ? 'bg-emerald-50 text-emerald-900' : 'bg-[#F7F7F5]'}`}>{option.optionText}{option.isCorrect ? ' · Correct' : ''}</li>)}
                      </ul>
                      {question.explanation && <p className="mt-3 text-xs leading-5 text-[#191919]/55">{question.explanation}</p>}
                    </div>
                  ))}
                </div>
              </>
            )}
          </div>
        </div>
      )}
    </div>
  );
};

const inputClass = 'w-full rounded-xl border border-gray-200 px-3.5 py-2.5 text-xs outline-none transition focus:border-[#191919]';

const Field: React.FC<{ label: string; children: React.ReactNode }> = ({ label, children }) => (
  <label className="block space-y-1.5">
    <span className="block text-[10px] font-semibold uppercase tracking-wider text-[#191919]/60">{label}</span>
    {children}
  </label>
);

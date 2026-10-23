import { api } from './apiClient';
import {
  CreateLearningModuleRequest,
  LearningModule,
  UpdateLearningModuleRequest,
} from '../types/learning';

export const learningService = {
  getPublishedModules: async (audience?: string): Promise<LearningModule[]> => {
    const query = audience && audience !== 'All' ? `?audience=${encodeURIComponent(audience)}` : '';
    return api.get<LearningModule[]>(`/learning-modules${query}`);
  },

  getAllModulesAdmin: async (): Promise<LearningModule[]> => {
    return api.get<LearningModule[]>('/learning-modules/admin/all');
  },

  getModuleById: async (id: number): Promise<LearningModule> => {
    return api.get<LearningModule>(`/learning-modules/${id}`);
  },

  getModuleByIdAdmin: async (id: number): Promise<LearningModule> => {
    return api.get<LearningModule>(`/learning-modules/admin/${id}`);
  },

  getModulesByCategory: async (categoryId: number): Promise<LearningModule[]> => {
    return api.get<LearningModule[]>(`/learning-modules/category/${categoryId}`);
  },

  createModule: async (module: CreateLearningModuleRequest): Promise<LearningModule> => {
    return api.post<LearningModule>('/learning-modules', module);
  },

  updateModule: async (id: number, module: UpdateLearningModuleRequest): Promise<LearningModule> => {
    return api.put<LearningModule>(`/learning-modules/${id}`, module);
  },

  setPublishStatus: async (id: number, isPublished: boolean): Promise<LearningModule> => {
    return api.patch<LearningModule>(`/learning-modules/${id}/publish`, { isPublished });
  },

  deleteModule: async (id: number): Promise<void> => {
    return api.delete<void>(`/learning-modules/${id}`);
  },
};

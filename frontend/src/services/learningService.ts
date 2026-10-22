import { api } from './apiClient';
import {
  CreateLearningModuleRequest,
  LearningModule,
  UpdateLearningModuleRequest,
} from '../types/learning';

export const learningService = {
  getPublishedModules: async (): Promise<LearningModule[]> => {
    return api.get<LearningModule[]>('/learning-modules');
  },

  getModuleById: async (id: number): Promise<LearningModule> => {
    return api.get<LearningModule>(`/learning-modules/${id}`);
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
};

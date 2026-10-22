import { api } from './apiClient';
import {
  CreateUserProgressRequest,
  UpdateUserProgressRequest,
  UserProgress,
} from '../types/progress';

export const progressService = {
  getMyProgress: async (): Promise<UserProgress[]> => {
    return api.get<UserProgress[]>('/progress/me');
  },

  getModuleProgress: async (learningModuleId: number): Promise<UserProgress> => {
    return api.get<UserProgress>(`/progress/me/${learningModuleId}`);
  },

  createProgress: async (data: CreateUserProgressRequest): Promise<UserProgress> => {
    return api.post<UserProgress>('/progress', data);
  },

  updateProgress: async (learningModuleId: number, data: UpdateUserProgressRequest): Promise<UserProgress> => {
    return api.put<UserProgress>(`/progress/me/${learningModuleId}`, data);
  },
};

import { api } from './apiClient';
import {
  CreateFraudCategoryRequest,
  FraudAnalysisResponse,
  FraudCategory,
  FraudCheckRequest,
  UpdateFraudCategoryRequest,
} from '../types/fraud';

export const fraudService = {
  checkMessage: async (request: FraudCheckRequest): Promise<FraudAnalysisResponse> => {
    return api.post<FraudAnalysisResponse>('/fraud/check', request);
  },

  getCategories: async (): Promise<FraudCategory[]> => {
    return api.get<FraudCategory[]>('/fraud-categories');
  },

  getCategoryById: async (id: number): Promise<FraudCategory> => {
    return api.get<FraudCategory>(`/fraud-categories/${id}`);
  },

  createCategory: async (category: CreateFraudCategoryRequest): Promise<FraudCategory> => {
    return api.post<FraudCategory>('/fraud-categories', category);
  },

  updateCategory: async (id: number, category: UpdateFraudCategoryRequest): Promise<FraudCategory> => {
    return api.put<FraudCategory>(`/fraud-categories/${id}`, category);
  },
};
